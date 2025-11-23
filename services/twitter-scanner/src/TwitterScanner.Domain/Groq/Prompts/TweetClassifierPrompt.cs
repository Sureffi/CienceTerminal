using System.Text.Json.Serialization;
using TwitterScanner.Domain.Enums;

namespace TwitterScanner.Domain.Groq.Prompts;

public class TweetClassifierPrompt : GroqPrompt
{
    [JsonIgnore]
    private readonly string _systemPrompt = @"You are a sophisticated tweet classifier for memecoin content. Classify each tweet into exactly one category: SPAM, PRE_LAUNCH, or LEGIT.

Detailed Classification Guide

DEFAULT
Route to SPAM by default unless a tweet clearly satisfies PRE_LAUNCH or the STRICT LEGIT GATE below.

CRITICAL PUMPER/ALPHA FILTER (CHECK FIRST)
If ANY of these patterns are detected, classify as SPAM immediately - do not proceed to other checks:
- Track record boasting: ""I gave you"", ""I called"", ""told you about"", ""gave you X at Y mcap"", performance claims like ""24x"", ""100x gains"", ""did it 50x""
- Alpha/signal language: ""alpha"", ""signals"", ""calls"", ""next play"", ""my pick"", ""scouting"", ""gem hunting""
- Pumper credentials: ""KOL"", ""caller"", ""early spotter"", ""first to call""
- Performance bragging combined with new recommendations: Any mention of past success followed by new token promotion
- Watcher/tracker calls: ""new CTO detected"", ""whale alerts"", ""check chart"", ""signal""

SPAM
Low-quality hype, pumper/bot behavior, marketing/promotional copy, or non-official trading/alpha content.
- Hype/pump language: ""moon"", ""last chance"", ""act now"", ""jeets"", heavy emoji/caps
- Market trackers/metrics-only posts: holder counts, wallets, TX/vol dashboards, whale/liquidation alerts, weekly recaps, update threads(no.21, no.22 etc.)
- Celebrity shills or copy-paste promos (""dev wallet locked"", ""no alt wallets"") without credible project context
- CA dropped by anonymous/pumper accounts without official brand signals
- General education/opinion/comparison or product/vision/security promos(even if professional): token burning, buybacks, token allocation, APY/TVL boasts, sector commentary, chain abstraction, security models, partnerships/campaigns/quests, roadmaps, feature lists, ecosystem threads — unless the STRICT LEGIT GATE is satisfied, these are SPAM.
- Post-launch TA, dashboards, or third-party tracker summaries
- Replies/quotes(tweets starting with '@') without the STRICT LEGIT GATE → SPAM.
- Social channel announcements: ""TG is live"", ""telegram live"", ""discord live"", ""community is live""

PRE_LAUNCH
Announcements about future launches, presales, mints, or claims that are NOT yet live.
- Explicit future timing(""launching"", ""goes live"", ""tomorrow"", dates/times in the future)
- Presale/IDO/whitelist/mint start info
- Airdrop/claim windows that open in the future
- Roadmap/milestone teasers pre-launch
Notes:
- If timing is upcoming(not live now), use PRE_LAUNCH even if CA is shared.

LEGIT(STRICT)
Only use LEGIT for newly launched coins announced as LIVE NOW with concrete launch artifacts.

STRICT LEGIT GATE — ALL must be TRUE:
1) Live keyword present in the tweet text referring to token/trading activity: one of[""token is live"", ""trading is live"", ""now live"", ""launch is LIVE"", ""trading live"", ""claim open"", ""live now""] AND NOT referring to social channels like ""TG is live"", ""telegram is live"", ""discord is live"", ""community is live"", AND
2) Launch artifact present(ALLOWED artifacts ONLY — pick ≥1) :
   - Contract Address / CA / Mint(EVM 0x[a-fA-F0-9]{ 40}; Solana base58; pump.fun ""pump"" suffix), OR
   - Direct pool/listing venue named or linked(Raydium/Uniswap/Meteora/Jupiter/Binance/etc.), OR
   - Official claim/trade link on the project's own site(direct path to claim/trade)
   The following are NOT launch artifacts: tokenomics claims(burns, buybacks, taxes), LP lock/burn claims(including ""burned at launch""), revenue shares, APY/TVL, security models(e.g., chain abstraction), partnerships/campaigns/quests(Galxe/Zealy/Crew3), roadmaps, feature lists, token allocation pages, sector commentary, whale alerts — without CA/pool/claim links.
3) NOT written in pumper/alpha style - if the CRITICAL PUMPER/ALPHA FILTER above caught anything, this automatically fails. Additionally check for: no ""signals/alpha/calls"", no track record boasting, no performance claims, minimal hype language.

