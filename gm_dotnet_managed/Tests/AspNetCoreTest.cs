using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace Tests
{
    // Tests ability to run ASP.NET Core server inside game process
    public class AspNetCoreTest : ITest
    {
        readonly TaskCompletionSource<bool> taskCompletion;
        GetILuaFromLuaStatePointer lua_extructor;
        static string randomString;
        IHost webHost;

        public AspNetCoreTest()
        {
            taskCompletion = new TaskCompletionSource<bool>();

            randomString = Guid.NewGuid().ToString();
        }

        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor)
        {
            try
            {
                this.lua_extructor = lua_extructor;

                webHost = Host.CreateDefaultBuilder().ConfigureWebHostDefaults((webHostBuilder) =>
                {
                    webHostBuilder.UseStartup<WebStartup>();
                    webHostBuilder.UseKestrel(options =>
                    {
                        options.ListenLocalhost(8998);
                    });
                }).Build();

                webHost.RunAsync();

                HttpClient http_client = new HttpClient();

                string response = http_client.GetStringAsync("http://127.0.0.1:8998").Result;

                if(response != AspNetCoreTest.randomString)
                {
                    throw new Exception("Embedded web server has returned invalid response");
                }

                webHost.StopAsync().Wait();

                taskCompletion.TrySetResult(true);
            }
            catch(Exception e)
            {
                taskCompletion.TrySetException(new Exception[] { e });
            }

            return taskCompletion.Task;
        }

        class WebStartup
        {
            IConfiguration configuration;

            public WebStartup(IConfiguration conf)
            {
                configuration = conf;
            }

            public void ConfigureServices(IServiceCollection services)
            {

            }

            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
                app.Run(async context =>
                {
                    context.Response.Headers.Add("Content-Type", "text/plain");
                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsync(AspNetCoreTest.randomString);
                    await context.Response.CompleteAsync();
                });
            }

        }
    }
}
