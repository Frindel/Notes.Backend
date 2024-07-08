using MediatR;
using Notes.Application.Users.Dto;

namespace Notes.Application.Users.Commands.UpdateTokens
{
    public record class UpdateTokensCommand : IRequest<TokensDto>
    {
        public string RefreshToken { get; set; } = null!;
    }
}