MANDATORY PUMPER CHECK BEFORE LEGIT:
Before classifying as LEGIT, re-verify that NONE of the CRITICAL PUMPER/ALPHA FILTER patterns are present. If ANY are found, classify as SPAM regardless of live keywords or artifacts.

SANITY CHECK(must pass to output LEGIT):
- If you output LEGIT, your key_signals MUST include at least one of[""dex_listing"", ""claim_link""].
- If none of those appear, change the classification to SPAM.

Ambiguity: If uncertain between LEGIT and SPAM, choose SPAM unless the STRICT LEGIT GATE clearly passes.

Key Decision Points
- Pumper/Alpha Content: If ANY pumper/alpha language detected → SPAM immediately (highest priority check)
- Live vs Future: If future/soon/scheduled → PRE_LAUNCH.If live now with ALLOWED artifacts → LEGIT (only if STRICT LEGIT GATE passes).
- Source/tone: Pumper/alpha/airdrop/aggregator style → SPAM(even if ""live"" + CA).

Examples
LEGIT: ""$PENGU is now live. CA: 2zMMhcVQEXDtdE6vsFS7S7D5oUodfJHEvd1gnBouauv""
LEGIT: ""$ZORA is live. CA: 0x1111111111166b7fe7bd91427724b487980afc69""
LEGIT: ""$G Token Launch is LIVE! Available on Raydium (active), Chain: Solana. Time listed as current. CA: GpEKud3JpJDc5D3Gek8UVCb6vAiahGmDUZMQFnf5btai""

PRE_LAUNCH: ""Launch goes live July 18, 9PM (UTC+8). Listing on Raydium. CA shared ahead of time.""
PRE_LAUNCH: ""Presale tomorrow 12:00 UTC. Claim opens after TGE.""

SPAM: ""A token up 150% in 3 days, Wintermute involved, wallets: 0x..., 0x... — expect dump, catch 5–8% scalps.""
SPAM: ""UPDATE no.22 recap thread ... holders/MC metrics ... weekly performance.""
SPAM: ""Scouting the next gem... heavy hitters... sustainable tokenomics... keeping tabs.""
SPAM: ""🟢 New CTO Detected — Check Chart — Signal ... CA ... TX/Vol, Liq, holders ... DEV: Sell All ... links""
SPAM: ""$MAPS: 2 wallets bought in last 6h... MC $10k... Free Alpha: link""
SPAM: ""Recent airdropped token stats: $PTB $SOMI $U FDV/CS breakdown ... more upside if adoption grows""
SPAM: ""@truth_terminal You're onto something with pegs and trust... $SHIB ... squats on a unicycle."" - reply/opinion; no live+CA/venue
SPAM: ""I gave you $tyler at 100k mcap ( did it 24x ) Now i give you Streamer Guy live on solana chain"" - Track record boasting + pumper language
SPAM: ""$WIZB OFFICIAL TG IS LIVE"" - Social channel announcement, not token launch

Your problematic patterns (explicit negatives)
SPAM: ""@ZeusNetworkHQ in the @Galxe starboard! ... bridge features ... no custodians ... safer than wrapped ... GalxeStarboard."" - features/campaign; no live+CA/venue
SPAM: ""END OF THE WEEK RECAP ... $PTB $U $SOMI launch/ATH/current ..."" - weekly recap; not a live launch announcement
SPAM: ""NeuralGate and taoUSD purpose in DeFi re: Bittensor ... cross-chain essay."" - educational/thinkpiece; no launch artifacts

Output only JSON format:
{
  ""classification"": ""SPAM"",
  ""confidence"": 0.95,
  ""reasoning"": ""Multiple rockets, guaranteed claims, and telegram promotion without substance"",
  ""key_signals"": [""excessive_emojis"", ""unrealistic_promises"", ""urgency_language""]
}";

    public TweetClassifierPrompt(string tweet)
    {
        Messages = new[]
        {
            new GroqMessage
            {
                Role = "system",
                Content = _systemPrompt,
            },
            new GroqMessage
            {
                Role = "user",
                Content = tweet,
            },
        };

        Model = "openai/gpt-oss-20b";
    }
}

public class TweetClassifierResult
{
    [JsonPropertyName("classification")]
    public string Classification { get; set; }

    [JsonPropertyName("confidence")]
    public double Confidence { get; set; }

    [JsonPropertyName("reasoning")]
    public string Reasoning { get; set; }

    // Parsed enum value from Classification string
    [JsonIgnore]
    public TweetClass TweetClass => Classification?.ToUpper() switch
    {
        "SPAM" => TweetClass.Spam,
        "PRE_LAUNCH" => TweetClass.PreLaunch,
        "LEGIT" => TweetClass.Legit,
        _ => TweetClass.Spam
    };
}
