namespace MultipartFormParser.ContentDecoders
{
    /// <summary>
    /// Interface for content data decoders of Content-Type header in the HTTP request's multi-part form data item. Types, which implement this interface, are expected to use ContentTransferDecoderFactory to create instances of types implement IContentTransferDecoder interface to decode content from specified transfer encoding first (Content-Transfer-Encoding form header). It is required to annotate with ContentDecoderTypeAttribute types implement this interface
    /// </summary>
    /// <typeparam name="TClrType">Managed CLR type, into which content should be decoded</typeparam>
    public interface IContentDecoder<out TClrType>
    {
        /// <summary>
        /// Content types supported by content decoder
        /// </summary>
        string[] MimeContentTypes { get; }

        /// <summary>
        /// Multi-part form data item, which content should be decoded into type <see cref="TClrType"/>
        /// </summary>
        MultipartFormDataItem MultipartFormDataItem { get; set; }

        /// <summary>
        /// Returns if multi-part form data item could be decoded successfully
        /// </summary>
        bool CanDecode { get; }

        /// <summary>
        /// Decodes multi-part form data item into type <see cref="TClrType"/>
        /// </summary>
        /// <returns>Instance of a type <see cref="TClrType"/></returns>
        TClrType Decode();
    }
}
