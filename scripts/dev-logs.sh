#!/bin/bash

# CienceTerminal Development Logs Script
# View aggregated logs from all services

echo "📋 CienceTerminal Development Logs"
echo "=================================="

# Parse command line arguments
SERVICE=""
FOLLOW=false

while [[ $# -gt 0 ]]; do
    case $1 in
        --service)
            SERVICE="$2"
            shift 2
            ;;
        -f|--follow)
            FOLLOW=true
            shift
            ;;
        --help)
            echo "Usage: $0 [OPTIONS]"
            echo ""
            echo "Options:"
            echo "  --service <name>  Show logs for specific service (twitter-scanner, alert-service, localstack)"
            echo "  -f, --follow      Follow log output"
            echo "  --help            Show this help message"
            echo ""
            echo "Available services:"
            echo "  twitter-scanner   Twitter Scanner Service"
            echo "  alert-service     Alert Service"
            echo "  localstack        LocalStack (AWS simulation)"
            echo "  all               All Docker services (default)"
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            echo "Use --help for usage information"
            exit 1
            ;;
    esac
done

if [ "$FOLLOW" = true ]; then
    FOLLOW_FLAG="-f"
else
    FOLLOW_FLAG=""
fi

if [ ! -z "$SERVICE" ]; then
    case $SERVICE in
        twitter-scanner|alert-service|localstack)
            echo "📋 Showing logs for $SERVICE..."
            docker-compose logs $FOLLOW_FLAG $SERVICE
            ;;
        all)
            echo "📋 Showing logs for all Docker services..."
            docker-compose logs $FOLLOW_FLAG
            ;;
        *)
            echo "❌ Unknown service: $SERVICE"
            echo "Available services: twitter-scanner, alert-service, localstack, all"
            exit 1
            ;;
    esac
else
    echo "📋 Showing logs for all Docker services..."
    echo "Use --service <name> to filter to a specific service"
    echo "Use -f to follow logs in real-time"
    echo ""
    docker-compose logs $FOLLOW_FLAG
fi