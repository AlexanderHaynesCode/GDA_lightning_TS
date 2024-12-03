import { useEffect, useState } from "react";
import { useGlobalContext, IUser } from '../GlobalUser';
import UserInSearch from "./UserInSearch";

function Search () {
    const { User } = useGlobalContext();
    const [searchResults, setSearchResults] = useState([]); 
    const [loading, setLoading] = useState(true); 

    useEffect(() => {
        const PopulateSearchData = async () => {
            try {
                const response = await fetch('user/GetFilteredSearchResults/' + User.id + '/' + User.lookingFor);
                const data = await response.json();
                setSearchResults(data);
                setLoading(false);
            } catch (error) {
                // log error
            }
        };

        PopulateSearchData();
    }, []);


    if (loading) {
        return (
            <div>
                <p><em>Loading...</em></p>
            </div>
        )
    } else {
        let contents = renderSearchResultsTable(searchResults);
        return (
            <div>
                <h1>Search</h1>
                <p>Click on someone to go to their profile.  You can change your search settings in your profile.</p>
                {contents}
            </div>
        )
    }


    function renderSearchResultsTable(searchResults: IUser[]) {
        if (searchResults.length == 0) {
            return (
                <div>
                    <h2>There are no other users available at this moment, try again later.</h2>
                </div>
            )
        } else {
            return (
                <div>
                    {searchResults.map((user, index) => (
                        <UserInSearch key={index} username={user.username} id={user.id} lookingFor={user.lookingFor} profilePicPath={user.profilePicPath} gender={user.gender} age={user.age} />
                    ))}
                </div>
            )
        } 
    }
    
}

export default Search;