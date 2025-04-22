

using Microsoft.Extensions.Logging;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace Shared.Utilities
{
    public static class Utilities
    {
        public enum ProductSortOrder { Default, PriceAsc, PriceDesc, DiscountDesc, DiscountAsc, RatingAsc, RatingDesc, NameAsc, NameDesc, StockAsc, StockDesc }

        
        //INFO:  where T: class --> für Reftypen: string, eigene Objekte, nullable und nicht nullable
        [return: NotNull]
        public static T ReturnValueOrThrowExceptionWhenNull<T>(T value, string errorMessage) where T : class? //akzeptiert nullable und nicht nullable
        {
            return value?? throw new ArgumentNullException(errorMessage); ;
        }

        //INFO: where T: struct --> für Werttypen: int, Guid, decimal, nullable und nicht nullable
        public static T ReturnValueOrThrowExceptionWhenDefault<T>(T value, string errorMessage) where T : struct
        {
            if (EqualityComparer<T>.Default.Equals(value, default))
                throw new ArgumentException(errorMessage);

            return value;
        }

        public class ProductListReturn<T>
        {
            public List<T> ProductList = [];
            public int ProductCount;
            public int MaxPageCount;

        }

        //TODO:
        //DADURCH GEWINNE ICH NICHTS... EVTL, WENN ICH MEHR IM CATCH STEHEN HABE
        //LOHTN SICH IM MOMENT NOCH NICHT, DAS AUSZULAGERN IN SHARED
        //public static class ExceptionHandler
        //{
        //    public static async Task<T> ExecuteWithLoggingAsync<T>(
        //        Func<Task<T>> action,
        //        ILoggerFactory loggerFactory,
        //        string errorMessage)
        //    {
        //        var logger = loggerFactory.CreateLogger("GlobalExceptionHandler");

        //        try
        //        {
        //            logger.LogInformation("TestLogging");
        //            return await action();
        //        }
        //        catch (Exception ex)
        //        {
        //            logger.LogError(ex, errorMessage);
        //            throw;
        //        }
        //    }
        //}


        //AUFRUF IN DER KLASSE z.B: mit:
        //public async Task<List<Category>> GetAllCategoriesTest()
        //{
        //    var result = await ExceptionHandler.ExecuteWithLoggingAsync(
        //        async () =>
        //            await _context.Category.AsNoTracking().ToListAsync(),
        //            _loggerFactory,
        //            $"Datenbankfehler beim Abrufen der Categories");
        //    return result;
        //}


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

        //private static T ThrowExceptionIfNullOrEmpty<T>(T value, Exception exception) where T : class
        //{
        //    if (value == null)
        //    {
        //        throw exception ?? new ArgumentException(nameof(value));
        //    }
        //    return value;
        //}

        //private static T ThrowExceptionIfNull<T>(T? value, Exception exception) where T : struct
        //{
        //    //Prüfung, ob überhaupt ein Wert vorhanden ist und ob dieser der Standardwert ist: 0, Guid.Empty etc.
        //    if (!value.HasValue || EqualityComparer<T>.Default.Equals(value.Value, default))
        //    {
        //        throw exception ?? new ArgumentException(nameof(value));
        //    }
        //    return value.Value;
        //}

        //private async Task<T> ExecuteWithHandlingAsync<T>(Func<Task<T>> operation, string errorMessage)
        //{
        //    try
        //    {
        //        return await operation();
        //    }
        //    catch (Exception ex) // Hier kannst du spezifische Exceptions wie DbUpdateException abfangen
        //    {
        //        _logger.LogError(ex, errorMessage);
        //        throw;
        //    }
        //}

        //private async Task ExecuteWithHandlingAsync(Func<Task> operation, string errorMessage)
        //{
        //    try
        //    {
        //        await operation();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, errorMessage);
        //        throw;
        //    }
        //}
    }
}
