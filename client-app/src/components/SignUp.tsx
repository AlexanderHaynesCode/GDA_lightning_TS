import { useState } from "react";
import { useGlobalContext, IUser } from '../GlobalUser';
    
function SignUp () {
    const { User, setUser } = useGlobalContext();
    const [inputs, setInputs] = useState({
        username: "",
        password: "",
        pwConfirm: "",
        gender: 0,
        lookingFor: "",
        age: 0,
        ProfilePicFile: null,
        ProfilePicFileName: "", 
        profilePicPath: ""
    });    
    const [errors, setErrors] = useState({
        username: "",
        password: "",
        pwConfirm: "",
        gender: "",
        lookingFor: "",
        age: "",
        ProfilePicFile: "",
        ProfilePicFileName: "", 
        profilePicPath: ""
    }); 

    const handleSelectChange = (event: React.ChangeEvent<HTMLSelectElement>) => {
        const id = event.target.id;
        const value = event.target.value;
        setInputs(values => ({...values, [id]: value}));
    }

    const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const name = event.target.name;
        const value = event.target.value;
        setInputs(values => ({...values, [name]: value}));
    }

    const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        if (event.target.files && event.target.files.length > 0) {
            const file = event.target.files[0];
            setInputs(prevInputs => ({ ...prevInputs, ProfilePicFile: file as any, ProfilePicFileName: file.name }));
            document.getElementById('ProfilePicFileName')!.style.border = "3px solid lawngreen";
        }
    }

    function getExtension(filename: string) {
        return (filename.split('.').pop() + '');
    }

    const handleSubmit = async (event: React.FormEvent) => {
        event.preventDefault();
        let errCount = 0;
        
        if(!inputs.username) {  
            errCount++;
            setErrors((prevState) => {
                return {...prevState, username: "Username required" };
            });
        } else {
            setErrors((prevState) => {
                return {...prevState, username: "" };
            });
        }
        if(!inputs.password) {
            errCount++;
            setErrors((prevState) => {
                return {...prevState, password: "Password required" };
            });
        } else {
            setErrors((prevState) => {
                return {...prevState, password: "" };
            });
        }
        if(!inputs.pwConfirm) {
            errCount++;
            setErrors((prevState) => {
                return {...prevState, pwConfirm: "Password confirmation required" };
            });
        } else {
            setErrors((prevState) => {
                return {...prevState, pwConfirm: "" };
            });      
        }
        if (inputs.pwConfirm && inputs.password && !(inputs.password === inputs.pwConfirm)) {
            errCount++;
            setErrors((prevState) => {
                return {...prevState, pwConfirm: "Must match password" };
            });
            setErrors((prevState) => {
                return {...prevState, password: "Must match password confirmation" };
            });
        } 
        if (!inputs.gender || inputs.gender == 0) {
            errCount++;
            setErrors((prevState) => {
                return {...prevState, gender: "Please select your gender" };
            });
        } else {
            setErrors((prevState) => {
                return {...prevState, gender: "" };
            });  
        }
        if (!inputs.lookingFor || inputs.lookingFor == 'N/A') {
            errCount++;
            setErrors((prevState) => {
                return {...prevState, lookingFor: "Please select who you're looking for" };
            });
        } else {
            setErrors((prevState) => {
                return {...prevState, lookingFor: "" };
            });  
        }
        if (!inputs.age) {
            errCount++;
            setErrors((prevState) => {
                return {...prevState, age: "Please enter your age" };
            });
        } else {
            if (inputs.age < 18) {
                errCount++;
                setErrors((prevState) => {
                    return {...prevState, age: "You must be at least 18 years old" };
                });
            } else {
                setErrors((prevState) => {
                    return {...prevState, age: "" };
                }); 
            }       
        }
        if (inputs.ProfilePicFile != null) {
            let fileType = getExtension(inputs.ProfilePicFileName).toLowerCase();
            if (!(fileType == 'png' || fileType == 'jpg' || fileType == 'jpeg')) {
                errCount++;
                document.getElementById('ProfilePicFileName')!.style.border = "3px solid red";
                setErrors((prevState) => {
                    return {...prevState, ProfilePicFile: "Must be a .jpg, .jpeg, or .png file" };
                });
            } else {
                setErrors((prevState) => {
                    return {...prevState, ProfilePicFile: "" };
                }); 
            }
        }

        if (errCount == 0) {
            try {
                const formData = new FormData();
                formData.append('username', inputs.username);
                formData.append('password', inputs.password);
                formData.append('age', ""+inputs.age);
                formData.append('gender', ""+inputs.gender);
                formData.append('lookingFor', inputs.lookingFor); 
                formData.append('ProfilePicFile', inputs.ProfilePicFile as any);
                formData.append('ProfilePicFileName', inputs.ProfilePicFileName);       
                
                /* Since the user might have uploaded a profile picture, we have to use a special class called 
                UserForSASUri_GDA that contains variables for the file and filename.  If they are both
                not null then we try to upload the profile picture to our Azure storage container.
                If the upload is successful then we create a SAS URI token and pass that value to the 
                corresponding User_GDA ProfilePicPath variable.  ProfilePicPath defaults to an empty string.
                Finally, create the user in User_GDA table, based on the relevant UserForSASUri_GDA info. */
        
                let responseValue = 0;
                const CreateUserForSASUri = async () => {
                    try {
                        const response = await fetch('user/CreateForSASUri', {method: "POST", body: formData});
                        const data = await response.json();
                        responseValue = data;
                    } catch (e) {
                        // log error
                    }       
                }
                await CreateUserForSASUri();

                // 1 = success, 2 = username taken, else = general error
                if (responseValue == 1) {
                    alert("You have successfully signed up with GDA!  Welcome!  Sign in and you can search for people.  :)")

                    // Logout the current user if they're signed in, this way the SignIn redirect has no user logged in. 
                    // This is expected user experience for a new signup occuring while a user is already signed in.
                    if (User.id != 0) {
                        let formData = new FormData();
                        formData.append('userID', "" + User.id);
                        const response = await fetch('user/SignOut', { method: "PUT", body: formData });
                        const data = await response.json();

                        // Set the global user data to default values representing a non-signed-in user
                        const emptyUser: IUser = { id: 0, username: "", gender: 0, lookingFor: "", profilePicPath: "", age: 0 };
                        setUser(emptyUser);

                        // Clear the localStorage user data
                        localStorage.setItem('CurrentUserID', String(0));
                        localStorage.setItem('CurrentUserAge', String(0));
                        localStorage.setItem('CurrentUsername', "");
                        localStorage.setItem('CurrentGender', String(0));
                        localStorage.setItem('CurrentLookingFor', "");
                    }

                    window.location.replace('/SignIn');
                } else if (responseValue == 2) {
                    setErrors((prevState) => {
                        return {...prevState, username: "Username taken", };
                    });
                } else {
                    alert("There was a problem creating your profile.  Please try again.")
                }
            } catch (e) {
                // log error
            }
            
        }
    }

    return (
        <div>
            <h1>Sign-Up</h1>
            <form onSubmit={handleSubmit} >
                <p>Please enter your desired username, password, age, gender, and who you're looking for:</p>
                <div className="row PaddingBottom15">
                    <div className="col-md-4">
                        <label>Enter username:</label>
                        <input className='t1' type="text" name='username' value={inputs.username} onChange={handleChange} placeholder='Enter Username'/>
                        <p className='SignUpFormError' style={{color: "Red"}}>{errors.username ? errors.username : '' }</p>
                    </div>
                    <div className="col-md-4">
                        <label>Enter password:</label>
                        <input className='t1' type="password" name='password' value={inputs.password} onChange={handleChange} placeholder='Enter Password'/>
                        <p className='SignUpFormError' style={{color: "Red"}}>{errors.password ? errors.password : '' }</p>
                    </div>
                    <div className="col-md-4">
                        <label>Confirm password:</label>          
                        <input className='t1' type="password" name='pwConfirm' value={inputs.pwConfirm} onChange={handleChange} placeholder='Confirm Password'/>
                        <p className='SignUpFormError' style={{color: "Red"}}>{errors.pwConfirm ? errors.pwConfirm : '' }</p>
                    </div>
                </div>
                <div className="row tinyMarginTop">
                    <div className="col-md-4">
                        <label>How old are you?</label>
                        <input className='t1' type="number" name='age' value={inputs.age ? inputs.age : ''} onChange={handleChange} placeholder='Enter your age' />
                        <p className='SignUpFormError' style={{color: "Red"}}>{errors.age ? errors.age : '' }</p>
                    </div>  
                    <div className="col-md-4">
                        <label>Upload a profile picture (optional)</label>
                        <input type="file" className='fileInput' id='ProfilePicFileName' name="ProfilePicFileName" accept=".png,.jpg" onChange={handleFileChange} />  
                        <p className='SignUpFormError' style={{color: "Red"}}>{errors.ProfilePicFile ? errors.ProfilePicFile : '' }</p>
                    </div> 
                </div> 
                <br />
                <div className="row">
                    <div className="col-md-4">
                        <p>I am:</p>
                        <select id='gender' name='gender' onChange={handleSelectChange} className='selectGenderDDL' >
                            <option value="N/A"  defaultValue="Select your gender">Select your gender</option>
                            <option value="2">Female</option>
                            <option value="1">Male</option>
                        </select>
                        <p className='SignUpFormError' style={{color: "Red"}}>{errors.gender ? errors.gender : '' }</p>
                    </div>                    
                </div>                  
                <br />
                <div className="row">   
                    <div className="col-md-4">
                        <p>I am looking for:</p>
                        <select id="lookingFor" name="lookingFor" onChange={handleSelectChange} className='selectGenderDDL' >
                            <option value="N/A" defaultValue="Select">Select</option>
                            <option value="females">Females</option>
                            <option value="males">Males</option>
                            <option value="both">Both</option>
                            <option value="none">None</option>
                        </select>
                        <p className='SignUpFormError' style={{color: "Red"}}>{errors.lookingFor ? errors.lookingFor : '' }</p>
                    </div>                    
                </div>  
                <br />
                <div className="row text-center">
                    <input type="submit" value="Sign-Up" className='SignUpBtn btn-withGradient' />
                </div>

                <br />
                <br />
                <br />              
            </form>    
        </div>
    )
}

export default SignUp;