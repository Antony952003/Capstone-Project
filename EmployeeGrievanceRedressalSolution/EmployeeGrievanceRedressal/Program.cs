using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using EmployeeGrievanceRedressal.Repositories;
using Microsoft.EntityFrameworkCore;
using EmployeeGrievanceRedressal.Services;
using EmployeeGrievanceRedressal.Interfaces.RepositoryInterfaces;
using EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces;
using EmployeeGrievanceRedressal.Models.AzureConfiguration;
using Microsoft.Extensions.Configuration;
using Azure.Identity;

namespace EmployeeGrievanceRedressal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            var keyVaultUri = new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/");
            builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());


            builder.Services.Configure<AzureBlobStorageSettings>(options =>
            {
                options.ConnectionString = builder.Configuration["AzureBlobStorageConnectionString"];
                options.ContainerName = builder.Configuration["AzureBlobStorageContainerName"];
            });

            builder.Services.AddSingleton<BlobStorageService>();


            builder.Services.AddControllers();

            // Add DbContext configuration
            builder.Services.AddDbContext<EmployeeGrievanceContext>(options =>
                options.UseSqlServer(builder.Configuration["DefaultConnection"]));

            #region Repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IGrievanceRepository, GrievanceRepository>();
            builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            builder.Services.AddScoped<IApprovalRequestRepository, ApprovalRequestRepository>();
            builder.Services.AddScoped<ISolutionRepository, SolutionRepository>();
            builder.Services.AddScoped<IGrievanceHistoryRepository, GrievanceHistoryRepository>();
            builder.Services.AddScoped<IRatingRepository, RatingRepository>();
            #endregion

            #region Services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IApprovalRequestService, ApprovalRequestService>();
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddScoped<IGrievanceService, GrievanceService>();
            builder.Services.AddScoped<ISolutionService, SolutionService>();
            builder.Services.AddScoped<IFeedbackService, FeedbackService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IGrievanceHistoryService, GrievanceHistoryService>();
            builder.Services.AddScoped<IRatingService, RatingService>();
            #endregion

            // Configure JWT Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["TokenKeyJWT"]))
                    };
                });

            // Add Authorization
            builder.Services.AddAuthorization();
            builder.Services.AddHttpContextAccessor();
            // Configure Swagger with JWT support
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EmployeeGrievanceRedressal API", Version = "v1" });

                // Add security definitions for JWT
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                        Array.Empty<string>()
                    }
                });
            });
            #region Cors
            builder.Services.AddCors(opts =>
            {
                opts.AddPolicy("MyCors", options =>
                {
                    options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                });
            });
            #endregion

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("MyCors");

            // Use Authentication
            app.UseAuthentication();

            // Use Authorization
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
