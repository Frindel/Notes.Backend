namespace Notes.Application.Common.Exceptions
{
    public class InvalidLoginOrPasswordException : ApplicationException
    {
        public InvalidLoginOrPasswordException(string message) : base(message)
        { }
    }
}
