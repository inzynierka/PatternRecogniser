using System;

namespace PatternRecogniser.Errors
{
    public class NotFoundExeption:Exception
    {
        public NotFoundExeption(string message): base(message) { }
    }
    public class UnauthorizedExeption : Exception
    {
        public UnauthorizedExeption(string message) : base(message) { }
    }

    public class BadRequestExeption : Exception
    {
        public BadRequestExeption(string message) : base(message) { }
    }
}
