using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipartFormParser.Exceptions
{
    public class UnknownEncodingException : Exception
    {
        public UnknownEncodingException()
        {
            
        }

        public UnknownEncodingException(string charsetName) : base("Encoding not found for charset " + charsetName)
        {
            
        }
    }
}
