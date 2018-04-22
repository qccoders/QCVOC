using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using QCVOC.Server.Data.Repository;
using QCVOC.Server.Security;
using System.Data;
using System.Runtime.ExceptionServices;
using System.Security.Claims;
using System.Text;

namespace QCVOC.Server
{
    public class Startup
    {
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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseMvc();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
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

            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<IJwtFactory, JwtFactory>();
            services.AddTransient<IDbConnection, NpgsqlConnection>(serviceProvider => 
                new NpgsqlConnection("User ID=QCVOC;Password=QCVOC;Host=postgresql.celg76k5a9gh.us-east-1.rds.amazonaws.com;Port=5432;Database=QCVOC;Pooling = true;"));
        }

        #endregion Public Methods
    }
}