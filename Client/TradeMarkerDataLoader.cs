#if SPT_CLIENT
using System.Diagnostics;
using System.Threading.Tasks;
using MoeTradeMarker.Client.Data;
using MoeTradeMarker.Shared;
using Newtonsoft.Json;
using SPT.Common.Http;

namespace MoeTradeMarker.Client;

internal static class TradeMarkerDataLoader
{
    private static readonly object Gate = new();
    private static readonly Stopwatch Clock = Stopwatch.StartNew();
    private static RefreshSchedule schedule = new(5);
    private static MarkerCache cache = new();
    private static bool completed;
    private static bool stopped;
    private static int generation;
    private static string? pendingLanguage;
    private static string? syncedLanguage;

    public static void Start()
    {
        lock (Gate)
        {
            generation++;
            stopped = false;
            completed = false;
            cache = new MarkerCache();
            schedule = new RefreshSchedule(5);
            pendingLanguage = null;
            syncedLanguage = null;
            schedule.Request();
        }
    }

    public static void Stop()
    {
        lock (Gate)
        {
            stopped = true;
            generation++;
            completed = false;
            schedule.Stop();
        }
    }

    public static bool TryGetTraderNameForItem(string itemId, out string traderName, bool requestRefresh = true)
    {
        lock (Gate)
        {
            if (requestRefresh) schedule.Request();
            return cache.Snapshot.TryGetTraderName(itemId, out traderName);
        }
    }

    public static bool IsItemRestrictedFromRagfair(string itemId)
    {
        lock (Gate)
        {
            schedule.Request();
            return cache.Snapshot.GetRestriction(itemId) != RagfairRestriction.Allowed;
        }
    }

    public static void RequestRefresh()
    {
        lock (Gate) schedule.Request();
    }

    public static void ApplyItemMarkers(Dictionary<string, string> additions)
    {
        lock (Gate)
        {
            if (stopped || additions.Count == 0) return;
            cache.Apply(additions);
            completed = true;
            schedule.Request();
        }
    }

    public static void QueueLanguage(string language)
    {
        lock (Gate)
        {
            if (stopped || (language == syncedLanguage && pendingLanguage is null)) return;
            pendingLanguage = language;
            schedule.Request();
        }
    }

    public static void Tick()
    {
        lock (Gate)
        {
            if (!schedule.TryStart(Clock.Elapsed.TotalSeconds)) return;
            var currentGeneration = generation;
            var revision = cache.Revision;
            var language = pendingLanguage;
            pendingLanguage = null;
            Task.Run(() => RefreshCore(currentGeneration, revision, language));
        }
    }

    public static bool ConsumeRefreshCompleted()
    {
        lock (Gate)
        {
            var result = completed;
            completed = false;
            return result;
        }
    }

    private static void RefreshCore(int currentGeneration, long revision, string? language)
    {
        var success = false;
        try
        {
            if (language is not null)
            {
                try
                {
                    RequestHandler.PostJson(TradeMarkerConstants.LanguageRoute, JsonConvert.SerializeObject(new { language }));
                    lock (Gate)
                    {
                        if (stopped || generation != currentGeneration) return;
                        syncedLanguage = language;
                    }
                }
                catch (Exception exception)
                {
                    lock (Gate)
                    {
                        if (stopped || generation != currentGeneration) return;
                        pendingLanguage ??= language;
                        schedule.Request();
                    }
                    Plugin.Log.LogDebug($"Moe-TradeMarker language sync failed: {exception.Message}");
                }
            }

            var loaded = MarkerSnapshot.Parse(
                RequestHandler.GetJson(TradeMarkerConstants.TraderInfoRoute),
                RequestHandler.GetJson(TradeMarkerConstants.ItemMarkerRoute),
                RequestHandler.GetJson(TradeMarkerConstants.RagfairRestrictedTraderRoute));
            lock (Gate)
            {
                if (stopped || generation != currentGeneration) return;
                completed |= cache.Replace(revision, loaded);
                success = true;
            }
        }
        catch (Exception exception)
        {
            Plugin.Log.LogDebug($"Moe-TradeMarker marker refresh failed: {exception.Message}");
        }
        finally
        {
            lock (Gate)
            {
                if (!stopped && generation == currentGeneration) schedule.Complete(success);
            }
        }
    }
}
#endif
