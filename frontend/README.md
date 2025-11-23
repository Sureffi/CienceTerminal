# CienceTerminal Frontend

A React + TypeScript + Vite application for real-time cryptocurrency monitoring and alerts.

## 🚧 Refactoring in Progress

**Current Branch**: `frontend-demo-mode`

This frontend is undergoing a complete refactor to production-ready standards with styled-components and modern architecture.

📚 **Refactoring Documentation**:
- **[REFACTORING_PLAN.md](./REFACTORING_PLAN.md)** - Complete 6-week refactoring strategy
- **[COMPONENT_ARCHITECTURE.md](./COMPONENT_ARCHITECTURE.md)** - Component patterns and best practices
- **[GETTING_STARTED.md](./GETTING_STARTED.md)** - Setup guide for the new architecture

✅ **Completed**:
- Installed styled-components + TypeScript types
- Created comprehensive design system (theme, tokens, animations)
- Built atomic component examples (Button, Badge)
- Set up path aliases (`@/` imports)
- Written detailed documentation

🚧 **Next Steps**:
- Customize theme with designer's specifications
- Build complete component library
- Migrate existing features to new architecture

---

## Current Implementation (Legacy)

## Features

- Real-time Twitter alerts for cryptocurrency mentions
- Contract Address (CA) mention tracking
- SignalR-based live updates
- Auth0 authentication integration
- **Demo mode with mock data** - No backend or authentication required!

## Quick Start

### Demo Mode (Recommended for Design/Development)

The easiest way to get started - no backend or authentication setup required:

```bash
cd frontend
npm install

# Create .env file with demo mode enabled
echo "VITE_DEMO_MODE=true" > .env

# Start development server
npm run dev
```

