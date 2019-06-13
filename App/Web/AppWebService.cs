using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace QApp.Web
{
    public class AppWebService
    {
        public AppWebService(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.Configure<ForwardedHeadersOptions>(options => {
                options.ForwardedHeaders =
                    ForwardedHeaders.All;
                options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("::ffff:0.0.0.0"), 0));
            });
            services.AddMvc();
            if (!Debugger.IsAttached) {
                services.AddHsts(options => {
                    options.Preload = true;
                    options.IncludeSubDomains = true;
                    options.MaxAge = TimeSpan.FromDays(60);
                });
                services.AddHttpsRedirection(options =>
                {
                    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                    options.HttpsPort = 443;
                });
            }
            
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new Info { Title = "QApp API", Version = "v1" });
            });
            services.AddSignalR();
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (!Debugger.IsAttached) {
                app.UseForwardedHeaders();
                app.UseHsts();
                app.UseHttpsRedirection();
            }
            else {
                app.UseDeveloperExceptionPage();
            }

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

            if (Debugger.IsAttached) {
                app.UseCors(builder => builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin()
                .AllowCredentials()
                );
            }

            app.UseMvc();
            app.UseSignalR(routes => {
                //routes.MapHub<UpdateHub>("updates");
            });
        }
    }
}
