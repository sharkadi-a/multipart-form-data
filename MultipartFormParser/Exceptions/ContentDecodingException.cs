using System;

namespace MultipartFormParser.Exceptions
{
    /// <summary>
    /// This exception raises when content or content transfer decoder cannot process multi/part-form data
    /// </summary>
    public class ContentDecodingException : Exception
    {
        public ContentDecodingException(string message) : base(message)
        {

        }
    }
}
