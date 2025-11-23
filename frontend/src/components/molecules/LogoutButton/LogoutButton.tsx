import { Button } from "@/components/atoms";
import { useAuth0 } from "@auth0/auth0-react";

export const LogoutButton = () => {
    const {
        isAuthenticated,
        logout,
    } = useAuth0();

    return isAuthenticated && (
        <Button variant="secondary" onClick={() => {
            logout({
                logoutParams: {
                    returnTo: window.location.origin
                }
            });
        }}>logout </Button>
    );
}
