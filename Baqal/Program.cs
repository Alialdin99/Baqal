using Baqal.DataContext;
using Baqal.Entities.Models;
using Baqal.DataAccess.ExtensionMethods;
using Baqal.Application.ExtensionMethods;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Baqal.DataAccess.Interfaces;
using Baqal.DataAccess;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Services
builder.Services.AddService();
builder.Services.AddRepository();
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();


builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
