export type TwitterAlertType =
  | 'TwitterLegit'
  | 'TwitterSpam'
  | 'TwitterPreLaunch';

export type CaMentionAlertType = 'TwitterCaMention';

export type AlertType = TwitterAlertType | CaMentionAlertType;

export type AlertSeverity =
  | 'Low'
  | 'Medium'
  | 'High';

export type TrendDirection =
  | 'None'
  | 'Up'
  | 'Down';

export interface BaseAlert {
  id: string;
  timestamp: string;
  severity: AlertSeverity;
  type: AlertType;
  title: string;
  message: string;
}

export interface TwitterLegitAlert extends BaseAlert {
  type: 'TwitterLegit';
  tweetLink: string;
  authorName: string;
  authorUsername: string;
  authorProfilePicture: string;
  authorFollowers: number;
  authorIsBlueVerified: boolean;
  authorIsGoldVerified: boolean;
  tweetContent: string;
  coinName: string;
  coinSymbol: string;
  coinMint: string;
  coinImageUrl?: string;
  launchpad?: string;
  coinFirstPoolCreatedAt?: string;
  coinAge?: string;
  coinMarketCap?: number;
  coinHolderCount?: number;
  coinLiquidity?: number;
  coinVolume24h?: number;
  coinMentionCount24h?: number;
  coinTop10Holders?: number;
  coinDevHolding?: number;
  coinIsFreezeDisabled?: boolean;
  coinIsMintDisabled?: boolean;
}

export interface TwitterSpamAlert extends BaseAlert {
  type: 'TwitterSpam';
  tweetLink: string;
  authorName: string;
  authorUsername: string;
  authorProfilePicture: string;
  authorFollowers: number;
  authorIsBlueVerified: boolean;
  authorIsGoldVerified: boolean;
  tweetContent: string;
  coinName: string;
  coinSymbol: string;
  coinMint: string;
  coinImageUrl?: string;
  launchpad?: string;
  coinFirstPoolCreatedAt?: string;
  coinAge?: string;
  coinMarketCap?: number;
  coinHolderCount?: number;
  coinLiquidity?: number;
  coinVolume24h?: number;
  coinMentionCount24h?: number;
  coinTop10Holders?: number;
  coinDevHolding?: number;
  coinIsFreezeDisabled?: boolean;
  coinIsMintDisabled?: boolean;
}

export interface TwitterPreLaunchAlert extends BaseAlert {
  type: 'TwitterPreLaunch';
  tweetLink: string;
  authorName: string;
  authorUsername: string;
  authorProfilePicture: string;
  authorFollowers: number;
  authorIsBlueVerified: boolean;
  authorIsGoldVerified: boolean;
  tweetContent: string;
  coinName: string;
  coinSymbol: string;
  coinMint: string;
  coinImageUrl?: string;
  launchpad?: string;
  coinFirstPoolCreatedAt?: string;
  coinAge?: string;
  coinMc?: number;
  coinHolderCount?: number;
  coinVolume24h?: number;
  coinMentionCount24h?: number;
}

export interface CaMention {
  username: string;
  followers: number;
  isVerified: boolean;
  ca: string;
  mentionedAt: string;
  isReply: boolean;
  isQuote: boolean;
  isRetweet: boolean;
}

export interface TwitterCaMentionAlert extends BaseAlert {
  type: 'TwitterCaMention';
  coinAddress: string;
  coinSymbol: string;
  coinImageUrl?: string;
  topMentionerImageUrls?: string[];
  trendScore: number;
  mentionCount24Hour: number;
  lastMentioned?: string;
  rank?: number;

  // Token financial metrics
  marketCap?: number;
  liquidity?: number;
  volume24h?: number;
  priceChange24H?: number;
  holderCount?: number;
  topHoldersPercentage?: number;
  firstPoolCreatedAt?: string;

  // Legacy field name (deprecated, use firstPoolCreatedAt)
  launchpad?: string;
  coinFirstPoolCreatedAt?: string;
}

export type TwitterAlert = TwitterLegitAlert | TwitterSpamAlert | TwitterPreLaunchAlert;

export type CaMentionAlert = TwitterCaMentionAlert;

export type Alert = TwitterAlert | CaMentionAlert;