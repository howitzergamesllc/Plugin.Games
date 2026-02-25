
using System;

namespace Plugin.Games
{    
    /// <summary>
    /// Games Score metadata
    /// </summary>
    public class GamesScoreMetadata
    {
        /// <summary>
        /// (Apple Only) Game Center stores this custom game-specific property for you. It allows you to associate an arbitrary 
        /// 64-bit unsigned integer value with the score data that you report to Game Center. For abstraction long was used as its
        /// most compatible with 64-bit unsigned integers. You decide how to interpret this value in your game. 
        /// Throws <see cref="GamesError.InvalidScoreMetadata"/> if the value is greater than the maximum value for an unsigned 64-bit integer (18446744073709551615).
        /// </summary>
        public long AppleContext { get; set; }  // Apple

        /// <summary>
        /// (Android Only) Google's optional metadata about this score. The value may contain no more than 64 URI-safe 
        /// characters as defined by section 2.3 of RFC 3986. Throws <see cref="GamesError.InvalidScoreMetadata"/> exception
        /// if greater than 64 characters.
        /// </summary>
        public string GoogleScoreTag { get; set; }  // Google
    }

    /// <summary>
    /// (Android Only) The Leaderboard's page direction of score data.
    /// </summary>
    public enum GamesPageDirection
    {
        //Android Types
        /// <summary>
        /// Direction advancing toward the end of the data set.
        /// </summary>
        Next = 0,

        /// <summary>
        /// Constant indicating that no pagination is occurring.
        /// </summary>
        None = -1,

        /// <summary>
        /// Direction advancing toward the beginning of the data set.
        /// </summary>
        Prev = 1,

        /// <summary>
        /// Direction is unknown
        /// </summary>
        Unknown
    }

    /// <summary>
    /// Score info specific to Apple Platforms
    /// </summary>
    [Preserve(AllMembers = true)]
    public class GamesScoreAppleExtras
    {
        /// <summary>
        /// The player's unique identifier.
        /// </summary>
        public string GamePlayerId { get; set; }

        /// <summary>
        /// The player's team's unique identifier.
        /// </summary>
        public string TeamPlayerId { get; set; }
        
        /// <summary>
        /// The player's alias used when the player is not friends.
        /// </summary>
        public string Alias { get; set; }
        
        /// <summary>
        /// Indicates if the player can be invited to a game.
        /// </summary>
        public bool IsInvitable { get; set; }

        /// <summary>
        /// Player's hi-res image
        /// </summary>
        public byte[] PlayerLargeImage { get; set; }

        /// <summary>
        /// Player's normal icon image
        /// </summary>
        public byte[] PlayerIconImage { get; set; }
    }

    /// <summary>
    /// Score info specific to Android Platforms
    /// </summary>
    [Preserve(AllMembers = true)]
    public class GamesScoreAndroidExtras
    {

        /// <summary>
        /// Player's hi-res image
        /// </summary>
        public Uri PlayerLargeImageUri { get; set; }

        /// <summary>
        /// Player's hi-res image
        /// </summary>
        public string PlayerLargeImageUrl { get; set; }

        /// <summary>
        /// Player's normal icon image
        /// </summary>
        public Uri PlayerIconImageUri { get; set; }

        /// <summary>
        /// Player's normal icon image URL
        /// </summary>
        public string PlayerIconImageUrl { get; set; }
    }

    /// <summary>
    /// Represents a single score on a leaderboard
    /// </summary>
    [Preserve(AllMembers = true)]
    public class GamesLeaderboardScore
    {
        /// <summary>
        /// Player's display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Raw score value
        /// </summary>
        public long RawScore { get; set; }

        /// <summary>
        /// Formatted score for display purposes
        /// </summary>
        public string DisplayScore { get; set; }

        /// <summary>
        /// Rank of the score in the leaderboard
        /// </summary>
        public long Rank { get; set; }

        /// <summary>
        /// Formatted rank for display purposes
        /// </summary>
        public string DisplayRank { get; set; }

        /// <summary>
        /// Date and time when the score was recorded (converted from platform-native timestamp in UTC)
        /// </summary>
        public DateTime DateAndTime { get; set; }

        /// <summary>
        /// Metadata associated with the score
        /// </summary>
        public GamesScoreMetadata Metadata { get; set; }

        /// <summary>
        /// Extra information for apple platforms
        /// </summary>
        public GamesScoreAppleExtras Apple { get; init; }

        /// <summary>
        /// Extra information for Android platforms
        /// </summary>
        public GamesScoreAndroidExtras Android { get; init; }
    }

