using Application.Extentions;
using Application.Services;
using Application.Services.Authen;
using Application.Services.Authen.UI;
using Application.Services.Inbound;
using Application.Services.Outbound;
using Application.Services.Suppliers;
using Blazored.LocalStorage;
using Blazored.SessionStorage; 
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Radzen;
using RestEase.HttpClientFactory;
using System.Globalization;
using System.Reflection.Metadata;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using WebUIFinal;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//// Thêm dịch vụ Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
//builder.Services.AddLocalization();

// Lấy ngôn ngữ đã lưu trong localStorage
var jsInterop = builder.Build().Services.GetRequiredService<IJSRuntime>();
var result = await jsInterop.InvokeAsync<string>("blazorCulture.get");
var culture = result ?? "ja-JP";  // Nếu không tìm thấy ngôn ngữ trong localStorage, mặc định là "ja-JP"

// Thiết lập ngôn ngữ cho ứng dụng
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(culture);
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(culture);


var config = builder.Configuration;

#region BreadCumb initial
//GlobalVariable.BreadCrumbDataMaster.Add(new BreadCrumbModel()
//{
//    Path = "User Manager",
//    Text= "Users List|"
//});
#endregion

builder.Services.AddRadzenComponents();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthServices, AuthServices>();

builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>()
    .AddScoped(sp => (ApiAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>())
    .AddScoped(sp => (IAccessTokenProvider)sp.GetRequiredService<AuthenticationStateProvider>())
    .AddScoped<IAccessTokenProviderAccessor, AccessTokenProviderAccessor>()
    .AddScoped<AuthenticationHeaderHandler>();

builder.Services.AddHttpClientInterceptor();
builder.Services.AddScoped<IHttpInterceptorManager, HttpInterceptorManager>();

builder.Services.AddAuthorizationCore(b =>
{
    b.AddPolicy("Admin", p =>
    {
        p.RequireRole(ConstantExtention.Roles.WarehouseAdmin);
        //p.RequireClaim("Permission", "1");
    });

    b.AddPolicy("Staff", p =>
    {
        p.RequireRole(ConstantExtention.Roles.WarehouseStaff);
        //p.RequireClaim("Permission", "1");
    });

    b.AddPolicy("System", p =>
    {
        p.RequireRole(ConstantExtention.Roles.WarehouseSystem);
        //p.RequireClaim("Permission", "1");
    });

    b.AddPolicy("AdminAndSystem", p =>
    {
        p.RequireRole(new string[] { ConstantExtention.Roles.WarehouseAdmin,ConstantExtention.Roles.WarehouseSystem});
    });

    b.AddPolicy("AdminAndStaff", p =>
    {
        p.RequireRole(ConstantExtention.Roles.WarehouseAdmin, ConstantExtention.Roles.WarehouseStaff);
    });

    //// Combined policy: requires Admin or Manager role AND specific permission
    //b.AddPolicy("CombinedPolicy", policy =>
    //{
    //    policy.RequireAssertion(context =>
    //        (context.User.IsInRole(ConstantExtention.Roles.WarehouseAdmin) || context.User.IsInRole(ConstantExtention.Roles.WarehouseSystem)) &&
    //        context.User.HasClaim(c => c.Type == "Permission" &&
    //                                   (c.Value == "ViewPage" || c.Value == "EditPage")));
    //});

    //// Tạo policy kết hợp
    //b.AddPolicy("AdminOrUserPolicy", policy =>
    //{
    //    policy.RequireAssertion(context =>
    //        context.User.IsInRole(ConstantExtention.Roles.WarehouseAdmin) || context.User.IsInRole(ConstantExtention.Roles.WarehouseSystem));
    //});
});

builder.Services.AddCascadingAuthenticationState();

var url = config["AppSettings:ApiBaseUrl"];
//Register client and services use RestEase library
// Register the RestEase client
builder.Services.AddHttpClient("API")
    .ConfigureHttpClient((sp, x) =>
    {
        x.BaseAddress = new Uri(config["AppSettings:ApiBaseUrl"]!);
        x.EnableIntercept(sp);
    })
    .AddHttpMessageHandler<AuthenticationHeaderHandler>()
    .UseWithRestEaseClient<IProducts>()
    .UseWithRestEaseClient<IPermissions>()
    .UseWithRestEaseClient<IRoleToPermissions>()
    .UseWithRestEaseClient<IVendors>()
    .UseWithRestEaseClient<ILocations>()
    .UseWithRestEaseClient<IProducts>() 
    .UseWithRestEaseClient<IDevices>()   
    .UseWithRestEaseClient<ITenants>()   
    .UseWithRestEaseClient<IUserToTenant>()
    .UseWithRestEaseClient<IProductCategory>()
    .UseWithRestEaseClient<IUnits>()
    .UseWithRestEaseClient<IProductJanCodes>()
    .UseWithRestEaseClient<ISuppliers>()   
    .UseWithRestEaseClient<IBins>()  
    .UseWithRestEaseClient<IWarehousePutAway>()
    .UseWithRestEaseClient<IWarehousePutAwayLine>()
    .UseWithRestEaseClient<IProductCategory>()  
    .UseWithRestEaseClient<IBins>()    
    .UseWithRestEaseClient<IProductCategory>()
    .UseWithRestEaseClient<INumberSequences>()
    .UseWithRestEaseClient<IBatches>()
    .UseWithRestEaseClient<IWarehouseReceiptOrder>() 

    .UseWithRestEaseClient<IWarehouseReceiptOrderLine>() 
    .UseWithRestEaseClient<ICurrency>()
    .UseWithRestEaseClient<IShippingBox>()

    .UseWithRestEaseClient<IShippingCarrier>();

builder.Services.AddScoped<HttpClient>(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

await builder.Build().RunAsync();
