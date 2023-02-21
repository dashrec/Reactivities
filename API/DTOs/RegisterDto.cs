using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
  public class RegisterDto
  {
    [Required]
    [EmailAddress] //tell it that it has a type of email adress
    public string Email { get; set; }

    [Required]
    [RegularExpression("(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{4,8}$", ErrorMessage = "Pasword must be complex")]
    public string Password { get; set; }
    [Required]
    public string DisplayName { get; set; }

    [Required]
    public string Username { get; set; }
  }
}

// Data Transfer Object
// .* <- represents any caracter in password
// \\d <- represents that one of them must be a number 
// .*[a-z] <- at least one of our carracter needs to be lower case
// needs to be between 4,8 char long  and finish with $