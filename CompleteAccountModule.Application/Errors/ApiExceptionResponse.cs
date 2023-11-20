using CompleteAccountModule.Application.Errors;

namespace ExamSystem.Application.Errors
{
    public class ApiExceptionResponse : ApiErrorResponse
    {
        public string? Details { get; }

        public ApiExceptionResponse(int statusCode, string? errorMessage = null, string? details = null)
            : base(statusCode, errorMessage)
        {
            Details = details;
        }

    }
}
