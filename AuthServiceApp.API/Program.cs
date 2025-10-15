using AuthServiceApp.API.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using AuthServiceApp.API.Data;
using Microsoft.EntityFrameworkCore;
using AuthServiceApp.API.Interfaces.Services;
using AuthServiceApp.API.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthServiceApp.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthorization();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        RequireExpirationTime = true,
                        RequireSignedTokens = true,
                    };

                    options.Events = new JwtBearerEvents {
                        OnTokenValidated = async (context) =>
                        {
                            var claims = context.Principal?.Claims.ToList();

                            var jti = claims?.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

                            if (string.IsNullOrEmpty(jti))
                            {
                                context.Fail("Empty user id in JWT-token");
                                return;
                            }

                            var dbContext = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();

                            var userId = claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                            if (string.IsNullOrEmpty(userId))
                            {
                                context.Fail("Empty user id in JWT-token");
                                return;
                            }
                            
                            var user = await dbContext.Users.FindAsync(Guid.Parse(userId));
                            if (user is null)
                            {
                                context.Fail("User not found");
                                return;
                            }

                            if (user!.Jti == null || user!.Jti != jti)
                            {
                                context.Fail("Invalid or revoked JTI");
                                return;
                            }
                        }
                    };
                });

            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
                    {
                        [JwtBearerDefaults.AuthenticationScheme] = new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.Http,
                            Scheme = JwtBearerDefaults.AuthenticationScheme,
                            In = ParameterLocation.Header,
                            BearerFormat = "JWT"
                        }
                    };

                    document.SecurityRequirements = new List<OpenApiSecurityRequirement>
                    {
                        new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "Bearer"
                                    }
                                },

                                new List<string>()
                            }
                        }
                    };

                    return Task.CompletedTask;
                });
            });
            builder.Services.AddControllers();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite(builder.Configuration.GetConnectionString("Database"));
            });

            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();

            var app = builder.Build();
           
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI v1");
                });
            }

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
