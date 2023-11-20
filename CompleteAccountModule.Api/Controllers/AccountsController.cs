using CompleteAccountModule.Application.Dtos.Authentication;
using CompleteAccountModule.Application.Dtos.Authentication.ConfirmEmail;
using CompleteAccountModule.Application.Dtos.Authentication.ConfirmPhone;
using CompleteAccountModule.Application.Dtos.Authentication.ForgetPassword;
using CompleteAccountModule.Application.Dtos.MailDtos;
using CompleteAccountModule.Application.Dtos.SMSDtos;
using CompleteAccountModule.Application.Services.Contract;
using CompleteAccountModule.Domain.Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CompleteAccountModule.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMailService _mailService;
        private readonly ISMSService _smsService;

        public AccountsController(IAccountService accountService, UserManager<AppUser> userManager, IConfiguration configuration, IMailService mailService, ISMSService smsService)
        {
            this._accountService = accountService;
            this._userManager = userManager;
            this._configuration = configuration;
            this._mailService = mailService;
            this._smsService = smsService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseModel>> Login(LoginDto loginDto)
        {
            return await _accountService.LoginAsync(loginDto);
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {
            await _accountService.RegisterAsync(registerDto);
            return await SendConfirmMail(new SendConfirmEmailDto { Email = registerDto.Email });
        }

        #region Confirm Email

        [HttpPost("confirmMail")]
        public async Task<ActionResult> SendConfirmMail([FromBody] SendConfirmEmailDto confirmEmailDto)
        {
            var user = await _userManager.FindByEmailAsync(confirmEmailDto.Email)
                ?? throw new ValidationException("Email Not Found!");

            if (await _userManager.IsEmailConfirmedAsync(user))
                throw new ValidationException("Email is confirmed!");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var confirmationLink = Url.Action("ConfirmEmail", "Accounts", new { email = confirmEmailDto.Email, token = token }, Request.Scheme);

            _mailService.SendMail(new MailRequest() { ToEmail = confirmEmailDto.Email, Subject = "Confirm Email", Body = confirmationLink });

            return Ok("Confirmation email has been sent successfully");
        }

        [HttpGet("confirmMail")]
        public async Task<ActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
        {
            await _accountService.ConfirmMail(email, token);

            return Ok("Your email is confirmed, you can login now");
        }
        #endregion

        #region Forget Password

        [HttpPost("sendForgetPasswordMail")]
        public async Task<ActionResult> SendForgetPasswordMail([FromBody] SendForgetPasswordEmailDto emailDto)
        {
            var user = await _userManager.FindByEmailAsync(emailDto.Email)
                ?? throw new ValidationException("Email Not Found!");

            if (!await _userManager.IsEmailConfirmedAsync(user))
                throw new ValidationException("Email is not confirmed!");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var confirmationLink = Url.Action("ForgetPassword", "Accounts", new { email = emailDto.Email, token = token }, Request.Scheme);

            _mailService.SendMail(new MailRequest() { ToEmail = emailDto.Email, Subject = "Forget Password", Body = confirmationLink });

            return Ok("Forget Password email has been sent successfully");
        }

        [HttpPost("forgetPassword")]
        public async Task<ActionResult> ForgetPassword(ForgetPasswordDto forgetPasswordDto)
        {
            await _accountService.ForgetPassword(forgetPasswordDto);

            return Ok("Your password is rested, you can login now with new password");
        }
        #endregion

        #region Confirm Phone Number

        [HttpPost("sendConfirmPhoneOtp")]
        public async Task<ActionResult> SendConfirmPhoneOtp([FromBody] SendConfirmPhoneDto phoneDto)
        {
            var user = await _userManager.FindByEmailAsync(phoneDto.Email)
                ?? throw new ValidationException("Email Not Found!");

            if (!await _userManager.IsEmailConfirmedAsync(user))
                throw new ValidationException("Email is not confirmed!");

            var token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneDto.PhoneNumber);

            _smsService.Send(new SMSDto() { PhoneNumber = phoneDto.PhoneNumber, Body = $"Your verification OTP is: {token}" });

            return Ok("verification OTP SMS has been sent successfully");
        }

        [HttpPost("confirmPhone")]
        public async Task<ActionResult> ConfirmPhone(ConfirmPhoneDto confirmPhoneDto)
        {
            await _accountService.ConfirmPhone(confirmPhoneDto);

            return Ok("Your phone number has been confirmed");
        }
        #endregion


    }
}
