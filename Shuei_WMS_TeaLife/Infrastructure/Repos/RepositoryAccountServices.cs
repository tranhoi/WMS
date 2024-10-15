using Application.DTOs.Request.Account;
using Application.DTOs.Response;
using Application.DTOs.Response.Account;
using Application.Extentions;
using Application.Services.Authen;
using Azure;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Domain.Entity.Commons;
using Domain.Entity.WMS.Authentication;
using Infrastructure.Data;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestEase;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static Application.Extentions.ConstantExtention;

using Magicodes.ExporterAndImporter.Pdf;
using Microsoft.AspNetCore.Http;
using Application.DTOs;
using QRCoder.Core;
using DocumentFormat.OpenXml.Office2010.Excel;
using Application.Models;
using Microsoft.Data.SqlClient;
using Dapper;
using Domain.Enums;
using Application;

namespace Infrastructure.Repos
{
    public class RepositoryAccountServices(RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager, IConfiguration config,
        SignInManager<ApplicationUser> signInManager,
        ApplicationDbContext dbContext) : IAccount
    {
        #region Methods
        private async Task<ApplicationUser> FindUserByEmailAsync(string email)
            => await userManager.FindByEmailAsync(email);
        private async Task<ApplicationUser> FindUserByNameAsync(string name)
            => await userManager.FindByNameAsync(name);
        private async Task<ApplicationUser> FindUserByIdAsync(string id)
            => await userManager.FindByIdAsync(id);
        private async Task<IdentityRole> FindRoleByNameAsync(string roleName)
            => await roleManager.FindByNameAsync(roleName);

        private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        private async Task<GeneralResponse> AssignUserToRole(ApplicationUser user, List<CreateRoleRequestDTO> roles)
        {
            string roleString = string.Empty;

            if (user == null || roles is null)
                return new GeneralResponse()
                {
                    Flag = false,
                    Message = "Model state cannot be empty"
                };

            IdentityResult result = null;

            foreach (var role in roles)
            {
                var r = await FindRoleByNameAsync(role.Name);
                if (r == null) await CreateRoleAsync(new CreateRoleRequestDTO() { Name = r.Name });

                var utr = await userManager.GetRolesAsync(user);
                var fistRole = utr.FirstOrDefault(x => x == role.Name);

                if (fistRole == null)
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                    roleString = $"{roleString};{role.Name}";
                }
            }

            string error = CheckReponse(result);
            if (!string.IsNullOrEmpty(error))
                return new GeneralResponse()
                {
                    Flag = false,
                    Message = error
                };
            else
                return new GeneralResponse()
                {
                    Flag = true,
                    Message = $"{user.FullName} assigned to {roleString} role"
                };
        }

        private static string CheckReponse(IdentityResult result)
        {
            if (!result.Succeeded)
            {
                var title = result.Errors;
                var error = result.Errors.Select(_ => _.Description);
                return string.Join(Environment.NewLine, error);
            }

            return null;
        }

