using FluentValidation;
using System.Net;
using System.Text.Json;
using Notes.Application.Common.Exceptions;

namespace Notes.WebApi.Middleware
{
    public class ExceptionsHandlerMiddleware
    {
        RequestDelegate _next;

        public ExceptionsHandlerMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception exc)
            {
                await HandleException(context, exc);
            }
        }

        private Task HandleException(HttpContext context, Exception exc)
        {
            var error = ParseException(exc);
            Task response = CreateResponse(context.Response, error.code, error.message);
            return response;
        }

        (HttpStatusCode code, string message) ParseException(Exception exc)
        {
            HttpStatusCode code = HttpStatusCode.InternalServerError;
            switch (exc)
            {
                case ValidationException:
                    code = HttpStatusCode.BadRequest;
                    break;
                case NotFoundException:
                    code = HttpStatusCode.NotFound;
                    break;
                case InvalidLoginOrPasswordException:
                    code = HttpStatusCode.Forbidden;
                    break;
                case UserIsAlreadyRegisteredException:
                    code = HttpStatusCode.Conflict;
                    break;
            }

            string message = JsonSerializer.Serialize(new { error = exc.Message });
            return (code, message);
        }

        Task CreateResponse(HttpResponse response, HttpStatusCode code, string message)
        {
            response.StatusCode = (int)code;
            response.ContentType = "application/json";
            return response.WriteAsync(message);
        }
    }
}