using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Veyrin.Core.Exceptions;
using Veyrin.Core.Models;

namespace Veyrin.Core.Validation
{
    public static partial class Guard
    {
        /// <summary>
        /// 確保集合不為空 (利用 CollectionUtils.IsNotEmpty)
        /// </summary>
        public static void NotEmpty<T>(IEnumerable<T>? collection, [CallerArgumentExpression("collection")] string paramName = "", string message="")
        {
            if (!CollectionUtils.IsNotEmpty(collection))
            {
                message = message.IsEmpty() ? $"{paramName} cannot be null or empty." : message;
                throw new ValidationException(message);
            }
        }
        /// <summary>
        /// 確保集合不含 Null 元素 (利用 CollectionUtils.ContainsNull)
        /// </summary>
        public static void NotContainsNull<T>(IEnumerable<T?>? collection, [CallerArgumentExpression("collection")] string paramName = "") where T : class
        {
            Guard.NotNull(collection, paramName);

            if (collection.ContainsNull())
                throw new ValidationException($"{paramName} contains null elements.", paramName);
        }

        /// <summary>
        /// 確保集合數量在區間內 (利用 CollectionUtils.IsCountInRange)
        /// </summary>
        public static void CountInRange<T>(
            IEnumerable<T>? collection,
            int min,
            int max,
            [CallerArgumentExpression("collection")] string paramName = "",
            RangeBoundary boundary = RangeBoundary.Inclusive)
        {
            Guard.NotNull(collection, paramName);

            if (!collection.IsCountInRange(min, max, boundary))
            {
                var msg = GetRangeErrorMessage(min, max, boundary);
                throw new ValidationException($"{paramName}.Count {msg}", paramName);
            }
        }
    }
}