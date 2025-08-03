using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using STApp.Client.Service;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();

await builder.Build().RunAsync();
