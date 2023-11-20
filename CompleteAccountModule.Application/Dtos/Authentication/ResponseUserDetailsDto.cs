namespace CompleteAccountModule.Application.Dtos.Authentication
{
    public class ResponseUserDetailsDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
    }
}
