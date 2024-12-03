import { useState } from 'react';
import { Navigate } from "react-router-dom";
import { Routes, Route, BrowserRouter } from 'react-router-dom';
import { MyGlobalUser, IUser, GlobalUser } from './GlobalUser';
import './App.css';
import './components/Main.css';
import NavMenu from './components/NavMenu';
import Home from './components/Home';
import SignUp from './components/SignUp';
import SignIn from './components/SignIn';
import Search from './components/Search';
import Profile from './components/Profile';
import UserInSearch from './components/UserInSearch';
import ViewUserProfile from './components/ViewUserProfile';
import Conversation from './components/Conversation';
import ViewAllConversations from './components/ViewAllConversations';
import ViewConvoMessages from './components/ViewConvoMessages';
import Admin from './components/admin';

function App() {
  const [currUser, setCurrUser] = useState<IUser>({
    id: (+localStorage.getItem('CurrentUserID')! != null) ? +localStorage.getItem('CurrentUserID')! : 0,
    age: (+localStorage.getItem('CurrentUserAge')! != null) ? +localStorage.getItem('CurrentUserAge')! : 0,
    username: (localStorage.getItem('CurrentUsername')! != null) ? localStorage.getItem('CurrentUsername')! : "",
    gender: (+localStorage.getItem('CurrentGender')! != null) ? +localStorage.getItem('CurrentGender')! : 0,
    lookingFor: (localStorage.getItem('CurrentLookingFor')! != null) ? localStorage.getItem('CurrentLookingFor')! : "",
    profilePicPath: (localStorage.getItem('CurrentProfilePicPath')! != null) ? localStorage.getItem('CurrentProfilePicPath')! : ""    
  })

  const value: GlobalUser = {
    User: currUser,
    setUser: setCurrUser
  };
  
  return (
    <div className="App">     
      <MyGlobalUser.Provider value={value}>
        <BrowserRouter>
          <NavMenu />
          <div className='container'>
            <Routes>
              <Route path="/" element={<Home />} />
              <Route path="/SignUp" element={<SignUp />} />
              <Route path="/SignIn" element={<SignIn />} />
              <Route path="/Search" element={value.User.id != 0 
                ? <Search /> 
                : <Navigate to="/" />} />
              <Route path="/Profile" element={<Profile /> } />
              <Route path="/UserInSearch" element={value.User.id != 0 
                ? <UserInSearch id={0} username={''} lookingFor={''} profilePicPath={''} gender={0} age={0} /> 
                : <Navigate to="/" /> } />
              <Route path="/ViewUserProfile" element={value.User.id != 0 
                ? <ViewUserProfile /> 
                : <Navigate to="/" />} />
              <Route path="/Conversation" element={value.User.id != 0 
                ? <Conversation conversation_ID={0} person1_ID={0} person2_ID={0} person1_InConvo={true} person2_InConvo={true} person1_Username={''} 
                  person2_Username={''} has_Unread_Message={false} index={0} /> 
                : <Navigate to="/" />} />
              <Route path="/ViewAllConversations" element={value.User.id != 0 
                ? <ViewAllConversations /> 
                : <Navigate to="/" />} />
              <Route path="/ViewConvoMessages" element={value.User.id != 0 
                ? <ViewConvoMessages /> 
                : <Navigate to="/" />} />
              <Route path="/Admin" element={value.User.id == 163 
                ? <Admin /> 
                : <Navigate to="/" />} />
            </Routes>
          </div>
        </BrowserRouter>
      </MyGlobalUser.Provider>        
    </div>);
}

export default App;
