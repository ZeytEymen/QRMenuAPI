using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QRMenuAPI.Data;
using QRMenuAPI.Models;
using QRMenuAPI.Controllers;

namespace QRMenuAPI;

public class Program
{
    private readonly InitializeData _initializeData;
    public Program(InitializeData initializeData)
    {
        _initializeData = initializeData;
    }
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<ApplicationContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationDatabase"))); //eklendi

        //builder.Services.AddIdentityCore<ApplicationUser>().AddEntityFrameworkStores<ApplicationContext>(); //eklendi

        builder.Services.AddAuthentication();// eklendi

        builder.Services.AddAuthorization();

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();


        var app = builder.Build();


        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();

        app.UseAuthentication();//eklendi

        app.MapControllers();

        app.Run();
    }
}

