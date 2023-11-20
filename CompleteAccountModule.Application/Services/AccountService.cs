using AutoMapper;
using CompleteAccountModule.Application.Dtos.Authentication;
using CompleteAccountModule.Application.Dtos.Authentication.ConfirmPhone;
using CompleteAccountModule.Application.Dtos.Authentication.ForgetPassword;
using CompleteAccountModule.Application.Dtos.MailDtos;
using CompleteAccountModule.Application.Services.Contract;
using CompleteAccountModule.Domain.Entities.Identity;
using CompleteAccountModule.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace CompleteAccountModule.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IMailService _mailService;

        public AccountService(
            AppDbContext db,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            IMapper mapper,
            IMailService mailService
            )
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _mapper = mapper;
            this._mailService = mailService;
        }

        public async Task<AuthResponseModel> LoginAsync(LoginDto model)
        {

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
                throw new ValidationException("Email or Password is incorrect!");

            if(!await _userManager.IsEmailConfirmedAsync(user))
                throw new ValidationException("Email is not confirmed!");

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            var authModel = new AuthResponseModel();
            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.Roles = rolesList.ToList();

            return authModel;

        }
        public async Task RegisterAsync(RegisterDto model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                throw new ValidationException("Email is already registered!");

            if (await _userManager.FindByNameAsync(model.UserName) is not null)
                throw new ValidationException("Username is already registered!");

            var user = new AppUser
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = new StringBuilder();
                foreach (var error in result.Errors)
                    errors.Append($"{error.Description}, ");

                throw new ValidationException(errors.ToString());
            }

            await _userManager.AddToRoleAsync(user, "User");
            //var rolesList = await _userManager.GetRolesAsync(user);

            //var jwtSecurityToken = await CreateJwtToken(user);

            //return new AuthResponseModel
            //{
            //    IsAuthenticated = true,
            //    Email = user.Email,
            //    Username = user.UserName,
            //    Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            //    Roles = rolesList.ToList(),
            //    ExpiresOn = jwtSecurityToken.ValidTo
            //};
        }

        #region  Confirm Email

        public async Task ConfirmMail(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email)
                ?? throw new ValidationException("Email Not Found!");

            if (await _userManager.IsEmailConfirmedAsync(user))
                throw new ValidationException("Email is confirmed!");

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                var errors = new StringBuilder();
                foreach (var error in result.Errors)
                    errors.Append($"{error.Description}, ");

                throw new ValidationException(errors.ToString());
            }
        }
        #endregion

        #region  Forget Password
        public async Task ForgetPassword(ForgetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email)
                ?? throw new ValidationException("Email Not Found!");

            if (!await _userManager.IsEmailConfirmedAsync(user))
                throw new ValidationException("Email is not confirmed!");

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);

            if (!result.Succeeded)
            {
                var errors = new StringBuilder();
                foreach (var error in result.Errors)
                    errors.Append($"{error.Description}, ");

                throw new ValidationException(errors.ToString());
            }
        }
        #endregion

        #region  Confirm Phone
        public async Task ConfirmPhone(ConfirmPhoneDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email)
                ?? throw new ValidationException("Email Not Found!");

            if (!await _userManager.IsEmailConfirmedAsync(user))
                throw new ValidationException("Email is not confirmed!");

            var result = await _userManager.ChangePhoneNumberAsync(user, dto.PhoneNumber, dto.Token);

            if (!result.Succeeded)
            {
                var errors = new StringBuilder();
                foreach (var error in result.Errors)
                    errors.Append($"{error.Description}, ");

                throw new ValidationException(errors.ToString());
            }
        }
        #endregion

        #region Commented

        public async Task<AuthResponseModel> RegisterAdminAsync(RegisterAdminDto model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthResponseModel { Message = "Email is already registered!" };

            if (await _userManager.FindByNameAsync(model.UserName) is not null)
                return new AuthResponseModel { Message = "Username is already registered!" };

            var user = new AppUser
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = new StringBuilder();
                foreach (var error in result.Errors)
                    errors.Append($"{error.Description},");

                return new AuthResponseModel { Message = errors.ToString() };
            }

            await _userManager.AddToRoleAsync(user, "ADMIN");

            var jwtSecurityToken = await CreateJwtToken(user);

            return new AuthResponseModel
            {
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.UserName
            };
        }

        private async Task<JwtSecurityToken> CreateJwtToken(AppUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            var roleClaims = userRoles.Select(r => new Claim("roles", r)).ToList();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddDays(int.Parse(_configuration["JWT:DurationInDays"]!)),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }



        public async Task DeleteUserAsync(string id)
        {
            var result = await _db.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (result != null) { _db.Users.Remove(result); }
        }

        public async Task<IEnumerable<ResponseUserDetailsDto>> GetAllUsersAsync()
        {
            var users = await _db.Users.ToListAsync();

            var usersResponseDto = _mapper.Map<IEnumerable<ResponseUserDetailsDto>>(users);

            return usersResponseDto;
        }

        public async Task<ResponseUserDetailsDto?> GetUserByIdAsync(string id)
        {
            var user = await _db.Users.Where(x => x.Id == id).FirstOrDefaultAsync();

            var userResponseDto = _mapper.Map<ResponseUserDetailsDto>(user);

            return userResponseDto;
        }

        public async Task<ResponseUserDetailsDto> UpdateUserAsync(UpdateUserDetailsDto Dto)
        {
            var existingUser = await _db.Users.FirstOrDefaultAsync(x => x.Id == Dto.Id);

            if (existingUser is null) throw new KeyNotFoundException("User Id Not Found");

            existingUser.PhoneNumber = Dto.PhoneNumber;

            await _db.SaveChangesAsync();

            var responseUserDto = _mapper.Map<ResponseUserDetailsDto>(existingUser);

            return responseUserDto;
        }

        public Task<bool> ChangePassword(string userId, string password)
        {
            throw new NotImplementedException();
        } 
        #endregion
    }
}
