using HRIntimeAutomation.Configuration;

namespace HRIntimeAutomation.Helper;

internal static class BrowserExecutionLimiter
{
    private static readonly object SyncRoot = new();
    private static SemaphoreSlim? _executionSlots;
    private static int? _configuredLimit;

    public static async Task<IDisposable> AcquireAsync(TestSettings settings)
    {
        var executionLimit = settings.ParallelEnabled ? settings.ParallelWorkers : 1;
        SemaphoreSlim executionSlots;

        lock (SyncRoot)
        {
            if (_executionSlots is null)
            {
                _configuredLimit = executionLimit;
                _executionSlots = new SemaphoreSlim(executionLimit, executionLimit);
            }
            else if (_configuredLimit != executionLimit)
            {
                throw new InvalidOperationException(
                    "The browser execution limit cannot change after the test run has started.");
            }

            executionSlots = _executionSlots;
        }

        await executionSlots.WaitAsync();
        return new ExecutionSlot(executionSlots);
    }

    private sealed class ExecutionSlot(SemaphoreSlim executionSlots) : IDisposable
    {
        private SemaphoreSlim? _executionSlots = executionSlots;

        public void Dispose() => Interlocked.Exchange(ref _executionSlots, null)?.Release();
    }
}
