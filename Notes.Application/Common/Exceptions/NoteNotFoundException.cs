namespace Notes.Application.Common.Exceptions
{
    public class NoteNotFoundException : ApplicationException
    {
        public NoteNotFoundException(string description) : base(description)
        {

        }
    }
}
