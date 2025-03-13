using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public static class ServiceUtilities
    {
        public static IEnumerable<TResult> MapWithNullChecks<T, TResult>(
            IEnumerable<T> values,
            Func<T, TResult> mappingsFunction,
            string errorValue) where T : class
        {
            values = Utilities.ReturnValueOrThrowExceptionWhenNull(values, $"{errorValue} ist null");

            values = values.Any(value => value == null) ? throw new ArgumentNullException($"{errorValue} contains null values") : values;

            return values.Select(c => mappingsFunction(c));
        }

        public static IEnumerable<TResult> MapWithGuidWithNullChecks<T, TResult>(
            IEnumerable<T> values,
            Func<T, Guid, TResult> mappingsFunction,
            Guid id,
            string errorValue) where T : class
        {
            values = Utilities.ReturnValueOrThrowExceptionWhenNull(values, $"{errorValue} ist null");

            values = values.Any(value => value == null) ? throw new ArgumentNullException($"{errorValue} contains null values") : values;

            return values.Select(c => mappingsFunction(c, id));
        }
    }
}
