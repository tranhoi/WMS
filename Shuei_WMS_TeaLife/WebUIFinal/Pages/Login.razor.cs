using Application.DTOs.Request.Account;
using Application.DTOs.Response;
using Application.Extentions;
using Domain.Entity.Commons;
using Domain.Entity.WMS.Authentication;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Radzen;
using System.Security.Claims;

namespace WebUIFinal.Pages
{
    [AllowAnonymous]
    public partial class Login
    {
        LoginResponse login;
        private string token;

        LoginRequestDTO _loginRequest = new LoginRequestDTO();

        bool password = true;
        void TogglePassword()
        {
            password = !password;
        }

        bool _remember = false;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        async void Submit(LoginRequestDTO arg)
        {
            Console.WriteLine($"Username: {arg.EmailAddress} and password: {arg.Password}");
            try
            {
                //var result = await _authenServices.LoginAccountAsync(new LoginRequestDTO()
                //{
                //    EmailAddress = arg.EmailAddress,
                //    Password = arg.Password
                //}); 
                var result = await _authenServices.LoginAccountAsync(arg);

                //Fail
                if (!result.Flag)
                {
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = result.Message,
                        Duration = 5000
                    });
                    return;
                }

                token = result.Token;
                login = result;

                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = "Successfull",
                    Detail = "Login OK",
                    Duration = 5000
                });
                //await InvokeAsync(StateHasChanged);
                //StateHasChanged();

                var claimsIdentity = new ClaimsIdentity(JwtHelper.GetClaimsFromJwt(token), "jwt");
                var user = new ClaimsPrincipal(claimsIdentity);

                //_httpInterceptorManager.RegisterEvent();
                GlobalVariable.UserAuthorizationInfo.UserName = user.Identity.Name;
                GlobalVariable.UserAuthorizationInfo.FullName = user.FindFirst("FullName").Value;
                GlobalVariable.UserAuthorizationInfo.EmailName = user.FindFirst(ClaimTypes.Email).Value;
                GlobalVariable.UserAuthorizationInfo.UserId = user.FindFirst("UserId").Value;

                var permission = user.FindFirst("RoleToPermission").Value;
                var permissionList = JsonConvert.DeserializeObject<List<RoleToPermission>>(permission);

                var claimRole = user.FindAll(ClaimTypes.Role)?.ToList();

                foreach (var item in claimRole)
                {
                    var per = permissionList.Where(x => x.RoleName == item.Value).ToList();

                    GlobalVariable.UserAuthorizationInfo.Roles.Add(new Roles()
                    {
                        Name = item.Value,
                        Permissions = per
                    });
                }

                if (GlobalVariable.UserAuthorizationInfo.Roles.FirstOrDefault().Name == ConstantExtention.Roles.WarehouseAdmin)
                    _navigation.NavigateTo("/userlist");
                else if (GlobalVariable.UserAuthorizationInfo.Roles.FirstOrDefault().Name == ConstantExtention.Roles.WarehouseStaff)
                    _navigation.NavigateTo("/warehouse-receiptlist");
                else 
                    _navigation.NavigateTo("/numbersequence");
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Error",
                    Detail = $"Login fail: {ex.Message}",
                    Duration = 5000
                });
            }
        }

        void Cancel()
        {

        }

        void OnRegister(string name)
        {
            Console.WriteLine($"{name} -> Register");
        }

        void OnResetPassword(string value, string name)
        {
            Console.WriteLine($"{name} -> ResetPassword for user: {value}");
        }
    }
}
