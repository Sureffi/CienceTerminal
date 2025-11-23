#!/bin/bash

# CienceTerminal Development Setup Script
# One-time setup for development environment

set -e # Exit on any error

echo "🚀 Setting up CienceTerminal development environment..."

# Function to check if command exists
command_exists() {
	command -v "$1" >/dev/null 2>&1
}

# Check required tools
echo "📋 Checking prerequisites..."

if ! command_exists "docker"; then
	echo "❌ Docker is required but not installed"
	exit 1
fi

if ! command_exists "docker-compose"; then
	echo "❌ Docker Compose is required but not installed"
	exit 1
fi

if ! command_exists "dotnet"; then
	echo "❌ .NET 8 SDK is required but not installed"
	exit 1
fi

if ! command_exists "node"; then
	echo "❌ Node.js is required but not installed"
	exit 1
fi

echo "✅ All prerequisites found"

# Set executable permissions on scripts
echo "📝 Setting script permissions..."
chmod +x scripts/*.sh

# Restore .NET dependencies
echo "📦 Restoring .NET dependencies..."
dotnet restore

# Install frontend dependencies
echo "🎨 Installing frontend dependencies..."
cd frontend
npm install
cd ..

# Start LocalStack and setup AWS resources
echo "☁️ Starting LocalStack and setting up AWS resources..."
docker-compose up -d localstack

# Setup AWS resources
echo "⚙️ Setting up AWS resources..."
./scripts/setup-aws-resources-docker.sh

# Wait for LocalStack to be ready
echo "⏳ Waiting for LocalStack to be ready..."
for i in {1..30}; do
	if curl -s http://localhost:4566/_localstack/health | grep -q '"sns": "running"'; then
		echo "✅ LocalStack is ready!"
		break
	fi
	echo "Attempt $i: Waiting for LocalStack..."
	sleep 2
done

echo ""
echo "🎉 Development environment setup complete!"
echo ""
echo "Next steps:"
echo "  • Run 'scripts/dev-start.sh' to start all services"
echo "  • Run 'scripts/dev-logs.sh' to view service logs"
echo "  • Run 'scripts/dev-stop.sh' to stop all services"
echo ""

