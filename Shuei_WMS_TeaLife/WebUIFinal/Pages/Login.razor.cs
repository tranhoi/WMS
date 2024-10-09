using Application.DTOs.Request.Account;
using Application.DTOs.Response;
using Domain.Entity.Commons;
using Microsoft.AspNetCore.Authorization;
using Radzen;

namespace WebUIFinal.Pages
{
    [AllowAnonymous]
    public partial class Login
    {
        LoginResponse login;
        private string token;

        LoginRequestDTO _loginRequest=new LoginRequestDTO();

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
                var result = await _authenServices.LoginAccountAsync(new LoginRequestDTO()
                {
                    EmailAddress = arg.EmailAddress,
                    Password = arg.Password
                });

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

                _navigation.NavigateTo("/productlist");
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
