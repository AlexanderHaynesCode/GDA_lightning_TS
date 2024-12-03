import { useEffect } from 'react';
import { useGlobalContext, IUser } from '../GlobalUser';
    
function Profile () {
    const { User, setUser } = useGlobalContext();
    let username = User.username;

    useEffect(() => {
        // In case the user manually enters the Profile URL and isn't logged in.
        // This functionality was needed because having the logic in the App router created a quick flicker
        //      to the home page before the SignIn page after a logout, which isn't a good UX.
        if (User.id == 0) {
            window.location.replace('/');
        }
    }, []);    

    const LogOutClick = async () => {
        let formData = new FormData();
        formData.append('userID', ""+User.id);
        const response = await fetch('user/SignOut', { method: "PUT", body: formData });
        const data = await response.json();

        // Set the global user data to default values representing a non-signed-in user
        const emptyUser: IUser = {id: 0, username: "", gender: 0, lookingFor: "", profilePicPath: "", age: 0};
        setUser(emptyUser);

        // Clear the localStorage user data
        localStorage.setItem('CurrentUserID', String(0));
        localStorage.setItem('CurrentUserAge', String(0));
        localStorage.setItem('CurrentUsername', "");
        localStorage.setItem('CurrentGender', String(0));
        localStorage.setItem('CurrentLookingFor', "");
        localStorage.setItem('CurrentProfilePicPath', "");

        if (data == 1) {
            window.location.replace('/SignIn');
        }
    }

    return (
        <div>
            <h1>Profile</h1>
            <h3>Username: {username}</h3>
            <p>To log out click the button below.</p>
            <div className="row">
                <div className="col-md-3">
                    <button onClick={LogOutClick} className='LogOutBtn'>LOG OUT</button>
                </div>
            </div>
        </div>
    )
}

export default Profile;