
namespace GDA_lightning_TS.Models;

public partial class Message_GDA
{
    public Message_GDA() 
    {}
    public Message_GDA(int convoID, int fromID, int toID, string message)
    {
        Conversation_ID = convoID;   
        From_ID = fromID;
        To_ID = toID;
        Message = message;
    }
    
    public int Conversation_ID { get; set; }    

    public int Message_ID { get; set; }

    public int From_ID { get; set; }

    public int To_ID { get; set; }

    public DateTime DateCreated { get; set; } 
    public string Message { get; set; } 
    public bool To_ID_Read { get; set; } = false;
    public Conversation_GDA Conversation { get; set; }
}
