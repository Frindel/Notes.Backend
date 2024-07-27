using AutoMapper;
using Notes.Application.Common.Mapping;
using Notes.Domain;

namespace Notes.Application.Notes.Dto
{
    public class NoteDto : MappingBase<Note>
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public DateTime Time { get; set; }

        public bool IsCompleted { get; set; }

        public List<CategoryDto> Categories { get; set; } = null!;

        public override void Mapping(Profile profile)
        {
            profile.CreateMap<Note, NoteDto>()
                .ForMember(noteDto => noteDto.Id,
                opt => opt.MapFrom(note => note.PersonalId))
                .ForMember(noteDto => noteDto.Name,
                opt => opt.MapFrom(note => note.Name))
                .ForMember(noteDto => noteDto.Description,
                opt => opt.MapFrom(note => note.Description))
                .ForMember(noteDto => noteDto.Time,
                opt => opt.MapFrom(note => note.Time))
                .ForMember(noteDto => noteDto.IsCompleted,
                opt => opt.MapFrom(note => note.IsCompleted))
                .ForMember(noteDto => noteDto.Categories,
                opt => opt.MapFrom(note => note.Categories.Select(c => new CategoryDto()
                {
                    Id = c.PersonalId,
                    Name = c.Name,
                })
                ));

        }
    }
}
