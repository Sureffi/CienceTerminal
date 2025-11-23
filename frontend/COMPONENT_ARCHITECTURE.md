# Component Architecture Guide

## Overview

This frontend follows **Atomic Design principles** combined with **feature-based organization** for scalability and maintainability.

## Directory Structure

```
src/
├── components/          # Shared reusable components (Atomic Design)
│   ├── atoms/          # Basic building blocks
│   ├── molecules/      # Simple combinations of atoms
│   └── organisms/      # Complex UI sections
│
├── features/           # Feature-specific modules
│   └── [feature]/
│       ├── components/ # Feature-specific components
│       ├── hooks/      # Feature-specific hooks
│       ├── services/   # API calls for this feature
│       └── types/      # Feature-specific types
│
└── layouts/            # Page layout templates
```

## Atomic Design Hierarchy

### Atoms (components/atoms/)

**Definition**: The smallest, indivisible UI components. These are the basic building blocks.

**Examples**:
- `Button` - Clickable buttons with variants and states
- `Badge` - Labels and tags for status/categories
- `Input` - Text input fields
- `Icon` - SVG icon wrapper
- `Card` - Basic container component
- `Avatar` - User avatar/profile image
- `Spinner` - Loading indicator

**Characteristics**:
- No business logic
- Highly reusable across the entire app
- Accept simple props for configuration
- Fully styled with theme tokens
- Type-safe with TypeScript interfaces

**Example**:
```tsx
import { Button } from '@/components/atoms';

<Button variant="primary" size="md" onClick={handleClick}>
  Submit
</Button>
```

### Molecules (components/molecules/)

**Definition**: Simple combinations of atoms that form functional UI elements.

**Examples**:
- `SearchBar` - Input + Icon + Button
- `AlertBadge` - Badge + Icon with alert type
- `UserAvatar` - Avatar + Text (name/status)
- `FilterBar` - Multiple buttons/inputs for filtering
- `InfoCard` - Card + Icon + Text in specific layout

**Characteristics**:
- Combine 2-5 atoms
- Serve a single purpose
- Still highly reusable
- May have minimal internal state
- Props-driven behavior

**Example**:
```tsx
import { SearchBar } from '@/components/molecules';

<SearchBar
  value={searchTerm}
  onChange={setSearchTerm}
  placeholder="Search alerts..."
/>
```

### Organisms (components/organisms/)

**Definition**: Complex, feature-rich components that combine atoms and molecules.

**Examples**:
- `Header` - Logo + Navigation + UserMenu
- `AlertCard` - Complex alert display with actions
- `Sidebar` - Navigation + User info + Actions
- `DataTable` - Full-featured data table

**Characteristics**:
- Combine multiple atoms and molecules
- May contain business logic
- Less reusable (more specific)
- Can manage complex state
- Often connected to contexts/hooks

**Example**:
```tsx
import { AlertCard } from '@/components/organisms';

<AlertCard
  alert={alertData}
  onDismiss={handleDismiss}
  onViewDetails={handleViewDetails}
/>
```

## Feature Modules

### Structure

Each feature is self-contained with all its dependencies:

```
features/
└── twitter-alerts/
    ├── components/         # Feature-specific components
    │   ├── TwitterAlertCard.tsx
    │   ├── TwitterAlertList.tsx
    │   └── TwitterAlertFilters.tsx
    ├── hooks/             # Custom hooks
    │   ├── useTwitterAlerts.ts
    │   └── useTwitterAlertFilters.ts
    ├── services/          # API calls
    │   └── twitterAlertService.ts
    ├── types/             # Type definitions
    │   └── index.ts
    ├── utils/             # Feature utilities
    │   └── formatters.ts
    └── index.ts           # Public API (what to export)
```

### Benefits

1. **Encapsulation**: All related code is together
2. **Scalability**: Easy to add new features without affecting others
3. **Maintainability**: Changes are localized
4. **Testability**: Self-contained units are easier to test
5. **Reusability**: Export only what's needed

### Example Feature Module

```typescript
// features/twitter-alerts/index.ts
// This is the public API - only export what other features need

export { TwitterAlertList } from './components/TwitterAlertList';
export { useTwitterAlerts } from './hooks/useTwitterAlerts';
export type { TwitterAlert, TwitterAlertFilters } from './types';
```

## Component Best Practices

### 1. Component Structure

```tsx
import React from 'react';
import styled from 'styled-components';

// Props interface
interface MyComponentProps {
  title: string;
  onAction?: () => void;
  variant?: 'primary' | 'secondary';
}

// Component
export const MyComponent: React.FC<MyComponentProps> = ({
  title,
  onAction,
  variant = 'primary',
}) => {
  return (
    <Container variant={variant}>
      <Title>{title}</Title>
      {onAction && <Button onClick={onAction}>Action</Button>}
    </Container>
  );
};

// Styled components (in same file for small components)
const Container = styled.div<{ variant: string }>`
  padding: ${({ theme }) => theme.spacing.md};
  background: ${({ theme, variant }) =>
    variant === 'primary'
      ? theme.colors.primary.main
      : theme.colors.secondary.main
  };
`;

const Title = styled.h3`
  color: ${({ theme }) => theme.colors.text.primary};
  font-size: ${({ theme }) => theme.typography.fontSize.lg};
`;

const Button = styled.button`
  /* styles */
`;
```

### 2. File Organization

**Small components (< 100 lines)**:
```
Button/
├── Button.tsx     # Component + styles
└── index.ts       # Exports
```

**Large components (> 100 lines)**:
```
AlertCard/
├── AlertCard.tsx       # Main component
├── AlertCard.styles.ts # Styled components
├── AlertCard.types.ts  # Type definitions
├── AlertCard.utils.ts  # Helper functions
└── index.ts            # Exports
```

