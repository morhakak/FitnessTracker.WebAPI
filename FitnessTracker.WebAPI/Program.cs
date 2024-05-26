using FitnessTracker.WebAPI.Data;
using FitnessTracker.WebAPI.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<FitnessTrackerDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("FitnessTrackerConnectionString"));
});

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<FitnessTrackerDbContext>()
    .AddDefaultTokenProviders()
    .AddTokenProvider<DataProtectorTokenProvider<User>>("Fitness"); ;

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
