namespace Notes.Application.Common.Exceptions
{
    public class CategoryNotFoundException : ApplicationException
    {
        public CategoryNotFoundException(string description) : base(description) { }
    }
}
