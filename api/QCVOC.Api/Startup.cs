// <copyright file="Startup.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Cors.Infrastructure;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.AspNetCore.Mvc.Versioning;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using NLog;
    using QCVOC.Api.Data.ConnectionFactory;
    using QCVOC.Api.Data.Model;
    using QCVOC.Api.Data.Model.Security;
    using QCVOC.Api.Data.Repository;
    using QCVOC.Api.Middleware;
    using QCVOC.Api.Security;
    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using Swashbuckle.AspNetCore.SwaggerUI;

    public class Startup
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<LoggingMiddleware>();

            app.UseAuthentication();
            app.UseCors("AllowAll");

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(options => ConfigureSwaggerUIOptions(options, provider));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ITokenFactory, TokenFactory>();
            services.AddSingleton<ITokenValidator, TokenValidator>(serviceProvider =>
                new TokenValidator(GetTokenValidationParameters()));

            services.AddSingleton<IDbConnectionFactory, NpgsqlDbConnectionFactory>(serviceProvider =>
                new NpgsqlDbConnectionFactory("User ID=QCVOC;Password=QCVOC;Host=SQL;Port=5432;Database=QCVOC;Pooling = true;"));

            services.AddScoped<IRepository<Account>, AccountRepository>(serviceProvider =>
                new AccountRepository(serviceProvider.GetService<IDbConnectionFactory>()));
            services.AddScoped<IRepository<RefreshToken>, RefreshTokenRepository>(serviceProvider =>
                new RefreshTokenRepository(serviceProvider.GetService<IDbConnectionFactory>()));
            services.AddScoped<IRepository<Patron>, PatronRepository>(serviceProvider =>
                new PatronRepository(serviceProvider.GetService<IDbConnectionFactory>()));
            services.AddScoped<IRepository<Service>, ServiceRepository>(serviceProvider =>
                new ServiceRepository(serviceProvider.GetService<IDbConnectionFactory>()));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => GetTokenValidationParameters());
            services.AddCors(options => ConfigureCorsOptions(options));

            services.AddMvc()
                .AddJsonOptions(options => ConfigureJsonOptions(options));
            services.AddMvcCore()
                .AddVersionedApiExplorer(options => ConfigureApiExplorerOptions(options));

            services.AddApiVersioning(options => ConfigureApiVersioningOptions(options));

            services.AddSwaggerGen(options => ConfigureSwaggerGenOptions(options, services));
        }

        private static void ConfigureApiExplorerOptions(ApiExplorerOptions options)
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        }

        private static void ConfigureApiVersioningOptions(ApiVersioningOptions options)
        {
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(2, 0);
        }

        private static void ConfigureCorsOptions(CorsOptions options)
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials();
            });
        }

        private static void ConfigureJsonOptions(MvcJsonOptions options)
        {
            options.SerializerSettings.Converters.Add(new StringEnumConverter());
            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        }

        private static void ConfigureSwaggerGenOptions(SwaggerGenOptions options, IServiceCollection services)
        {
            var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
            }

            options.DocInclusionPredicate((docName, apiDesc) =>
            {
                var versions = apiDesc.ControllerAttributes()
                    .OfType<ApiVersionAttribute>()
                    .SelectMany(attr => attr.Versions);

                return versions.Any(v => $"v{v.ToString()}" == docName);
            });

            options.IncludeXmlComments(GetXmlCommentsFilePath());

            var apiKeyScheme = new ApiKeyScheme()
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = "header",
                Type = "apiKey",
            };

            options.AddSecurityDefinition("Bearer", apiKeyScheme);

            options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", new string[] { } }
                });
        }

        private static void ConfigureSwaggerUIOptions(SwaggerUIOptions options, IApiVersionDescriptionProvider provider)
        {
            var root = Utility.GetEnvironmentVariable("APP_ROOT");

            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint($"{root}/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
            }
        }

        private static Info CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new Info()
            {
                Title = $"QCVOC API v{description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
                Description = "An event management application for the Quad Cities Veteran's Outreach Center.",
                Contact = new Contact() { Name = "QC Coders", Email = "info@qccoders.org" },
                License = new License() { Name = "GPLv3", Url = "https://www.gnu.org/licenses/gpl-3.0.en.html" }
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }

        private static TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters
            {
                ClockSkew = TimeSpan.FromMinutes(5),
                RequireSignedTokens = true,
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ValidIssuer = Utility.GetSetting<string>(Settings.JwtIssuer),
                ValidateIssuer = true,
                ValidAudience = Utility.GetSetting<string>(Settings.JwtAudience),
                ValidateAudience = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Utility.GetSetting<string>(Settings.JwtKey))),
                ValidateIssuerSigningKey = true,
            };
        }

        private static string GetXmlCommentsFilePath()
        {
            var basePath = AppContext.BaseDirectory;
            var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
            return Path.Combine(basePath, fileName);
        }
    }
}