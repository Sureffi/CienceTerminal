import React from 'react';
import styled, { css } from 'styled-components';

export interface BadgeProps {
  /**
   * Content to display in the badge
   */
  children: React.ReactNode;

  /**
   * Visual variant of the badge
   */
  variant?: 'default' | 'success' | 'warning' | 'error' | 'info' | 'neutral';

  /**
   * Size of the badge
   */
  size?: 'sm' | 'md' | 'lg';

  /**
   * Whether the badge should be rounded (pill shape)
   */
  rounded?: boolean;

  /**
   * Optional icon to display before text
   */
  icon?: React.ReactNode;
}

/**
 * Badge component for labels, status indicators, and tags
 *
 * @example
 * <Badge variant="success" size="sm">Active</Badge>
 * <Badge variant="error" icon={<ErrorIcon />}>Failed</Badge>
 */
export const Badge: React.FC<BadgeProps> = ({
  children,
  variant = 'default',
  size = 'md',
  rounded = false,
  icon,
}) => {
  return (
    <StyledBadge $variant={variant} $size={size} $rounded={rounded}>
      {icon && <IconWrapper>{icon}</IconWrapper>}
      {children}
    </StyledBadge>
  );
};

// Styled Components

const StyledBadge = styled.span<{
  $variant: BadgeProps['variant'];
  $size: BadgeProps['size'];
  $rounded: boolean;
}>`
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: ${({ theme }) => theme.spacing.xs};

  font-family: ${({ theme }) => theme.typography.fontFamily.primary};
  font-weight: ${({ theme }) => theme.typography.fontWeight.medium};
  line-height: 1;
  white-space: nowrap;

  border-radius: ${({ theme, $rounded }) =>
    $rounded ? theme.borderRadius.full : theme.borderRadius.sm};

  /* Size variants */
  ${({ $size, theme }) => {
    switch ($size) {
      case 'sm':
        return css`
          padding: ${theme.spacing.xs} ${theme.spacing.sm};
          font-size: ${theme.typography.fontSize.xs};
        `;
      case 'lg':
        return css`
          padding: ${theme.spacing.sm} ${theme.spacing.md};
          font-size: ${theme.typography.fontSize.base};
        `;
      default: // md
        return css`
          padding: ${theme.spacing.xs} ${theme.spacing.sm};
          font-size: ${theme.typography.fontSize.sm};
        `;
    }
  }}

  /* Color variants */
  ${({ $variant, theme }) => {
    switch ($variant) {
      case 'success':
        return css`
          background: ${theme.colors.statusSuccessDark};
          color: ${theme.colors.textPrimary};
          border: 1px solid ${theme.colors.statusSuccess};
        `;

      case 'warning':
        return css`
          background: ${theme.colors.statusWarningDark};
          color: ${theme.colors.textPrimary};
          border: 1px solid ${theme.colors.statusWarning};
        `;

      case 'error':
        return css`
          background: ${theme.colors.statusErrorDark};
          color: ${theme.colors.textPrimary};
          border: 1px solid ${theme.colors.statusError};
        `;

      case 'info':
        return css`
          background: ${theme.colors.statusInfoDark};
          color: ${theme.colors.textPrimary};
          border: 1px solid ${theme.colors.statusInfo};
        `;

      case 'neutral':
        return css`
          background: ${theme.colors.gray700};
          color: ${theme.colors.textSecondary};
          border: 1px solid ${theme.colors.gray600};
        `;

      default: // default
        return css`
          background: ${theme.colors.accentGreenDark};
          color: ${theme.colors.textOnAccent};
          border: 1px solid ${theme.colors.accentGreen};
        `;
    }
  }}
`;

const IconWrapper = styled.span`
  display: inline-flex;
  align-items: center;
  justify-content: center;
  font-size: 0.875em;
`;
