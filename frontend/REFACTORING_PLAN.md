# Frontend Refactoring Plan

## Overview
Complete refactor of the CienceTerminal frontend to establish a production-ready, maintainable, and scalable codebase with styled-components and modern React patterns.

## Current State Analysis

### Issues to Address
1. **Mixed styling approaches**: CSS modules scattered across components
2. **Inconsistent component patterns**: Mix of AI-generated code without clear structure
3. **No design system**: No centralized theme, tokens, or design consistency
4. **Unclear separation of concerns**: Business logic mixed with presentation
5. **Demo mode handled poorly**: Top-level dynamic imports are messy
6. **No testing infrastructure**: No unit or integration tests
7. **Inconsistent TypeScript usage**: Could be more type-safe
8. **No component documentation**: Needs Storybook or similar

### Current Structure
```
src/
├── components/          # Mixed presentation/logic components
├── contexts/            # Auth context (+ demo version)
├── config/              # API configuration
├── models/              # TypeScript types
├── mocks/               # Mock data for demo mode
├── utils/               # API utilities
└── *.css                # Scattered CSS files
```

## New Architecture

### Proposed Folder Structure
```
src/
├── app/                     # App-level configuration
│   ├── App.tsx             # Main app component
│   ├── providers/          # App providers wrapper
│   └── routes/             # Route configuration
│
├── assets/                 # Static assets
│   ├── icons/
│   ├── images/
│   └── fonts/
│
├── components/             # Shared/reusable components
│   ├── atoms/             # Smallest building blocks (Button, Input, Badge, etc.)
│   ├── molecules/         # Simple combinations (SearchBar, AlertBadge, etc.)
│   └── organisms/         # Complex components (Header, AlertCard, etc.)
│
├── features/              # Feature-based modules
│   ├── auth/
│   │   ├── components/    # Feature-specific components
│   │   ├── hooks/         # Feature-specific hooks
│   │   ├── context/       # Feature context
│   │   ├── services/      # API calls
│   │   ├── types/         # Feature types
│   │   └── utils/         # Feature utilities
│   │
│   ├── alerts/
│   │   ├── components/
│   │   ├── hooks/
│   │   ├── services/
│   │   ├── types/
│   │   └── utils/
│   │
│   ├── twitter-alerts/
│   └── ca-mention-alerts/
│
├── hooks/                 # Shared custom hooks
│   ├── useDebounce.ts
│   ├── useLocalStorage.ts
│   └── useMediaQuery.ts
│
├── layouts/              # Layout components
│   ├── MainLayout/
│   ├── DashboardLayout/
│   └── AuthLayout/
│
├── services/             # Global services
│   ├── api/             # API client setup
│   ├── websocket/       # SignalR configuration
│   └── analytics/       # Analytics service
│
├── styles/              # Design system
│   ├── theme/
│   │   ├── index.ts
│   │   ├── colors.ts
│   │   ├── typography.ts
│   │   ├── spacing.ts
│   │   ├── breakpoints.ts
│   │   └── shadows.ts
│   ├── GlobalStyles.ts
│   └── animations.ts
│
├── types/               # Global TypeScript types
│   ├── api.ts
│   ├── models.ts
│   └── styled.d.ts     # Styled-components theme typing
│
├── utils/               # Shared utilities
│   ├── formatters/
│   ├── validators/
│   └── constants/
│
├── config/              # App configuration
│   ├── env.ts          # Environment variables with validation
│   └── constants.ts
│
└── __tests__/           # Test utilities and setup
    ├── setup.ts
    ├── mocks/
    └── fixtures/
```

## Design System Foundation

### Theme Structure
```typescript
interface Theme {
  colors: {
    primary: {...}
    secondary: {...}
    semantic: {
      success: {...}
      warning: {...}
      error: {...}
      info: {...}
    }
    neutral: {...}
    background: {...}
    text: {...}
  }
  typography: {
    fontFamily: {...}
    fontSize: {...}
    fontWeight: {...}
    lineHeight: {...}
  }
  spacing: {
    xs: string
    sm: string
    md: string
    lg: string
    xl: string
    // ...
  }
  breakpoints: {
    mobile: string
    tablet: string
    desktop: string
    wide: string
  }
  shadows: {
    sm: string
    md: string
    lg: string
    xl: string
  }
  transitions: {
    fast: string
    normal: string
    slow: string
  }
  borderRadius: {...}
  zIndex: {...}
}
```

## Component Patterns

### 1. Atomic Design Principles
- **Atoms**: Basic building blocks (Button, Input, Icon, Badge)
- **Molecules**: Simple combinations (SearchBar, AlertBadge, UserAvatar)
- **Organisms**: Complex UI sections (AlertCard, Header, Sidebar)
- **Templates**: Page layouts without specific content
- **Pages**: Actual pages with real content

### 2. Component Structure
```typescript
// Example: components/atoms/Button/Button.tsx
import styled from 'styled-components';

interface ButtonProps {
  variant?: 'primary' | 'secondary' | 'outline';
  size?: 'sm' | 'md' | 'lg';
  isLoading?: boolean;
  disabled?: boolean;
  children: React.ReactNode;
  onClick?: () => void;
}

export const Button: React.FC<ButtonProps> = ({
  variant = 'primary',
  size = 'md',
  isLoading,
  disabled,
  children,
  onClick,
}) => {
  return (
    <StyledButton
      variant={variant}
      size={size}
      disabled={disabled || isLoading}
      onClick={onClick}
    >
      {isLoading ? <Spinner /> : children}
    </StyledButton>
  );
};

const StyledButton = styled.button<{
  variant: ButtonProps['variant'];
  size: ButtonProps['size'];
}>`
  /* Styles using theme */
  background: ${({ theme, variant }) => theme.colors[variant].main};
  padding: ${({ theme, size }) => theme.spacing[size]};
  /* ... */
