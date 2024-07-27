using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notes.Application.Notes.Commands.CreateNote;
using Notes.Application.Notes.Commands.DeleteNote;
using Notes.Application.Notes.Commands.EditNote;
using Notes.Application.Notes.Queries.GetAllNotes;
using Notes.Application.Notes.Queries.GetNote;
using Notes.WebApi.Models.Notes;

namespace Notes.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/notes")]
    public class NotesController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var query = new GetAllNotesQuery()
            {
                UserId = CurrentUserId,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var notes = await Mediator.Send(query);
            return Ok(notes.Notes);
        }

        [HttpGet("{noteId}")]
        public async Task<IActionResult> GetById([FromRoute] int noteId)
        {
            var query = new GetNoteQuery()
            {
                UserId = CurrentUserId,
                NoteId = noteId
            };
            var note = await Mediator.Send(query);
            return Ok(note);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateNoteRequest request)
        {
            var command = new CreateNoteCommand()
            {
                UserId = CurrentUserId,
                Name = request.Name,
                Description = request.Description,
                Time = request.Time,
                CategoriesIds = request.CategoriesIds ?? []
            };
            var createdNote = await Mediator.Send(command);
            return Ok(createdNote);
        }

        [HttpPut]
        public async Task<IActionResult> Edit([FromBody] EditNoteRequest request)
        {
            var command = new EditNoteCommand()
            {
                UserId = CurrentUserId,
                NoteId = request.Id,
                Name = request.Name,
                Description = request.Description,
                Time = request.Time,
                CategoriesIds = request.CategoriesIds ?? []
            };
            var changedNote = await Mediator.Send(command);
            return Ok(changedNote);
        }

        [HttpDelete("{noteId}")]
        public async Task<IActionResult> Delete([FromRoute] int noteId)
        {
            var command = new DeleteNoteCommand()
            {
                UserId = CurrentUserId,
                NoteId = noteId
            };
            await Mediator.Send(command);
            return NoContent();
        }
    }
}