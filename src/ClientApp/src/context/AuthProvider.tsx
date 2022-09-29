import { createContext, useState } from "react";

export interface IUser {
    userName: string;
    password: string;
};


export type AuthContextType = {
    user: IUser;
    setUser: (newUser: IUser) => void;
}

export const AuthContext = createContext<AuthContextType | undefined>(undefined);


export interface AuthProviderProps{
    children: React.ReactNode
}
 


const AuthProvider  = ({ children } :  AuthProviderProps) => {

    const [user, setUser] = useState<IUser>();

    return (
        <AuthContext.Provider  value={user ? {user, setUser} : undefined}>
            {children}
        </AuthContext.Provider>
    )
}

export default AuthProvider;
