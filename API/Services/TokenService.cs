using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
  public class TokenService
  {

    private readonly IConfiguration _config;
    public TokenService(IConfiguration config) // access to our configuration
    {
      _config = config; 

    }

    public string CreateToken(AppUser user) // arg AppUser and we call it user
    {
      var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
        };

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenKey"]));

      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

      var tokenDescripteor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddDays(7),
        SigningCredentials = creds
      };

      var tokenHandler = new JwtSecurityTokenHandler();
      var token = tokenHandler.CreateToken(tokenDescripteor);
      return tokenHandler.WriteToken(token);

    }
  }
}


// CREATING TOKEN
// WE use this Claim to authenticate api
// Claims are smt that users claim about themself
// i claim that my name is jane for example
// use identity server for a large apps auth services