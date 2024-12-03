import { createContext, useContext } from "react"
export interface IUser {
    id: number;
    username: string;
    gender: number;
    lookingFor: string;
    profilePicPath: string;
    age: number;
}
export type GlobalUser = {
    User: IUser;
    setUser: (u: IUser) => void;
}
export const MyGlobalUser = createContext<GlobalUser>({
    User: {
        id: 0,
        username: "",
        gender: 0,
        lookingFor: "",
        profilePicPath: "",
        age: 0   
    }, // set a default value
    setUser: () => { },
})
export const useGlobalContext = () => useContext(MyGlobalUser)