### 3. Props Guidelines

```typescript
// ✅ Good: Specific, type-safe props
interface ButtonProps {
  variant: 'primary' | 'secondary';
  size: 'sm' | 'md' | 'lg';
  onClick: () => void;
  disabled?: boolean;
  children: React.ReactNode;
}

// ❌ Bad: Too generic, unsafe
interface ButtonProps {
  style?: any;
  className?: string;
  [key: string]: any;
}

// ✅ Good: Use HTML element props when needed
interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant: 'primary' | 'secondary';
}
```

### 4. Styling Conventions

```tsx
// ✅ Good: Use theme tokens
const Container = styled.div`
  padding: ${({ theme }) => theme.spacing.md};
  color: ${({ theme }) => theme.colors.text.primary};
  font-size: ${({ theme }) => theme.typography.fontSize.base};
`;

// ❌ Bad: Hardcoded values
const Container = styled.div`
  padding: 16px;
  color: #F9FAFB;
  font-size: 16px;
`;

// ✅ Good: Responsive with theme helpers
const Container = styled.div`
  padding: ${({ theme }) => theme.spacing.sm};

  @media (min-width: ${({ theme }) => theme.breakpoints.tablet}) {
    padding: ${({ theme }) => theme.spacing.md};
  }
`;
```

### 5. State Management

```tsx
// ✅ Good: Local state for component-specific data
const [isOpen, setIsOpen] = useState(false);

// ✅ Good: Context for global/shared state
const { user } = useAuth();

// ✅ Good: Custom hooks for complex logic
const { alerts, loading, error } = useTwitterAlerts();

// ❌ Bad: Prop drilling through many levels
// Use context or state management instead
```

### 6. Performance Optimization

```tsx
// ✅ Memoize expensive computations
const sortedAlerts = useMemo(
  () => alerts.sort((a, b) => b.timestamp - a.timestamp),
  [alerts]
);

// ✅ Memoize callbacks passed to child components
const handleRemove = useCallback(
  (id: string) => {
    removeAlert(id);
  },
  [removeAlert]
);

// ✅ Memoize components that rarely change
const MemoizedAlertCard = React.memo(AlertCard);
```

### 7. Accessibility

```tsx
// ✅ Good: Semantic HTML and ARIA labels
<button
  aria-label="Close alert"
  aria-pressed={isActive}
  onClick={handleClose}
>
  <CloseIcon aria-hidden="true" />
</button>

// ✅ Good: Keyboard navigation
const handleKeyDown = (e: React.KeyboardEvent) => {
  if (e.key === 'Enter' || e.key === ' ') {
    handleAction();
  }
};
```

## Styling with styled-components

### Theme Usage

```tsx
import styled from 'styled-components';

// Access theme via props
const Button = styled.button`
  color: ${({ theme }) => theme.colors.primary.main};
  padding: ${({ theme }) => theme.spacing.md};
  border-radius: ${({ theme }) => theme.borderRadius.md};
  transition: ${({ theme }) => theme.transitions.normal};
`;

// Use css helper for conditional styles
import { css } from 'styled-components';

const Button = styled.button<{ $isActive: boolean }>`
  ${({ $isActive, theme }) =>
    $isActive &&
    css`
      background: ${theme.colors.primary.main};
      color: ${theme.colors.primary.contrast};
    `}
`;
```

### Transient Props

Use `$` prefix for props that shouldn't be passed to DOM:

```tsx
// ✅ Good: $ prefix prevents DOM attribute warning
const Box = styled.div<{ $isHighlighted: boolean }>`
  background: ${({ $isHighlighted, theme }) =>
    $isHighlighted ? theme.colors.primary.main : 'transparent'
  };
`;

<Box $isHighlighted={true}>Content</Box>

// ❌ Bad: isHighlighted will appear as DOM attribute
const Box = styled.div<{ isHighlighted: boolean }>`
  /* ... */
`;
```

## Import Aliases

Use path aliases for clean imports:

```typescript
// ✅ Good: Clean, absolute imports
import { Button, Badge } from '@/components/atoms';
import { useAuth } from '@/features/auth';
import { theme } from '@/styles/theme';

// ❌ Bad: Relative import hell
import { Button } from '../../../components/atoms/Button';
import { useAuth } from '../../../../features/auth/hooks/useAuth';
```

Configure in `vite.config.ts`:
```typescript
resolve: {
  alias: {
    '@': path.resolve(__dirname, './src'),
  },
},
```

## Testing Strategy

### Atoms
- Unit tests for all variants and states
- Snapshot tests for visual regression
- Accessibility tests

### Molecules
- Integration tests with atoms
- User interaction tests
- Edge case handling

### Organisms
- Complex interaction flows
- Mock external dependencies
- Test context integration

### Features
- End-to-end feature tests
- API mocking with MSW
- Real-world user scenarios

## Migration Strategy

When refactoring existing components:

1. **Create new component** following this architecture
2. **Keep old component** working (no breaking changes)
3. **Gradually migrate** usage to new component
4. **Remove old component** when fully migrated

Example:
```tsx
// Old: TwitterAlertCard.tsx (keep for now)
// New: components/organisms/AlertCard/AlertCard.tsx

// Gradual migration:
// Step 1: Create new component
// Step 2: Use new component in one page
// Step 3: Migrate other pages
// Step 4: Remove old component
```

## Questions?

- Unsure if component is atom/molecule/organism? → Start with atom, promote if it grows
- Component used in one feature only? → Put in feature folder
- Component used across features? → Put in shared components
- Complex state management? → Consider custom hook in feature
- Reusable logic? → Create utility function or custom hook
