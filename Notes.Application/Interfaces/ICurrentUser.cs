using Notes.Domain;

namespace Notes.Application.Interfaces
{
    public interface ICurrentUser
    {
        User CurrentUser { get; }
    }
}
