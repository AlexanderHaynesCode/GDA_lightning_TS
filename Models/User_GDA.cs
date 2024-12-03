
namespace GDA_lightning_TS.Models;

public partial class User_GDA
{
    public User_GDA () 
    {
        
    }
    public User_GDA(UserForSASUrl_GDA userSAS)
    {
        Username = userSAS.Username;   
        Password = userSAS.Password;
        Gender = userSAS.Gender;
        LookingFor = userSAS.LookingFor;
        DateCreated = userSAS.DateCreated;
        Age = userSAS.Age;        
    }
    
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    // 1 = male, 2 = female
    public int Gender { get; set; }    

    public string LookingFor { get; set; } = null!;    

    public DateTime DateCreated { get; set; }
    public string? ProfilePicPath { get; set; }    
    public int Age { get; set; }
}
