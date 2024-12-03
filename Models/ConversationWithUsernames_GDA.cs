
namespace GDA_lightning_TS.Models;

public partial class ConversationWithUsernames_GDA
{
    public ConversationWithUsernames_GDA () 
    {
        
    }
    public ConversationWithUsernames_GDA(int Person1__id, int Person2__id)
    {
        Person1_ID = Person1__id;   
        Person2_ID = Person2__id;
    }
    
    public int Conversation_ID { get; set; }

    public int Person1_ID { get; set; }

    public int Person2_ID { get; set; }

    public bool Person1_InConvo { get; set; } = true;

    public bool Person2_InConvo { get; set; } = true;
    public string Person1_Username { get; set; } = "";

    public string Person2_Username { get; set; } = "";
}
