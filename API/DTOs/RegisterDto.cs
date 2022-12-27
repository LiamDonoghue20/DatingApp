
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDto
    {
        [Required] public string Username { get; set;}
        [Required] public string KnowAs {get; set;}
        [Required] public string Gender {get; set;}
        //dateOnly as optional so it can be null when defined, if it isnt optional it'll automatically use todays date
        [Required] public DateOnly? DateOfBirth {get; set;} //optional to make required work!
        [Required] public string City {get; set;}
        [Required] public string Country {get; set;}

        [Required] [StringLength(8, MinimumLength = 4)]public string Password { get; set;}
    }
}