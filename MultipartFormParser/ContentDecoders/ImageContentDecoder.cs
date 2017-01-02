using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultipartFormParser.Helpers;

namespace MultipartFormParser.ContentDecoders
{
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
            if (!CanDecode) throw new Exception();
            byte[] bytes = MultipartFormDataItem.Content;
            if (!string.IsNullOrEmpty(MultipartFormDataItem.ContentTransferEncoding))
            {
                var decoder =
                    ContentTransferDecoderFactory.FindAndCreateInstance<byte>(
                        MultipartFormDataItem.ContentTransferEncoding);
                if (decoder == null) throw new Exception();
                bytes = decoder.Decode(MultipartFormDataItem).ToArray();
            }
            return Image.FromStream(new MemoryStream(bytes));
        }
    }
}
