using Microsoft.EntityFrameworkCore;
using EmployeeGrievanceRedressal;
using EmployeeGrievanceRedressal.Interfaces;
using EmployeeGrievanceRedressal.Repositories;

namespace EmployeeGrievanceRedressal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Add DbContext configuration
            builder.Services.AddDbContext<EmployeeGrievanceContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            #region Repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IGrievanceRepository, GrievanceRepository>();
            builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            #endregion

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
