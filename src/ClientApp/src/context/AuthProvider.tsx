import { createContext, ReactNode, useState } from "react";

export type User = {
  user: {
    userName: string;
    email: string;
    emailConfirmed: boolean;
    avatarUrl: string;
    roles: Array<string>;
  } | null;
};

export interface AuthContextInterface {
  auth: User | null;
  //getuserProfile: () => UserProfile;
  setAuth: (user: User) => void;
}

const defaultUserProfile: User = {
  user: null,
};

const defaultAuthContext: AuthContextInterface = {
  auth: null,
  setAuth: (value: User) => {},
};

const AuthContext = createContext<AuthContextInterface>(defaultAuthContext);

type AuthProviderProps = {
  user: User;
  children: ReactNode;
};

export function AuthProvider({ children, user }: AuthProviderProps) {
  console.log("AuthProvider user : " + user?.user);
  const [auth, setAuth] = useState<User>(user);

  return <AuthContext.Provider value={{ auth, setAuth }}>{children}</AuthContext.Provider>;
}

export default AuthContext;
