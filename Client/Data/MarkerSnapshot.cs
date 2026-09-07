using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MoeTradeMarker.Client.Data;

internal sealed class MarkerSnapshot
{
    public static readonly MarkerSnapshot Empty = Parse("{}", "{}", "[]");
    private readonly Dictionary<string, string> names;
    private readonly Dictionary<string, string> markers;
    private readonly HashSet<string> restricted;

    private MarkerSnapshot(Dictionary<string, string> names, Dictionary<string, string> markers, HashSet<string> restricted)
    {
        this.names = names;
        this.markers = markers;
        this.restricted = restricted;
    }

    public bool TryGetTraderName(string itemId, out string name)
    {
        name = string.Empty;
        if (!markers.TryGetValue(itemId, out var traderId)) return false;
        name = names.TryGetValue(traderId, out var traderName) ? traderName : traderId;
        return true;
    }

    public bool IsRestricted(string itemId) =>
        markers.TryGetValue(itemId, out var traderId) && restricted.Contains(traderId);

    public MarkerSnapshot WithMarkers(Dictionary<string, string> additions)
    {
        var merged = new Dictionary<string, string>(markers, StringComparer.OrdinalIgnoreCase);
        foreach (var pair in additions) merged[pair.Key] = pair.Value;
        return new MarkerSnapshot(names, merged, restricted);
    }

    public static bool TryReadItemMarker(string itemId, JToken? upd, out string traderId)
    {
        traderId = string.Empty;
        if (!IsId(itemId) || upd is not JObject obj ||
            obj["tradeMarker"] is not JObject marker ||
            marker["traderId"] is not JValue { Type: JTokenType.String } value ||
            !IsId((string?)value)) return false;
        traderId = (string)value!;
        return true;
    }

    public bool SameAs(MarkerSnapshot other) =>
        Equal(names, other.names) && Equal(markers, other.markers) && restricted.SetEquals(other.restricted);

    private static bool Equal(Dictionary<string, string> left, Dictionary<string, string> right) =>
        left.Count == right.Count && left.All(pair => right.TryGetValue(pair.Key, out var value) && value == pair.Value);

    public static MarkerSnapshot Parse(string namesJson, string markersJson, string restrictedJson)
    {
        var names = ParseDictionary(namesJson, false);
        var markers = ParseDictionary(markersJson, true);
        if (ReadToken(restrictedJson) is not JArray array)
            throw new JsonException("Expected a trader ID array.");
        var restricted = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var token in array)
        {
            if (token.Type != JTokenType.String || !IsId((string?)token))
                throw new JsonException("Invalid restricted trader ID.");
            restricted.Add((string)token!);
        }
        return new MarkerSnapshot(names, markers, restricted);
    }

    private static Dictionary<string, string> ParseDictionary(string json, bool valuesAreIds)
    {
        if (ReadToken(json) is not JObject obj)
            throw new JsonException("Expected an ID dictionary.");
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var property in obj.Properties())
        {
            var value = property.Value.Type == JTokenType.String ? (string?)property.Value : null;
            // Server keys and marker values are MongoId values, not arbitrary error fields.
            if (!IsId(property.Name) || string.IsNullOrWhiteSpace(value) || (valuesAreIds && !IsId(value)))
                throw new JsonException("Invalid marker dictionary entry.");
            result.Add(property.Name, value!);
        }
        return result;
    }

    private static bool IsId(string? value) =>
        value is { Length: 24 } && value.All(c => c is >= '0' and <= '9' or >= 'a' and <= 'f' or >= 'A' and <= 'F');

    private static JToken ReadToken(string json)
    {
        using var input = new StringReader(json);
        using var reader = new JsonTextReader(input) { DateParseHandling = DateParseHandling.None };
        var token = JToken.Load(reader, new JsonLoadSettings { DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error });
        if (reader.Read()) throw new JsonException("Unexpected trailing response content.");
        return token;
    }
}
