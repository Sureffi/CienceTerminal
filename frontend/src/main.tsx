import React from 'react'
import ReactDOM from 'react-dom/client'
import { Auth0Provider, useAuth0 } from '@auth0/auth0-react'
import { ThemeProvider } from 'styled-components'
import { theme } from './styles/theme'
import App from './App'
import './styles/fonts.css'
import './index.css'

const rootElement = document.getElementById('root')!;

const Wrapper = ({ children }) => {
    const { isLoading, error } = useAuth0();

    if (isLoading) {
        return <div>Loading......</div>
    }
    if (error) {
        return <div>Oops... {error.message}</div>
    }
    return <>{children}</>
}

ReactDOM.createRoot(rootElement).render(
    <React.StrictMode>
        <ThemeProvider theme={theme}>
            <Auth0Provider
                domain={import.meta.env.VITE_AUTH0_DOMAIN || 'your-auth0-domain.us.auth0.com'}
                clientId={import.meta.env.VITE_AUTH0_CLIENT_ID || 'your-auth0-client-id'}
                authorizationParams={{
                    redirect_uri: window.location.origin,
                }}
            >
                <Wrapper>
                    <App />
                </Wrapper>
            </Auth0Provider>
        </ThemeProvider>
    </React.StrictMode>
)

