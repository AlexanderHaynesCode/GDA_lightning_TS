import { useEffect, useState } from "react";
import { useGlobalContext } from '../GlobalUser';
import { useLocation } from "react-router-dom";
import { IConversation } from './IConversation';
import './ViewConvoMessages.css'

interface IMessage {
    conversation_ID: number;
    message_ID: number;
    from_ID: number;
    to_ID: number;
    dateCreated: Date;
    message: string;
    to_ID_Read: boolean;
}

function ViewConvoMessages() {
    const { User } = useGlobalContext(); 
    const [messageResults, setMessageResults] = useState([]);
    const [loading, setLoading] = useState(true);
    const [newMessage, setNewMessage] = useState("");
    const location = useLocation();
    const convo = location.state as IConversation;
    const OtherUsername = (User.username == convo.person1_Username) ? convo.person2_Username : convo.person1_Username;
    const OtherUserID = (User.id == convo.person1_ID) ? convo.person2_ID : convo.person1_Username;

    useEffect(() => {
        PopulateMessages();
    }, []);

    function PopulateMessages() {
        const PopulateMessagesDoWork = async () => {
            try {
                const response = await fetch('user/GetMessagesByConversation/' + convo.conversation_ID + '/' + User.id);
                const data = await response.json();
                setMessageResults(data);
                setLoading(false);
                setNewMessage("");
            } catch (error) {
                // log error
            }
        };

        PopulateMessagesDoWork();
    }

    const handleTextArea = (event: React.ChangeEvent<HTMLTextAreaElement>) => {
        setNewMessage(event.target.value);
    }

    const handleSubmit = async (event: React.FormEvent) => {
        event.preventDefault();
        if (newMessage != "") {
            const SendMessageBtnClick = async (): Promise<void> => {
                try {
                    const formData = new FormData();
                    formData.append('convo_ID', "" + convo.conversation_ID);
                    formData.append('newMessage', newMessage);
                    formData.append('fromID', "" + User.id); // Current global user
                    formData.append('toID', "" + OtherUserID); // Other user current user is viewing profile of

                    const response = await fetch('user/SendMessageToAnotherUser', { method: "POST", body: formData });
                    const data = await response.json() as Number;

                    PopulateMessages();
                } catch (e) {
                    throw (e);
                }
            };
            const signInResult = await SendMessageBtnClick();
        }
    }


    if (loading) {
        return (
            <div>
                <p><em>Loading...</em></p>
            </div>
        )
    } else {
        let contents = renderMessageResultsTable(messageResults);
        return (
            <div>
                <h1>Messages</h1>
                <p>Messages with {OtherUsername}</p>
                {contents}
            </div>
        )
    }

    function renderMessageResultsTable(messageResults: IMessage[]) {
        return (
            <div>
                <div>
                    {messageResults.map((message, index) => (
                        <div key={index} className="row messageRow">
                            <div className={(!(message.from_ID == User.id) ? 'col-md-2 text-center userWhoSent' : 'col-md-2 text-center userWhoSent hideOnPhone')}>
                                <p className={(!(message.from_ID == User.id) ? 'floatLeft2 d68' : '')}>{(!(message.from_ID == User.id)) ? OtherUsername : ''}</p>
                            </div>
                            <div className={"col-md-8 " + ((message.from_ID == User.id) ? 'MessageFrom' : 'MessageTo')}>
                                <p>{message.message}</p>
                            </div>
                            {/* <div className='col-md-2 text-center userWhoSent'> */}
                            <div className={((message.from_ID == User.id) ? 'col-md-2 text-center userWhoSent' : 'col-md-2 text-center userWhoSent hideOnPhone')}>
                                <p className={((message.from_ID == User.id) ? 'floatRight2 d68' : '')}>{(message.from_ID == User.id) ? User.username : ''}</p>
                            </div>
                        </div>
                    ))}
                </div>
                <br />
                <form onSubmit={handleSubmit} >
                    <div className="row">
                        <div className="col-md-12 text-center d69">
                            <h3 className='fontLucida'>Send {OtherUsername} a message:</h3>
                            <textarea value={newMessage} rows={6} className='messageTextArea' onChange={handleTextArea} />
                        </div>
                    </div>
                    <div className="row btn-center">
                        <input type="submit" className='sendMessageBtn' value="Send Message" />
                    </div>
                </form>
                
                <br />
            </div>
        )
    }
}

export default ViewConvoMessages;