using System;

namespace MultipartFormParser.Exceptions
{
    public class ContentDecodingException : Exception
    {
        public ContentDecodingException(string message) : base(message)
        {

        }
    }
}
