#!/bin/bash
set -e

IMAGE_TAG=${1:-latest}
AWS_REGION=${AWS_REGION:-eu-central-1}

# List of all backend services
SERVICES=(
    "api-gateway"
    "twitter-scanner"
    "alert-service"
    "token-metrics-service"
)

echo "========================================="
echo "Deploying all backend services"
echo "Image tag: $IMAGE_TAG"
echo "AWS Region: $AWS_REGION"
echo "========================================="
echo ""

# Deploy each service
for SERVICE in "${SERVICES[@]}"; do
    echo ""
    echo "========================================="
    echo "Deploying $SERVICE..."
    echo "========================================="
    ./scripts/deploy-service.sh "$SERVICE" "$IMAGE_TAG"

    if [ $? -eq 0 ]; then
        echo "✓ $SERVICE deployed successfully"
    else
        echo "✗ Failed to deploy $SERVICE"
        exit 1
    fi
done

echo ""
echo "========================================="
echo "All services deployed successfully!"
echo "========================================="
echo ""
echo "Deployed services:"
for SERVICE in "${SERVICES[@]}"; do
    echo "  ✓ $SERVICE:$IMAGE_TAG"
done
