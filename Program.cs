using FairMount_api.Application.Interfaces;
using FairMount_api.Data;
using FairMount_api.Interfaces;
using FairMount_api.Repository; // Keep this one for CommercialInvoiceRepository
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddScoped<IOrdermanagementRepository, OrderManagementRepository>();
builder.Services.AddScoped<IUserManagementRepository, UserManagementRepository>();
builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
builder.Services.AddScoped<ITypeManagementRepository, TypeManagementRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoicesRepository>();
builder.Services.AddScoped<ICommercialInvoiceRepository, CommercialInvoiceRepository>();
builder.Services.AddScoped<IPackingListRepository, PackingListRepository>();
builder.Services.AddScoped<ISLIRepository, SLIRepository>();


builder.Services.AddHttpContextAccessor();

// DB Connection
var connectionString = builder.Configuration.GetConnectionString("FairMountDbContext");

builder.Services.AddDbContext<FairmountDbContext>(options =>
  options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
     .EnableSensitiveDataLogging()
     .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));



// CORS
builder.Services.AddCors(p => p.AddPolicy("corsapp", corsBuilder =>
{
    corsBuilder.WithOrigins(
        "http://localhost:4200",
        "https://localhost:5297",
        "http://localhost:5224",
        "https://www.zdesire.com",
        "http://www.zdesire.com",
          "https://api.zdesire.com",
            "http://api.zdesire.com",
        "https://zdesire.com",
        "http://zdesire.com",
        "https://morsemonk.com"

      )
      .AllowAnyMethod()
      .AllowAnyHeader();
}));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//JWT configurations
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
  AddJwtBearer(options =>
  {
      options.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,

          ValidIssuer = builder.Configuration["jwt:Issuer"],
          ValidAudience = builder.Configuration["jwt:Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.
        Configuration["jwt:Key"])),
          ClockSkew = TimeSpan.Zero
      };
  });
builder.Services.AddAuthorization();
builder.Services.AddHttpClient();
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
// Middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/error");
}

//app.UseHttpsRedirection();

// Enable static file serving (optional, but prevents asset errors)
app.UseStaticFiles();

// Enable CORS
app.UseCors("corsapp");

// Auth and routing
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();