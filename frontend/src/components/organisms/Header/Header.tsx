import { Button } from "@/components/atoms";
import styled from "styled-components";
import logoIcon from "@/assets/logo-small.svg";
import screenerIcon from "@/assets/screener-icon.svg";
import terminalIcon from "@/assets/terminal-icon.svg";
import watchlistIcon from '@/assets/heart-icon.svg';
import { LoginButton, SearchButton } from "@/components/molecules";
import { useLocation, useNavigate } from "react-router-dom";

export const Header = () => {
    const location = useLocation();
    const navigate = useNavigate();

    // TODO(human):
    // Add onClick and isActive props to each button below:
    // - SCREENER: onClick={() => navigate('/screener')} isActive={location.pathname === '/screener'}
    // - TERMINAL: onClick={() => navigate('/terminal')} isActive={location.pathname === '/terminal'}
    // - WATCHLIST: onClick={() => navigate('/watchlist')} isActive={location.pathname === '/watchlist'}

    return (
        <HeaderContainer>
            {/* LEFT SECTION */}
            <LeftSection>
                <Logo src={logoIcon} alt="CienceTerminal Logo" />
                <NavButtons>
                    <Button size="lg" leftIcon={<ButtonIcon src={screenerIcon} alt="Screener" />} onClick={() => navigate('/screener')} isActive={location.pathname === '/screener'}>SCREENER</Button>
                    <Button size="lg" leftIcon={<ButtonIcon src={terminalIcon} alt="Terminal" />} onClick={() => navigate('/terminal')} isActive={location.pathname === '/terminal'}>TERMINAL</Button>
                    <Button size="lg" leftIcon={<ButtonIcon src={watchlistIcon} alt="Watchlist" />} onClick={() => navigate('/watchlist')} isActive={location.pathname === '/watchlist'}>WATCHLIST</Button>
                </NavButtons>
            </LeftSection>

            {/* RIGHT SECTION */}
            <RightSection>
                {/* Search, login/connect wallet */}
                <SearchButton></SearchButton>
                <Button size="lg">CONNECT WALLET</Button>
            </RightSection>
        </HeaderContainer>
    );
}

// Styled Components
const HeaderContainer = styled.header`
    position: sticky;
    top: 0;
    z-index: 100;
    padding: 20px 20px;
    display: flex;
    justify-content: space-between;
    align-items: center;
    width: 100%;
    max-width: 100vw;
    box-sizing: border-box;
    border: 1px solid ${({ theme }) => theme.colors.borderGhost};
    background: ${({ theme }) => theme.colors.bgDark};
    backdrop-filter: blur(8px);
`;

const Logo = styled.img`
    height: 32px;
    width: auto;
    display: block;
`;

const NavButtons = styled.div`
    display: flex;
    gap: ${({ theme }) => theme.spacing.sm};
    align-items: center;
`;

const LeftSection = styled.div`
    display: flex;
    align-items: center;
    gap: ${({ theme }) => theme.spacing.lg};
    flex-shrink: 0;
    min-width: 0;
`;

const RightSection = styled.div`
    display: flex;
    align-items: center;
    gap: ${({ theme }) => theme.spacing.sm};
    flex-shrink: 0;
    min-width: 0;
`;

const ButtonIcon = styled.img`
    height: 14px;
    width: 20px;
    display: block;
`;
