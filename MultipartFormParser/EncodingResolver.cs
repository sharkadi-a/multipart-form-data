using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultipartFormParser
{
    /// <summary>
    /// Resolves encoding from charset using multi-part form data item
    /// </summary>
    public static class EncodingResolver
    {
        private static readonly IDictionary<string, Func<Encoding>> EncodingFactories = new ConcurrentDictionary<string, Func<Encoding>>();
        
        /// <summary>
        /// Finds encoding for specified charset <paramref name="charSetName"/> or null, if not found. This method scans user-defined encodings first, and then searches in all known CLR encodings
        /// </summary>
        /// <param name="charSetName">Charset name, as in charset of Content-Type header</param>
        /// <returns><see cref="Encoding"/></returns>
        public static Encoding FindEncoding(string charSetName)
        {
            if (EncodingFactories.ContainsKey(charSetName))
            {
                return EncodingFactories[charSetName]();
            }
            try
            {
                return Encoding.GetEncoding(charSetName);
            }
            catch (ArgumentException)
            {
                 return null;
            }
        }

        /// <summary>
        /// Returns all known charsets (user-defined only)
        /// </summary>
        /// <returns></returns>
        public static string[] GetUserDefinedCharSetNames()
        {
            return EncodingFactories.Keys.ToArray();
        }

        /// <summary>
        /// Sets encoding factory <paramref name="encodingFactory"/> for specified charset <paramref name="charSetName"/>. If a factory for the charset is already defined, the new factory Func replaces it
        /// </summary>
        /// <param name="charSetName">Charset name, as in charset of Content-Type header</param>
        /// <param name="encodingFactory">Encoding factory for the specified charset</param>
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
