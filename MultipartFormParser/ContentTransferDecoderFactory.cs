using System;
using System.Linq;
using MultipartFormParser.ContentTransferDecoders;
using MultipartFormParser.Helpers;

namespace MultipartFormParser
{
    /// <summary>
    /// Creates decoder class instance for the specific content transfer encoding (from HTTP request). Each decoder returns an enumerable of specific type (mostly byte or char)
    /// </summary>
    public static class ContentTransferDecoderFactory
    {
        private static Tuple<ContentTransferDecoderTypeAttribute, Type>[] _contentTypes;

        static ContentTransferDecoderFactory()
        {
            Refresh();
        }

        /// <summary>
        /// Refreshes list of types annotated with ContentTransferDecoderTypeAttribute through reflection. Use this method to add custom content transfer decoders
        /// </summary>
        public static void Refresh()
        {
            _contentTypes = ReflectionHelper.FindAllTypesMatching<ContentTransferDecoderTypeAttribute>();            
        }

        /// <summary>
        /// Creates 7-bit content transfer decoder ("7bit" Content-Transfer-Encoding value)
        /// </summary>
        /// <returns>IContentTransferDecoder which returns characters</returns>
        public static IContentTransferDecoder<char> Create7BitDecoder()
        {
            return new Char7BitDecoder();
        }

        /// <summary>
        /// Creates 8-bit content transfer decoder ("8bit" Content-Transfer-Encoding value)
        /// </summary>
        /// <returns>IContentTransferDecoder which returns characters</returns>
        public static IContentTransferDecoder<char> Create8BitDecoder()
        {
            return new Char8BitDecoder();
        }

        /// <summary>
        /// Creates quoted-printable content transder decoder ("quoted-printable" Content-Transfer-Encoding value)
        /// </summary>
        /// <returns>IContentTransferDecoder which returns characters</returns>
        public static IContentTransferDecoder<char> CreateQuotedPrintableDecoder()
        {
            return new QuotedPrintableDecoder();
        }
        
        /// <summary>
        /// Creates Base64 content transfer decoder ("base64" Content-Transfer-Encoding value)
        /// </summary>
        /// <returns>IContentTransferDecoder which returns bytes</returns>
        public static IContentTransferDecoder<byte> CreateBase64Decoder()
        {
            return new Base64Decoder();
        }

        /// <summary>
        /// Creates binary content transfer decoder ("binary" Content-Transfer-Encoding value). This decoder simply returns binary data of contents
        /// </summary>
        /// <returns>IContentTransferDecoder which returns bytes</returns>
        public static IContentTransferDecoder<byte> CreateBinaryDecoder()
        {
            return new BinaryDecoder();
        }

        /// <summary>
        /// Finds type which implements <see cref="IContentTransferDecoder{TEnumeration}"/> interface for the specific Content-Transfer-Encoding <paramref name="contentTransferName"/>
        /// </summary>
        /// <typeparam name="TEnumeration">Enumeration type into which decoding should be perfomed</typeparam>
        /// <param name="contentTransferName">Content-Transfer-Encoding name</param>
        /// <returns>Type which implements <see cref="IContentTransferDecoder{TEnumeration}"/> or null, if such not found</returns>
        public static Type FindType<TEnumeration>(string contentTransferName)
        {
            return
                ReflectionHelper.Filter(_contentTypes, contentTransferName, typeof (TEnumeration))
                    .FirstOrDefault();
        }

        /// <summary>
        /// Finds and creates instance of a type which implements <see cref="IContentTransferDecoder{TEnumeration}"/> interface for specific Content-Transfer-Encoding in <paramref name="multipartFormDataItem"/>
        /// </summary>
        /// <typeparam name="TEnumeration">Enumeration of type into which decoding should be perfomed</typeparam>
        /// <param name="multipartFormDataItem">Multi-part form data item</param>
        /// <returns>Instance of a type which decodes form content into enumerable of <see cref="TEnumeration"/> or null, if such not found</returns>
        public static IContentTransferDecoder<TEnumeration> FindAndCreateInstance<TEnumeration>(MultipartFormDataItem multipartFormDataItem)
        {
            if (multipartFormDataItem == null) throw new ArgumentNullException("multipartFormDataItem");
            var type = FindType<TEnumeration>(multipartFormDataItem.ContentTransferEncoding);
            if (type == null) return null;
            //if (type != typeof(IContentTransferDecoder<TEnumeration>)) throw new Exception();
            return (IContentTransferDecoder<TEnumeration>)Activator.CreateInstance(type);
        } 
    }
}
