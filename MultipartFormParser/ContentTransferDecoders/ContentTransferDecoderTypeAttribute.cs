using System;

namespace MultipartFormParser.ContentTransferDecoders
{
    /// <summary>
    /// This attribute is being used by ContentTransferDecoderFactory to find appropriate decoders for specified EnumerationType
    /// </summary>
    public class ContentTransferDecoderTypeAttribute : Attribute
    {
        /// <summary>
        /// Type of enumerable which is used by annotated decoder
        /// </summary>
        public Type EnumerationType { get; private set; }

        /// <summary>
        /// Content-Transfer-Encoding name as specified in multi-part form headers
        /// </summary>
        public string ContentTransferEncodingName { get; private set; }

        /// <summary>
        /// Adds attribute to decoder
        /// </summary>
        /// <param name="contentTransferEncodingName">Type of enumerable which is used by annotated decoder</param>
        /// <param name="enumerationType">Content-Transfer-Encoding name as specified in multi-part form headers</param>
        public ContentTransferDecoderTypeAttribute(string contentTransferEncodingName, Type enumerationType)
        {
            EnumerationType = enumerationType;
            ContentTransferEncodingName = contentTransferEncodingName;
        }
    }
}
