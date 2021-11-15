using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using HtmlPdfApi.Helpers.Exceptions;
using HtmlPdfApi.Helpers.HealthChecks;
using HtmlPdfApi.Helpers.Http;
using HtmlPdfApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace HtmlPdfApi
{
    [ExcludeFromCodeCoverage]
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
            // Singleton, transient, scoped
            services.AddSingleton<ResourceService>();
            services.AddSingleton<HandlebarsService>();
            services.AddSingleton<HtmlPdfService>();

            // Http base client
            services.AddHttpClient("default")
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientCertificateHandler(Configuration.GetValue<bool>("CERTIFICATE_VALIDATION_ENABLED")));

            // Auth enabled
            if (Configuration.GetValue<bool>("OIDC_AUTH_ENABLED") == true)
            {
                IdentityModelEventSource.ShowPII = true; //Add this line
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                        {
                            options.Authority = Configuration.GetValue<string>("OIDC_ISSUER");
                            options.RequireHttpsMetadata = Configuration.GetValue<bool>("CERTIFICATE_VALIDATION_ENABLED");
                            options.BackchannelHttpHandler = new HttpClientCertificateHandler(Configuration.GetValue<bool>("CERTIFICATE_VALIDATION_ENABLED"));
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateAudience = false,
                                ValidateIssuer = true,
                                ValidateIssuerSigningKey = true,
                                ValidIssuer = Configuration.GetValue<string>("OIDC_ISSUER")
                            };
                        });
            }
            // If auth disabled returns true to avery [Authorize] request
            else
            {
                services.AddAuthorization(x =>
                    {
                        x.DefaultPolicy = new AuthorizationPolicyBuilder()
                            .RequireAssertion(_ => true)
                            .Build();
                    });
            }

            // Health checks
            services.AddHealthChecks()
                .AddCheck<KeycloakHealthCheck>("keycloak_health_check");

            // Controllers
            services.AddControllers().AddNewtonsoftJson();

            // Swagger
            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "Investigations API",
                        Description = "Investigations API description",
                        Contact = new OpenApiContact
                        {
                            Name = "Faberbee S.r.l.",
                            Email = "support@faberbee.com",
                            Url = new Uri("https://www.faberbee.com"),
                        }
                    });
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Investigations API V1");
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<ExceptionHandlerMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
            });
        }
    }
}
