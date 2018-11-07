using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BolNMS.Mobile.Repository;
using BolNMS.Mobile.Core.IRepository;
using BolNMS.Mobile.Service;
using BolNMS.Mobile.Core.IService;
using BolNMS.Mobile.Common.ICache;
using BolNMS.Mobile.Core.Cache;
using BolNMS.Mobile.Common;
using Elmah.Io.Extensions.Logging;
using BolNMS.Mobile.Common.MiddleWares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BolNMS.Mobile.Api
{
    public class Startup
    {

        private string _elmahAPIKey;
        private string _elmahLogId;
        private int year = 365;
        private string appSettings = HelperUtility.appSettings;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _elmahAPIKey = Configuration.GetSection(appSettings)["ElmahAPIKey"];
            _elmahLogId = Configuration.GetSection(appSettings)["ElmahLogId"];
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IFilterRepository, FilterRepository>();
            services.AddScoped<INewsFileRepository, NewsFileRepository>();
            services.AddScoped<IUserDeviceInfoRepository, UserDeviceInfoRepository>();

            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IFilterService, FilterService>();
            services.AddScoped<INewsFileService, NewsFileService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserDeviceInfoService, UserDeviceInfoService>();

            services.AddSingleton<ICacheManager, CacheManager>();
            services.AddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor, Microsoft.AspNetCore.Http.HttpContextAccessor>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwtBearerOptions =>
            {
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection(appSettings)["JWTSecurityKey"])),

                    ValidateIssuer = true,
                    ValidIssuer = Configuration.GetSection(appSettings)["Issuer"],

                    ValidateAudience = true,
                    ValidAudience = Configuration.GetSection(appSettings)["Audience"],

                    ValidateLifetime = true, //validate the expiration and not before values in the token

                    ClockSkew = TimeSpan.FromDays(year) //5 minute tolerance for the expiration date
                };
            });


            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddElmahIo(_elmahAPIKey, new Guid(_elmahLogId));

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            app.UseMvc();
        }
    }
}
