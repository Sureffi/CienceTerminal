#!/bin/bash
set -e

SERVICE_NAME=$1
IMAGE_TAG=${2:-latest}
AWS_REGION=${AWS_REGION:-eu-central-1}
AWS_ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text)
ECR_REGISTRY="$AWS_ACCOUNT_ID.dkr.ecr.$AWS_REGION.amazonaws.com"

if [ -z "$SERVICE_NAME" ]; then
	echo "Usage: ./deploy-service.sh <service-name> [image-tag]"
	echo "Example: ./deploy-service.sh twitter-scanner latest"
	echo ""
	echo "Available services:"
	echo "  - api-gateway"
	echo "  - twitter-scanner"
	echo "  - alert-service"
	echo "  - token-metrics-service"
	exit 1
fi

echo "==> Logging into ECR..."
aws ecr get-login-password --region $AWS_REGION |
	docker login --username AWS --password-stdin $ECR_REGISTRY

echo "==> Building $SERVICE_NAME..."
docker build -t $SERVICE_NAME:$IMAGE_TAG \
	-f services/$SERVICE_NAME/Dockerfile .

echo "==> Tagging image..."
docker tag $SERVICE_NAME:$IMAGE_TAG \
	$ECR_REGISTRY/$SERVICE_NAME:$IMAGE_TAG

echo "==> Pushing to ECR..."
docker push $ECR_REGISTRY/$SERVICE_NAME:$IMAGE_TAG

echo ""
echo "==> Done! Image pushed to:"
echo "    $ECR_REGISTRY/$SERVICE_NAME:$IMAGE_TAG"
