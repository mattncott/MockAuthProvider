using System.Diagnostics.CodeAnalysis;
using Mattncott;
using Microsoft.EntityFrameworkCore;
using MockAuthProvider.Services;
using MockAuthProvider.Services.Interfaces;
using OpenIddict.Abstractions;

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

            services.AddCors(options =>
                {
                    options.AddPolicy("AllowAll", builder =>
                        builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());
                });

            services.AddDbContext<DbContext>(options =>
            {
                // Configure Entity Framework Core to use Microsoft SQL Server.
                options.UseNpgsql(
                    Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("MockAuthProvider"));

                // Register the entity sets needed by OpenIddict.
                // Note: use the generic overload if you need to replace the default OpenIddict entities.
                options.UseOpenIddict();
            });

            services.AddOpenIddict()
                // Register the OpenIddict core components.
                .AddCore(options =>
                {
                    // Configure OpenIddict to use the Entity Framework Core stores and models.
                    // Note: call ReplaceDefaultEntities() to replace the default entities.
                    options.UseEntityFrameworkCore()
                        .UseDbContext<DbContext>();
                });

            services.AddOpenIddict()
                // Register the OpenIddict server components.
                .AddServer(options =>
                {
                    options.SetTokenEndpointUris("connect/token");
                    options.SetAuthorizationEndpointUris("connect/authorize");
                    options.SetUserInfoEndpointUris("connect/userinfo");

                    options.RegisterScopes(
                        OpenIddictConstants.Scopes.Profile, 
                        OpenIddictConstants.Scopes.Email, 
                        OpenIddictConstants.Scopes.OfflineAccess);


                    options.AllowClientCredentialsFlow();
                    options.AllowAuthorizationCodeFlow();
                    options.AllowRefreshTokenFlow();

                    // Register the signing and encryption credentials.
                    options.AddDevelopmentEncryptionCertificate()
                        .AddDevelopmentSigningCertificate();

                    options.DisableAccessTokenEncryption();

                    options.UseAspNetCore()
                        .EnableTokenEndpointPassthrough()
                        .EnableAuthorizationEndpointPassthrough()
                        .EnableUserInfoEndpointPassthrough();
                });

            services.AddHostedService<Worker>();
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

            app.UseCors("AllowAll");

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(options =>
            {
                options.MapControllers();
                options.MapDefaultControllerRoute();
            });
        }
    }
}
