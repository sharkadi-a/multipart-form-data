using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultipartFormParser.Exceptions;

namespace MultipartFormParser
{
    public static class MultipartFormDataHelper
    {
        public static Encoding GetEncoding(MultipartFormDataItem item)
        {
            try
            {
                return Encoding.GetEncoding(item.Charset);               
            }
            catch (ArgumentException)
            {
                throw new EncodingNotFoundException("Encoding for charset \"" + item.Charset + "\" not found");
            }
        }
    }
}
