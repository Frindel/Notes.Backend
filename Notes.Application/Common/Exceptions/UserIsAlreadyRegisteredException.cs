namespace Notes.Application.Common.Exceptions
{
    public class UserIsAlreadyRegisteredException : ApplicationException
    {
        public UserIsAlreadyRegisteredException(string message) : base(message)
        { }
    }
}
