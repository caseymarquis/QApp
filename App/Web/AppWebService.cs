using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace App.Web
{
    public class AppWebService
    {
        public AppWebService(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddMvc();
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new Info { Title = "QApp API", Version = "v1" });
            });
            services.AddSignalR();
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
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
            if (env.IsDevelopment()) {
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
