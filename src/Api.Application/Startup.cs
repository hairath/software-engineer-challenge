using System;
using System.Collections.Generic;
using System.IO;
using Api.Application.Domain.Security;
using Api.Application.Services;
using API.PP.Data.Repository;
using API.PP.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace API.PP
{
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
            //Inject dependency
            services.AddScoped<IRepository, Repository>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IUserService, UserService>();

            var signingConfigurations = new SigningConfigurations();
            services.AddSingleton(signingConfigurations);

            var tokenConfigurations = new TokenConfigurations();
            new ConfigureFromConfigurationOptions<TokenConfigurations>(
                Configuration.GetSection("TokenConfigurations")).Configure(tokenConfigurations);

            services.AddSingleton(tokenConfigurations);
            services.AddControllers();
            //End Inject dependency

            //Setting Authentication
            services.AddAuthentication(opts =>
            {
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(bopts =>
            {
                var paramValidation = bopts.TokenValidationParameters;
                paramValidation.IssuerSigningKey = signingConfigurations.Key;
                paramValidation.ValidAudience = Configuration["TokenConfigurations:Audience"];
                paramValidation.ValidIssuer = Configuration["TokenConfigurations:Issuer"];
                paramValidation.ValidateIssuerSigningKey = true;
                paramValidation.ValidateLifetime = true;
                paramValidation.ValidateIssuer = true;
                paramValidation.ValidateAudience = true;
                paramValidation.ClockSkew = TimeSpan.FromMinutes(1);
            });

            services.AddAuthorization(aut =>
            {
                aut.AddPolicy(
                    "Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build());
            });
            //End Setting Authentication

            //setting middleware swagger
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "API PP Teste",
                    Description = "API com .Net Core 3.1",
                    Contact = new OpenApiContact
                    {
                        Name = "Haira Alves",
                        Email = "haira.alves@live.com"
                    }
                });

                //add description
                x.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "API.PP.xml"));

                //Adicionando autenticação para endpoint
                x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Entre com o Token JWT",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                x.AddSecurityRequirement(new OpenApiSecurityRequirement
                 {
                     {
                         new OpenApiSecurityScheme
                         {
                             Reference = new OpenApiReference
                             {
                                 Id = "Bearer",
                                 Type = ReferenceType.SecurityScheme
                             }
                         }, new List<string>()
                     }
                 });
            });
            //end settings middleware swagger

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "API com .Net Core 3.1");
                x.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
