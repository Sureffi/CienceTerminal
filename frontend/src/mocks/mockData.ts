import type { TwitterAlert, CaMentionAlert, CaMention } from '../models/Alert';

// Mock Twitter Alerts
export const mockTwitterAlerts: TwitterAlert[] = [
  {
    id: 'mock-twitter-1',
    timestamp: new Date(Date.now() - 5 * 60 * 1000).toISOString(), // 5 minutes ago
    severity: 'High',
    type: 'TwitterLegit',
    title: 'High-Profile Mention',
    message: 'Major influencer mentioned a new token',
    tweetLink: 'https://twitter.com/example/status/123456789',
    authorName: 'Crypto Influencer',
    authorUsername: 'cryptoinfluencer',
    authorProfilePicture: 'https://i.pravatar.cc/150?img=12',
    authorFollowers: 250000,
    authorIsBlueVerified: true,
    authorIsGoldVerified: false,
    tweetContent: 'Just discovered $MOON - this could be huge! Contract: ABC123xyz',
    coinName: 'MoonShot',
    coinSymbol: 'MOON',
    coinMint: 'ABC123xyz456',
    coinImageUrl: 'https://raw.githubusercontent.com/solana-labs/token-list/main/assets/mainnet/So11111111111111111111111111111111111111112/logo.png',
    launchpad: 'Raydium',
    coinFirstPoolCreatedAt: new Date(Date.now() - 2 * 60 * 60 * 1000).toISOString(),
    coinAge: '2H',
    coinMarketCap: 500000,
    coinHolderCount: 1250,
    coinLiquidity: 150000,
    coinVolume24h: 2400000,
    coinMentionCount24h: 150,
    coinTop10Holders: 25,
    coinDevHolding: 5,
    coinIsFreezeDisabled: true,
    coinIsMintDisabled: true,
  },
  {
    id: 'mock-twitter-3',
    timestamp: new Date(Date.now() - 30 * 60 * 1000).toISOString(), // 30 minutes ago
    severity: 'Low',
    type: 'TwitterSpam',
    title: 'Spam Alert',
    message: 'Potential spam detected',
    tweetLink: 'https://twitter.com/example/status/456789123',
    authorName: 'Spam Bot',
    authorUsername: 'spambot123',
    authorProfilePicture: 'https://i.pravatar.cc/150?img=42',
    authorFollowers: 100,
    authorIsBlueVerified: false,
    authorIsGoldVerified: false,
    tweetContent: 'BUY $SCAM NOW! 100x GUARANTEED!!!',
    coinName: 'ScamCoin',
    coinSymbol: 'SCAM',
    coinMint: 'GHI789mno012',
    coinImageUrl: 'https://raw.githubusercontent.com/solana-labs/token-list/main/assets/mainnet/EPjFWdd5AufqSSqeM2qN1xzybapC8G4wEGGkZwyTDt1v/logo.png',
    coinMarketCap: 1000,
    coinHolderCount: 5,
    coinVolume24h: 500,
    coinMentionCount24h: 3,
  },
];

