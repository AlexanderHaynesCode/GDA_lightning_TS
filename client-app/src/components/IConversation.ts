export interface IConversation {
    conversation_ID: number;
    person1_ID: number;
    person2_ID: number;
    person1_InConvo: boolean;
    person2_InConvo: boolean;
    person1_Username: string;
    person2_Username: string;
    has_Unread_Message: boolean;
    index: number;
}