    /// <summary>
    /// Gets timespan associated with the current leaderboard
    /// </summary>
    public enum GamesLeaderBoardTimeSpan
    {
        /// <summary>
        /// Scores are never reset.
        /// </summary>
        AllTime = 2,

        /// <summary>
        /// Scores are reset every day. The reset occurs at 11:59PM PST.
        /// </summary>
        Daily = 0,

        /// <summary>
        /// Scores are reset once per week. The reset occurs at 11:59PM PST on Sunday.
        /// </summary>
        Weekly = 1,

        /// <summary>
        /// An unknown type, possibly recently added in a new update.
        /// </summary>
        Unknown = -1
    }

    /// <summary>
    /// The type of Leaderboard
    /// </summary>
    public enum GamesLeaderboardCollection
    {
        //Android Types
        /// <summary>
        /// Collection constant for friends leaderboards. These leaderboards contain the scores of players in the viewing player's friends list.
        /// </summary>
        Friends = 3,

        /// <summary>
        /// Collection constant for public leaderboards. Public leaderboards contain the scores of players who are sharing their gameplay activity publicly.
        /// </summary>
        Public = 0,

        /// <summary>
        /// An unknown type, possibly recently added in a new update.
        /// </summary>
        Unknown = -1
    }

    /// <summary>
    /// (Android Only)The Leaderboard's sort order
    /// </summary>
    public enum GamesLeaderboardSortOrder
    {
        //Android Types
        /// <summary>
        /// Score order constant for leaderboards where scores are sorted in descending order. Larger scores at the top.
        /// </summary>
        Descending = 1,

        /// <summary>
        /// Score order constant for leaderboards where scores are sorted in ascending order. Smaller scores at the top.
        /// </summary>
        Ascending = 0,

        /// <summary>
        /// An unknown type, possibly recently added in a new update.
        /// </summary>
        Unknown = -1
    }

    /// <summary>
    /// (Apple Only) The type of Leaderboard
    /// </summary>
    public enum GamesLeaderboardType
    {
        //Android Types
        /// <summary>
        /// Classic leaderboard type
        /// </summary>
        Classic = 1,

        /// <summary>
        /// Recurring leaderboard type
        /// </summary>
        Recurring = 0,

        /// <summary>
        /// An unknown type, possibly recently added in a new update.
        /// </summary>
        Unknown = -1
    }

    /// <summary>
    /// Leaderboard info specific to Apple Platforms
    /// </summary>
    [Preserve(AllMembers = true)]
    public class GamesLeaderboardAppleExtras
    {
        /// <summary>
        /// The type of leaderboard.
        /// </summary>
        public GamesLeaderboardType LeaderboardType { get; set; }

        /// <summary>
        /// Icon Image for the leaderboard.
        /// </summary>
        public byte[] IconImageBytes { get; set; }
    }

    /// <summary>
    /// Leaderboard info specific to Android platform
    /// </summary>
    [Preserve(AllMembers = true)]
    public class GamesLeaderboardAndroidExtras
    {
        /// <summary>
        /// How scores are ordered on this leaderboard.
        /// </summary>
        public GamesLeaderboardSortOrder SortOrder { get; set; }

        /// <summary>
        /// Total number of scores in this variant (NUM_SCORES_UNKNOWN if unknown)
        /// </summary>
        public long NumScores { get; set; }

        /// <summary>
        /// Icon Image for the leaderboard.
        /// </summary>
        public Uri IconImageUri { get; set; }

        /// <summary>
        /// Icon Image for the leaderboard.
        /// </summary>
        public string IconImageUrl { get; set; }
    }

	/// <summary>
	/// Leaderboard metadata
	/// </summary>
	[Preserve(AllMembers = true)]
	public class GamesLeaderboard
    {
        /// <summary>
        /// The unique leaderboard ID.
        /// </summary>
        public string LeaderboardId { get; set; }

        /// <summary>
        /// The display name of the leaderboard.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Time span of this variant (Daily, Weekly, AllTime)
        /// </summary>
        public GamesLeaderBoardTimeSpan TimeSpan { get; set; }

        /// <summary>
        /// Collection type (Public or Friends)
        /// </summary>
        public GamesLeaderboardCollection Collection { get; set; }

        /// <summary>
        /// Extra information for apple platforms
        /// </summary>
        public GamesLeaderboardAppleExtras Apple { get; init; }

        /// <summary>
        /// Extra information for Android platforms
        /// </summary>
        public GamesLeaderboardAndroidExtras Android { get; init; }
    }
}