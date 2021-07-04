using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthBasics.Authorization_Requirements;
using AuthBasics.Controllers;
using AuthBasics.Transformer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace AuthBasics
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("CookieAuth")
                .AddCookie("CookieAuth", config =>
                    {
                        config.Cookie.Name = ("CarolineCookie");
                        config.LoginPath = "/Home/Authenticate";
                    });

            services.AddAuthorization(config =>
            {
                //var defaultAuthBuilder = new AuthorizationPolicyBuilder();
                //var defaultAuthPolicy = defaultAuthBuilder
                //.RequireAuthenticatedUser()
                //.RequireClaim(ClaimTypes.DateOfBirth)
                //.Build();

                //config.DefaultPolicy = defaultAuthPolicy;

                //config.AddPolicy("Claim.DOB", policyBuidler =>
                //{
                //    policyBuidler.RequireClaim(ClaimTypes.DateOfBirth);
                //});
                config.AddPolicy("Admin", policyBuilder => policyBuilder.RequireClaim(ClaimTypes.Role, "Admin"));

                config.AddPolicy("Claim.DOB", policyBuidler =>
                {
                policyBuidler.AddRequirements(new CustomRequireClaim(ClaimTypes.DateOfBirth));
                });

            });

            services.AddScoped<IAuthorizationHandler, CustomRequireClaimHandler>();
            services.AddScoped<IAuthorizationHandler, CookieJarAuthorizationHandler>();
            services.AddScoped<IClaimsTransformation, ClaimsTransformation>();

            services.AddControllersWithViews(config =>
            {

                var defaultAuthBuilder = new AuthorizationPolicyBuilder();
                var defaultAuthPolicy = defaultAuthBuilder
                .RequireAuthenticatedUser()
                .Build();

                // Global Authorization Filter
                //config.Filters.Add(new AuthorizeFilter(defaultAuthPolicy));
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            //Next line asks: who are you? Always ask this question first
            app.UseAuthentication();

            //Next line asks: are you allowed?
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
