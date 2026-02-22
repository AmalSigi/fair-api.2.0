using FairMount_api.Data;
using FairMount_api.Interfaces;
using FairMount_api.Models;
using FairMount_api.Models.Tables;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;

namespace FairMount_api.Repository
{
  public class UserManagementRepository : IUserManagementRepository
  {
    private readonly FairmountDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _contextAccessor;

    public UserManagementRepository(FairmountDbContext context,IConfiguration configuration, IHttpContextAccessor contextAccessor)
    {
      _context = context;
      _configuration = configuration;
      _contextAccessor = contextAccessor;
    }
    public async Task<int> CreateUser(User user)
    {
      var passwordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
      user.PasswordHash=passwordHash;
      _context.Users.Add(user);
      await _context.SaveChangesAsync();
      return 0;
    }
   


    public async Task<int> Login(UserDto user)
    {
      var exisitingUser = _context.Users
          .FirstOrDefault(u => u.Email.ToLower() == user.Email.ToLower());
      if (exisitingUser == null)
      {
        return 0;
      }
      var passwordMatch = BCrypt.Net.BCrypt.Verify(user.Password, exisitingUser.PasswordHash);
      if (!passwordMatch)
      {
        return -1;
      }
      var token =await GenerateJwtToken(exisitingUser);
      // Store token in HttpOnly cookie
      var cookieOptions = new CookieOptions
      {
        HttpOnly = true,  // cannot be accessed from JavaScript
        Secure = true,    // use HTTPS only
        SameSite = SameSiteMode.None, // prevent CSRF
        Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:LifeTime"]))
      };

      _contextAccessor.HttpContext.Response.Cookies.Append("AuthToken", token, cookieOptions);

      return exisitingUser.UserId;
    }

    //jwt token generation
    private async Task<String> GenerateJwtToken(User user)
    {
     var jwtKey = _configuration["Jwt:Key"];
      var jwtIssuer = _configuration["Jwt:Issuer"];
      var jwtAudience = _configuration["Jwt:Audience"];
      var jwtLifeTime = int.Parse(_configuration["Jwt:LifeTime"]);

      var claims = new[]
             {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
            };
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
      var credentials =new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
      var token = new JwtSecurityToken(issuer:jwtIssuer,
        audience:jwtAudience,
        claims:claims,
        expires:DateTime.UtcNow.AddMinutes(jwtLifeTime),
        signingCredentials: credentials);

      var jwt =new JwtSecurityTokenHandler().WriteToken(token);
      return "bearer " + jwt;
         
    }
  }

}
