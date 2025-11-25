# CienceTerminal Backend Deployment Guide

## Overview
This guide covers deploying the CienceTerminal backend microservices to AWS ECS Fargate and connecting to the Amplify-hosted frontend.

## Prerequisites
- AWS Account with appropriate permissions
- AWS CLI installed and configured
- Docker installed locally
- Frontend deployed on AWS Amplify

## Architecture Overview
```
Amplify (Frontend) → ALB → ECS Fargate Services
                              ↓
                          SNS/SQS
                              ↓
                          RDS PostgreSQL
```

## Step 1: Create AWS Resources

### 1.1 Create RDS PostgreSQL Database
```bash
# Create RDS instance
aws rds create-db-instance \
  --db-instance-identifier cienceterminal-postgres \
  --db-instance-class db.t3.micro \
  --engine postgres \
  --master-username cienceadmin \
  --master-user-password YOUR_SECURE_PASSWORD \
  --allocated-storage 20 \
  --vpc-security-group-ids sg-xxxxx \
  --db-name cienceterminal \
  --publicly-accessible false

# Note the endpoint after creation:
aws rds describe-db-instances \
  --db-instance-identifier cienceterminal-postgres \
  --query 'DBInstances[0].Endpoint.Address'
```

### 1.2 Create SNS Topics (Production)
```bash
# Create SNS topics
aws sns create-topic --name cienceterminal-twitter-alerts
aws sns create-topic --name cienceterminal-ca-mention-detected
aws sns create-topic --name cienceterminal-mention-aggregates-updated
aws sns create-topic --name cienceterminal-alert-removal
aws sns create-topic --name cienceterminal-coin-blacklisted
aws sns create-topic --name cienceterminal-token-metrics-updated

# Save the ARNs for later
```

### 1.3 Create SQS Queues
```bash
# Create SQS queues
aws sqs create-queue --queue-name alert-service-twitter-alerts-queue
aws sqs create-queue --queue-name token-metrics-service-ca-mention-detected-queue
aws sqs create-queue --queue-name alert-service-mention-aggregates-updated-queue
aws sqs create-queue --queue-name alert-service-alert-removal-queue
aws sqs create-queue --queue-name alert-service-coin-blacklisted-queue
aws sqs create-queue --queue-name alert-service-token-metrics-updated-queue

# Subscribe queues to topics
aws sns subscribe \
  --topic-arn arn:aws:sns:us-east-1:YOUR_ACCOUNT:cienceterminal-twitter-alerts \
  --protocol sqs \
  --notification-endpoint arn:aws:sqs:us-east-1:YOUR_ACCOUNT:alert-service-twitter-alerts-queue

# Repeat for other topic-queue subscriptions
```

### 1.4 Create ECR Repositories
```bash
# Create ECR repositories for each service
aws ecr create-repository --repository-name cienceterminal/api-gateway
aws ecr create-repository --repository-name cienceterminal/twitter-scanner
aws ecr create-repository --repository-name cienceterminal/alert-service
aws ecr create-repository --repository-name cienceterminal/token-metrics
```

## Step 2: Build and Push Docker Images

### 2.1 Login to ECR
```bash
aws ecr get-login-password --region us-east-1 | \
  docker login --username AWS --password-stdin YOUR_ACCOUNT.dkr.ecr.us-east-1.amazonaws.com
```

