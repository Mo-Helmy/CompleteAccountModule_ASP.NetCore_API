using CompleteAccountModule.Application.Dtos.Authentication;
using CompleteAccountModule.Application.Dtos.Authentication.ConfirmPhone;
using CompleteAccountModule.Application.Dtos.Authentication.ForgetPassword;

namespace CompleteAccountModule.Application.Services.Contract
{
    public interface IAccountService
    {
        Task<AuthResponseModel> LoginAsync(LoginDto loginDto);
        Task RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseModel> RegisterAdminAsync(RegisterAdminDto model);

        Task ConfirmMail(string email, string token);
        Task ForgetPassword(ForgetPasswordDto dto);
        Task ConfirmPhone(ConfirmPhoneDto dto);

        Task<IEnumerable<ResponseUserDetailsDto>> GetAllUsersAsync();
        Task<ResponseUserDetailsDto> GetUserByIdAsync(string id);
        Task<ResponseUserDetailsDto> UpdateUserAsync(UpdateUserDetailsDto Dto);
        Task DeleteUserAsync(string id);

        //ChangePassword
        Task<bool> ChangePassword(string userId, string password);
    }
}
