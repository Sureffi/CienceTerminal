# Getting Started with the Refactored Frontend

## Quick Start

### 1. Install Dependencies

Dependencies are already installed:
- ✅ `styled-components` - CSS-in-JS styling
- ✅ `@types/styled-components` - TypeScript definitions

### 2. Set Up Theme Provider

Update your `main.tsx` to wrap the app with `ThemeProvider`:

```tsx
import React from 'react';
import ReactDOM from 'react-dom/client';
import { ThemeProvider } from 'styled-components';
import App from './App';
import { theme } from './styles/theme';
import { GlobalStyles } from './styles/GlobalStyles';

const rootElement = document.getElementById('root')!;

ReactDOM.createRoot(rootElement).render(
  <React.StrictMode>
    <ThemeProvider theme={theme}>
      <GlobalStyles />
      <App />
    </ThemeProvider>
  </React.StrictMode>
);
```

### 3. Configure Path Aliases (Optional but Recommended)

Add to `vite.config.ts`:

```typescript
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import path from 'path';

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
});
```

Update `tsconfig.json`:

```json
{
  "compilerOptions": {
    "baseUrl": ".",
    "paths": {
      "@/*": ["./src/*"]
    }
  }
}
```

### 4. Start Using Components

```tsx
import { Button, Badge } from '@/components/atoms';
import { theme } from '@/styles/theme';

function MyComponent() {
  return (
    <div>
      <Button variant="primary" size="md" onClick={handleClick}>
        Click me
      </Button>
      <Badge variant="success">Active</Badge>
    </div>
  );
}
```

## What's Been Set Up

### ✅ Design System

- **Theme** - Centralized design tokens (`src/styles/theme/`)
  - Colors (primary, secondary, semantic, neutral)
  - Typography (font families, sizes, weights)
  - Spacing (consistent 8px grid)
  - Breakpoints (mobile, tablet, desktop, wide)
  - Shadows, transitions, border radius, z-index

- **GlobalStyles** - CSS reset and global styling
- **Animations** - Reusable keyframe animations

### ✅ TypeScript Theme Typing

- Full IntelliSense for theme in styled-components
- Type safety for all theme tokens
- See `src/types/styled.d.ts`

### ✅ Example Components

- **Button** - Full-featured button component with variants
- **Badge** - Label/tag component for status indicators

### ✅ Documentation

- **REFACTORING_PLAN.md** - Complete refactoring strategy
- **COMPONENT_ARCHITECTURE.md** - Component patterns and best practices
- **GETTING_STARTED.md** - This file

## Next Steps

### Phase 1: Customize Theme (Work with Designer)

1. Update colors in `src/styles/theme/colors.ts`
2. Update typography in `src/styles/theme/typography.ts`
3. Adjust spacing, shadows, etc. to match design system

### Phase 2: Build Core Components

Based on your designer's needs, create:

#### Atoms
- [ ] Input component
- [ ] Card component
- [ ] Icon component (SVG wrapper)
- [ ] Avatar component
- [ ] Spinner/Loader component
- [ ] Checkbox component
- [ ] Radio component

#### Molecules
- [ ] SearchBar (Input + Icon + Button)
- [ ] FilterBar
- [ ] AlertBadge (Badge + Icon)
- [ ] UserAvatar (Avatar + Text)

#### Organisms
- [ ] Header component
- [ ] AlertCard (Twitter/CA mention specific)
- [ ] Sidebar/Navigation

### Phase 3: Migrate Features

Start with one feature at a time:

1. **Auth feature** - Simplest, good starting point
   ```
   features/auth/
   ├── components/
   ├── hooks/
   ├── context/
   └── index.ts
   ```

2. **Twitter Alerts feature** - Main functionality
   ```
   features/twitter-alerts/
   ├── components/
   ├── hooks/
   ├── services/
   └── index.ts
   ```

3. **CA Mention Alerts feature** - Similar to Twitter
   ```
   features/ca-mention-alerts/
   ├── components/
   ├── hooks/
   ├── services/
   └── index.ts
   ```

### Phase 4: Remove Old Code

Once new components are working:
1. Remove old CSS modules (`.module.css` files)
2. Remove old components
3. Clean up unused imports

## Working with Your Designer

### Questions to Ask

1. **Design System**
   - Do you have a Figma/Sketch file?
   - What are the brand colors?
   - What fonts should we use?
   - What's the spacing system?

