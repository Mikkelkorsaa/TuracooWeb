using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TuracooWeb;
using TuracooWeb.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Add these lines to your Program.cs file where services are registered
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://api.turacoo.com") });
builder.Services.AddScoped<ContactService>();

await builder.Build().RunAsync();