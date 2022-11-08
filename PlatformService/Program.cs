using System;
using System.Security.AccessControl;
using PlatformService.Data;
using Microsoft.EntityFrameworkCore;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddDbContext<AppDBContext>(opt =>
    {
        if (builder.Environment.IsProduction())
        {
            Console.WriteLine("--> Using SqlServer Db");
            opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn"));
        }
        else
        {
            Console.WriteLine("--> Using InMem Db");
            opt.UseInMemoryDatabase("InMem");
        }
    }
);

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();

builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

PrepDb.PrepPopulation(app, app.Environment.IsProduction());

Console.WriteLine($"--> CommandService EndPoint {app.Configuration["CommandService"]}");

app.Run();
