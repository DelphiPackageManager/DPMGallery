import { createContext, ReactNode, useState } from "react";

//Note - this type should mirror the type created in GenerateProfileObject on the server

export type User = {
  userName: string;
  email: string;
  emailConfirmed: boolean;
  avatarUrl: string;
  roles: Array<string>;
  twoFactorEnabled: boolean;
};

export type Auth = {
  user: User | null;
};

export interface AuthContextInterface {
  currentUser: User | null;
  login: (user: User | null) => void;
  logout: () => void;
}

const defaultAuth: Auth = {
  user: null,
};

const defaultAuthContext: AuthContextInterface = {
  currentUser: null,
  login: (value: User | null) => {},
  logout: () => {},
};

const AuthContext = createContext<AuthContextInterface>(defaultAuthContext);

type AuthProviderProps = {
  user: User | null;
  children: ReactNode;
};

export function AuthProvider({ children, user }: AuthProviderProps) {
  const [currentUser, setUser] = useState<User | null>(user);
  const logout = () => {
    setUser(null);
  };
  const login = (user: User | null) => {
    setUser(user);
  };
  return <AuthContext.Provider value={{ currentUser, login, logout }}>{children}</AuthContext.Provider>;
}

export default AuthContext;
