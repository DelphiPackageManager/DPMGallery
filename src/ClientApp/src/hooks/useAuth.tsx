import React, { useContext, useDebugValue } from "react";
import AuthContext, { AuthContextType } from "../context/AuthProvider";

const useAuth  = () => {
    //const auth = useContext(AuthContext); 
    //useDebugValue(auth, auth => auth?.user ? "Logged In" : "Logged Out")
    return useContext(AuthContext);
}

export default useAuth