### 2.2 Build and Push Images
```bash
# Set your AWS account ID
export AWS_ACCOUNT_ID=YOUR_ACCOUNT_ID
export AWS_REGION=us-east-1
export ECR_REGISTRY=${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_REGION}.amazonaws.com

# Build and push API Gateway
docker build -t cienceterminal/api-gateway -f services/api-gateway/Dockerfile .
docker tag cienceterminal/api-gateway:latest ${ECR_REGISTRY}/cienceterminal/api-gateway:latest
docker push ${ECR_REGISTRY}/cienceterminal/api-gateway:latest

# Build and push Twitter Scanner
docker build -t cienceterminal/twitter-scanner -f services/twitter-scanner/Dockerfile .
docker tag cienceterminal/twitter-scanner:latest ${ECR_REGISTRY}/cienceterminal/twitter-scanner:latest
docker push ${ECR_REGISTRY}/cienceterminal/twitter-scanner:latest

# Build and push Alert Service
docker build -t cienceterminal/alert-service -f services/alert-service/Dockerfile .
docker tag cienceterminal/alert-service:latest ${ECR_REGISTRY}/cienceterminal/alert-service:latest
docker push ${ECR_REGISTRY}/cienceterminal/alert-service:latest

# Build and push Token Metrics Service (if Dockerfile exists)
docker build -t cienceterminal/token-metrics -f services/token-metrics-service/Dockerfile .
docker tag cienceterminal/token-metrics:latest ${ECR_REGISTRY}/cienceterminal/token-metrics:latest
docker push ${ECR_REGISTRY}/cienceterminal/token-metrics:latest
```

## Step 3: Create ECS Cluster and Task Definitions

### 3.1 Create ECS Cluster
```bash
aws ecs create-cluster --cluster-name cienceterminal-cluster
```

### 3.2 Create IAM Role for ECS Tasks
Create a file `ecs-task-execution-role-policy.json`:
```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "ecr:GetAuthorizationToken",
        "ecr:BatchCheckLayerAvailability",
        "ecr:GetDownloadUrlForLayer",
        "ecr:BatchGetImage",
        "logs:CreateLogStream",
        "logs:PutLogEvents",
        "logs:CreateLogGroup",
        "sns:Publish",
        "sqs:ReceiveMessage",
        "sqs:DeleteMessage",
        "sqs:GetQueueAttributes"
      ],
      "Resource": "*"
    }
  ]
}
```

```bash
# Create IAM role
aws iam create-role \
  --role-name ecsTaskExecutionRole \
  --assume-role-policy-document file://ecs-trust-policy.json

aws iam attach-role-policy \
  --role-name ecsTaskExecutionRole \
  --policy-arn arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy

# Create custom policy for SNS/SQS
aws iam put-role-policy \
  --role-name ecsTaskExecutionRole \
  --policy-name SNSSQSAccess \
  --policy-document file://ecs-task-execution-role-policy.json
```

### 3.3 Create Task Definitions

Save each as a separate JSON file and register with ECS.

**api-gateway-task.json**:
```json
{
  "family": "cienceterminal-api-gateway",
  "networkMode": "awsvpc",
  "requiresCompatibilities": ["FARGATE"],
  "cpu": "256",
  "memory": "512",
  "executionRoleArn": "arn:aws:iam::YOUR_ACCOUNT:role/ecsTaskExecutionRole",
  "taskRoleArn": "arn:aws:iam::YOUR_ACCOUNT:role/ecsTaskExecutionRole",
  "containerDefinitions": [
    {
      "name": "api-gateway",
      "image": "YOUR_ACCOUNT.dkr.ecr.us-east-1.amazonaws.com/cienceterminal/api-gateway:latest",
      "portMappings": [
        {
          "containerPort": 8080,
          "protocol": "tcp"
        }
      ],
      "environment": [
        {
          "name": "ASPNETCORE_ENVIRONMENT",
          "value": "Production"
        },
        {
          "name": "AWS__UseLocalStack",
          "value": "false"
        },
        {
          "name": "AWS__Region",
          "value": "us-east-1"
        },
        {
          "name": "AUTH0_DOMAIN",
          "value": "dev-nbeb6mxwzmie2bep.us.auth0.com"
        },
        {
          "name": "AUTH0_AUDIENCE",
          "value": "https://cienceterminal-api"
        }
      ],
      "secrets": [
        {
          "name": "ApiKeys__Groq",
          "valueFrom": "arn:aws:secretsmanager:us-east-1:YOUR_ACCOUNT:secret:cienceterminal/groq-key"
        }
      ],
      "logConfiguration": {
        "logDriver": "awslogs",
        "options": {
          "awslogs-group": "/ecs/cienceterminal-api-gateway",
          "awslogs-region": "us-east-1",
          "awslogs-stream-prefix": "ecs"
        }
      }
    }
  ]
}
```

