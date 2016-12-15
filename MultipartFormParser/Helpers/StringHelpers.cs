using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultipartFormParser.Exceptions;

namespace MultipartFormParser.Helpers
{
    public static class StringHelpers
    {
        private static Func<string, Encoding> _resolver;

        public static string GetString(MultipartFormDataItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            if (item.Content == null) return null;
            if (item.Content.Length == 0) return "";
            if (string.IsNullOrEmpty(item.Charset)) throw new ArgumentException("Charset must not be null");
            try
            {
                return Encoding.GetEncoding(item.Charset).GetString(item.Content);
            }
            catch (ArgumentException)
            {
                if (_resolver != null)
                {
                    var encoding = _resolver(item.Charset);
                    if (encoding != null) return encoding.GetString(item.Content);
                }
                throw new UnknownEncodingException(item.Charset);
            }
        }

        public static void RegisterUnknownEncodingResolver(Func<string, Encoding> resolver)
        {
            _resolver = resolver;
        }
    }
}
