using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using Vega.USiteBuilder.DocumentTypeBuilder;

namespace Vega.USiteBuilder
{
    /// <summary>
    /// Content helper extensions class
    /// </summary>
    public static class ContentHelperExtensions
    {
        /// <summary>
        /// Ases the specified nodes.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="nodes">The nodes.</param>
        /// <returns></returns>
        public static IEnumerable<TType> As<TType>(this IEnumerable<IPublishedContent> nodes)
            where TType : DocumentTypeBase, new()
        {
            return nodes.Select(n => ContentHelper.GetByNodeId<TType>(n.Id));
        }
    }
}