import { KBarProvider } from 'kbar';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import './App.css';
import { Header } from './components/organisms/Header';
import { ScreenerPage, TerminalPage, WatchlistPage, ChartViewPage } from './pages';
import { AppProviders } from './providers/AppProviders';

// KBar related
const actions = [
    {
        id: 'home',
        name: 'Home',
        shortcut: ['h'],
        keywords: 'home',
        perform: () => console.log('Home'),
    },
    {
        id: 'screener',
        name: 'Screener',
        shortcut: ['s'],
        keywords: 'screener',
        perform: () => console.log('Screener'),
    },
];

export default function App() {
    return (
        <AppProviders>
            <BrowserRouter>
                <KBarProvider actions={actions}>
                    <Header />
                    <Routes>
                        <Route path="/" element={<Navigate to="/screener" replace />} />
                        <Route path="/screener" element={<ScreenerPage />} />
                        <Route path="/terminal" element={<TerminalPage />} />
                        <Route path="/watchlist" element={<WatchlistPage />} />
                        <Route path="/search/:address" element={<ChartViewPage />} />
                    </Routes>
                </KBarProvider>
            </BrowserRouter>
        </AppProviders>
    );
}
