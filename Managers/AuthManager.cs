using Interfaces.IManager;
using Models.DTOs;
using Microsoft.AspNetCore.Identity;
using  DataAccess.Entities;
using Models.Providers;
namespace Managers
{
    public class AuthManager: IAuthManager
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly TokenProvider _tokenProvider;
        public AuthManager(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, TokenProvider tokenProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenProvider = tokenProvider;
        }
        public async Task<Result<AuthResponse>> RegisterAsync(Register register)
        {
            try
            {
                var userExists=await _userManager.FindByEmailAsync(register.Email);
                if (userExists != null)
                {
                    return new Result<AuthResponse>
                    {
                        Success = false,
                        Message= "User already exists!"
                    };
                }
                var user=new AppUser
                {
                    UserName=register.Email,
                    Email=register.Email,
                    FullName=register.FullName,

                };
                var res=await _userManager.CreateAsync(user,register.Password);
                if(!res.Succeeded)
                {
                    return new Result<AuthResponse>
                    {
                        Success = false,
                        Message= "User creation failed! Please check user details and try again."
                    };
                }
                // Assign Roles
                if(!await _roleManager.RoleExistsAsync(register.Roles))
                {
                    await _roleManager.CreateAsync(new IdentityRole(register.Roles));
                }
                await _userManager.AddToRoleAsync(user, register.Roles);
                // Generate Token (Assuming a method GenerateToken exists)
                var token=_tokenProvider.GetAccessToken(new TokenData
                {
                    UserId=user.Id,
                    Email=user.Email,
                    Roles=await _userManager.GetRolesAsync(user)
                });
                return new Result<AuthResponse>
                {
                    Success = true,
                    Data=new AuthResponse
                    {
                        Token=token,
                        UserId=user.Id,
                        Email=user.Email
                    }
                };
                
            }catch(Exception ex)
            {
                return new Result<AuthResponse>
                {
                    Success = false,
                    Message= ex.Message
                };
            }
        }
        public async Task<Result<AuthResponse>> LoginAsync(Login login)
        {
            try
            {
                var user=await _userManager.FindByEmailAsync(login.Email);
                if(user==null)
                {
                    return new Result<AuthResponse>
                    {
                        Success = false,
                        Message= "User does not exist!"
                    };
                }
                var res=await _signInManager.PasswordSignInAsync(user,login.Password,false,false);
                if(!res.Succeeded)
                {
                    return new Result<AuthResponse>
                    {
                        Success = false,
                        Message= "Invalid credentials!"
                    };
                }
                var token=_tokenProvider.GetAccessToken(new TokenData
                {
                    UserId=user.Id,
                    Email=user.Email!,
                    Roles=await _userManager.GetRolesAsync(user)
                });
                return new Result<AuthResponse>
                {
                    Success = true,
                    Data=new AuthResponse
                    {
                        Token=token,
                        UserId=user.Id,
                        Email=user.Email!
                    }
                };
            }catch(Exception ex)
            {
                return new Result<AuthResponse>
                {
                    Success = false,
                    Message= ex.Message
                };
            }
        }
    }
}