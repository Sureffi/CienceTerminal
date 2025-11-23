import SearchIcon from "@/assets/search-icon.svg?react";
import ControlIcon from "@/assets/control-icon.svg?react";
import { styled } from "styled-components";

export const SearchButton = () => {
    return (
        <SearchContainer>
            <LeftContent>
                <StyledSearchIcon />
                <SearchText>SEARCH</SearchText>
            </LeftContent>
            <KeyboardShortcutBadge>
                <StyledControlIcon />
                <ShortcutKey>K</ShortcutKey>
            </KeyboardShortcutBadge>
        </SearchContainer>
    );
};

const SearchContainer = styled.button`
    display: flex;
    align-items: center;
    justify-content: space-between;
    min-width: 400px;
    height: 32px;
    padding: 6px 4px 6px 12px;
    background: transparent;
    opacity: 0.5;
    border: 2px solid ${({ theme }) => theme.colors.borderGhost};
    border-radius: ${({ theme }) => theme.borderRadius.md};
    cursor: pointer;
    transition: all ${({ theme }) => theme.transitions.fast};

    &:focus-visible {
        outline: 2px solid ${({ theme }) => theme.colors.accentGreen};
        outline-offset: 2px;
    }
`;

const LeftContent = styled.div`
    display: flex;
    align-items: center;
    gap: ${({ theme }) => theme.spacing.sm};
`;

const SearchText = styled.span`
    font-size: ${({ theme }) => theme.typography.fontSize.lg};
    font-weight: ${({ theme }) => theme.typography.fontWeight.semibold};
    color: ${({ theme }) => theme.colors.textPrimary};
    font-family: ${({ theme }) => theme.typography.fontFamily.primary};
`;

const StyledControlIcon = styled(ControlIcon)`
    width: 12px;
    height: 100%;
    opacity: 1;

    path {
        fill: ${({ theme }) => theme.colors.textPrimary};
    }
`;

const StyledSearchIcon = styled(SearchIcon)`
    width: 100%;
    height: 100%;
    opacity: 1;

    path {
        fill: ${({ theme }) => theme.colors.textPrimary};
    }
`;

const KeyboardShortcutBadge = styled.div`
    display: flex;
    align-items: center;
    gap: 6px;
    padding: 3px 4px 3px 7px;
    background-color: ${({ theme }) => theme.colors.borderGhost};
    border-radius: 3px;
`;

const ShortcutKey = styled.span`
    font-size: 12px;
    font-weight: 600;
    color: ${({ theme }) => theme.colors.textPrimary};
`;
