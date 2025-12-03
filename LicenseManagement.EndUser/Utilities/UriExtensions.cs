using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Hymma.Lm.EndUser.Extensions
{
    public static class UriExtensions
    {
        /// <summary>
        /// Adds a query string to a uri
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="collection"></param>
        /// <returns>string</returns>
        public static string WithQueryString(this Uri uri, List<KeyValuePair<string, string>> collection)
        {
            //var array = (
            //    from key in collection.AllKeys
            //    from value in collection.GetValues(key)
            //    select string.Format(
            //"{0}={1}",
            //Uri.EscapeDataString(key),
            //Uri.EscapeDataString(value)
            //    )).ToArray();

            var sb = new StringBuilder();
            foreach (var item in collection)
            {
                sb.AppendFormat("{0}={1}&", Uri.EscapeDataString(item.Key), Uri.EscapeDataString(item.Value));
            }
            var uriBuilder = new UriBuilder(uri);
            uriBuilder.Query = sb.ToString(0, sb.Length - 1);
            return uriBuilder.Uri.AbsoluteUri;
        }
    }
}
