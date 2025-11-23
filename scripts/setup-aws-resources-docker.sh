#!/bin/bash

echo "Setting up AWS resources in LocalStack using Docker AWS CLI..."

# Configuration
LOCALSTACK_ENDPOINT="http://cienceterminal-localstack:4566"
NETWORK_NAME="cienceterminal_cienceterminal-network"
AWS_DOCKER_CMD="docker run --rm --network $NETWORK_NAME -e AWS_ACCESS_KEY_ID=test -e AWS_SECRET_ACCESS_KEY=test -e AWS_DEFAULT_REGION=us-east-1 amazon/aws-cli:latest"

echo "Using LocalStack endpoint: $LOCALSTACK_ENDPOINT"
echo "Using Docker network: $NETWORK_NAME"

# Wait for LocalStack to be ready
echo "Waiting for LocalStack to be ready..."
for i in {1..30}; do
    if $AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sns list-topics > /dev/null 2>&1; then
        echo "LocalStack is ready!"
        break
    fi
    echo "Attempt $i: Waiting for LocalStack..."
    sleep 2
done

echo "Creating AWS resources..."

# Create SNS Topics
echo "Creating SNS topics..."
$AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sns create-topic --name cienceterminal-twitter-alerts
$AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sns create-topic --name cienceterminal-ca-mention-detected
$AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sns create-topic --name cienceterminal-mention-aggregates-updated
$AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sns create-topic --name cienceterminal-alert-removal
$AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sns create-topic --name cienceterminal-coin-blacklisted
$AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sns create-topic --name cienceterminal-token-metrics-updated

# Create SQS Queues
echo "Creating SQS queues..."
$AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sqs create-queue --queue-name alert-service-twitter-alerts-queue
$AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sqs create-queue --queue-name alert-service-mention-aggregates-updated-queue
$AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sqs create-queue --queue-name alert-service-alert-removal-queue
$AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sqs create-queue --queue-name alert-service-coin-blacklisted-queue
$AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sqs create-queue --queue-name alert-service-token-metrics-updated-queue
$AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sqs create-queue --queue-name token-metrics-service-ca-mention-detected-queue

# Subscribe SQS queues to SNS topics
echo "Subscribing SQS queues to SNS topics..."

# Get topic ARNs
TWITTER_TOPIC_ARN=$($AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sns list-topics --query 'Topics[?contains(TopicArn, `cienceterminal-twitter-alerts`)].TopicArn' --output text)
CA_MENTION_DETECTED_TOPIC_ARN=$($AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sns list-topics --query 'Topics[?contains(TopicArn, `cienceterminal-ca-mention-detected`)].TopicArn' --output text)
MENTION_AGGREGATES_UPDATED_TOPIC_ARN=$($AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sns list-topics --query 'Topics[?contains(TopicArn, `cienceterminal-mention-aggregates-updated`)].TopicArn' --output text)
ALERT_REMOVAL_TOPIC_ARN=$($AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sns list-topics --query 'Topics[?contains(TopicArn, `cienceterminal-alert-removal`)].TopicArn' --output text)
COIN_BLACKLISTED_TOPIC_ARN=$($AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sns list-topics --query 'Topics[?contains(TopicArn, `cienceterminal-coin-blacklisted`)].TopicArn' --output text)
TOKEN_METRICS_UPDATED_TOPIC_ARN=$($AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sns list-topics --query 'Topics[?contains(TopicArn, `cienceterminal-token-metrics-updated`)].TopicArn' --output text)

echo "Twitter Topic ARN: $TWITTER_TOPIC_ARN"
echo "CA Mention Detected Topic ARN: $CA_MENTION_DETECTED_TOPIC_ARN"
echo "Mention Aggregates Updated Topic ARN: $MENTION_AGGREGATES_UPDATED_TOPIC_ARN"
echo "Alert Removal Topic ARN: $ALERT_REMOVAL_TOPIC_ARN"
echo "Coin Blacklisted Topic ARN: $COIN_BLACKLISTED_TOPIC_ARN"
echo "Token Metrics Updated Topic ARN: $TOKEN_METRICS_UPDATED_TOPIC_ARN"

