import { IUser } from '../GlobalUser';
import { Link } from "react-router-dom";    
import MessageIcon from '../Images/MessageImage_blueWhite.png';

function UserInSearch(user: IUser) {
    var genderString = user.gender == 1 ? "M" : "F";

    return (
        <Link to="/ViewUserProfile" className='' state={{ data: user }} >
            <div className='col-lg-4 col-md-6 col-sm-6 UIS_mainDiv'>
                <div className='row text-center imgRow'>
                    <img className='UserInSearchImage' src={user.profilePicPath} />
                </div>
                <div className='row UIS_bottomBox'>
                    <div className='col-md-6 col-sm-6 bottomBox text-center'>
                        <p className='UIS_username'>{user.username}</p>
                        <p>{genderString}</p>
                    </div>
                    <div className='col-md-6 col-sm-6 bottomBox vert-and-horiz-center'>
                        <img className='MessageIcon vertical-center' src={MessageIcon} alt="Logo" />
                    </div>
                </div>
            </div>
        </Link>
    )
}

export default UserInSearch;