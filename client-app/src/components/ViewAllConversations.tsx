import { useEffect, useState } from "react";
import { useGlobalContext, IUser } from '../GlobalUser';
import Conversation from "./Conversation";
import { IConversation } from './IConversation';

function ViewAllConversations() {
    const { User } = useGlobalContext(); 
    const [conversationResults, setConversationResults] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const PopulateConversationData = async () => {
            try {
                const response = await fetch('user/GetConversationsByUserWithUsernames/' + User.id);
                const data = await response.json();
                setConversationResults(data);
                setLoading(false);
            } catch (error) {
                // log error
            }
        };

        PopulateConversationData();
    }, []);


    if (loading) {
        return (
            <div>
                <p><em>Loading...</em></p>
            </div>
        )
    } else {
        let contents = renderConversationResultsTable(conversationResults);
        return (
            <div>
                <h1>Conversations</h1>
                <p>Click on a conversation to see the messages in it.</p>
                {contents}
            </div>
        )
    }

    function renderConversationResultsTable(conversationResults: IConversation[]) {
        if (conversationResults.length == 0) {
            return (
                <div>
                    <h3>You don't have any conversations yet.  Message someone to start one! Click "Search" to find people.</h3>
                </div>
            )
        } else {
            return (
                <div>
                    {conversationResults.map((convo, index) => (
                        <Conversation key={index} conversation_ID={convo.conversation_ID} person1_ID={convo.person1_ID} person2_ID={convo.person2_ID} person1_InConvo={convo.person1_InConvo} person2_InConvo={convo.person2_InConvo} person1_Username={convo.person1_Username} person2_Username={convo.person2_Username} has_Unread_Message={convo.has_Unread_Message} index={index}/>
                    ))}
                </div>
            )
        }
    }
}

export default ViewAllConversations;