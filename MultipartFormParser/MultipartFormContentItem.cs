using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipartFormParser
{
    public class MultipartFormContentItem
    {
        public string Name { get; internal set; }
        public string Filename { get; internal set; }
        public string ContentType { get; internal set; }
        public byte[] Content { get; internal set; }
    }
}
