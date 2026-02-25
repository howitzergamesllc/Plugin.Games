using System;
using System.Collections.Generic;

namespace Plugin.Games
{
    /// <summary>
    /// Extra information for Apple leaderboard submissions.
    /// </summary>
    [Preserve(AllMembers = true)]
    public sealed class GamesScoreSubmissionResultAppleExtras
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
        /// Player's hi-res image
        /// </summary>
        public byte[] PlayerLargeImage { get; set; }

        /// <summary>
        /// Player's normal icon image
        /// </summary>
        public byte[] PlayerIconImage { get; set; }

        /// <summary>
        /// Player's normal icon image
        /// </summary>
        public long Context { get; set; }
    }

    /// <summary>
    /// Extra information for Android leaderboard submissions.
    /// </summary>
    [Preserve(AllMembers = true)]
    public sealed class GamesScoreSubmissionResultAndroidExtras
    {
        /// <summary>
        /// Whether this score was the player's new best for this time span.
        /// </summary>
        public bool NewBest { get; init; }
    }

    /// <summary>
    /// Represents the result of submitting a score for a specific time span.
    /// </summary>
    [Preserve(AllMembers = true)]
    public sealed class GamesScoreSubmissionResult
    {
        /// <summary>
        /// The raw numeric score.
        /// </summary>
        public long RawScore { get; init; }

        /// <summary>
        /// Formatted score suitable for display.
        /// </summary>
        public string FormattedScore { get; init; }

        /// <summary>
        /// Metadata associated with the score
        /// </summary>
        public GamesScoreMetadata Metadata { get; set; }

        /// <summary>
        /// Optional extra information specific to Apple platforms.
        /// </summary>
        public GamesScoreSubmissionResultAppleExtras Apple { get; init; }

        /// <summary>
        /// Optional extra information specific to Android platforms.
        /// </summary>
        public GamesScoreSubmissionResultAndroidExtras Android { get; init; }
    }

    /// <summary>
    /// Represents a score submission to a leaderboard.
    /// </summary>
    [Preserve(AllMembers = true)]
    public sealed class GamesScoreSubmission
    {
        /// <summary>
        /// Leaderboard ID the score was submitted to.
        /// </summary>
        public string LeaderboardId { get; init; }

        /// <summary>
        /// Player ID that submitted the score.
        /// </summary>
        public string PlayerId { get; init; }

        /// <summary>
        /// Results of the score submission for different time spans.
        /// </summary>
        public IReadOnlyDictionary<GamesLeaderBoardTimeSpan, GamesScoreSubmissionResult> Results { get; init; }

        /// <summary>
        /// Retrieves the score submission result for the given time span, if available.
        /// </summary>
        /// <param name="timeSpan">Time span to retrieve result for.</param>
        /// <returns>Score result or null if no result exists.</returns>
        public GamesScoreSubmissionResult GetScoreResult(GamesLeaderBoardTimeSpan timeSpan)
        {
            if (Results != null && Results.TryGetValue(timeSpan, out var result))
                return result;

            return null;
        }
    }
}
