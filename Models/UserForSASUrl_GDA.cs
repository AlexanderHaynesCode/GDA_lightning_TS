
namespace GDA_lightning_TS.Models;

// This model is almost identical to the User_GDA.cs model, except this UserForSASUrl_GDA model 
//  supports variables that hold the profile picture file and filename, to be used for generating
//  a SAS URL for the picture/blob and assigning the URL to the ProfilePicPath column in
//  the corresponding User_GDA.cs table by User ID.
public partial class UserForSASUrl_GDA
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int Gender { get; set; }
    // 1 = male, 2 = female

    public string LookingFor { get; set; } = null!;    

    public DateTime DateCreated { get; set; }
    public IFormFile? ProfilePicFile { get; set; }  
    public string? ProfilePicFileName { get; set; }  
    public int Age { get; set; }
}