`;
```

### 3. Feature Module Pattern
Each feature is self-contained with its own components, logic, and types.

```typescript
// features/twitter-alerts/
// ├── components/
// │   ├── TwitterAlertCard.tsx
// │   ├── TwitterAlertList.tsx
// │   └── TwitterAlertFilters.tsx
// ├── hooks/
// │   ├── useTwitterAlerts.ts
// │   └── useTwitterAlertFilters.ts
// ├── services/
// │   └── twitterAlertService.ts
// ├── types/
// │   └── index.ts
// └── index.ts (public API)
```

## Demo Mode Strategy

### Current Issues
- Top-level dynamic imports in App.tsx
- Duplicate provider files (.demo.tsx)
- Messy conditional logic

### New Approach
```typescript
// services/api/client.ts
export const apiClient = import.meta.env.VITE_DEMO_MODE === 'true'
  ? createMockClient()
  : createRealClient();

// features/auth/context/AuthProvider.tsx
export const AuthProvider = import.meta.env.VITE_DEMO_MODE === 'true'
  ? MockAuthProvider
  : Auth0AuthProvider;

// Cleaner, centralized, and tree-shakeable
```

## Migration Strategy

### Phase 1: Foundation (Week 1)
1. ✅ Install styled-components
2. Set up design system structure
   - Create theme files
   - Define design tokens
   - Set up GlobalStyles
3. Create base component library
   - Atoms: Button, Input, Badge, Icon, Card
   - Set up component templates
4. Set up TypeScript strict mode and theme typing

### Phase 2: Core Components (Week 2)
1. Build molecules
   - SearchBar, FilterBar, AlertBadge, UserAvatar
2. Build organisms
   - Header, Sidebar, AlertCard (generic)
3. Create layouts
   - MainLayout, DashboardLayout, AuthLayout

### Phase 3: Feature Migration (Week 3-4)
1. Migrate auth feature
   - Create feature structure
   - Extract auth logic
   - Build auth components with styled-components
2. Migrate twitter-alerts feature
   - Create feature structure
   - Refactor providers to use proper state management
   - Build alert components with new design
3. Migrate ca-mention-alerts feature
   - Similar to twitter-alerts

### Phase 4: Infrastructure (Week 5)
1. Implement proper demo mode
   - Create mock services
   - Centralize mode detection
   - Remove duplicate files
2. Set up testing
   - Jest + React Testing Library
   - Test utilities
   - Component tests
3. Add error boundaries
4. Add loading states
5. Performance optimization

### Phase 5: Polish (Week 6)
1. Accessibility audit
2. Responsive design refinement
3. Animation and transitions
4. Documentation
5. Code review and cleanup

## Code Quality Standards

### 1. TypeScript
- Strict mode enabled
- No `any` types (use `unknown` when needed)
- Proper type inference
- Discriminated unions for complex types

### 2. Component Guidelines
- Functional components with hooks
- Props interfaces defined inline or separately
- Default props using ES6 defaults
- Proper prop destructuring
- Memoization where beneficial (React.memo, useMemo, useCallback)

### 3. Styled Components
- Use theme for all design tokens
- No magic numbers or hardcoded colors
- Use theme helpers for responsive design
- Name styled components clearly (Styled prefix or descriptive name)

### 4. State Management
- Context for global state (auth, theme)
- Local state for component-specific state
- Consider Zustand for complex alert management
- SignalR connection in provider

### 5. File Naming
- Components: PascalCase (Button.tsx, AlertCard.tsx)
- Utilities: camelCase (formatDate.ts, validators.ts)
- Types: PascalCase (Alert.ts, User.ts)
- Hooks: camelCase with use prefix (useAuth.ts)

### 6. Import Order
```typescript
// 1. External dependencies
import React from 'react';
import styled from 'styled-components';

// 2. Internal absolute imports
import { Button } from '@/components/atoms';
import { useAuth } from '@/features/auth';

// 3. Relative imports
import { formatDate } from './utils';
import { AlertProps } from './types';

// 4. Type imports
import type { Theme } from '@/types';
```

## Key Technologies

### Core
- React 19
- TypeScript 5.8
- Styled Components 6
- React Router 7

### State & Data
- React Context API
- SignalR for real-time
- Consider: Zustand for complex state

### Testing (to add)
- Vitest
- React Testing Library
- MSW for API mocking

### Tools
- ESLint
- Prettier
- Husky (git hooks)

## Performance Considerations

1. **Code splitting**: Lazy load routes and heavy components
2. **Memoization**: Use React.memo for expensive components
3. **Virtual scrolling**: For long alert lists (react-window)
4. **Optimized re-renders**: Proper dependency arrays in hooks
5. **Image optimization**: Use WebP, proper sizing
6. **Bundle analysis**: Regularly check bundle size

## Accessibility Goals

1. Semantic HTML
2. ARIA labels where needed
3. Keyboard navigation
4. Focus management
5. Screen reader support
6. Color contrast compliance (WCAG AA)

## Next Steps

1. Review this plan with the team/designer
2. Set up project board for tracking
3. Begin Phase 1: Foundation
4. Schedule regular design sync meetings
5. Plan for incremental delivery (avoid big bang release)

## Questions for Designer

1. Do you have a design system or component library to reference?
2. What are the brand colors, typography, spacing system?
3. Are there specific animation/transition patterns?
4. Mobile-first or desktop-first?
5. Any accessibility requirements?
6. Do you have design files (Figma, Sketch, etc.)?

## Notes

- Keep old code working during migration
- Use feature flags to gradually roll out new UI
- Maintain backward compatibility with backend APIs
- Document design decisions in code comments
- Regular code reviews during refactoring
