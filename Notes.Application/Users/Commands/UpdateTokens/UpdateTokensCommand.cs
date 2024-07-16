using MediatR;
using Notes.Application.Users.Dto;

namespace Notes.Application.Users.Commands.UpdateTokens
{
    public record class UpdateTokensCommand : IRequest<TokensDto>
    {
        public int UserId { get; set; }

        public string RefreshToken { get; set; } = null!;
    }
}
