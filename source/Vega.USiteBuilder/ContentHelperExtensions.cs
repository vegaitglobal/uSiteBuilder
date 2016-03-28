using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;

namespace Vega.USiteBuilder
{
    public static class ContentHelperExtensions
    {
        public static IEnumerable<TType> As<TType>(this IEnumerable<IPublishedContent> nodes)
            where TType : DocumentTypeBase, new()
        {
            return nodes.Select(n => ContentHelper.GetByNodeId<TType>(n.Id));
        }
    }
}