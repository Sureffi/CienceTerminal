#!/bin/bash

# CienceTerminal Development Start Script
# Starts the development environment with options

set -e

echo "🚀 Starting CienceTerminal development environment..."

# Parse command line arguments
MODE="hybrid"  # Default mode
FRONTEND_ONLY=false

while [[ $# -gt 0 ]]; do
    case $1 in
        --docker)
            MODE="docker"
            shift
            ;;
        --hybrid)
            MODE="hybrid"
            shift
            ;;
        --frontend-only)
            FRONTEND_ONLY=true
            shift
            ;;
        --help)
            echo "Usage: $0 [OPTIONS]"
            echo ""
            echo "Options:"
            echo "  --docker        Run everything in Docker"
            echo "  --hybrid        Run LocalStack in Docker, services natively (default)"
            echo "  --frontend-only Run backend in Docker, frontend natively"
            echo "  --help          Show this help message"
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            echo "Use --help for usage information"
            exit 1
            ;;
    esac
done

# Load environment variables
if [ -f .env ]; then
    echo "📝 Loading environment variables from .env"
    export $(grep -v '^#' .env | xargs)
fi

if [ "$FRONTEND_ONLY" = true ]; then
    echo "🐳 Starting backend services in Docker..."
    docker-compose up -d

    echo "⏳ Waiting for services to be ready..."
    sleep 10

    echo "🎨 Starting frontend in development mode..."
    cd frontend
    npm run dev &
    FRONTEND_PID=$!
    cd ..

    echo ""
    echo "✅ Frontend development server started!"
    echo "   Frontend: http://localhost:3000"
    echo "   Backend API: http://localhost:5148"
    echo ""
    echo "Press Ctrl+C to stop all services"

    # Trap Ctrl+C to clean up
    trap 'echo "Stopping services..."; kill $FRONTEND_PID 2>/dev/null; docker-compose down; exit 0' INT
    wait $FRONTEND_PID

elif [ "$MODE" = "docker" ]; then
    echo "🐳 Starting full Docker stack..."
    docker-compose up

elif [ "$MODE" = "hybrid" ]; then
    echo "🐳 Starting LocalStack..."
    docker-compose up -d localstack

    # Wait for LocalStack
    echo "⏳ Waiting for LocalStack to be ready..."
    LOCALSTACK_READY=false
    for i in {1..30}; do
        if curl -s http://localhost:4566/_localstack/health | grep -q '"sns": "available"'; then
            echo "✅ LocalStack is ready!"
            LOCALSTACK_READY=true
            break
        fi
        echo "Attempt $i: Waiting for LocalStack..."
        sleep 2
    done

    if [ "$LOCALSTACK_READY" = false ]; then
        echo "❌ LocalStack failed to start properly after 60 seconds"
        echo "Try running: docker-compose logs localstack"
        exit 1
    fi

    # Ensure AWS resources exist
    echo "⚙️ Setting up AWS resources..."
    ./scripts/setup-aws-resources-docker.sh

    echo ""
    echo "✅ Hybrid development infrastructure ready!"
    echo "   LocalStack: http://localhost:4566"
    echo ""
    echo "To start services manually:"
    echo "   API Gateway:      dotnet run --project services/api-gateway/src/ApiGateway.API"
    echo "   Twitter Scanner:  dotnet run --project services/twitter-scanner/src/TwitterScanner.API"
    echo "   Alert Service:    dotnet run --project services/alert-service/src/AlertService.API"
    echo "   Token Metrics:    dotnet run --project services/token-metrics-service/src/TokenMetrics.API"
    echo "   User Management:  dotnet run --project services/user-management/src/UserManagement.API"
    echo "   Frontend:         cd frontend && npm run dev"
    echo ""
    echo "Services will run on:"
    echo "   API Gateway:      http://localhost:${API_GATEWAY_PORT:-5149}"
    echo "   Twitter Scanner:  http://localhost:${TWITTER_SCANNER_PORT:-5147}"
    echo "   Alert Service:    http://localhost:${ALERT_SERVICE_PORT:-5148}"
    echo "   Token Metrics:    http://localhost:${TOKEN_METRICS_PORT:-5237}"
    echo "   User Management:  http://localhost:${USER_MANAGEMENT_PORT:-5150}"
    echo "   Frontend:         http://localhost:${FRONTEND_PORT:-3000}"
    echo ""
    echo "Note: Frontend will connect to services through the API Gateway at http://localhost:${API_GATEWAY_PORT:-5149}"
    echo ""
    echo "Use './scripts/dev-stop.sh' to stop LocalStack when done"
fi