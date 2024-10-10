using Application.Services;
using Application.Services.Authen;
using Application.Services.Authen.UI;
using Application.Services.Inbound;
using Application.Services.Outbound;
using Application.Services.Suppliers;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.JSInterop;
using Radzen;
using RestEase.HttpClientFactory;
using System.Globalization;
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
    b.AddPolicy("Warehouse Admin", p =>
    {
        p.RequireRole("Warehouse Admin");
        p.RequireClaim("ABC", "1");
    });
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
    .UseWithRestEaseClient<IProductCategory>()
    .UseWithRestEaseClient<INumberSequences>()
    .UseWithRestEaseClient<IBatches>()
    .UseWithRestEaseClient<IWarehouseReceiptOrder>() 
    .UseWithRestEaseClient<IWarehouseReceiptOrderLine>()
    .UseWithRestEaseClient<IShippingCarrier>();

builder.Services.AddScoped<HttpClient>(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

await builder.Build().RunAsync();
