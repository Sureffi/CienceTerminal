#!/bin/bash

# CienceTerminal Development Stop Script
# Stops all development services

echo "🛑 Stopping CienceTerminal development environment..."

# Stop Docker containers
echo "🐳 Stopping Docker containers..."
docker-compose down

# Kill any running .NET processes for our services
echo "🔧 Stopping .NET services..."

# Find and kill Twitter Scanner processes
TWITTER_PIDS=$(pgrep -f "TwitterScanner.API" || true)
if [ ! -z "$TWITTER_PIDS" ]; then
    echo "Stopping Twitter Scanner processes: $TWITTER_PIDS"
    kill $TWITTER_PIDS 2>/dev/null || true
fi

# Find and kill Alert Service processes
ALERT_PIDS=$(pgrep -f "AlertService.API" || true)
if [ ! -z "$ALERT_PIDS" ]; then
    echo "Stopping Alert Service processes: $ALERT_PIDS"
    kill $ALERT_PIDS 2>/dev/null || true
fi

# Kill any remaining dotnet processes related to our project
PROJECT_PIDS=$(pgrep -f "CienceTerminal" || true)
if [ ! -z "$PROJECT_PIDS" ]; then
    echo "Stopping remaining CienceTerminal processes: $PROJECT_PIDS"
    kill $PROJECT_PIDS 2>/dev/null || true
fi

# Stop frontend dev server if running
FRONTEND_PIDS=$(pgrep -f "vite" || true)
if [ ! -z "$FRONTEND_PIDS" ]; then
    echo "Stopping frontend development server: $FRONTEND_PIDS"
    kill $FRONTEND_PIDS 2>/dev/null || true
fi

echo "✅ All services stopped!"
echo ""
echo "To start again, run: scripts/dev-start.sh"