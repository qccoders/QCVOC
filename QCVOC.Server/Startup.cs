// <copyright file="Startup.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Server
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.AspNetCore.Mvc.Versioning;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.PlatformAbstractions;
    using Microsoft.IdentityModel.Tokens;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using NLog;
    using QCVOC.Server.Data.ConnectionFactory;
    using QCVOC.Server.Data.Model.Security;
    using QCVOC.Server.Data.Repository;
    using QCVOC.Server.Middleware;
    using QCVOC.Server.Security;
    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using Swashbuckle.AspNetCore.SwaggerUI;

    public class Startup
    {
        #region Private Fields

        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        #endregion Private Fields

        #region Public Constructors

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #endregion Public Constructors

        #region Public Properties

        public IConfiguration Configuration { get; }

        #endregion Public Properties

        #region Public Methods

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<LoggingMiddleware>();
            app.UseAuthentication();
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

            services.AddScoped<IRepository<Account>, Repository<Account>>(serviceProvider =>
                new Repository<Account>(serviceProvider.GetService<IDbConnectionFactory>()));
            services.AddScoped<IRepository<RefreshToken>, Repository<RefreshToken>>(serviceProvider =>
                new Repository<RefreshToken>(serviceProvider.GetService<IDbConnectionFactory>()));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => GetTokenValidationParameters());

            services.AddMvc()
                .AddJsonOptions(options => ConfigureJsonOptions(options));
            services.AddMvcCore()
                .AddVersionedApiExplorer(options => ConfigureApiExplorerOptions(options));

            services.AddApiVersioning(options => ConfigureApiVersioningOptions(options));

            services.AddSwaggerGen(options => ConfigureSwaggerGenOptions(options, services));
        }

        #endregion Public Methods

        #region Private Methods

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
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
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
            var basePath = PlatformServices.Default.Application.ApplicationBasePath;
            var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
            return Path.Combine(basePath, fileName);
        }

        #endregion Private Methods
    }
}