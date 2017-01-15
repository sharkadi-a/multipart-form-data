using System;

namespace MultipartFormParser.Exceptions
{
    /// <summary>
    /// This exception raises when <see cref="MultipartFormParser"/> cannot parase raw request data
    /// </summary>
    public class MultiPartFormParsingException : Exception
    {
        internal MultiPartFormParsingException(string message) : base(message)
        {
            
        }
    }
}
