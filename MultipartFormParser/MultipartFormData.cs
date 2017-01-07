namespace MultipartFormParser
{
    /// <summary>
    /// Multi-part form data (processed from HTTP request) 
    /// </summary>
    public sealed class MultipartFormData
    {
        /// <summary>
        /// Multi-part form data items
        /// </summary>
        public MultipartFormDataItem[] Content { get; internal set; }
    }
}
