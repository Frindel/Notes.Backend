using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Notes.WebApi.Controllers
{
    public class BaseController : ControllerBase
    {
        private IMediator _mediator = null!;

        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>()!;

        protected int CurrentUserId { get => int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value); }
    }
}