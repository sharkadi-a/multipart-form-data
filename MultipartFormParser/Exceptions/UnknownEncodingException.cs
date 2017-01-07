using System;

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
