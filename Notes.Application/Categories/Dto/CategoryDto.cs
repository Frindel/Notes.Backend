using System.Drawing;
using AutoMapper;
using Notes.Application.Common.Mapping;
using Notes.Domain;

namespace Notes.Application.Categories.Dto
{
    public class CategoryDto : MappingBase<Category>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public string Color { get; set; } = null!;

        public override void Mapping(Profile profile)
        {
            profile.CreateMap<Category, CategoryDto>()
                .ForMember(categoryDto => categoryDto.Id,
                    opt => opt.MapFrom(category => category.PersonalId))
                .ForMember(categoryDto => categoryDto.Name,
                    opt => opt.MapFrom(category => category.Name))
                .ForMember(categoryDto => categoryDto.Color,
                    opt => opt.MapFrom(category => ColorTranslator.ToHtml(category.Color)));
        }
    }
}