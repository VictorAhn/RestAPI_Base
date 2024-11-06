using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RestAPI_Base.Infrastructure;
using System.Net;
using System.Text;

namespace RestAPI_Base
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public static string ContentRoot { get; private set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            var jwtTokenConfig = Configuration.GetSection("jwtTokenConfig").Get<JwtTokenConfig>();
            jwtTokenConfig.Secret = ProjectInfo.SigningKey;
            jwtTokenConfig.Issuer = ProjectInfo.HostName;
            jwtTokenConfig.Audience = ProjectInfo.HostName;
            services.AddSingleton(jwtTokenConfig);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtTokenConfig.Issuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfig.Secret)),
                    ValidAudience = jwtTokenConfig.Audience,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/chathub")))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            services.AddSignalR(options => { options.MaximumReceiveMessageSize = 209715200; }); //200MB
            services.AddHttpClient();
            services.AddSingleton<IJwtAuthManager, JwtAuthManager>();
            services.AddHostedService<JwtRefreshTokenCache>();


            ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

            Console.WriteLine(System.AppDomain.CurrentDomain.BaseDirectory);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("User", new OpenApiInfo { Title = "[사용자] API", Version = $"{ProjectInfo.ServiceName}_v1" });
                c.SwaggerDoc("Admin", new OpenApiInfo { Title = "[관리자] API", Version = $"{ProjectInfo.ServiceName}_v1" });
                c.IncludeXmlComments(string.Format(@"{0}RestAPI_Base.xml", System.AppDomain.CurrentDomain.BaseDirectory));
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Sample API",
                    Description = "JWT Api",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer", // must be lower case
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };
                c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {securityScheme, new string[] { }}
                });
                c.TagActionsBy(apiDesc =>
                {
                    var controllerName = apiDesc.ActionDescriptor.RouteValues["controller"];
                    var pathName = apiDesc.ActionDescriptor.AttributeRouteInfo.Template;

                    if (pathName.Contains("api/Admin/"))
                    {
                        List<string> Pathes = pathName.Split('/').ToList();
                        if (Pathes.Count == 3) return new[] { controllerName };
                        else
                        {
                            Pathes.RemoveAt(0);
                            Pathes.RemoveAt(Pathes.Count - 1);
                            return new[] { Pathes.Last() };
                        }
                    }
                    return new[] { controllerName };
                });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
            });

            services.AddHttpContextAccessor();

        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ContentRoot = env.ContentRootPath;

            app.Use(async (context, next) =>
            {
                try
                {
                    await next.Invoke();
                }
                catch (Exception exception)
                {
                    context.Response.StatusCode = Microsoft.AspNetCore.Http.StatusCodes.Status502BadGateway;
                    return;
                }
            });

            if (env.IsDevelopment())
            {
            }

            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("./swagger/User/swagger.json", "[사용자] API");
                c.SwaggerEndpoint("./swagger/Admin/swagger.json", "[관리자] API");
                c.DocumentTitle = "PolyMath API";
                c.DisplayRequestDuration();
                c.RoutePrefix = string.Empty;
            });

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
            });
            app.UseRouting();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}
