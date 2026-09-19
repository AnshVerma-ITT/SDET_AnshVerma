using NUnit.Framework;

// Fixtures are eligible for parallel execution. BrowserExecutionLimiter applies
// the runtime limit from appsettings.json before any browser is launched.
[assembly: Parallelizable(ParallelScope.Fixtures)]
