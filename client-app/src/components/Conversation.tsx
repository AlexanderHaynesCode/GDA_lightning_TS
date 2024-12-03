import { useGlobalContext } from '../GlobalUser';
import { Link } from "react-router-dom";  
import { IConversation } from './IConversation';
import './Conversation.css'

function Conversation(convo: IConversation) {
    const { User } = useGlobalContext();

    return (
        <Link to="/ViewConvoMessages" state={convo} >
            <div className='row mobileRow'>
                <div className={convo.has_Unread_Message ? 'col-md-6 ConversationBoxUnread' : 'col-md-6 ConversationBox'}>
                    <p>{convo.index + 1}. {(User.username == convo.person1_Username) ? convo.person2_Username : convo.person1_Username}</p>
                </div>
            </div>
        </Link>  
    )
}

export default Conversation;