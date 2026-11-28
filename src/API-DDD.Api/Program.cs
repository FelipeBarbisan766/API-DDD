using API_DDD.Application.Services.Auth;
using API_DDD.Domain.Aggregates.Users;
using API_DDD.Domain.SeedWork;
using API_DDD.Domain.Services;
using API_DDD.Infrastructure.Persistence;
using API_DDD.Infrastructure.Persistence.Repositories;
using API_DDD.Infrastructure.Security;
using API_DDD.Middlewares;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<AppDbContext>());
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

builder.Services.AddOptions<GoogleAuthOptions>()
    .Bind(builder.Configuration.GetSection(GoogleAuthOptions.SectionName))
    .Validate(options => !string.IsNullOrWhiteSpace(options.ClientId), "Google ClientId is required.")
    .ValidateOnStart();
builder.Services.AddSingleton<IGoogleTokenValidator, GoogleTokenValidator>();

builder.Services.AddScoped<LoginHandler>();
builder.Services.AddScoped<SignUpHandler>();
builder.Services.AddScoped<GoogleLoginHandler>();
builder.Services.AddScoped<ICookieAuthService, CookieAuthService>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();