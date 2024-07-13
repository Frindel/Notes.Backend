using Notes.Application.Common.Exceptions;
using Notes.Application.Notes.Commands.DeleteNote;
using Notes.ApplicationTests.Common;
using Notes.Domain;
using Notes.Persistence.Data;

namespace Notes.ApplicationTests.Notes
{
	[TestFixture]
	internal class DeleteNoteTests : TestsBase
	{
		[Test]
		public void SuccessfulDeleteUser()
		{
			// Arrange
			DataContext context = ContextManager.CreateEmptyDataContex();
			User savedUser = Helper.AddUserWithNumbers(context, 1).First();
			List<Category> savedCategories = Helper.AddCategoriesWithNumbers(context, savedUser, 1, 2);
			Note savedNote = Helper.AddNotesWithNumbers(context, savedUser, savedCategories, 1).First();

			var heandler = CreateHeandler(context);
			var command = CreateCommand(savedUser, savedNote);

			// Act / Accert
			Assert.IsTrue(heandler.Handle(command, CancellationToken.None).IsCompletedSuccessfully);
		}

		[Test]
		public void UserIsNotFoundException()
		{
			// Arrange
			DataContext context = ContextManager.CreateEmptyDataContex();
			User notSavedUser = Helper.CreateUserOfNumber(1);
			Note notSavedNote = Helper.CreateNoteOfNumber(1, notSavedUser);

			var heandler = CreateHeandler(context);
			var command = CreateCommand(notSavedUser, notSavedNote);

			// Act / Accert
			Assert.ThrowsAsync<UserNotFoundException>(() => heandler.Handle(command, CancellationToken.None));
		}

        [Test]
        public void NoteIsNotFoundException()
        {
            // Arrange
            DataContext context = ContextManager.CreateEmptyDataContex();
			User savedUser = Helper.AddUserWithNumbers(context, 1).First();
            Note notSavedNote = Helper.CreateNoteOfNumber(1, savedUser);

            var heandler = CreateHeandler(context);
            var command = CreateCommand(savedUser, notSavedNote);

            // Act / Accert
            Assert.ThrowsAsync<NoteNotFoundException>(() => heandler.Handle(command, CancellationToken.None));
        }

        DeleteNoteCommandHeandler CreateHeandler(DataContext context)
		{
			var heandler = new DeleteNoteCommandHeandler(context, context, Mapper);
			return heandler;
		}

		DeleteNoteCommand CreateCommand(User user, Note note)
		{
			var command = new DeleteNoteCommand()
			{
				NoteId = note.PersonalId,
				UserId = user.Id
			};
			return command;
		}
	}
}
