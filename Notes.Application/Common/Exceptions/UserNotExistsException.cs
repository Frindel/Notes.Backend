namespace Notes.Application.Common.Exceptions
{
    public class UserNotExistsException : ApplicationException
    {
        public UserNotExistsException(string message) : base(message)
        {

        }
    }
}
