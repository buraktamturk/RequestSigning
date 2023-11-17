using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Tamturk.AspNetCore.RequestSigning.SampleWeb {
    public class Startup {
        private readonly IConfiguration Configuration;

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services) {
            services
                .AddRequestSigning(Configuration["signingKey"]) // use HMACSHA256 with this key that is taken from appconfig
                .AddInMemoryRevokedHashTable() // use in memory table to store revoked tokens (optional!)
                .AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            
            app
                .UseRouting()
                .UseEndpoints(a => a.MapControllers());
        }
    }
}