# Get queue ARNs
TWITTER_QUEUE_URL="$LOCALSTACK_ENDPOINT/000000000000/alert-service-twitter-alerts-queue"
MENTION_AGGREGATES_UPDATED_QUEUE_URL="$LOCALSTACK_ENDPOINT/000000000000/alert-service-mention-aggregates-updated-queue"
ALERT_REMOVAL_QUEUE_URL="$LOCALSTACK_ENDPOINT/000000000000/alert-service-alert-removal-queue"
COIN_BLACKLISTED_QUEUE_URL="$LOCALSTACK_ENDPOINT/000000000000/alert-service-coin-blacklisted-queue"
TOKEN_METRICS_UPDATED_QUEUE_URL="$LOCALSTACK_ENDPOINT/000000000000/alert-service-token-metrics-updated-queue"
CA_MENTION_DETECTED_QUEUE_URL="$LOCALSTACK_ENDPOINT/000000000000/token-metrics-service-ca-mention-detected-queue"

TWITTER_QUEUE_ARN=$($AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sqs get-queue-attributes --queue-url $TWITTER_QUEUE_URL --attribute-names QueueArn --query 'Attributes.QueueArn' --output text)
MENTION_AGGREGATES_UPDATED_QUEUE_ARN=$($AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sqs get-queue-attributes --queue-url $MENTION_AGGREGATES_UPDATED_QUEUE_URL --attribute-names QueueArn --query 'Attributes.QueueArn' --output text)
ALERT_REMOVAL_QUEUE_ARN=$($AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sqs get-queue-attributes --queue-url $ALERT_REMOVAL_QUEUE_URL --attribute-names QueueArn --query 'Attributes.QueueArn' --output text)
COIN_BLACKLISTED_QUEUE_ARN=$($AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sqs get-queue-attributes --queue-url $COIN_BLACKLISTED_QUEUE_URL --attribute-names QueueArn --query 'Attributes.QueueArn' --output text)
TOKEN_METRICS_UPDATED_QUEUE_ARN=$($AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sqs get-queue-attributes --queue-url $TOKEN_METRICS_UPDATED_QUEUE_URL --attribute-names QueueArn --query 'Attributes.QueueArn' --output text)
CA_MENTION_DETECTED_QUEUE_ARN=$($AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sqs get-queue-attributes --queue-url $CA_MENTION_DETECTED_QUEUE_URL --attribute-names QueueArn --query 'Attributes.QueueArn' --output text)

echo "Twitter Queue ARN: $TWITTER_QUEUE_ARN"
echo "Mention Aggregates Updated Queue ARN: $MENTION_AGGREGATES_UPDATED_QUEUE_ARN"
echo "Alert Removal Queue ARN: $ALERT_REMOVAL_QUEUE_ARN"
echo "Coin Blacklisted Queue ARN: $COIN_BLACKLISTED_QUEUE_ARN"
echo "Token Metrics Updated Queue ARN: $TOKEN_METRICS_UPDATED_QUEUE_ARN"
echo "CA Mention Detected Queue ARN: $CA_MENTION_DETECTED_QUEUE_ARN"

# Subscribe queues to topics
echo "Creating subscriptions..."
$AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sns subscribe --topic-arn $TWITTER_TOPIC_ARN --protocol sqs --notification-endpoint $TWITTER_QUEUE_ARN
$AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sns subscribe --topic-arn $CA_MENTION_DETECTED_TOPIC_ARN --protocol sqs --notification-endpoint $CA_MENTION_DETECTED_QUEUE_ARN
$AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sns subscribe --topic-arn $MENTION_AGGREGATES_UPDATED_TOPIC_ARN --protocol sqs --notification-endpoint $MENTION_AGGREGATES_UPDATED_QUEUE_ARN
$AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sns subscribe --topic-arn $ALERT_REMOVAL_TOPIC_ARN --protocol sqs --notification-endpoint $ALERT_REMOVAL_QUEUE_ARN
$AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sns subscribe --topic-arn $COIN_BLACKLISTED_TOPIC_ARN --protocol sqs --notification-endpoint $COIN_BLACKLISTED_QUEUE_ARN
$AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sns subscribe --topic-arn $TOKEN_METRICS_UPDATED_TOPIC_ARN --protocol sqs --notification-endpoint $TOKEN_METRICS_UPDATED_QUEUE_ARN

echo "AWS resources setup complete!"

# List created resources
echo "Created resources:"
echo "SNS Topics:"
$AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sns list-topics --query 'Topics[].TopicArn' --output text

echo "SQS Queues:"
$AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sqs list-queues --query 'QueueUrls[]' --output text

echo "SNS Subscriptions:"
$AWS_DOCKER_CMD --endpoint-url=$LOCALSTACK_ENDPOINT sns list-subscriptions --query 'Subscriptions[].[TopicArn,Protocol,Endpoint]' --output text