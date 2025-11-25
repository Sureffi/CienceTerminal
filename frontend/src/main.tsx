import React from 'react'
import ReactDOM from 'react-dom/client'
// import { Auth0Provider, useAuth0 } from '@auth0/auth0-react'
import { ThemeProvider } from 'styled-components'
import { theme } from './styles/theme'
import App from './App'
import './styles/fonts.css'
import './index.css'

const rootElement = document.getElementById('root')!;

// Auth0 temporarily disabled
// const Wrapper = ({ children }: { children: React.ReactNode }) => {
//     const { isLoading, error } = useAuth0();
//
//     if (isLoading) {
//         return <div>Loading......</div>
//     }
//     if (error) {
//         return <div>Oops... {error.message}</div>
//     }
//     return <>{children}</>
// }

ReactDOM.createRoot(rootElement).render(
    <React.StrictMode>
        <ThemeProvider theme={theme}>
            <App />
        </ThemeProvider>
    </React.StrictMode>
)

