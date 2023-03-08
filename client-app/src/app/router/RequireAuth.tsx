import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useStore } from "../stores/store";

export default function RequireAuth() {
    const {userStore: {isLoggedIn}} = useStore();
    const location = useLocation(); 

    if (!isLoggedIn) {
        return <Navigate to='/' state={{from: location}} /> // where user navigated from to send them back
    }

    return <Outlet />
}