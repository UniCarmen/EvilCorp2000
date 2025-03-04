

using Microsoft.Extensions.Logging;
using Serilog;

namespace Shared.Utilities
{
    public class Utilities
    {
        public static class ExceptionHandler
        {
            public static async Task<T> ExecuteWithLoggingAsync<T>(
                Func<Task<T>> action,
                ILoggerFactory loggerFactory)
            {
                var logger = loggerFactory.CreateLogger("GlobalExceptionHandler");

                try
                {
                    logger.LogInformation("TestLogging");
                    return await action();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Fehler in der ausgeführten Methode.");
                    throw;
                }
            }
        }


        //public static async Task<T> ExecuteWithHandling<T>(
        //    Func<Task<T>> operation, ILogger logger, string errorMessage)
        //{
        //    try
        //    {
        //        return await operation();
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(errorMessage, ex); throw;
        //    }
        //}

        //public static async Task ExcuteWithHandling(
        //    Func<Task> operation, ILogger logger, string errorMessage)
        //{
        //    try
        //    {
        //        await operation();
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(errorMessage, ex); throw;
        //    }
        //}

        private static T ThrowExceptionIfNullOrEmpty<T>(T value, Exception exception) where T : class
        {
            if (value == null)
            {
                throw exception ?? new ArgumentException(nameof(value));
            }
            return value;
        }

        private static T ThrowExceptionIfNull<T>(T? value, Exception exception) where T : struct
        {
            //Prüfung, ob überhaupt ein Wert vorhanden ist und ob dieser der Standardwert ist: 0, Guid.Empty etc.
            if (!value.HasValue || EqualityComparer<T>.Default.Equals(value.Value, default))
            {
                throw exception ?? new ArgumentException(nameof(value));
            }
            return value.Value;
        }

        private async Task<T> ExecuteWithHandlingAsync<T>(Func<Task<T>> operation, string errorMessage)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex) // Hier kannst du spezifische Exceptions wie DbUpdateException abfangen
            {
                _logger.LogError(ex, errorMessage);
                throw;
            }
        }

        private async Task ExecuteWithHandlingAsync(Func<Task> operation, string errorMessage)
        {
            try
            {
                await operation();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, errorMessage);
                throw;
            }
        }
    }
}
