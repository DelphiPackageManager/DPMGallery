import React, { createContext, useState } from "react";

export type User = {
    userName: string;
    password: string;
};


export type AuthContextType = {
    user: User | null;
    setUser: (user : User) => void;
    roles : string[];
    setRoles : (newroles : string[]) => void;
}

const AuthContext = createContext<AuthContextType | null>({
    user : null,
    setUser : (user : User) => {},
    roles : [],
    setRoles: (newRoles : string[]) => {}
});

type AuthProviderProps = {
    children: React.ReactNode
}


export const AuthProvider  = ({ children } :  AuthProviderProps) => {

    const [user, setUser] = useState<User | null>(null);
    const [roles, setRoles] = useState<string[]>([]);

    return (
        <AuthContext.Provider  value={{user, setUser, roles, setRoles}}>
            {children}
        </AuthContext.Provider>
    )
}

export default AuthContext;