Register task definitions:
```bash
aws ecs register-task-definition --cli-input-json file://api-gateway-task.json
aws ecs register-task-definition --cli-input-json file://twitter-scanner-task.json
aws ecs register-task-definition --cli-input-json file://alert-service-task.json
aws ecs register-task-definition --cli-input-json file://token-metrics-task.json
```

## Step 4: Create Application Load Balancer

```bash
# Create security group for ALB
aws ec2 create-security-group \
  --group-name cienceterminal-alb-sg \
  --description "Security group for CienceTerminal ALB" \
  --vpc-id vpc-xxxxx

# Allow HTTP/HTTPS traffic
aws ec2 authorize-security-group-ingress \
  --group-id sg-xxxxx \
  --protocol tcp \
  --port 80 \
  --cidr 0.0.0.0/0

aws ec2 authorize-security-group-ingress \
  --group-id sg-xxxxx \
  --protocol tcp \
  --port 443 \
  --cidr 0.0.0.0/0

# Create ALB
aws elbv2 create-load-balancer \
  --name cienceterminal-alb \
  --subnets subnet-xxxxx subnet-yyyyy \
  --security-groups sg-xxxxx

# Create target groups
aws elbv2 create-target-group \
  --name cienceterminal-api-gateway-tg \
  --protocol HTTP \
  --port 8080 \
  --vpc-id vpc-xxxxx \
  --target-type ip \
  --health-check-path /health

aws elbv2 create-target-group \
  --name cienceterminal-alert-service-tg \
  --protocol HTTP \
  --port 8080 \
  --vpc-id vpc-xxxxx \
  --target-type ip \
  --health-check-path /health

# Create listener and rules
aws elbv2 create-listener \
  --load-balancer-arn arn:aws:elasticloadbalancing:... \
  --protocol HTTP \
  --port 80 \
  --default-actions Type=forward,TargetGroupArn=arn:aws:elasticloadbalancing:...
```

## Step 5: Create ECS Services

```bash
# Create API Gateway service
aws ecs create-service \
  --cluster cienceterminal-cluster \
  --service-name api-gateway \
  --task-definition cienceterminal-api-gateway \
  --desired-count 1 \
  --launch-type FARGATE \
  --network-configuration "awsvpcConfiguration={subnets=[subnet-xxxxx,subnet-yyyyy],securityGroups=[sg-xxxxx],assignPublicIp=ENABLED}" \
  --load-balancers targetGroupArn=arn:aws:elasticloadbalancing:...,containerName=api-gateway,containerPort=8080

# Create Alert Service (with SignalR)
aws ecs create-service \
  --cluster cienceterminal-cluster \
  --service-name alert-service \
  --task-definition cienceterminal-alert-service \
  --desired-count 1 \
  --launch-type FARGATE \
  --network-configuration "awsvpcConfiguration={subnets=[subnet-xxxxx,subnet-yyyyy],securityGroups=[sg-xxxxx],assignPublicIp=ENABLED}" \
  --load-balancers targetGroupArn=arn:aws:elasticloadbalancing:...,containerName=alert-service,containerPort=8080

# Create Twitter Scanner (background service, no load balancer)
aws ecs create-service \
  --cluster cienceterminal-cluster \
  --service-name twitter-scanner \
  --task-definition cienceterminal-twitter-scanner \
  --desired-count 1 \
  --launch-type FARGATE \
  --network-configuration "awsvpcConfiguration={subnets=[subnet-xxxxx,subnet-yyyyy],securityGroups=[sg-xxxxx],assignPublicIp=ENABLED}"

# Create Token Metrics Service (background service)
aws ecs create-service \
  --cluster cienceterminal-cluster \
  --service-name token-metrics \
  --task-definition cienceterminal-token-metrics \
  --desired-count 1 \
  --launch-type FARGATE \
  --network-configuration "awsvpcConfiguration={subnets=[subnet-xxxxx,subnet-yyyyy],securityGroups=[sg-xxxxx],assignPublicIp=ENABLED}"
```

