using System;

namespace Plugin.Games
{
    /// <summary>
    /// Represents types of errors that can occur during authentication.
    /// </summary>
    public enum GamesError
    {
        /// <summary>
        /// The games SDK hasn't been initialized properly. If applicable, call <see cref="CrossGames"/>.Initialize(Context).
        /// </summary>
        NotInitialized,

        #region Achievements

        /// <summary>
        /// Achievements failed to display from the games SDK
        /// </summary>
        FailedToShowAchievements,

        /// <summary>
        /// Failed to increment an achievements current steps from the games SDK
        /// </summary>
        FailedToIncrementAchievement,

        /// <summary>
        /// Achievements failed to load from the games SDK
        /// </summary>
        FailedToGetAchievements,

        /// <summary>
        /// Failed to reveal an achievement from the games SDK
        /// </summary>
        FailedToRevealAchievement,

        /// <summary>
        /// Achievement failed to set achievement's current progress (steps or percent) from the games SDK
        /// </summary>
        FailedToSetAchievementProgress,

        /// <summary>
        /// Failed to unlock an achievement from the games SDK
        /// </summary>
        FailedToUnlockAchievement,

        #endregion Achievements

        #region Leaderboards

        /// <summary>
        /// Failed to display the leaderboard UI from the games SDK.
        /// </summary>
        FailedToShowLeaderboard,

        /// <summary>
        /// Failed to load leaderboard score's rom the games SDK.
        /// </summary>
        FailedToLoadLeaderboardScore,

        /// <summary>
        /// Failed to load leaderboard metadata from the games SDK.
        /// </summary>
        FailedToLoadLeaderboard,

        /// <summary>
        /// Invalid page direciton used to request LeaderboardScores from the games SDK.
        /// </summary>
        InvalidGamesPageDireciton,

        /// <summary>
        /// The maximum results input is out of bounds.
        /// </summary>
        LeaderboardMaxResultsOutofBounds,

        /// <summary>
        /// LoadMoreScores called before LoadTopScores or LoadPlayerCenteredScores.
        /// </summary>
        LeaderboardScoresNotInitialized,

        /// <summary>
        /// Invalid time span used to request LeaderboardScores from the games SDK.
        /// </summary>
        InvalideGamesLeaderboardTimeSpan,

        /// <summary>
        /// Invalid collection type used to request LeaderboardScores from the games SDK.
        /// </summary>
        InvalideGamesLeaderboardCollection,

        /// <summary>
        /// Invalid score metadata provided to submit score to the games SDK.
        /// </summary>
        InvalidScoreMetadata,

        /// <summary>
        /// Failed to submit score to the leaderboard from the games SDK.
        /// </summary>
        FailedToSubmitScore,

        #endregion

        #region SignIn

        /// <summary>
        /// Failed to sign the user or server in to the games SDK.
        /// </summary>
        AuthenticationFailure,

        /// <summary>
        /// Invalid type used to request server access to the games SDK.
        /// </summary>
        InvalidAuthScope,

        #endregion
    }

    /// <summary>
    /// Represents an exception that occurs during authentication.
    /// </summary>
    public class GamesException : Exception
    {
        /// <summary>
        /// Gets the type of purchase error that caused the exception.
        /// </summary>
        public GamesError Error { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GamesException"/> class
        /// with a specified error type and inner exception.
        /// </summary>
        /// <param name="error">The type of games SDK error.</param>
        /// <param name="ex">The inner exception that caused this exception.</param>
        public GamesException(GamesError error, Exception ex) : base($"Games SDK Error : {error:G}.", ex) => Error = error;

        /// <summary>
        /// Initializes a new instance of the <see cref="GamesException"/> class
        /// with a specified error type and custom message.
        /// </summary>
        /// <param name="error">The type of games SDK error.</param>
        /// <param name="message">The custom error message.</param>
        public GamesException(GamesError error, string message) : base(message) => Error = error;


        /// <summary>
        /// Initializes a new instance of the <see cref="GamesException"/> class
        /// with a specified error type.
        /// </summary>
        /// <param name="error">The type of games SDK error.</param>
        public GamesException(GamesError error) : base($"Games SDK Error : {error:G}.") => Error = error;
    }
}