// Mock CA Mention Alerts
export const mockCaMentionAlerts: CaMentionAlert[] = [
  {
    id: 'mock-ca-1',
    timestamp: new Date(Date.now() - 10 * 60 * 1000).toISOString(), // 10 minutes ago
    severity: 'High',
    type: 'TwitterCaMention',
    title: 'Trending Contract Address',
    message: 'Contract mentioned 15 times in 5 minutes',
    coinAddress: 'ABC123xyz456',
    coinSymbol: 'MOON',
    coinImageUrl: 'https://raw.githubusercontent.com/solana-labs/token-list/main/assets/mainnet/So11111111111111111111111111111111111111112/logo.png',
    topMentionerImageUrls: [
      'https://i.pravatar.cc/150?img=1',
      'https://i.pravatar.cc/150?img=2',
      'https://i.pravatar.cc/150?img=3',
    ],
    trendScore: 95.5,
    mentionCount24Hour: 150,
    lastMentioned: new Date(Date.now() - 2 * 60 * 1000).toISOString(),
    rank: 1,
    marketCap: 500000,
    liquidity: 150000,
    volume24h: 2400000,
    priceChange24H: 45.2,
    holderCount: 1250,
    topHoldersPercentage: 25,
    firstPoolCreatedAt: new Date(Date.now() - 2 * 60 * 60 * 1000).toISOString(),
  },
  {
    id: 'mock-ca-2',
    timestamp: new Date(Date.now() - 25 * 60 * 1000).toISOString(), // 25 minutes ago
    severity: 'Medium',
    type: 'TwitterCaMention',
    title: 'Growing Interest',
    message: 'Contract mentioned 8 times in 5 minutes',
    coinAddress: 'XYZ789abc123',
    coinSymbol: 'STAR',
    coinImageUrl: 'https://raw.githubusercontent.com/solana-labs/token-list/main/assets/mainnet/EPjFWdd5AufqSSqeM2qN1xzybapC8G4wEGGkZwyTDt1v/logo.png',
    topMentionerImageUrls: [
      'https://i.pravatar.cc/150?img=5',
      'https://i.pravatar.cc/150?img=6',
    ],
    trendScore: 72.3,
    mentionCount24Hour: 100,
    lastMentioned: new Date(Date.now() - 3 * 60 * 1000).toISOString(),
    rank: 2,
    marketCap: 250000,
    liquidity: 80000,
    volume24h: 1200000,
    priceChange24H: 12.8,
    holderCount: 850,
    topHoldersPercentage: 30,
    firstPoolCreatedAt: new Date(Date.now() - 6 * 60 * 60 * 1000).toISOString(),
  },
];

// Mock CA Mention Details (individual mentions for the modal)
export const mockCaMentionDetails: Record<string, CaMention[]> = {
  'ABC123xyz456': [
    {
      username: 'cryptowhale',
      followers: 125000,
      isVerified: true,
      ca: 'ABC123xyz456',
      mentionedAt: new Date(Date.now() - 5 * 60 * 1000).toISOString(),
      isReply: false,
      isQuote: false,
      isRetweet: false,
    },
    {
      username: 'tokentrader',
      followers: 45000,
      isVerified: false,
      ca: 'ABC123xyz456',
      mentionedAt: new Date(Date.now() - 12 * 60 * 1000).toISOString(),
      isReply: true,
      isQuote: false,
      isRetweet: false,
    },
    {
      username: 'defi_hunter',
      followers: 89000,
      isVerified: true,
      ca: 'ABC123xyz456',
      mentionedAt: new Date(Date.now() - 18 * 60 * 1000).toISOString(),
      isReply: false,
      isQuote: true,
      isRetweet: false,
    },
    {
      username: 'moon_tracker',
      followers: 34000,
      isVerified: false,
      ca: 'ABC123xyz456',
      mentionedAt: new Date(Date.now() - 25 * 60 * 1000).toISOString(),
      isReply: false,
      isQuote: false,
      isRetweet: true,
    },
    {
      username: 'solana_gems',
      followers: 210000,
      isVerified: true,
      ca: 'ABC123xyz456',
      mentionedAt: new Date(Date.now() - 35 * 60 * 1000).toISOString(),
      isReply: false,
      isQuote: false,
      isRetweet: false,
    },
  ],
  'XYZ789abc123': [
    {
      username: 'crypto_analyst',
      followers: 67000,
      isVerified: true,
      ca: 'XYZ789abc123',
      mentionedAt: new Date(Date.now() - 8 * 60 * 1000).toISOString(),
      isReply: false,
      isQuote: false,
      isRetweet: false,
    },
    {
      username: 'altcoin_daily',
      followers: 95000,
      isVerified: false,
      ca: 'XYZ789abc123',
      mentionedAt: new Date(Date.now() - 20 * 60 * 1000).toISOString(),
      isReply: true,
      isQuote: false,
      isRetweet: false,
    },
    {
      username: 'token_alerts',
      followers: 52000,
      isVerified: false,
      ca: 'XYZ789abc123',
      mentionedAt: new Date(Date.now() - 40 * 60 * 1000).toISOString(),
      isReply: false,
      isQuote: false,
      isRetweet: true,
    },
  ],
};

