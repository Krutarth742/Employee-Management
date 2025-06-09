
using Employee_Management_Backend.CustomHandler;
using Employee_Management_Backend.Data;
using Employee_Management_Backend.Models;
using Employee_Management_Backend.Repositories;
using Employee_Management_Backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Employee_Management_Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
               .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders();

            // JWT Token Authentication 
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                //options.TokenValidationParameters = new TokenValidationParameters
                //{
                //    ValidateIssuer = true,
                //    ValidateAudience = true,
                //    ValidateLifetime = true,
                //    ValidateIssuerSigningKey = true,
                //    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                //    ValidAudience = builder.Configuration["Jwt:Audience"],
                //    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                //};
                options.Authority = "https://dev-8v7vvlbu53evh5jt.us.auth0.com/";
                options.Audience = "http://localhost:5235";
            });
            //.AddGoogle(option =>
            //{
            //    option.ClientId = builder.Configuration["Authentication:Google:ClientId"];
            //    option.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
            //    option.SignInScheme = IdentityConstants.ExternalScheme;
            //});

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp", builder =>
                {
                    builder.WithOrigins("http://localhost:4200", "https://localhost:4200")
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });
            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseMiddleware<CustomJwtHandler>();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("AllowAngularApp");
            app.MapControllers();
            //app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.Run();
        }
    }
}