        private async Task<JwtSecurityToken> GenerateToken(ApplicationUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var expiryToken = DateTime.Now.AddSeconds(double.TryParse(config["Jwt:JwtExpiryTimeToken"], out double value) ? value : 10);

            #region add claim
            //user info
            var userClaims = new List<Claim>()
               {
                    new Claim(ClaimTypes.Name,user.UserName),
                    new Claim(ClaimTypes.Email,user.Email),
                    new Claim("FullName",user.FullName),
                    new Claim("UserId",user.Id),
                };
            //roles
            var roles = await userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x));
            userClaims.AddRange(roleClaims);
            //permission
            foreach (var roleItem in roles)
            {
                var roleToPer = await dbContext.RoleToPermissions.Where(x => x.RoleName == roleItem).ToListAsync();

                userClaims.Add(new Claim("RoleToPermission", JsonConvert.SerializeObject(roleToPer)));
            }
            #endregion

            var token = new JwtSecurityToken(
                issuer: config["JWT:Issuer"],
                audience: config["JWT:Audience"],
                expires: expiryToken,
                claims: userClaims,
                signingCredentials: credentials
                );

            return token;
        }

        public AuthenticationProperties GetAuthenticationProperties(bool isPersistent)
        {
            return new AuthenticationProperties()
            {
                IsPresistent = isPersistent,
                ExpriesUtc = DateTime.UtcNow.AddMinutes(10),
                //RedirectUrl = 
            };
        }
        private async Task<GeneralResponse> SaveRefreshTokenAsync(string userId, string token, string refreshToken, DateTime expiration, string refreshTokenCurrent = null)
        {
            try
            {
                var res = await dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.RefreshToken == refreshTokenCurrent);
                if (res is not null)
                {
                    res.Activated = false;
                    //dbContext.RefreshTokens.Update(res);
                }

                await dbContext.RefreshTokens.AddAsync(new RefreshTokens() { UserId = userId, Token = token, RefreshToken = refreshToken, ExpirationTime = expiration, Activated = true });

                await dbContext.SaveChangesAsync();
                return new GeneralResponse()
                {
                    Flag = true,
                    Message = "Save refresh token successfull"
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse()
                {
                    Flag = false,
                    Message = $"{ex.Message}{Environment.NewLine}{ex.InnerException}"
                };
            }
        }
        #endregion

        public async Task<GeneralResponse> ChangeUserRoleAsync(AssignUserRoleRequestDTO model)
        {
            try
            {
                var user = await FindUserByNameAsync(model.UserName);

                if (user == null)
                    return new GeneralResponse()
                    {
                        Flag = false,
                        Message = "User not found"
                    };

                if (await FindRoleByNameAsync(model.Roles.FirstOrDefault().Name) is null)
                    return new GeneralResponse()
                    {
                        Flag = false,
                        Message = "Role not found"
                    };

                var previousRole = (await userManager.GetRolesAsync(user)).FirstOrDefault();
                var removeOldRole = await userManager.RemoveFromRoleAsync(user, previousRole);

                var error = CheckReponse(removeOldRole);
                if (!string.IsNullOrEmpty(error))
                    return new GeneralResponse()
                    {
                        Flag = false,
                        Message = error
                    };

                var result = await userManager.AddToRoleAsync(user, model.Roles.FirstOrDefault().Name);
                var response = CheckReponse(result);
                if (!string.IsNullOrEmpty(response))
                    return new GeneralResponse()
                    {
                        Flag = false,
                        Message = response
                    };
                else
                    return new GeneralResponse()
                    {
                        Flag = true,
                        Message = "Role changed"
                    };
            }
            catch (Exception ex)
            {
                return new GeneralResponse()
                {
                    Flag = false,
                    Message = $"{ex.Message}{Environment.NewLine}{ex.InnerException}"
                };
            }
        }

        public async Task<GeneralResponse> CreateAccountAsync(CreateAccountRequestDTO model)
        {
            try
            {
                if (await userManager.FindByNameAsync(model.UserName) != null)
                    return new GeneralResponse()
                    {
                        Flag = false,
                        Message = "Sorry, user is already created."
                    };

                var user = new ApplicationUser()
                {
                    FullName = model.FullName,
                    UserName = model.UserName,
                    Email = model.Email,
                    PasswordHash = model.Password,
                    Status = model.Status
                };

                var result = await userManager.CreateAsync(user, model.Password);

                string error = CheckReponse(result);
                if (!string.IsNullOrEmpty(error))
                    return new GeneralResponse()
                    {
                        Flag = false,
                        Message = error
                    };

                var res = await AssignUserToRole(user, model.Roles);
                return new GeneralResponse()
                {
                    Flag = res.Flag,
                    Message = res.Message
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse()
                {
                    Flag = false,
                    Message = $"{ex.Message}{Environment.NewLine}{ex.InnerException}"
                };
            }
        }

        public async Task<GeneralResponse> CreateSuperAdminAsync()
        {
            try
            {
                var roles = new List<CreateRoleRequestDTO>();
                roles.Add(new CreateRoleRequestDTO()
                {
                    Name = ConstantExtention.Roles.WarehouseAdmin
                });

                var supeAdmin = new CreateAccountRequestDTO()
                {
                    FullName = "Admin",
                    UserName = "Admin",
                    Password = "fbt_wms_admin@tealife.co.jp",
                    Email = "fbt_wms_admin@tealife.co.jp",
                    Roles = roles,
                    Status = EnumStatus.Activated
                };

                return await CreateAccountAsync(supeAdmin);
            }
            catch (Exception ex)
            {
                return new GeneralResponse() { Flag = false, Message = $"{ex.Message}{Environment.NewLine}{ex.InnerException}" };
            }
        }

        public async Task<GeneralResponse> CreateRoleAsync(CreateRoleRequestDTO model)
        {
            try
            {
                if (await FindRoleByNameAsync(model.Name) != null)
                    return new GeneralResponse()
                    {
                        Flag = false,
                        Message = $"{model.Name} already created."
                    };

                var respone = await roleManager.CreateAsync(new IdentityRole() { Name = model.Name });
                var error = CheckReponse(respone);
                if (!string.IsNullOrEmpty(error))
                    return new GeneralResponse()
                    {
                        Flag = false,
                        Message = error
                    };
                else
                    return new GeneralResponse()
                    {
                        Flag = true,
                        Message = $"{model.Name} created."
                    };
            }
            catch (Exception ex)
            {
                return new GeneralResponse()
                {
                    Flag = false,
                    Message = $"{ex.Message}{Environment.NewLine}{ex.InnerException}"
                };
            }
        }

        public async Task<List<GetRoleResponseDTO>> GetRolesAsync()
            => (await roleManager.Roles.ToListAsync()).Adapt<List<GetRoleResponseDTO>>();

        public async Task<List<GetUserWithRoleResponseDTO>> GetUsersWithRolesAsync()
        {
            try
            {
                var allUsers = await userManager.Users.ToListAsync();
                if (allUsers == null) return null;

                var list = new List<GetUserWithRoleResponseDTO>();
                foreach (var user in allUsers)
                {
                    var getUserRole = (await userManager.GetRolesAsync(user)).ToList();
                    var roles = new List<GetRoleResponseDTO>();

                    foreach (var roleName in getUserRole)
                    {
                        var getRoleInfo = await roleManager.Roles.FirstOrDefaultAsync(x => x.Name.ToLower() == roleName.ToLower());

                        roles.Add(new GetRoleResponseDTO()
                        {
                            Id = getRoleInfo.Id,
                            Name = getRoleInfo.Name,
                        });
                    }

                    //var getRoleInfo = await roleManager.Roles.FirstOrDefaultAsync(x => x.Name.ToLower() == getUserRole.ToLower());
                    list.Add(new GetUserWithRoleResponseDTO()
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        UserName = user.UserName,
                        Email = user.Email,
                        //PasswordHash=user.PasswordHash,
                        Roles = roles,
                        //RoleId = getRoleInfo.Id,
                        //RoleName = getRoleInfo.Name,
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<LoginResponse> LoginAccountAsync(LoginRequestDTO model)
        {
            try
            {
                //var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
                //if(!result.Succeeded) return new LoginResponse() { flag = false ,message="Username and password are invalid."};
                var startTime = DateTime.Now;

                var user = await FindUserByEmailAsync(model.EmailAddress);
                if (user == null) return new LoginResponse()
                {
                    Flag = false,
                    Message = "Email not found",
                };
                await dbContext.LogTimes.AddAsync(new Domain.Entity.WMS.LogTime()
                {
                    LogName = $"userManager.FindByNameAsync({model.EmailAddress})",
                    EslapseTime = (DateTime.Now - startTime).TotalMilliseconds,
                    CreatedDate = DateTime.Now
                });

                //await dbContext.SaveChangesAsync();

                SignInResult result = null;

                result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                dbContext.LogTimes.Add(new Domain.Entity.WMS.LogTime()
                {
                    LogName = $"await signInManager.CheckPasswordSignInAsync(user, model.Password, false)",
                    EslapseTime = (DateTime.Now - startTime).TotalMilliseconds,
                    CreatedDate = DateTime.Now
                });
                //await dbContext.SaveChangesAsync();

                if (!result.Succeeded) return new LoginResponse()
                {
                    Flag = false,
                    Message = "Invalid credentials"
                };

                var jwtToken = await GenerateToken(user);
                string token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
                string refreshToken = GenerateRefreshToken();
                dbContext.LogTimes.Add(new Domain.Entity.WMS.LogTime()
                {
                    LogName = $"await GenerateToken(user)|",
                    EslapseTime = (DateTime.Now - startTime).TotalMilliseconds,
                    CreatedDate = DateTime.Now
                });
                await dbContext.SaveChangesAsync();

                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(refreshToken))
                    return new LoginResponse()
                    {
                        Flag = false,
                        Message = "Error occured while in account, please contact administrator."
                    };

                //save token after login successfull 
                var expiryRefreshToken = config["Jwt:AddTimeType"] != "Second" ?
                               DateTime.Now.AddDays(double.TryParse(config["Jwt:JwtExpiryTimeRefreshToken"], out double value) ? value : 60) :
                               DateTime.Now.AddSeconds(double.TryParse(config["Jwt:JwtExpiryTimeRefreshToken"], out value) ? value : 60);

                var saveResult = await SaveRefreshTokenAsync(user.Id, token, refreshToken, expiryRefreshToken);

                dbContext.LogTimes.Add(new Domain.Entity.WMS.LogTime()
                {
                    LogName = $"await SaveRefreshTokenAsync(user.Id, token, refreshToken, expiryRefreshToken)",
                    EslapseTime = (DateTime.Now - startTime).TotalMilliseconds,
                    CreatedDate = DateTime.Now
                });
                await dbContext.SaveChangesAsync();

                if (saveResult.Flag)
                    return new LoginResponse()
                    {
                        Flag = true,
                        Message = $"{user.FullName} successfully logged in.",
                        Token = token,
                        RefreshToken = refreshToken,
                        Expiration = jwtToken.ValidTo.ToString()
                    };
                else return new LoginResponse();
            }
            catch (Exception ex)
            {
                return new LoginResponse()
                {
                    Flag = false,
                    Message = $"{ex.Message}{Environment.NewLine}{ex.InnerException}",
                };
            }
        }

        public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequestDTO model)
        {
            try
            {
                var token = await dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.RefreshToken == model.RefreshToken && x.Activated == true);
                if (token is null) return new LoginResponse() { Flag = false, Message = "Refresh token is invalid" };

                if (token.ExpirationTime < DateTime.Now)
                {
                    token.Activated = false;
                    //dbContext.RefreshTokens.Update(token);
                    //await dbContext.SaveChangesAsync();

                    return new LoginResponse()
                    {
                        Flag = false,
                        Message = "The refresh token has expired."
                    };
                }

                var user = await userManager.FindByIdAsync(token.UserId);
                var newJwtToken = await GenerateToken(user);
                string newToken = new JwtSecurityTokenHandler().WriteToken(newJwtToken);
                string newRefreshToken = GenerateRefreshToken();

                var expiryRefreshToken = config["Jwt:AddTimeType"] != "Second" ?
                                       DateTime.Now.AddDays(double.TryParse(config["Jwt:JwtExpiryTimeRefreshToken"], out double value) ? value : 10) :
                                       DateTime.Now.AddSeconds(double.TryParse(config["Jwt:JwtExpiryTimeRefreshToken"], out value) ? value : 10);
                var saveResult = await SaveRefreshTokenAsync(user.Id, newToken, newRefreshToken, expiryRefreshToken, model.RefreshToken);
                if (saveResult.Flag)
                    return new LoginResponse()
                    {
                        Flag = true,
                        Message = $"{user.FullName} successfully re-logged in.",
                        Token = newToken,
                        RefreshToken = newRefreshToken,
                        Expiration = newJwtToken.ValidTo.ToString()
                    };
                else
                    return new LoginResponse()
                    {
                        Flag = saveResult.Flag,
                        Message = saveResult.Message
                    };
            }
            catch (Exception ex)
            {
                return new LoginResponse()
                {
                    Flag = false,
                    Message = $"{ex.Message}{Environment.NewLine}{ex.InnerException}",
                };
            }
        }

        public async Task<GeneralResponse> ChangePassAsync(ChangePassRequestDTO model)
        {
            try
            {
                var user = await FindUserByIdAsync(model.Id);
                if (user == null)
                    return new GeneralResponse()
                    {
                        Flag = false,
                        Message = "User not found"
                    };

                await userManager.RemovePasswordAsync(user);
                await userManager.AddPasswordAsync(user, model.NewPassword);

                return new GeneralResponse()
                {
                    Flag = true,
                    Message = "Passwork changed"
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse()
                {
                    Flag = false,
                    Message = $"{ex.Message}{Environment.NewLine}{ex.InnerException}",
                };
            }
        }

        public async Task<GeneralResponse> AssignUserRoleAsync(AssignUserRoleRequestDTO model)
        {
            try
            {
                if (model == null)
                    return new GeneralResponse()
                    {
                        Flag = false,
                        Message = "Model state cannot be empty"
                    };

                var user = await FindUserByNameAsync(model.UserName);
                if (user == null)
                    return new GeneralResponse()
                    {
                        Flag = false,
                        Message = "User not found."
                    };



                var res = await AssignUserToRole(user, model.Roles);
                return new GeneralResponse()
                {
                    Flag = res.Flag,
                    Message = res.Message
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse()
                {
                    Flag = false,
                    Message = $"{ex.Message}{Environment.NewLine}{ex.InnerException}"
                };
            }
        }

        public async Task<GeneralResponse> DeleteUserAsync(UpdateDeleteRequestDTO model)
        {
            try
            {
                if (model == null)
                    return new GeneralResponse()
                    {
                        Flag = false,
                        Message = "Model state cannot be empty"
                    };

                var user = await FindUserByIdAsync(model.Id);
                if (user == null)
                    return new GeneralResponse()
                    {
                        Flag = false,
                        Message = "User not found."
                    };

                var result = await userManager.DeleteAsync(user);

                string error = CheckReponse(result);
                if (!string.IsNullOrEmpty(error))
                    return new GeneralResponse()
                    {
                        Flag = false,
                        Message = error
                    };
                else
                    return new GeneralResponse()
                    {
                        Flag = true,
                        Message = $"{user.FullName} has ben deleted"
                    };
            }
            catch (Exception ex)
            {
                return new GeneralResponse()
                {
                    Flag = false,
                    Message = $"{ex.Message}{Environment.NewLine}{ex.InnerException}"
                };
            }
        }

        public async Task<GeneralResponse> DeleteUserRoleAsync(AssignUserRoleRequestDTO model)
        {
            try
            {
                if (model == null)
                    return new GeneralResponse()
                    {
                        Flag = false,
                        Message = "Model state cannot be empty"
                    };

                var user = await FindUserByNameAsync(model.UserName);
                if (user == null)
                    return new GeneralResponse()
                    {
                        Flag = false,
                        Message = "User not found."
                    };


                IdentityResult result = null;
                foreach (var role in model.Roles)
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                return new GeneralResponse()
                {
                    Flag = result.Succeeded,
                    Message = result.Errors.FirstOrDefault()?.Description
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse()
                {
                    Flag = false,
                    Message = $"{ex.Message}{Environment.NewLine}{ex.InnerException}"
                };
            }
        }

        public async Task<GeneralResponse> UpdateRoleAsync(UpdateDeleteRequestDTO model)
        {
            try
            {
                if (model == null)
                    return new GeneralResponse()
                    {
                        Flag = false,
                        Message = "Model state cannot be empty"
                    };

                var currentRole = await roleManager.FindByIdAsync(model.Id);

                if (currentRole == null)
                {
                    return new GeneralResponse()
                    {
                        Flag = false,
                        Message = $"Current role not found."
                    };
                }

                // Đổi tên vai trò
                var currentName = currentRole.Name;
                currentRole.Name = model.Name;

                // Cập nhật vai trò
                var result = await roleManager.UpdateAsync(currentRole);

                var error = CheckReponse(result);

                if (!string.IsNullOrEmpty(error))
                {
                    return new GeneralResponse()
                    {
                        Flag = false,
                        Message = error
                    };
                }

                return new GeneralResponse()
                {
                    Flag = true,
                    Message = $"Role name has been changed from '{currentName}' to '{model.Name}'."
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse()
                {
                    Flag = false,
                    Message = $"{ex.Message}{Environment.NewLine}{ex.InnerException}",
                };
            }
        }

        public async Task<GeneralResponse> UpdateUserInfoAsync(UpdateUserInfoRequestDTO model)
        {
            try
            {
                if (model == null)
                    return new GeneralResponse()
                    {
                        Flag = false,
                        Message = "Model state cannot be empty"
                    };

                var user = await FindUserByIdAsync(model.Id);

                if (user == null)
                    return new GeneralResponse()
                    {
                        Flag = false,
                        Message = "User cannot be found"
                    };

                //update user info
                user.FullName = model.FullName;
                user.UserName = model.UserName;
                user.Email = model.Email;
                var result = await userManager.UpdateAsync(user);

                string error = CheckReponse(result);
                if (!string.IsNullOrEmpty(error))
                    return new GeneralResponse()
                    {
                        Flag = false,
                        Message = error
                    };

                ////update password
                //await userManager.RemovePasswordAsync(user);
                //result = await userManager.AddPasswordAsync(user, model.Password);
                //error = CheckReponse(result);
                //if (!string.IsNullOrEmpty(error))
                //    return new GeneralResponse()
                //    {
                //        Flag = false,
                //        Message = error
                //    };

                //remove all role
                var currentRoles = await userManager.GetRolesAsync(user);
                result = await userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!string.IsNullOrEmpty(error))
                    return new GeneralResponse()
                    {
                        Flag = false,
                        Message = error
                    };

                var res = await AssignUserToRole(user, model.Roles);
                return new GeneralResponse()
                {
                    Flag = res.Flag,
                    Message = res.Message
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse()
                {
                    Flag = false,
                    Message = $"{ex.Message}{Environment.NewLine}{ex.InnerException}",
                };
            }
        }

        public async Task<GetUserWithRoleResponseDTO> UserGetById([Path] string id)
        {
            try
            {
                var user = await FindUserByIdAsync(id);
                if (user == null) return null;

                var getUserRole = await userManager.GetRolesAsync(user);
                var roles = new List<GetRoleResponseDTO>();

                foreach (var roleName in getUserRole)
                {
                    var getRoleInfo = await roleManager.Roles.FirstOrDefaultAsync(x => x.Name.ToLower() == roleName.ToLower());

                    roles.Add(new GetRoleResponseDTO()
                    {
                        Id = getRoleInfo.Id,
                        Name = getRoleInfo.Name,
                    });
                }

                //var getRoleInfo = await roleManager.Roles.FirstOrDefaultAsync(x => x.Name.ToLower() == getUserRole.ToLower());
                GetUserWithRoleResponseDTO res = new GetUserWithRoleResponseDTO()
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles
                };
                return res;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<GeneralResponse> DeleteRoleAsync([Body] UpdateDeleteRequestDTO model)
        {
            try
            {
                var roleInfo = await roleManager.FindByIdAsync(model.Id);

                var user = (await userManager.GetUsersInRoleAsync(model.Name)).FirstOrDefault();

                string error = string.Empty;

                if (user != null)
                {
                    var previousRole = (await userManager.GetRolesAsync(user!)).FirstOrDefault();
                    var removeOldRole = await userManager.RemoveFromRoleAsync(user!, previousRole!);

                    error = CheckReponse(removeOldRole);
                    if (!string.IsNullOrEmpty(error))
                        return new GeneralResponse()
                        {
                            Flag = false,
                            Message = error
                        };
                }


                var rtp = dbContext.RoleToPermissions.Where(x => x.RoleId == Guid.Parse(model.Id)).ToList();
                dbContext.RoleToPermissions.RemoveRange(rtp!);

                var result = await roleManager.DeleteAsync(roleInfo);
                error = CheckReponse(result);
                if (!string.IsNullOrEmpty(error))
                    return new GeneralResponse()
                    {
                        Flag = false,
                        Message = error
                    };

                return new GeneralResponse()
                {
                    Flag = true,
                    Message = $"Role {model.Name} has been deleted"
                };

            }
            catch (Exception ex)
            {
                return new GeneralResponse()
                {
                    Flag = false,
                    Message = $"{ex.Message}{Environment.NewLine}{ex.InnerException}"
                };
            }
        }

        public async Task<GetUserWithRoleResponseDTO> UserGetByEmailAsync([Path] string email)
        {
            try
            {
                var user = await FindUserByEmailAsync(email);
                if (user == null) return null;

                var getUserRole = await userManager.GetRolesAsync(user);
                var roles = new List<GetRoleResponseDTO>();

                foreach (var roleName in getUserRole)
                {
                    var getRoleInfo = await roleManager.Roles.FirstOrDefaultAsync(x => x.Name.ToLower() == roleName.ToLower());

                    roles.Add(new GetRoleResponseDTO()
                    {
                        Id = getRoleInfo.Id,
                        Name = getRoleInfo.Name,
                    });
                }

                //var getRoleInfo = await roleManager.Roles.FirstOrDefaultAsync(x => x.Name.ToLower() == getUserRole.ToLower());
                GetUserWithRoleResponseDTO res = new GetUserWithRoleResponseDTO()
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles
                };
                return res;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<GetRoleResponseDTO> RoleGetById([Path] string id)
        {
            try
            {
                var role = await roleManager.FindByIdAsync(id);
                var roleInfo = new GetRoleResponseDTO()
                {
                    Id = id,
                    Name = role.Name
                };
                return roleInfo;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        #region Tạo PDF chuyển sang base64 rồi trả về client
        public async Task<string> GeneratePdf()
        {
            return await ExportReceipt();
        }

        private async Task ExportPdf()
        {
            var exporter = new PdfExporter();
            var result = await exporter.ExportListByTemplate("test.pdf", new List<Student>()
            {
                new Student
                {
                    Name = "MR.A",
                    Age = 18
                },
                new Student
                {
                    Name = "MR.B",
                    Age = 19
                },
                new Student
                {
                    Name = "MR.B",
                    Age = 20
                }
            });
        }
        public async Task<string> ExportReceipt()
        {
            var tplPath = Path.Combine(Directory.GetCurrentDirectory(), "LableTemplate",
                "receipt.html");
            var tpl = File.ReadAllText(tplPath);
            // Initialize the PDF exporter
            var exporter = new PdfExporter();
            //此处使用默认模板导出
            var exportPath = Path.Combine(Directory.GetCurrentDirectory(), "ExportFile",
                $"test.pdf");
            var result = await exporter.ExportByTemplate(exportPath,
                new ReceiptInfo
                {

                }, tpl);

            var exBase64 = File.ReadAllBytes(exportPath);

            return Convert.ToBase64String(exBase64);
        }

        private async Task<string> ExportReceiptLocal(ApplicationUser model)
        {
            //var tplPath = Path.Combine(Directory.GetCurrentDirectory(), "LableTemplate","receipt.html");
            var tplPath = Path.Combine(Directory.GetCurrentDirectory(), "LableTemplate", "LabelPrint.html");
            var tpl = File.ReadAllText(tplPath);
            // Initialize the PDF exporter
            var exporter = new PdfExporter();
            //此处使用默认模板导出
            var exportPath = Path.Combine(Directory.GetCurrentDirectory(), "ExportFile",
                $"test.pdf");
            string qr = GenerateQRCode($"{model.Id}|{model.Email}|{model.UserName}");

            //var result = await exporter.ExportByTemplate(exportPath,
            //    new ReceiptInfo
            //    {
            //        QrValue = qr,
            //        UserName = model.UserName,
            //        FullName = model.FullName,
            //        Email = model.Email
            //    }, tpl);            

            List<LabelInfoDto> labels = new List<LabelInfoDto>();
            labels.Add(new LabelInfoDto()
            {
                QrValue = qr,
                Title1 = "User Name:",
                Content1 = $"{model.UserName}",
                Title2 = "Email:",
                Content2 = $"{model.Email}"
            });
            labels.Add(new LabelInfoDto()
            {
                QrValue = qr,
                Title1 = "User Name 1:",
                Content1 = $"{model.UserName}",
                Title2 = "Email 1:",
                Content2 = $"{model.Email}"
            });

            var result = await exporter.ExportByTemplate(exportPath,
                labels, tpl);

            var exBase64 = File.ReadAllBytes(exportPath);

            return Convert.ToBase64String(exBase64);
        }
        public async Task<string> GetReportBase64([Path] string id)
        {
            try
            {
                var user = await userManager.FindByIdAsync(id);

                var ul = new List<ApplicationUser>();
                ul.Add(user);
                //var bytes = await GetReportBytes(ul);

                var response = ExportReceiptLocal(user);

                return await response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        /// <summary>
        /// Method to generate QR code base64.
        /// </summary>
        /// <param name="inputText"></param>
        /// <returns></returns>
        private string GenerateQRCode(string inputText)
        {
            if (string.IsNullOrEmpty(inputText))
                return null;

            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(inputText, QRCodeGenerator.ECCLevel.Q);
                PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
                byte[] qrCodeImage = qrCode.GetGraphic(20);

                return $"data:image/png;base64,{Convert.ToBase64String(qrCodeImage)}";
            }
        }
        public async Task<byte[]> GetReportBytes(List<ApplicationUser> data)
        {
            using (var wb = new XLWorkbook())
            {
                wb.Properties.Author = "the Author";
                wb.Properties.Title = "the Title";
                wb.Properties.Subject = "the Subject";
                wb.Properties.Category = "the Category";
                wb.Properties.Keywords = "the Keywords";
                wb.Properties.Comments = "the Comments";
                wb.Properties.Status = "the Status";
                wb.Properties.LastModifiedBy = "the Last Modified By";
                wb.Properties.Company = "the Company";
                wb.Properties.Manager = "the Manager";

                var ws = wb.Worksheets.Add("UserInfo");

                ws.Range(1, 1, 1, 8).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                .Font.SetFontSize(15).Font.SetBold(true)
                ;

                ws.Range(2, 1, 2, 8).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
               .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                ws.Cell(1, 1).Value = "REPORT";
                ws.Cell(1, 1).Style.Fill.BackgroundColor = XLColor.Orange;
                ws.Cell(2, 1).Value = $"Thời gian:";

                ws.Cell(3, 1).Value = "Thời gian";
                ws.Cell(3, 2).Value = "Id";
                ws.Cell(3, 3).Value = "Oven";
                ws.Cell(3, 4).Value = "Nhiệt độ đặt (oC)";
                ws.Cell(3, 5).Value = "Nhiệt độ (oC)";
                ws.Cell(3, 6).Value = "Profile";
                ws.Cell(3, 7).Value = "Step";
                ws.Cell(3, 8).Value = "Cảnh báo";

                ws.Range(3, 1, 3, 8).SetAutoFilter(true);
                ws.Range(3, 1, 3, 8).Style.Fill.BackgroundColor = XLColor.LightCyan;

                // Fill a cell with a date
                //ws.Range($"A3:A{data.Count + 3}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                //                                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                //                                   .DateFormat.Format = "yyyy-MM-dd HH:mm:ss";

                ws.Range($"A3:H{data.Count + 3}").Style.Border.SetInsideBorder(XLBorderStyleValues.Thin)
                                       .Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                var row = 0;
                foreach (var item in data)
                {
                    // The apostrophe is to force ClosedXML to treat the date as a string
                    //thay chi tiet cac cot data vao bem duoi.
                    ws.Cell(row + 4, 1).Value = item.Email;
                    ws.Cell(row + 4, 2).Value = item.FullName;
                    row += 1;
                }

                ws.Columns().AdjustToContents();//Adjust Row Height and Column Width to Contents

                var bytes = new byte[0];
                using (var ms = new MemoryStream())
                {
                    wb.SaveAs(ms);

                    bytes = ms.ToArray();
                }

                return bytes;
            }
        }
        #endregion

        public async Task<List<LabelInfoDto>> GetLabelByIdAsync([Path] string id)
        {
            try
            {
                var user = await FindUserByIdAsync(id);
                if (user == null) return null;

                List<LabelInfoDto> res = new List<LabelInfoDto>();

                var qrCode = EncodeMD5.EncryptString($"{user.Email}|{user.PasswordHash}", "WmsHt123@456");

                res.Add(new LabelInfoDto()
                {
                    QrValue = GlobalVariable.GenerateQRCode(qrCode),
                    //Title1="Email:",
                    //Content1=user.Email,
                    //Title2="User Name:",
                    //Content2=user.UserName
                    Title1 = "Email:",
                    Title2 = user.Email,
                });

                return res;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<LabelInfoDto>> GetLabelsAllAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// password ở đây nó là PasswordHash.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<LoginResponse> LoginAccountHTAsync([Body] LoginRequestDTO model)
        {
            try
            {
                //var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
                //if(!result.Succeeded) return new LoginResponse() { flag = false ,message="Username and password are invalid."};

                var user = await FindUserByEmailAsync(model.EmailAddress);
                if (user == null) return new LoginResponse()
                {
                    Flag = false,
                    Message = "Email not found",
                };

                var allPermission = new List<PermissionsListModel>();
                int rowCount = 0;
                var c = dbContext.Database.GetConnectionString();
                using (var connection = new SqlConnection(c))
                {
                    rowCount = (int)await connection.ExecuteScalarAsync($"SELECT COUNT(*) FROM wms.[AspNetUsers] WHERE PasswordHash = '{model.Password}'");
                }

                if (rowCount <= 0) return new LoginResponse()
                {
                    Flag = false,
                    Message = "Invalid credentials"
                };

                var jwtToken = await GenerateToken(user);
                string token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
                string refreshToken = GenerateRefreshToken();

                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(refreshToken))
                    return new LoginResponse()
                    {
                        Flag = false,
                        Message = "Error occured while in account, please contact administrator."
                    };
                else
                {
                    //save token after login successfull 
                    var expiryRefreshToken = config["Jwt:AddTimeType"] != "Second" ?
                                   DateTime.Now.AddDays(double.TryParse(config["Jwt:JwtExpiryTimeRefreshToken"], out double value) ? value : 60) :
                                   DateTime.Now.AddSeconds(double.TryParse(config["Jwt:JwtExpiryTimeRefreshToken"], out value) ? value : 60);
                    var saveResult = await SaveRefreshTokenAsync(user.Id, token, refreshToken, expiryRefreshToken);
                    if (saveResult.Flag)
                        return new LoginResponse()
                        {
                            Flag = true,
                            Message = $"{user.FullName} successfully logged in.",
                            Token = token,
                            RefreshToken = refreshToken,
                            Expiration = jwtToken.ValidTo.ToString()
                        };
                    else return new LoginResponse();
                }
            }
            catch (Exception ex)
            {
                return new LoginResponse()
                {
                    Flag = false,
                    Message = $"{ex.Message}{Environment.NewLine}{ex.InnerException}",
                };
            }
        }

        public async Task<Permissions> JobApi()
        {
            try
            {
                var res = await dbContext.Permissions.FirstOrDefaultAsync();

                return res;
            }
            catch
            {
                return null;
            }
        }
    }
}
