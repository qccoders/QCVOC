using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using NLog;
using Npgsql;
using QCVOC.Server.Data.ConnectionFactory;
using QCVOC.Server.Data.Repository;
using QCVOC.Server.Middleware;
using QCVOC.Server.Security;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace QCVOC.Server
{
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

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "QCVOC API V1");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "QCVOC API V2");
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<IJwtFactory, JwtFactory>();
            services.AddSingleton<IDbConnectionFactory, NpgsqlDbConnectionFactory>(serviceProvider =>
                new NpgsqlDbConnectionFactory("User ID=QCVOC;Password=QCVOC;Host=SQL;Port=5432;Database=QCVOC;Pooling = true;"));

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

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "QCVOC API V1", Version = "v1" });
                c.SwaggerDoc("v2", new Info { Title = "QCVOC API V2", Version = "v2" });

                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var versions = apiDesc.ControllerAttributes()
                        .OfType<ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions);

                    return versions.Any(v => $"v{v.ToString()}" == docName);
                });

                var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "QCVOC.Server.xml");
                c.IncludeXmlComments(filePath);

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });

                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", new string[] { } }
                });
            });
        }

        #endregion Public Methods
    }
}