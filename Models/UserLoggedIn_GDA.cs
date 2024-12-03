
namespace GDA_lightning_TS.Models;

public partial class UserLoggedIn_GDA
{
    public UserLoggedIn_GDA () 
    {
    }
    public UserLoggedIn_GDA (int userid) 
    {
        User_ID = userid;        
    }

    public UserLoggedIn_GDA (int userid, bool isLoggedIn) 
    {
        User_ID = userid;    
        IsLoggedIn = isLoggedIn;
    }
    
    public int Id { get; set; }

    public int User_ID { get; set; }
    public bool IsLoggedIn { get; set; } = false;

    public DateTime? ExpiresOn { get; set; }
    public string? IPAddress { get; set; }
    
}
