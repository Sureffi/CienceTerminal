namespace CienceTerminal.AWS.Configuration;

public class AwsOptions
{
    public const string SectionName = "AWS";

    public string Region { get; set; } = "us-east-1";
    public SnsOptions SNS { get; set; } = new();
    public SqsOptions SQS { get; set; } = new();
    public bool UseLocalStack { get; set; } = false;
    public string LocalStackUrl { get; set; } = "http://localhost:4566";
}

public class SnsOptions
{
    public string TwitterAlertsTopicArn { get; set; } = string.Empty;
    public string CaMentionDetectedTopicArn { get; set; } = string.Empty;
    public string MentionAggregatesUpdatedTopicArn { get; set; } = string.Empty;
    public string AlertRemovalTopicArn { get; set; } = string.Empty;
    public string CoinBlacklistedTopicArn { get; set; } = string.Empty;
    public string TokenMetricsUpdatedTopicArn { get; set; } = string.Empty;
}

public class SqsOptions
{
    public string TwitterAlertsQueueUrl { get; set; } = string.Empty;
    public string CaMentionDetectedQueueUrl { get; set; } = string.Empty;
    public string MentionAggregatesUpdatedQueueUrl { get; set; } = string.Empty;
    public string AlertRemovalQueueUrl { get; set; } = string.Empty;
    public string CoinBlacklistedQueueUrl { get; set; } = string.Empty;
    public string TokenMetricsUpdatedQueueUrl { get; set; } = string.Empty;
}
