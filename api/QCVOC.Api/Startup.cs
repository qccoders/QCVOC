// <copyright file="Startup.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
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
    using QCVOC.Api.Common;
    using QCVOC.Api.Common.Data.ConnectionFactory;
    using QCVOC.Api.Common.Data.Repository;
    using QCVOC.Api.Common.Middleware;
    using QCVOC.Api.Events.Data.Model;
    using QCVOC.Api.Events.Data.Repository;
    using QCVOC.Api.Security;
    using QCVOC.Api.Security.Data.Model;
    using QCVOC.Api.Security.Data.Repository;
    using QCVOC.Api.Services.Data.Repository;
    using QCVOC.Api.Veterans.Data.Model;
    using QCVOC.Api.Veterans.Data.Repository;
    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using Swashbuckle.AspNetCore.SwaggerUI;
    using Utility = Common.Utility;

    /// <summary>
    ///     The AspNetCore Startup configuration.
    /// </summary>
    public class Startup
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">Configuration properties for the application.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        ///     Gets onfiguration properties for the application.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        ///     Configures middleware for the application.
        /// </summary>
        /// <param name="app">The default application builder.</param>
        /// <param name="env">The hosting environment.</param>
        /// <param name="provider">The API version information provider.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider provider)
        {
            app.UseExceptionMiddleware(options => options.Verbosity = ExceptionMiddlwareVerbosity.Terse);

            app.UseLogger();

            app.UseAuthentication();
            app.UseCors("AllowAll");

            app.UseMvc();

            app.UseSwagger(options => ConfigureSwaggerOptions(options));
            app.UseSwaggerUI(options => ConfigureSwaggerUIOptions(options, provider));
        }

        /// <summary>
        ///     Configures services for the application.
        /// </summary>
        /// <param name="services">The application services collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ITokenFactory, TokenFactory>();
            services.AddSingleton<ITokenValidator, TokenValidator>(serviceProvider =>
                new TokenValidator(GetTokenValidationParameters()));

            var connectionString = Utility.GetSetting<string>(Settings.DbConnectionString);

            services.AddSingleton<IDbConnectionFactory, NpgsqlDbConnectionFactory>(serviceProvider =>
                new NpgsqlDbConnectionFactory(connectionString));

            services.AddScoped<IRepository<Account>, AccountRepository>(serviceProvider =>
                new AccountRepository(serviceProvider.GetService<IDbConnectionFactory>()));
            services.AddScoped<IRepository<RefreshToken>, RefreshTokenRepository>(serviceProvider =>
                new RefreshTokenRepository(serviceProvider.GetService<IDbConnectionFactory>()));
            services.AddScoped<IRepository<Veteran>, VeteranRepository>(serviceProvider =>
                new VeteranRepository(serviceProvider.GetService<IDbConnectionFactory>()));
            services.AddScoped<IRepository<Services.Data.Model.Service>, ServiceRepository>(serviceProvider =>
                new ServiceRepository(serviceProvider.GetService<IDbConnectionFactory>()));
            services.AddScoped<IRepository<Event>, EventRepository>(serviceProvider =>
                new EventRepository(serviceProvider.GetService<IDbConnectionFactory>()));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => ConfigureJwtBearerOptions(options));

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

        private static void ConfigureJwtBearerOptions(JwtBearerOptions options)
        {
            options.TokenValidationParameters = GetTokenValidationParameters();
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

        private static void ConfigureSwaggerOptions(SwaggerOptions options)
        {
            string camelCase(string key) =>
                string.Join('/', key.Split('/').Select(x => x.Contains("{") || x.Length < 2 ? x : char.ToLowerInvariant(x[0]) + x.Substring(1)));

            options.PreSerializeFilters.Add((document, request) =>
            {
                document.Paths = document.Paths.ToDictionary(p => camelCase(p.Key), p => p.Value);

                document.Paths.ToList()
                    .ForEach(path => typeof(PathItem).GetProperties().Where(p => p.PropertyType == typeof(Operation)).ToList()
                        .ForEach(operation => ((Operation)operation.GetValue(path.Value, null))?.Parameters.ToList()
                            .ForEach(prop => prop.Name = camelCase(prop.Name))));
            });
        }

        private static void ConfigureSwaggerUIOptions(SwaggerUIOptions options, IApiVersionDescriptionProvider provider)
        {
            var root = Utility.GetSetting<string>(Settings.AppRoot);

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
            var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
            var basePath = AppContext.BaseDirectory;

            return Path.Combine(basePath, fileName);
        }
    }
}