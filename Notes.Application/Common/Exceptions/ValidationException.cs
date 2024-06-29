namespace Notes.Application.Common.Exceptions
{
    public class ValidationException : ApplicationException
    {
        public ValidationException(string message = "") : base(message) { }
    }
}
