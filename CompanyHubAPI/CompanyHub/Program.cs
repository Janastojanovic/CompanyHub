using CompanyHub.Data;
using CompanyHub.Models;
using CompanyHub.Services;
using CompanyHub.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Neo4jClient;
using StackExchange.Redis;
using System.Text;
//7108
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Konekcija")));

builder.Services.AddIdentity<AppUser, IdentityRole>(
    options =>
    {
        options.Password.RequiredUniqueChars = 0;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireLowercase = false;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddUserManager<UserManager<AppUser>>()
    .AddSignInManager<SignInManager<AppUser>>()
    .AddRoleManager<RoleManager<IdentityRole>>()
    .AddDefaultTokenProviders();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CORS",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:5500",
                                    "https://localhost:3000",
                                    "https://localhost:3001",
                                    "https://localhost:5500",
                                    "https://127.0.0.1:5500",
                                    "http://localhost:3000",
                                    "http://localhost:5100",
                                    "https://localhost:5100",
                                    "https://127.0.0.1:5100")
                                    .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                        // policy.AllowAnyOrigin();
                        // policy.AllowAnyHeader();
                    });
});

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = true;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = "https://localhost:5001",
        ValidAudience = "https://localhost:5001",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("7AHevdx6ac/zUhjT7+0O/G2M63itIwbM/JGRkkGpMT0=")),
        ClockSkew = TimeSpan.Zero
    };
});
//var multiplexer = ConnectionMultiplexer.Connect("redis-19108.c267.us-east-1-4.ec2.cloud.redislabs.com:19108"); //redis cloud
//builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);

var configurationOptions = new ConfigurationOptions
{
    EndPoints = { "redis-19108.c267.us-east-1-4.ec2.cloud.redislabs.com:19108" },
    Password = "Cq2V3zaCZlQYJcAoMKxyOybuynyqPmTS",
    // Add other configuration options if necessary
};

var multiplexer = ConnectionMultiplexer.Connect(configurationOptions);
builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);

var client = new BoltGraphClient("neo4j://localhost:7687", "neo4j", "trinity-wisdom-zero-child-arsenal-4101");
await client.ConnectAsync();
builder.Services.AddSingleton<IGraphClient>(client);


builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IAppUserService, AppUserService>();
builder.Services.AddScoped<IChatUserMappingService, ChatUserMappingService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IProjectTaskService, ProjectTaskService>();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminOrUserPolicy", policy => policy.RequireRole("ADMIN", "USER"));
//});
var app = builder.Build();
app.UseCors("CORS");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
