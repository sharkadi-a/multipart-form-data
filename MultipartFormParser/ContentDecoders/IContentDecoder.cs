namespace MultipartFormParser.ContentDecoders
{
    public interface IContentDecoder<out TElement>
    {
        string[] MimeContentTypes { get; }
        MultipartFormDataItem MultipartFormDataItem { get; set; }
        bool CanDecode { get; }
        TElement Decode();
    }
}
