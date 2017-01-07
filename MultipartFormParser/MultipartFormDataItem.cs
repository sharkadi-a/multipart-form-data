namespace MultipartFormParser
{
    /// <summary>
    /// Multi-part form data item (from HTTP request)
    /// </summary>
    public sealed class MultipartFormDataItem
    {
        /// <summary>
        /// Parameter name
        /// </summary>
        public string Name { get; internal set; }
        /// <summary>
        /// File name
        /// </summary>
        public string Filename { get; internal set; }
        /// <summary>
        /// MIME content type
        /// </summary>
        public string ContentType { get; internal set; }
        /// <summary>
        /// Content character set
        /// </summary>
        public string Charset { get; internal set; }
        /// <summary>
        /// Content transfer encoding
        /// </summary>
        public string ContentTransferEncoding { get; internal set; }
        /// <summary>
        /// Multi-part form item content
        /// </summary>
        public byte[] Content { get; internal set; }

        /// <summary>
        /// Creates new instance of multi-part form data item
        /// </summary>
        public MultipartFormDataItem()
        {
            Charset = "us-ascii";
            ContentType = "text/plain";
            ContentTransferEncoding = "7bit";
        }
    }
}
