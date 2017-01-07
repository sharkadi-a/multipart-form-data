namespace MultipartFormParser
{
    public class MultipartFormDataItem
    {
        public string Name { get; internal set; }
        public string Filename { get; internal set; }
        public string ContentType { get; internal set; }
        public string Charset { get; internal set; }
        public string ContentTransferEncoding { get; internal set; }
        public byte[] Content { get; internal set; }

        public MultipartFormDataItem()
        {
            Charset = "us-ascii";
            ContentType = "text/plain";
            ContentTransferEncoding = "7bit";
        }
    }
}
