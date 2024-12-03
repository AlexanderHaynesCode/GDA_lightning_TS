namespace GDA_lightning_TS.Models;

public partial class User_GDA_Global
{
    public User_GDA_Global() 
    {
        
    }
    public User_GDA_Global(User_GDA user)
    {
        ID = user.Id;
        Username = user.Username;       
        LookingFor = user.LookingFor;
        ProfilePicPath = user.ProfilePicPath;
    }
    
    public int ID { get; set; }

    public string Username { get; set; } = null!;

    public string LookingFor { get; set; } = null!;  
    public string? ProfilePicPath { get; set; }    
}
