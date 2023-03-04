import { Navigate, Outlet, useLocation } from "react-router-dom";
import useAuth from "../hooks/useAuth";

export type RequireAuthProps = {
  allowedRoles: Array<string> | null;
};

const RequireAuthRoute = (props: RequireAuthProps) => {
  console.log("requireauth");
  const location = useLocation();
  const { auth } = useAuth();

  // const PROFILE_URL = "/ui/auth/profile";
  const user = auth?.user;
  console.log(user);

  return user?.roles?.find((role) => props.allowedRoles?.includes(role)) ? (
    <Outlet />
  ) : auth?.user ? (
    <Navigate to="/unauthorized" state={{ from: location.pathname }} replace />
  ) : (
    <Navigate to="/login" state={{ from: location.pathname }} replace />
  );
};

export default RequireAuthRoute;
