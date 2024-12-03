import { useState } from "react";
import { useGlobalContext, IUser } from '../GlobalUser';
import './SignIn.css';
    
function SignIn () {
    const { setUser } = useGlobalContext();
    const [inputs, setInputs] = useState({
        username: "",
        password: ""
    });    
    const [errors, setErrors] = useState({
        username: "",
        password: "",
        failedLogIn: ""
    }); 

    function SetGlobalCurrentUserData(user: IUser) {
        const fetchedUser: IUser = user;
        setUser(fetchedUser);
        
        localStorage.setItem('CurrentUserID', String(user.id));
        localStorage.setItem('CurrentUsername', user.username);
        localStorage.setItem('CurrentLookingFor', user.lookingFor);
        localStorage.setItem('CurrentUserAge', String(user.age));
        localStorage.setItem('CurrentGender', String(user.gender));
        localStorage.setItem('CurrentProfilePicPath', user.profilePicPath);
    }

    const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const name = event.target.name;
        const value = event.target.value;
        setInputs(values => ({...values, [name]: value}));
    }
    
    const handleSubmit = async (event: React.FormEvent) => {
        event.preventDefault();
        let errCount = 0;
        
        if(!inputs.username) {
            errCount++;
            setErrors((prevState) => {
                return {...prevState, username: "Username required", };
            });
        } else {
            setErrors((prevState) => {
                return {...prevState, username: "", };
            });
        }
        if(!inputs.password) {
            errCount++;
            setErrors((prevState) => {
                return {...prevState, password: "Password required", };
            });
        } else {
            setErrors((prevState) => {
                return {...prevState, password: "", };
            });
        }

        if (errCount == 0) {
            try {
                const SignInAttempt = async (): Promise<IUser> => {
                    try {
                        const response = await fetch('user/SignIn2/' + inputs.username + '/' + inputs.password);
                        const data = await response.json() as IUser;
                        SetGlobalCurrentUserData(data);

                        return data;
                    } catch (e) {
                        throw (e);
                    }          
                };  
                const signInResult = await SignInAttempt(); // Returns an 'IUser' type object
                
                if (signInResult.id != 0) {
                    window.location.replace('/Search');
                } else {
                    setErrors((prevState) => {
                      return {...prevState, failedLogIn: "Username and/or password invalid" };
                    });
                }
            } catch (e) {
                // log error
            }
            
        }
    }

    return (
        <div>
            <h1>Sign In</h1>

            <form onSubmit={handleSubmit}>
                <p>Your match is waiting for you!</p>
                <div className="row">
                <div className="col-md-6">
                    <label className='SignInLabel'>Enter Username:</label>
                    <input className='t1' type="text" name='username' value={inputs.username} onChange={handleChange} placeholder='Enter Username'/>
                    <p style={{color: "Red"}}>{errors.username ? errors.username : '' }</p>
                </div>
                <div className="col-md-6">
                    <label className='SignInLabel'>Enter Password:</label>
                    <input className='t1' type="password" name='password' value={inputs.password} onChange={handleChange} placeholder='Enter Password'/>
                    <p style={{color: "Red"}}>{errors.password ? errors.password : '' }</p>
                </div>
                </div>  
                <br />   
                <div className="row text-center">
                <p style={{color: "Red"}}>{errors.failedLogIn ? errors.failedLogIn : '' }</p>
                <div className='col-md-12'>
                    <input className='btnSignIn' type="submit" value='Sign In'/>
                </div>          
                </div>        
            </form>
        </div>
    )
}

export default SignIn;