## Step 6: Configure Frontend on Amplify

### 6.1 Set Environment Variables on Amplify

Go to Amplify Console → Your App → Environment Variables and add:

```
VITE_AUTH0_DOMAIN=dev-nbeb6mxwzmie2bep.us.auth0.com
VITE_AUTH0_CLIENT_ID=pawSMY3R2kIpa5Gmopqhu5DVal60ytGw
VITE_AUTH0_AUDIENCE=https://cienceterminal-api
VITE_API_BASE_URL=https://your-alb-dns-name.us-east-1.elb.amazonaws.com
```

### 6.2 Update CORS Configuration

Update the `.env` file and redeploy backend services with:

```bash
# Get your Amplify URL
AMPLIFY_URL=https://main.xxxxx.amplifyapp.com

# Update CORS settings in backend
CORS_ORIGIN_1=https://main.xxxxx.amplifyapp.com
```

You'll need to rebuild and redeploy the backend services with updated CORS settings.

## Step 7: Update Auth0 Configuration

1. Go to Auth0 Dashboard → Applications → Your App
2. Add Amplify URL to:
   - Allowed Callback URLs: `https://main.xxxxx.amplifyapp.com`
   - Allowed Logout URLs: `https://main.xxxxx.amplifyapp.com`
   - Allowed Web Origins: `https://main.xxxxx.amplifyapp.com`

## Step 8: Database Migration

Run database migrations against RDS:

```bash
# Update connection string in your migration tool
# Connection string format:
# Host=your-rds-endpoint.rds.amazonaws.com;Database=cienceterminal;Username=cienceadmin;Password=YOUR_PASSWORD

# Run migrations (adjust command based on your migration tool)
dotnet ef database update --project services/alert-service
```

## Alternative: Quick Deploy with AWS Copilot (Easier Option)

AWS Copilot CLI simplifies ECS deployment significantly:

```bash
# Install Copilot CLI
brew install aws/tap/copilot-cli  # macOS
# or download from https://aws.github.io/copilot-cli/

# Initialize application
copilot app init cienceterminal

# Create environment
copilot env init --name production

# Deploy services
copilot svc init --name api-gateway --svc-type "Load Balanced Web Service" --dockerfile services/api-gateway/Dockerfile
copilot svc init --name alert-service --svc-type "Load Balanced Web Service" --dockerfile services/alert-service/Dockerfile
copilot svc init --name twitter-scanner --svc-type "Backend Service" --dockerfile services/twitter-scanner/Dockerfile
copilot svc init --name token-metrics --svc-type "Backend Service" --dockerfile services/token-metrics-service/Dockerfile

# Deploy
copilot svc deploy --name api-gateway --env production
copilot svc deploy --name alert-service --env production
copilot svc deploy --name twitter-scanner --env production
copilot svc deploy --name token-metrics --env production
```

## Monitoring and Troubleshooting

### View Logs
```bash
# View ECS task logs
aws logs tail /ecs/cienceterminal-api-gateway --follow

# View service status
aws ecs describe-services \
  --cluster cienceterminal-cluster \
  --services api-gateway alert-service twitter-scanner token-metrics
```

### Common Issues

1. **CORS Errors**: Ensure Amplify URL is added to CORS configuration
2. **SignalR Connection Fails**: Check WebSocket support on ALB, ensure sticky sessions enabled
3. **SNS/SQS Errors**: Verify IAM permissions for ECS task role
4. **Database Connection**: Ensure RDS security group allows ECS tasks

## Cost Optimization

- Use Fargate Spot for background services (Twitter Scanner, Token Metrics)
- Set up auto-scaling based on CPU/memory metrics
- Use RDS instance scaling or Aurora Serverless for database
- Consider using NAT Gateway only for production, remove for dev

## Next Steps

1. Set up CI/CD with GitHub Actions or AWS CodePipeline
2. Configure CloudWatch alarms for monitoring
3. Set up AWS Secrets Manager for API keys
4. Enable AWS WAF on ALB for security
5. Set up Route 53 custom domain
6. Configure SSL/TLS certificate via ACM
