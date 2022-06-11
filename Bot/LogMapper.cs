using Discord;

namespace TheBaron.Bot;

public static class LogMapper
{
    public static Func<LogMessage, Task> GetFunc(ILogger logger)
    {
        return Task(message) =>
        {
            var severity = message.Severity switch
            {
                LogSeverity.Critical => LogLevel.Critical,
                LogSeverity.Error => LogLevel.Error,
                LogSeverity.Warning => LogLevel.Warning,
                LogSeverity.Info => LogLevel.Information,
                LogSeverity.Verbose => LogLevel.Trace,
                LogSeverity.Debug => LogLevel.Debug,
                _ => LogLevel.Information
            };

            logger.Log(severity, message.Exception, "[{Source}] {Message}", message.Source, message.Message);

            return Task.CompletedTask;
        };
    }
}