using System;

namespace StudentLibrary.BLL.Exceptions
{
    public class LimitExceededException : Exception
    {
        public LimitExceededException(string message) : base(message) { }
    }
}
