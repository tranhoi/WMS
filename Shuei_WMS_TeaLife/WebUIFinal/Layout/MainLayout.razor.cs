using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Newtonsoft.Json;
using Application.Services.Authen.UI;
using Domain.Entity.WMS.Authentication;
using Radzen;
using WebUIFinal;
using Radzen.Blazor;
using WebUIFinal.TemplateHtmlPrintLabel;
using WebUIFinal.Pages.Components;
using Microsoft.AspNetCore.Components;

namespace WebUIFinal.Layout
{
    public partial class MainLayout
    {
        //[Inject] IHttpInterceptorManager _httpInterceptorManager { get; set; }

        private ClaimsPrincipal? user;
        bool _sidebarExpanded = true;
        string _linkChangePass = string.Empty;
        protected override async Task OnInitializedAsync()
        {
            //var authState = await _authStateProvider.GetAuthenticationStateAsync();
            //try
            //{
            //    if (authState.User.Identity == null || !authState.User.Identity.IsAuthenticated)
            //    {
            //        _authenServices.LogoutAsync();
            //        return;
            //    }

                
            //    //_httpInterceptorManager.RegisterEvent();
            //    GlobalVariable.UserAuthorizationInfo.UserName = authState.User.Identity.Name;
            //    GlobalVariable.UserAuthorizationInfo.FullName = authState.User.FindFirst("FullName").Value;
            //    GlobalVariable.UserAuthorizationInfo.EmailName = authState.User.FindFirst(ClaimTypes.Email).Value;
            //    GlobalVariable.UserAuthorizationInfo.UserId = authState.User.FindFirst("UserId").Value;

            //    var permission = authState.User.FindFirst("RoleToPermission").Value;
            //    var permissionList = JsonConvert.DeserializeObject<List<RoleToPermission>>(permission);

            //    var claimRole = authState.User.FindAll(ClaimTypes.Role)?.ToList();

            //    foreach (var item in claimRole)
            //    {
            //        var per = permissionList.Where(x => x.RoleName == item.Value).ToList();

            //        GlobalVariable.UserAuthorizationInfo.Roles.Add(new Roles()
            //        {
            //            Name = item.Value,
            //            Permissions = per
            //        });
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _notificationService.Notify(new NotificationMessage
            //    {
            //        Severity = NotificationSeverity.Error,
            //        Summary = "Error",
            //        Detail = "Login fail",
            //        Duration = 5000
            //    });
            //}
            //user = authState.User;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {

            }

            base.OnAfterRender(firstRender);
        }

        void OnParentClicked(RadzenProfileMenuItem args)
        {
            var t = _localizer["Profile.Logout"];
            if (args.Text == _localizer["Profile.Logout"])
            {
                GlobalVariable.UserAuthorizationInfo = null;
                GlobalVariable.UserAuthorizationInfo = new UserAuthorizationInfo();
                _authenServices.LogoutAsync();
            }
        }

        private void OnClick(string text)
        {
            _notificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Info, Summary = "Button Clicked", Detail = text });
        }

        async void ChangePassClick()
        {
            var res = await _dialogService.OpenAsync<ChangePass>("Change Password",
                    new Dictionary<string, object>() { { "Id", GlobalVariable.UserAuthorizationInfo.UserId }},
                    new DialogOptions()
                    {
                        Width = "600px",
                        Height = "400px",
                        Resizable = true,
                        Draggable = true,
                        ShowClose = false,
                        CloseDialogOnOverlayClick = true
                    });

            if (res == "Success")
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Change password",
                    Detail = "Successfull",
                    Duration = 5000
                });
            }
        }

        public void Dispose()
        {
        }
    }
}
