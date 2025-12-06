import { Button } from "@/components/atoms";
import styled from "styled-components";
import logoIcon from "@/assets/logo-small.svg";
import screenerIcon from "@/assets/screener-icon.svg";
import terminalIcon from "@/assets/terminal-icon.svg";
// import watchlistIcon from '@/assets/heart-icon.svg';
// import { SearchButton } from "@/components/molecules";
import { useLocation, useNavigate } from "react-router-dom";
import { MOBILE_BREAKPOINT, DIMENSIONS, Z_INDEX } from './constants';

export const Header = () => {
    const location = useLocation();
    const navigate = useNavigate();

    return (
        <HeaderContainer>
            {/* LEFT SECTION */}
            <LeftSection>
                <Logo src={logoIcon} alt="CienceTerminal Logo" />
                <NavButtons>
                    <Button size="lg" leftIcon={<ButtonIcon src={screenerIcon} alt="Screener" />} onClick={() => navigate('/screener')} isActive={location.pathname === '/screener'}>SCREENER</Button>
                    <Button size="lg" leftIcon={<ButtonIcon src={terminalIcon} alt="Terminal" />} onClick={() => navigate('/terminal')} isActive={location.pathname === '/terminal'}>TERMINAL</Button>
                    {/* <Button size="lg" leftIcon={<ButtonIcon src={watchlistIcon} alt="Watchlist" />} onClick={() => navigate('/watchlist')} isActive={location.pathname === '/watchlist'}>WATCHLIST</Button> */}
                </NavButtons>
            </LeftSection>

            {/* RIGHT SECTION */}
            <RightSection>
                {/* Search, login/connect wallet */}
                {/* <SearchButton></SearchButton> */}
                {/* <Button size="lg">CONNECT WALLET</Button> */}
            </RightSection>
        </HeaderContainer>
    );
}

/**
 * Styled Components
 *
 * Responsive Design Pattern (following TokenTable/TokenRow):
 * - Constants imported from ./constants for maintainability
 * - Mobile breakpoint (768px) for consistent responsive behavior
 * - GPU acceleration (translateZ) on mobile for smooth performance
 * - Touch optimization (touch-action: manipulation) to prevent double-tap zoom
 * - Dimension scaling from desktop to mobile for better UX
 */
const HeaderContainer = styled.header`
    position: sticky;
    top: 0;
    z-index: ${Z_INDEX.header};
    padding: ${DIMENSIONS.header.paddingDesktop};
    display: flex;
    justify-content: space-between;
    align-items: center;
    width: 100%;
    max-width: 100vw;
    box-sizing: border-box;
    border-bottom: 1px solid ${({ theme }) => theme.colors.borderGhost};
    background: ${({ theme }) => theme.colors.bgDark};
    backdrop-filter: blur(8px);

    /* Mobile responsiveness */
    @media (max-width: ${MOBILE_BREAKPOINT}) {
        padding: ${DIMENSIONS.header.paddingMobile};
        /* GPU acceleration for smooth scrolling on mobile */
        transform: translateZ(0);
    }
`;

const Logo = styled.img`
    height: ${DIMENSIONS.logo.heightDesktop}px;
    width: auto;
    display: block;

    @media (max-width: ${MOBILE_BREAKPOINT}) {
        height: ${DIMENSIONS.logo.heightMobile}px;
    }
`;

const NavButtons = styled.div`
    display: flex;
    gap: ${DIMENSIONS.spacing.navButtonsGapDesktop};
    align-items: center;

    /* Touch optimization - prevents double-tap zoom delay on mobile */
    & > * {
        touch-action: manipulation;
    }

    @media (max-width: ${MOBILE_BREAKPOINT}) {
        gap: ${DIMENSIONS.spacing.navButtonsGapMobile};
    }
`;

const LeftSection = styled.div`
    display: flex;
    align-items: center;
    gap: ${DIMENSIONS.spacing.leftSectionGapDesktop};
    flex-shrink: 0;
    min-width: 0;

    @media (max-width: ${MOBILE_BREAKPOINT}) {
        gap: ${DIMENSIONS.spacing.leftSectionGapMobile};
    }
`;

const RightSection = styled.div`
    display: flex;
    align-items: center;
    gap: ${DIMENSIONS.spacing.rightSectionGapDesktop};
    flex-shrink: 0;
    min-width: 0;

    @media (max-width: ${MOBILE_BREAKPOINT}) {
        gap: ${DIMENSIONS.spacing.rightSectionGapMobile};
    }
`;

const ButtonIcon = styled.img`
    height: ${DIMENSIONS.buttonIcon.heightDesktop}px;
    width: ${DIMENSIONS.buttonIcon.widthDesktop}px;
    display: block;

    @media (max-width: ${MOBILE_BREAKPOINT}) {
        height: ${DIMENSIONS.buttonIcon.heightMobile}px;
        width: ${DIMENSIONS.buttonIcon.widthMobile}px;
    }
`;
