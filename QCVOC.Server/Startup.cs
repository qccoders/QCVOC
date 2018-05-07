namespace QCVOC.Server
{
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
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.PlatformAbstractions;
    using Microsoft.IdentityModel.Tokens;
    using NLog;
    using QCVOC.Server.Data.ConnectionFactory;
    using QCVOC.Server.Data.Model;
    using QCVOC.Server.Data.Repository;
    using QCVOC.Server.Middleware;
    using QCVOC.Server.Security;
    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;

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

            app.UseAuthentication();
            app.UseMiddleware<LoggingMiddleware>();

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IJwtFactory, JwtFactory>();
            services.AddSingleton<IDbConnectionFactory, NpgsqlDbConnectionFactory>(serviceProvider =>
                new NpgsqlDbConnectionFactory("User ID=QCVOC;Password=QCVOC;Host=SQL;Port=5432;Database=QCVOC;Pooling = true;"));
            services.AddTransient<IRepository<Account>, Repository<Account>>(serviceProvider =>
                new Repository<Account>(serviceProvider.GetService<IDbConnectionFactory>()));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = "QCVOC",
                        ValidateIssuer = true,
                        ValidAudience = "QCVOC",
                        ValidateAudience = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Utility.GetSetting<string>("JwtKey", "EE26B0DD4AF7E749AA1A8EE3C10AE9923F618980772E473F8819A5D4940E0DB27AC185F8A0E1D5F84F88BC887FD67B143732C304CC5FA9AD8E6F57F50028A8FF"))),
                        ValidateIssuerSigningKey = true,
                    };
                });

            services.AddMvc();

            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(2, 0);
            });

            services.AddMvcCore().AddVersionedApiExplorer(o =>
            {
                o.GroupNameFormat = "'v'VVV";
                o.SubstituteApiVersionInUrl = true;
            });

            services.AddSwaggerGen(c =>
            {
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                foreach (var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
                }

                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var versions = apiDesc.ControllerAttributes()
                        .OfType<ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions);

                    return versions.Any(v => $"v{v.ToString()}" == docName);
                });

                c.IncludeXmlComments(GetXmlCommentsFilePath());

                var apiKeyScheme = new ApiKeyScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey",
                };

                c.AddSecurityDefinition("Bearer", apiKeyScheme);

                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", new string[] { } }
                });
            });
        }

        #endregion Public Methods

        #region Private Methods

        private static Info CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new Info()
            {
                Title = $"Sample API {description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
                Description = "A sample application with Swagger, Swashbuckle, and API versioning.",
                Contact = new Contact() { Name = "Bill Mei", Email = "bill.mei@somewhere.com" },
                TermsOfService = "Shareware",
                License = new License() { Name = "MIT", Url = "https://opensource.org/licenses/MIT" }
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
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