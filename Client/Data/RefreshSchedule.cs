namespace MoeTradeMarker.Client.Data;

// Called under the loader lock; time is supplied so scheduling can be tested without sleeping.
internal sealed class RefreshSchedule
{
    private readonly double interval;
    private double nextStart = double.NegativeInfinity;
    private bool pending;
    private bool running;
    private bool stopped;

    public RefreshSchedule(double intervalSeconds) => interval = intervalSeconds;

    public void Request()
    {
        if (!stopped) pending = true;
    }

    public bool TryStart(double now)
    {
        if (stopped || running || !pending || now < nextStart) return false;
        pending = false;
        running = true;
        nextStart = now + interval;
        return true;
    }

    public void Complete(bool success)
    {
        running = false;
        if (!success && !stopped) pending = true;
    }

    public void Stop()
    {
        stopped = true;
        pending = false;
    }
}
