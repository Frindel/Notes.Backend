using AutoMapper;
using Notes.Application.Common.Mapping;
using Notes.Domain;

namespace Notes.Application.Users.Dto
{
    public class UserDto : MappingBase<User>
    {
        public string Login { get; set; } = null!;

        public string AccessToken { get; set; } = null!;

        public string RefreshToken { get; set; } = null!;

        public override void Mapping(Profile profile)
        {
            profile.CreateMap<User, UserDto>()
                .ForMember(userDto => userDto.Login,
                opt => opt.MapFrom(user => user.Login)) // маппинг лонига
                .ForMember(userDto => userDto.RefreshToken,
                opt => opt.MapFrom(user => user.RefreshToken)); // маппинг refresh-токена
        }
    }
}
