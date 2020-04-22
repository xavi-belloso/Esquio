﻿using Esquio.UI.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Esquio.UI.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Warning()
               .WriteTo.BrowserConsole()
               .CreateLogger();

            try
            {
                await ConfigureHost(args).Build().RunAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception is throwed when creating the WASM host!");
            }
        }

        static WebAssemblyHostBuilder ConfigureHost(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddOidcAuthentication(options =>
            {
                options.ProviderOptions.Authority = "https://demo.identityserver.io";
                options.ProviderOptions.ClientId = "interactive.public";
                options.ProviderOptions.DefaultScopes.Add("api");
                options.ProviderOptions.ResponseType = "code";
            });

            builder.Services.AddSingleton<IEsquioHttpClient, EsquioHttpClient>(sp =>
            {
                //TODO: Check preview 5 with new message handler instead IAcccessTokenProvider

                var tokenService = sp.GetRequiredService<IAccessTokenProvider>();
                var httpClient = sp.GetRequiredService<HttpClient>();
                
                return new EsquioHttpClient(httpClient, tokenService);
            });

            builder.Services.AddScoped<EsquioState>();
            builder.Services.AddScoped<ConfirmState>();
            builder.Services.AddScoped<ILocalStorage, LocalStorage>();
            builder.Services.AddScoped<IPolicyBuilder, DefaultPolicyBuilder>();
            builder.Services.AddScoped<INotifications, Notifications>();

            return builder;
        }
    }
}
