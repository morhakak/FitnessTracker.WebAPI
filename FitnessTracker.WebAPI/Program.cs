using FitnessTracker.WebAPI.Data;
using FitnessTracker.WebAPI.Models.Domain;
using FitnessTracker.WebAPI.Repositories.Interfaces;
using FitnessTracker.WebAPI.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<FitnessTrackerDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("FitnessTrackerConnectionString"));
});

builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<FitnessTrackerDbContext>()
    .AddDefaultTokenProviders()
    .AddTokenProvider<DataProtectorTokenProvider<User>>("Fitness");

builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredUniqueChars = 1;
});

builder.Services.AddScoped<IAuthRepository, SQLAuthRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
