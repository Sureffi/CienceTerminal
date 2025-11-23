import React from 'react';
import styled, { css } from 'styled-components';

export interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
    /**
     * Visual variant of the button
     */
    variant?: 'primary' | 'secondary' | 'outline' | 'ghost' | 'danger';

    /**
     * Size of the button
     */
    size?: 'sm' | 'md' | 'lg';

    /**
     * Loading state - shows spinner and disables interaction
     */
    isLoading?: boolean;

    /**
     * Full width button
     */
    fullWidth?: boolean;

    /**
     * Icon to display before text
     */
    leftIcon?: React.ReactNode;

    /**
     * Icon to display after text
     */
    rightIcon?: React.ReactNode;

    /**
     * Active state - highlights the button as currently selected
     */
    isActive?: boolean;
}

/**
 * Primary UI component for user interaction
 *
 * @example
 * <Button variant="primary" size="md" onClick={handleClick}>
 *   Click me
 * </Button>
 */
export const Button = React.forwardRef<HTMLButtonElement, ButtonProps>(
    (
        {
            variant = 'primary',
            size = 'md',
            isLoading = false,
            fullWidth = false,
            isActive = false,
            leftIcon,
            rightIcon,
            disabled,
            children,
            ...props
        },
        ref
    ) => {
        return (
            <StyledButton
                ref={ref}
                $variant={variant}
                $size={size}
                $fullWidth={fullWidth}
                $isActive={isActive}
                disabled={disabled || isLoading}
                {...props}
            >
                {isLoading && <Spinner />}
                {!isLoading && leftIcon && <IconWrapper>{leftIcon}</IconWrapper>}
                {children}
                {!isLoading && rightIcon && <IconWrapper>{rightIcon}</IconWrapper>}
            </StyledButton>
        );
    }
);

Button.displayName = 'Button';

// Styled Components

const StyledButton = styled.button<{
    $variant: ButtonProps['variant'];
    $size: ButtonProps['size'];
    $fullWidth: boolean;
    $isActive: boolean;
}>`
  display: inline-flex;
  align-items: center;
  justify-content: flex-start;
  gap: ${({ theme }) => theme.spacing.sm};

  font-family: ${({ theme }) => theme.typography.fontFamily.primary};
  font-weight: ${({ theme }) => theme.typography.fontWeight.semibold};

  border-radius: 4px;
  transition: .3s ease-out;
  box-shadow: 0 0 0 rgba(255, 255, 255, 0);

  cursor: pointer;
  user-select: none;
  white-space: nowrap;

  &:hover:not(:disabled) {
    border-color: rgba(255, 255, 255, 0.8);
    box-shadow: 0 0 0 3px rgba(255, 255, 255, 0.04);
  }

  ${({ $fullWidth }) =>
        $fullWidth &&
        css`
      width: 100%;
    `}

  /* Size variants */
  ${({ $size, theme }) => {
        switch ($size) {
            case 'sm':
                return css`
          padding: ${theme.spacing.xs} ${theme.spacing.md};
          font-size: ${theme.typography.fontSize.sm};
          height: 32px;
        `;
            case 'lg':
                return css`
          padding: 5px 12px 5px 12px;
          font-size: 14px;
          height: 32px;
        `;
            default: // md
                return css`
          padding: ${theme.spacing.sm} ${theme.spacing.lg};
          font-size: ${theme.typography.fontSize.base};
          height: 40px;
        `;
        }
    }}

  /* Active state */
  ${({ $isActive, theme }) =>
        $isActive &&
        css`
      border-color: ${theme.colors.accentGreen} !important;
      opacity: 1 !important;
    `}

  /* Color variants */
  ${({ $variant, theme }) => {
        switch ($variant) {
            case 'primary':
                return css`
          background: ${theme.colors.bgDark};
          color: ${theme.colors.textPrimary};
          border: 2px solid ${theme.colors.borderGhost};
        `;

            case 'secondary':
                return css`
          background: ${theme.colors.accentPurple};
          color: ${theme.colors.textPrimary};
          border: 2px solid ${theme.colors.accentPurple};
        `;

            case 'outline':
                return css`
          background: transparent;
          color: ${theme.colors.accentGreen};
          border: 2px solid ${theme.colors.accentGreen};
        `;

            case 'ghost':
                return css`
          background: transparent;
          color: ${theme.colors.textPrimary};
            opacity: 0.4;
          border: 2px solid ${theme.colors.borderGhost};
        `;

            case 'danger':
                return css`
          background: ${theme.colors.statusError};
          color: ${theme.colors.textPrimary};
          border: 2px solid ${theme.colors.statusError};
        `;
        }
    }}

  /* Disabled state */
  &:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

  /* Focus state */
  &:focus-visible {
    outline: 2px solid ${({ theme }) => theme.colors.accentGreen};
    outline-offset: 2px;
  }
`;

const IconWrapper = styled.span`
  display: inline-flex;
  align-items: center;
  justify-content: center;
`;

const Spinner = styled.div`
  width: 16px;
  height: 16px;
  border: 2px solid currentColor;
  border-top-color: transparent;
  border-radius: 50%;
  animation: spin 0.6s linear infinite;

  @keyframes spin {
    to {
      transform: rotate(360deg);
    }
  }
`;
