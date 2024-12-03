
namespace GDA_lightning_TS.Models;

public partial class ConversationWithUnread_GDA
{
    public ConversationWithUnread_GDA () 
    {
        
    }
    public ConversationWithUnread_GDA(ConversationWithUsernames_GDA conversation)
    {
        Conversation_ID = conversation.Conversation_ID;
        Person1_ID = conversation.Person1_ID;
        Person2_ID = conversation.Person2_ID;
        Person1_InConvo = conversation.Person1_InConvo;
        Person2_InConvo = conversation.Person2_InConvo;
        Person1_Username = conversation.Person1_Username;
        Person2_Username = conversation.Person2_Username;
    }
    
    public int Conversation_ID { get; set; }

    public int Person1_ID { get; set; }

    public int Person2_ID { get; set; }

    public bool Person1_InConvo { get; set; } = true;

    public bool Person2_InConvo { get; set; } = true;
    public string Person1_Username { get; set; } = "";

    public string Person2_Username { get; set; } = "";
    public bool Has_Unread_Message { get; set; } = false;
}