Open [http://localhost:5173](http://localhost:5173) and navigate to `/dashboard` - you'll see the app with live mock data, **no login required**!

### Production Mode (With Real Backend)

For connecting to the real backend with Auth0 authentication:

```bash
cd frontend
npm install

# Copy and configure environment variables
cp .env.example .env

# Edit .env and set:
# VITE_DEMO_MODE=false
# VITE_AUTH0_DOMAIN=your-auth0-domain
# VITE_AUTH0_CLIENT_ID=your-client-id
# VITE_AUTH0_AUDIENCE=your-audience
# VITE_API_GATEWAY_URL=http://localhost:5149

# Start development server
npm run dev
```

## Environment Configuration

### Demo Mode vs Production Mode

| Feature | Demo Mode (`VITE_DEMO_MODE=true`) | Production Mode (`VITE_DEMO_MODE=false`) |
|---------|-----------------------------------|------------------------------------------|
| Backend Required | ❌ No | ✅ Yes |
| Authentication | ❌ No login required | ✅ Auth0 required |
| Data Source | Mock data (local) | Real-time SignalR |
| New Alerts | Auto-generated every 30-60s | Live from backend |
| Use Case | Design review, demos, development | Production deployment |

### Environment Variables

Copy `.env.example` to `.env` and configure:

```bash
# Demo Mode Configuration
VITE_DEMO_MODE=true              # Enable demo mode (false for production)

# Auth0 Configuration (only needed when VITE_DEMO_MODE=false)
VITE_AUTH0_DOMAIN=your-domain.us.auth0.com
VITE_AUTH0_CLIENT_ID=your-client-id
VITE_AUTH0_AUDIENCE=https://cienceterminal-api

# API Gateway URL (only needed when VITE_DEMO_MODE=false)
VITE_API_GATEWAY_URL=http://localhost:5149
```

## Development Workflow

### Working with Designers

1. **Enable demo mode** for design review:
   ```bash
   echo "VITE_DEMO_MODE=true" > .env
   npm run dev
   ```

2. **Share the dashboard URL** with your designer:
   - Go to [http://localhost:5173/dashboard](http://localhost:5173/dashboard)
   - No login required - they see the UI immediately
   - Mock alerts appear automatically every 30-60 seconds

3. **Deploy to Vercel** for remote design review (see deployment section below)

### Testing with Real Backend

1. **Switch to production mode**:
   ```bash
   echo "VITE_DEMO_MODE=false" > .env
   # Also set Auth0 variables
   npm run dev
   ```

2. **Start backend services** (see main README)

3. **Test authentication flow** and live alerts

## Available Scripts

```bash
# Development server with hot reload
npm run dev

# Build for production
npm run build

# Preview production build locally
npm run preview

# Run linter
npm run lint
```

## Deployment

### Deploy to Vercel (Demo Mode - Recommended)

Perfect for sharing with designers and stakeholders without backend setup.

#### Option 1: Vercel CLI (Fastest)

```bash
cd frontend

# Install Vercel CLI globally (one time)
npm install -g vercel

# Deploy
vercel

# Follow prompts:
# - Link to existing project or create new
# - Accept default settings
```

The `vercel.json` configuration automatically sets `VITE_DEMO_MODE=true` for deployments.

#### Option 2: Vercel Dashboard

1. Go to [vercel.com](https://vercel.com) and sign in
2. Click **"Add New Project"**
3. Import your GitHub repository
4. **Important**: Set **Root Directory** to `frontend`
5. Environment variables are automatically set from `vercel.json`
6. Click **"Deploy"**

Your demo will be live at: `https://your-project.vercel.app`

**Share the dashboard URL** with anyone: `https://your-project.vercel.app/dashboard`
- No login required
- Works immediately
- Live mock data simulation

#### Continuous Deployment

Once connected to GitHub, Vercel automatically deploys:
- **Production**: Every push to `main` branch
- **Preview**: Every pull request gets its own URL

### Deploy to Production (With Backend)

For production deployment with real backend:

1. Set up your backend services
2. Configure environment variables in Vercel dashboard:
   ```
   VITE_DEMO_MODE=false
   VITE_AUTH0_DOMAIN=your-domain.us.auth0.com
   VITE_AUTH0_CLIENT_ID=your-client-id
   VITE_AUTH0_AUDIENCE=https://cienceterminal-api
   ```
3. Deploy

## Demo Mode Features

When `VITE_DEMO_MODE=true`:

- ✅ **No Backend Required** - All data is mocked locally
- ✅ **No Authentication** - Skip login, go straight to dashboard
- ✅ **Mock Alerts Pre-loaded** - 3 Twitter alerts, 2 CA mention alerts
- ✅ **Auto-Generated Alerts** - New alerts appear every 30-60 seconds
- ✅ **Full UI Functionality** - Remove alerts, view details, etc.
- ✅ **Console Logging** - See `[DEMO MODE]` messages in browser console

Perfect for:
- 🎨 Designer previews and feedback
- 📊 Stakeholder demos and presentations
- 🧪 UI/UX testing without backend
- 💻 Frontend development without dependencies

## Project Structure

```
frontend/
├── src/
│   ├── components/              # React components
│   │   ├── Auth/               # Authentication components
│   │   ├── TwitterAlerts/      # Twitter alert components
│   │   │   ├── TwitterAlertProvider.tsx       # Real provider (SignalR)
│   │   │   └── TwitterAlertProvider.demo.tsx  # Demo provider (mock)
│   │   ├── CaMentionAlerts/    # CA mention components
│   │   │   ├── CaMentionAlertProvider.tsx       # Real provider
│   │   │   └── CaMentionAlertProvider.demo.tsx  # Demo provider
│   │   ├── Landing/            # Landing page
│   │   └── Layout/             # Layout components
│   ├── contexts/               # React contexts
│   │   ├── AuthContext.tsx     # Real Auth0 context
│   │   └── AuthContext.demo.tsx # Demo auth (no login)
│   ├── mocks/                  # Mock data for demo mode
│   │   └── mockData.ts         # Mock alerts and generators
│   ├── models/                 # TypeScript interfaces
│   ├── config/                 # Configuration files
│   ├── utils/                  # Utility functions
│   ├── App.tsx                 # Main app (conditionally uses demo/real providers)
│   └── main.tsx                # Entry point (conditionally uses Auth0)
├── .env.example                # Environment variable template
├── vercel.json                 # Vercel deployment config (sets demo mode)
└── package.json
```

## Tech Stack

- **React 19**: UI framework with modern hooks
- **TypeScript 5.8**: Type safety and developer experience
- **Vite 7**: Lightning-fast build tool and dev server
- **SignalR**: Real-time communication (production mode)
- **Auth0**: Authentication (production mode)
- **React Router**: Client-side routing

## Troubleshooting

### Demo mode not working?

Check browser console for `[DEMO MODE]` messages. If you don't see them:

```bash
# Verify .env file
cat .env
# Should show: VITE_DEMO_MODE=true

# Restart dev server
npm run dev
```

### Alerts not appearing in demo mode?

Demo alerts auto-generate every 30-60 seconds. Check:
- Browser console for `[DEMO MODE] New mock Twitter alert generated`
- 3 pre-loaded alerts should appear immediately
- Wait 30-60 seconds for new alerts to appear

### Production mode authentication failing?

Verify your `.env` has correct Auth0 credentials:
```bash
VITE_DEMO_MODE=false
VITE_AUTH0_DOMAIN=your-actual-domain.us.auth0.com
VITE_AUTH0_CLIENT_ID=your-actual-client-id
VITE_AUTH0_AUDIENCE=https://cienceterminal-api
```

## Support

For issues or questions:
- Check the troubleshooting section above
- Review browser console for error messages
- Contact the development team
- Create an issue in the repository

---

**Quick Tips:**

- 🚀 **For designers**: Use demo mode - it's the fastest way to see the UI
- 🔧 **For developers**: Use demo mode for frontend work, production mode for integration testing
- 🌐 **For demos**: Deploy to Vercel in demo mode - instant live preview
- 📱 **For stakeholders**: Share Vercel URL - works on any device, no setup required
