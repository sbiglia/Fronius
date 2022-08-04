using Microsoft.Extensions.Logging;

namespace FroniusDataCollector;

/// <summary>
/// Dummy logger, we do not need to log anything from the fronius library.
/// </summary>
public class DummyLogger : ILogger
{
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        //This is a dummy logger 
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        throw new NotImplementedException();
    }
}