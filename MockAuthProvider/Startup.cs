using System.Diagnostics.CodeAnalysis;
using Mattncott;
using MockAuthProvider.Services;
using MockAuthProvider.Services.Interfaces;

namespace MockAuthProvider
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigurationHelper.Initialize(Configuration);

            services.AddTransient<IClientsService, ClientsService>();

            services.AddControllers();

            services.AddHttpContextAccessor();

            services.AddMemoryCache();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            
            app.UseRouting();

            app.UseHttpsRedirection();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private string? GetEnvironmentVar(string variableName)
            => Environment.GetEnvironmentVariable(variableName) ?? Configuration[variableName];
    }
}
