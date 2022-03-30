using System;
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
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure app configuration
builder.Host.ConfigureAppConfiguration(configurationBuilder =>
{
    DotNetEnv.Env.Load();
});

// Configure services
builder.Host.ConfigureServices(services =>
{
    var Configuration = builder.Configuration;

    // Singleton, transient, scoped
    services.TryAddSingleton<ResourceService>();
    services.TryAddSingleton<HandlebarsService>();
    services.TryAddSingleton<HtmlPdfService>();

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

});

// Configure app
var app = builder.Build();

#region CONFIGURE APP
if (app.Environment.IsDevelopment())
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
#endregion

app.Run();