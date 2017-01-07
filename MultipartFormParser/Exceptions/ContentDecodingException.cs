using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipartFormParser.Exceptions
{
    public class ContentDecodingException : Exception
    {
        public ContentDecodingException(string message) : base(message)
        {

        }
    }
}
