using System.Collections.Generic;

namespace MultipartFormParser.ContentTransferDecoders
{
    /// <summary>
    /// Interface for content transfer decoders of Content-Transfer-Encoding header in the HTTP request's multi-part form data item. Types, implementing this interface, returns enumeration of byte or char depending on whether decoders returns binary data or character data
    /// </summary>
    /// <typeparam name="TEnumeration"></typeparam>
    public interface IContentTransferDecoder<out TEnumeration>
    {
        /// <summary>
        /// Content transfer encoding name implemented by decoder
        /// </summary>
        string ContentTransferEncodingName { get; }

        /// <summary>
        /// Decodes content of multi-part form data item into enumeration of type <see cref="TEnumeration"/>
        /// </summary>
        /// <param name="multipartFormDataItem">Multi-part form data item</param>
        /// <returns>Enumeration of <see cref="TEnumeration"/></returns>
        IEnumerable<TEnumeration> Decode(MultipartFormDataItem multipartFormDataItem);
    }
}
