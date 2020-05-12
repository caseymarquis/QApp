using KC.Actin;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using QApp.Actors.Signalr;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace QApp.Web
{
    public class AppWebService
    {
        public AppWebService(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private bool useHSTS() {
            if (Debugger.IsAttached) {
                return false;
            }
            if (Environment.GetEnvironmentVariable("NOHSTS") != null) {
                return false;
            }
            return true;
        }

        const string corsAllowAll = "corsAllowAll";
        const string corsCustom = "corsCustom";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.Configure<ForwardedHeadersOptions>(options => {
                options.ForwardedHeaders =
                    ForwardedHeaders.All;
                options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("::ffff:0.0.0.0"), 0));
            });
            if (useHSTS()) {
                services.AddHsts(options => {
                    options.Preload = true;
                    options.IncludeSubDomains = true;
                    options.MaxAge = TimeSpan.FromDays(60);
                });
                services.AddHttpsRedirection(options => {
                    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                    options.HttpsPort = 443;
                });
            }

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "QApp API", Version = "v1" });
            });
            services.AddControllers();
            services.AddSignalR(c => {
                c.EnableDetailedErrors = true;
            });
            services.AddCors(options => {
                options.AddPolicy(corsAllowAll, builder => {
                    builder.SetIsOriginAllowed(_ => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                });
                options.AddPolicy(corsCustom, builder => {
                    //Add a customized cors policy as needed:
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (useHSTS()) {
                app.UseForwardedHeaders();
                app.UseHsts();
                app.UseHttpsRedirection();
            }
            else {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseDefaultFiles();

            app.UseStaticFiles();

            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".exe"] = "application/octet-stream";

            var staticFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwdev", "app", "_dev-server", "static");
            Directory.CreateDirectory(staticFilePath);
            var staticFileProvider = new PhysicalFileProvider(staticFilePath);
            app.UseStaticFiles(new StaticFileOptions() {
                FileProvider = staticFileProvider,
                RequestPath = new PathString("/static"),
                ContentTypeProvider = provider
            });

            app.UseDirectoryBrowser(new DirectoryBrowserOptions {
                FileProvider = staticFileProvider,
                RequestPath = new PathString("/static"),
            });

            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "QApp API v1");
            });

            var corsPolicy = Debugger.IsAttached ? corsAllowAll : corsCustom;
            app.UseCors(corsPolicy);

            object lockMiddleware = new object();
            var haveHubContext = false;
            app.Use(async (context, next) => {
                lock (lockMiddleware) {
                    if (!haveHubContext) {
                        var hubContext = context.RequestServices.GetRequiredService<IHubContext<UpdateHub, IUpdateHubClient>>();
                        if (hubContext != null) {
                            App.Director.AddSingletonDependency(hubContext, new Type[] { typeof(IHubContext<UpdateHub, IUpdateHubClient>) });
                            haveHubContext = true;
                        }
                    }
                }
                if (next != null) {
                    await next.Invoke();
                }
            });
            app.UseEndpoints(c => {
                c.MapHub<UpdateHub>("/updatehub");
                c.MapControllers();
            });
        }
    }
}
