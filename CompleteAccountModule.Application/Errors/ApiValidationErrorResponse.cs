using CompleteAccountModule.Application.Errors;

namespace ExamSystem.Application.Errors
{
    public class ApiValidationErrorResponse : ApiErrorResponse
    {
        public List<string> Errors { get; set; } = new List<string>();
        public ApiValidationErrorResponse() : base(400)
        {
        }
    }
}
