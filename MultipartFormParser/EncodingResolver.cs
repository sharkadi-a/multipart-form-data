using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultipartFormParser.Exceptions;

namespace MultipartFormParser
{
    public static class EncodingResolver
    {
        private static readonly IDictionary<string, Func<Encoding>> EncodingFactories = new ConcurrentDictionary<string, Func<Encoding>>();

        public static Encoding FindEncoding(string charSetName)
        {
            try
            {
                return Encoding.GetEncoding(charSetName);
            }
            catch (ArgumentException)
            {
                if (!EncodingFactories.ContainsKey(charSetName)) return null;
                return EncodingFactories[charSetName]();
            }
        }

        public static void SetEncoding(string charSetName, Func<Encoding> encodingFactory)
        {
            if (EncodingFactories.ContainsKey(charSetName))
            {
                EncodingFactories[charSetName] = encodingFactory;
            }
            else
            {
                EncodingFactories.Add(charSetName, encodingFactory);
            }
        }
    }
}
