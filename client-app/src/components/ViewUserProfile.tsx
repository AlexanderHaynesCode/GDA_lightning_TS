import { useLocation } from 'react-router-dom';
import { useGlobalContext } from '../GlobalUser';
import { useState } from 'react';
import './ViewUserProfile.css'

function ViewUserProfile() {
    const { User } = useGlobalContext();
    const [message, setMessage] = useState("");
    const location = useLocation();
    const user = location.state.data;
    var genderString = user.gender == 1 ? "M" : "F";

    const handleTextArea = (event: React.ChangeEvent<HTMLTextAreaElement>) => {
        setMessage(event.target.value);
    }

    const handleSubmit = async (event: React.FormEvent) => {
        event.preventDefault();
        if (message != "") {
            const SendMessageBtnClick = async (): Promise<void> => {
                try {
                    const formData = new FormData();
                    formData.append('message', message);
                    formData.append('person1_ID', "" + User.id); // Current global user
                    formData.append('person2_ID', "" + user.id); // Other user

                    const response = await fetch('user/StartConversationWithAnotherUser', { method: "POST", body: formData });
                    const data = await response.json() as Number;

                    if (data == 1) {
                        alert("Your message has successfully been sent!");
                        setMessage("");
                    } else {
                        alert("Apologies, your message was not able to be sent.");
                    }
                } catch (e) {
                    throw (e);
                }
            };
            const signInResult = await SendMessageBtnClick();
        }        
    }

    return (
        <div>
            <div className="row topDivViewProfile">
                <div className="col-md-6">
                    <h1>User Profile</h1>
                    <div className='userInfoDiv text-center'>
                        <h3 className='phoneCSS phoneCSS2'>{user.username}</h3>
                        <h3 className='phoneCSS phoneCSS2'>{genderString}</h3>
                        <h3 className='phoneCSS3'>{user.age}</h3>
                    </div>
                </div>
                <div className="col-md-6 text-center overflow-hidden">
                    <img className='ViewUserProfilePic' src={user.profilePicPath} alt="Logo1" />
                </div>
            </div>
            <form onSubmit={handleSubmit} >
                <div className="row">
                    <div className="col-md-12 text-center d69">
                        <h3>Send {user.username} a message:</h3>
                        <textarea name="messageTextArea" value={message} rows={6} className='messageTextArea' onChange={handleTextArea} />
                    </div>
                </div>
                <div className="row btn-center">
                    <input type="submit" className='sendMessageBtn' value="Send Message" />
                </div>
            </form>            
        </div>
    )
}

export default ViewUserProfile;