2. **Components**
   - Can you provide designs for key components?
   - What states should components have? (hover, active, disabled)
   - What animations/transitions?

3. **Responsive Design**
   - Mobile-first or desktop-first?
   - What are the breakpoints?
   - How should components adapt?

### Designer Handoff Process

1. **Get design files** (Figma, Sketch, Adobe XD)
2. **Extract design tokens**
   - Colors → `colors.ts`
   - Typography → `typography.ts`
   - Spacing → `spacing.ts`
3. **Build components** matching designs exactly
4. **Review together** and iterate

## Development Workflow

### Creating a New Component

```bash
# 1. Create component directory
mkdir -p src/components/atoms/NewComponent

# 2. Create component file
touch src/components/atoms/NewComponent/NewComponent.tsx

# 3. Create index file
touch src/components/atoms/NewComponent/index.ts

# 4. Implement component
# 5. Export from atoms/index.ts
# 6. Use in your app!
```

### Component Template

```tsx
// src/components/atoms/NewComponent/NewComponent.tsx
import React from 'react';
import styled from 'styled-components';

export interface NewComponentProps {
  // Define props
}

export const NewComponent: React.FC<NewComponentProps> = ({
  // Props
}) => {
  return (
    <Container>
      {/* Component content */}
    </Container>
  );
};

const Container = styled.div`
  /* Styles using theme */
`;
```

```typescript
// src/components/atoms/NewComponent/index.ts
export { NewComponent } from './NewComponent';
export type { NewComponentProps } from './NewComponent';
```

## Tips & Best Practices

### 1. Always Use Theme Tokens

```tsx
// ✅ Good
const Box = styled.div`
  color: ${({ theme }) => theme.colors.text.primary};
  padding: ${({ theme }) => theme.spacing.md};
`;

// ❌ Bad
const Box = styled.div`
  color: #F9FAFB;
  padding: 16px;
`;
```

### 2. Type Your Styled Props

```tsx
// ✅ Good
const Button = styled.button<{ $variant: 'primary' | 'secondary' }>`
  background: ${({ $variant, theme }) =>
    $variant === 'primary'
      ? theme.colors.primary.main
      : theme.colors.secondary.main
  };
`;

// ❌ Bad
const Button = styled.button`
  background: ${({ variant }: any) => variant === 'primary' ? '#00D9FF' : '#9D4EDD'};
`;
```

### 3. Use Transient Props ($)

Prefix props with `$` if they shouldn't be passed to the DOM:

```tsx
// ✅ Good - $ prevents React warning
<Box $isActive={true}>Content</Box>

// ❌ Bad - isActive appears as HTML attribute
<Box isActive={true}>Content</Box>
```

### 4. Keep Components Small

- Atoms: < 100 lines
- Molecules: < 200 lines
- Organisms: < 300 lines

If larger, split into smaller components.

### 5. Document Complex Components

```tsx
/**
 * AlertCard displays a cryptocurrency alert with actions
 *
 * @example
 * <AlertCard
 *   alert={alertData}
 *   onDismiss={handleDismiss}
 *   onViewDetails={handleView}
 * />
 */
export const AlertCard: React.FC<AlertCardProps> = ({ ... }) => {
  // ...
};
```

## Demo Mode Integration

Keep demo mode working during refactoring:

```typescript
// services/api/client.ts
const isDemoMode = import.meta.env.VITE_DEMO_MODE === 'true';

export const apiClient = isDemoMode
  ? createMockApiClient()
  : createRealApiClient();
```

## Troubleshooting

### Theme not available in styled-components

Make sure `ThemeProvider` wraps your app:

```tsx
import { ThemeProvider } from 'styled-components';
import { theme } from './styles/theme';

<ThemeProvider theme={theme}>
  <App />
</ThemeProvider>
```

### TypeScript errors with theme

Ensure `src/types/styled.d.ts` is included in your `tsconfig.json`:

```json
{
  "include": ["src/**/*"]
}
```

### Import errors with @ alias

Check `vite.config.ts` and `tsconfig.json` are configured correctly.

## Resources

- [styled-components docs](https://styled-components.com/)
- [Atomic Design methodology](https://bradfrost.com/blog/post/atomic-web-design/)
- [React TypeScript Cheatsheet](https://react-typescript-cheatsheet.netlify.app/)

## Support

If you have questions:
1. Check the documentation files in `/frontend/`
2. Review example components in `/components/atoms/`
3. Ask your team or designer for clarification

---

**Ready to build something amazing! 🚀**
