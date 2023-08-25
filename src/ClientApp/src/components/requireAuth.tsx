import { Navigate, Outlet, useLocation } from "react-router-dom";
import useAuth from "../hooks/useAuth";

export type RequireAuthProps = {
  allowedRoles: Array<string> | null;
};

const RequireAuthRoute = (props: RequireAuthProps) => {
  const location = useLocation();
  const { currentUser } = useAuth();

  return currentUser?.roles?.find((role) => props.allowedRoles?.includes(role)) ? (
    <Outlet />
  ) : currentUser ? (
    <Navigate to="/unauthorized" state={{ from: location.pathname }} replace />
  ) : (
    <Navigate to="/login" state={{ from: location.pathname }} replace />
  );
};

export default RequireAuthRoute;