// Function to generate new mock alerts periodically (no PreLaunch)
export const generateRandomTwitterAlert = (): TwitterAlert => {
  const types: Array<'TwitterLegit' | 'TwitterSpam'> = ['TwitterLegit', 'TwitterSpam'];
  const type = types[Math.floor(Math.random() * types.length)];
  const severities: Array<'Low' | 'Medium' | 'High'> = ['Low', 'Medium', 'High'];

  const coinNames = ['MoonShot', 'RocketCoin', 'StarToken', 'DiamondHands', 'ToTheMoon'];
  const coinSymbols = ['MOON', 'ROCKET', 'STAR', 'DIAMOND', 'TTM'];
  const idx = Math.floor(Math.random() * coinNames.length);

  const avatarId = Math.floor(Math.random() * 70) + 1;
  const tokenImages = [
    'https://raw.githubusercontent.com/solana-labs/token-list/main/assets/mainnet/So11111111111111111111111111111111111111112/logo.png',
    'https://raw.githubusercontent.com/solana-labs/token-list/main/assets/mainnet/EPjFWdd5AufqSSqeM2qN1xzybapC8G4wEGGkZwyTDt1v/logo.png',
    'https://raw.githubusercontent.com/solana-labs/token-list/main/assets/mainnet/Es9vMFrzaCERmJfrF4H2FYD4KCoNkY11McCe8BenwNYB/logo.svg',
  ];

  const baseAlert = {
    id: `mock-${type}-${Date.now()}-${Math.random()}`,
    timestamp: new Date().toISOString(),
    severity: severities[Math.floor(Math.random() * severities.length)],
    type,
    title: type === 'TwitterLegit' ? 'High-Profile Mention' : 'Spam Alert',
    message: type === 'TwitterLegit' ? 'Major influencer mentioned a new token' : 'Potential spam detected',
    tweetLink: `https://twitter.com/example/status/${Math.floor(Math.random() * 1000000000)}`,
    authorName: `User ${Math.floor(Math.random() * 1000)}`,
    authorUsername: `user${Math.floor(Math.random() * 1000)}`,
    authorProfilePicture: `https://i.pravatar.cc/150?img=${avatarId}`,
    authorFollowers: Math.floor(Math.random() * 500000),
    authorIsBlueVerified: Math.random() > 0.7,
    authorIsGoldVerified: Math.random() > 0.9,
    tweetContent: `Check out $${coinSymbols[idx]}! This is amazing!`,
    coinName: coinNames[idx],
    coinSymbol: coinSymbols[idx],
    coinMint: `${Math.random().toString(36).substring(2, 15)}`,
    coinImageUrl: tokenImages[Math.floor(Math.random() * tokenImages.length)],
    launchpad: Math.random() > 0.5 ? 'Raydium' : 'Jupiter',
    coinFirstPoolCreatedAt: new Date(Date.now() - Math.random() * 24 * 60 * 60 * 1000).toISOString(),
    coinAge: `${Math.floor(Math.random() * 24)}H`,
    coinMarketCap: Math.floor(Math.random() * 1000000),
    coinHolderCount: Math.floor(Math.random() * 5000),
    coinLiquidity: Math.floor(Math.random() * 500000),
    coinVolume24h: Math.floor(Math.random() * 5000000),
    coinMentionCount24h: Math.floor(Math.random() * 500),
    coinTop10Holders: Math.floor(Math.random() * 50),
    coinDevHolding: Math.floor(Math.random() * 20),
    coinIsFreezeDisabled: Math.random() > 0.5,
    coinIsMintDisabled: Math.random() > 0.5,
  };

  return baseAlert as TwitterAlert;
};
