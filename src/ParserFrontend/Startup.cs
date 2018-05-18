using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ParserFrontend.Logic;
using PdfTextReader;
using PdfTextReader.Azure.Queue;

namespace ParserFrontend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string azureStorage = Configuration["PDFTEXTREADER_FRONTEND_STORAGE"];

            string configFullAccess = Configuration["PDFTEXTREADER_FRONTEND_FULLACCESS"];
            bool hasFullAccess = (configFullAccess != null) && Boolean.Parse(configFullAccess);

            string azureQueueConfig = Configuration["PDFTEXTREADER_FRONTEND_QUEUE"];

            IVirtualFS2 virtualFS = (String.IsNullOrEmpty(azureStorage)) ?
                (IVirtualFS2)new WebVirtualFS() : new AzureFS(azureStorage);

            if(azureQueueConfig != null)
            {
                services.AddSingleton<JobManager>(new JobManager(AzureQueue.OpenAsync(azureQueueConfig).Result));
            }

            services.AddSingleton(new AccessManager(virtualFS, hasFullAccess));
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseMvc();
        }
    }
}
