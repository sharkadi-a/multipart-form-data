using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using MultipartFormParser.Exceptions;
using MultipartFormParser.Helpers;

namespace MultipartFormParser.ContentDecoders
{
    /// <summary>
    /// Content decoder for images from multi-part form item data. Use this class if content-type is image base type. Following image types are supported:
    /// image/png, image/bmp, image/emf, image/exif, image/gif, image/x-icon, image/vnd.microsoft.icon, image/jpeg, image/pjpeg
    /// </summary>
    [ContentDecoderType(typeof (Image), "image/png", "image/bmp", "image/emf", "image/exif", "image/gif", "image/x-icon",
        "image/vnd.microsoft.icon", "image/jpeg", "image/pjpeg")]
    public class ImageContentDecoder : IContentDecoder<Image>
    {
        public string[] MimeContentTypes
        {
            get
            {
                return new[]
                {
                    "image/png", "image/bmp", "image/emf", "image/exif", "image/gif", "image/x-icon",
                    "image/vnd.microsoft.icon", "image/jpeg", "image/pjpeg"
                };
            }
        }

        public MultipartFormDataItem MultipartFormDataItem { get; set; }

        public ImageContentDecoder(MultipartFormDataItem multipartFormDataItem)
        {
            if (multipartFormDataItem == null) throw new ArgumentNullException("multipartFormDataItem");
            MultipartFormDataItem = multipartFormDataItem;
        }

        public ImageContentDecoder()
        {

        }

        public bool CanDecode
        {
            get
            {
                if (MultipartFormDataItem == null) throw new ContentDecodingException("MultipartFormDataItem must not be null");
                return !string.IsNullOrEmpty(MultipartFormDataItem.ContentType) &&
                       MultipartFormDataItem.ContentType.Contains("image") &&
                       (string.IsNullOrEmpty(MultipartFormDataItem.ContentTransferEncoding) ||
                        ContentTransferDecoderFactory.FindType<byte>(MultipartFormDataItem.ContentTransferEncoding) !=
                        null);
            }
        }

        public ImageFormat ImageFormat
        {
            get
            {
                if (MultipartFormDataItem == null) throw new ContentDecodingException("MultipartFormDataItem must not be null");
                if (!CanDecode) return null;
                if (MultipartFormDataItem.ContentType.ToLower().Contains("png")) return ImageFormat.Png;
                if (MultipartFormDataItem.ContentType.ToLower().Contains("bmp")) return ImageFormat.Bmp;
                if (MultipartFormDataItem.ContentType.ToLower().Contains("emf")) return ImageFormat.Emf;
                if (MultipartFormDataItem.ContentType.ToLower().Contains("exif")) return ImageFormat.Exif;
                if (MultipartFormDataItem.ContentType.ToLower().Contains("gif")) return ImageFormat.Gif;
                if (MultipartFormDataItem.ContentType.ToLower().ContainsAny("vnd.microsoft.icon", "x-icon"))
                    return ImageFormat.Icon;
                if (MultipartFormDataItem.ContentType.ToLower().Contains("png")) return ImageFormat.Jpeg;
                if (MultipartFormDataItem.ContentType.ToLower().Contains("jpeg")) return ImageFormat.Tiff;
                return null;
            }
        }

        public Image Decode()
        {
            if (MultipartFormDataItem == null) throw new ContentDecodingException("MultipartFormDataItem must not be null");
            if (!CanDecode) throw new ContentDecodingException("Could not decode content");
            byte[] bytes = MultipartFormDataItem.Content;
            if (!string.IsNullOrEmpty(MultipartFormDataItem.ContentTransferEncoding))
            {
                var decoder =
                    ContentTransferDecoderFactory.FindAndCreateInstance<byte>(MultipartFormDataItem);
                if (decoder == null) throw new ContentDecodingException("Could not decode content");
                bytes = decoder.Decode(MultipartFormDataItem).ToArray();
            }
            return Image.FromStream(new MemoryStream(bytes));
        }
    }
}
