﻿using FluentValidation;

namespace Notes.Application.Users.Commands.UpdateTokens
{
    public class UpdateTokensCommandValidator : AbstractValidator<UpdateTokensCommand>
    {
        public UpdateTokensCommandValidator()
        {
            RuleFor(c => c.UserId).Must(ui => ui > 0).WithMessage("user id < 0");
            RuleFor(c => c.RefreshToken).NotNull().WithMessage("refresh-token is null");
        }
    }
}
