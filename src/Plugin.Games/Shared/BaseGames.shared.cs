
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plugin.Games
{
    /// <summary>
    /// Base implementation for Games
    /// </summary>

    public abstract class BaseGames : IGames
    {
        #region Achievements
        
        /// <summary>
        /// Requests that the platform's achievements user interface be displayed.
        /// </summary>
        /// <returns>A Task that completes once the request to display the achievements UI has been made.</returns>
        public abstract Task ShowAchievementsAsync();

        /// <summary>
        /// Increments an <see cref="GamesAchievementType.Incremental"/> achievement asynchronously. On Android: once the achievement reaches the maximum number of 
        /// steps, the achievement will automatically be unlocked, and any further mutation operations will be ignored. On apple: once the achievement 
        /// reaches 100% completion, the achievement will automatically be unlocked, and any further mutation operations will be ignored.
        /// </summary>
        /// <param name="achievementid">The id for the achievement.</param>
        /// <param name="amount">
        /// On Android, this is the number of steps to increment the achievement's <see cref="GamesAchievementAndroidExtras.CurrentSteps"/>.
        /// With Apple, this is the amount of <see cref="GamesAchievementAppleExtras.PercentComplete"/> to increment the achievement by.
        /// </param>
        /// <returns>
        /// Returns a Task which asynchronously increments an achievement by the given number of steps. 
        /// The Boolean in a successful response indicates whether the achievement is now unlocked.
        /// </returns>
        public abstract Task<bool> IncrementAchievementAsync(string achievementid, int amount);

        /// <summary>
        /// Asynchronously gets a list of achievements.
        /// </summary>
        /// <returns>A Task that returns a <see cref="IReadOnlyList{T}"/> of <see cref="GamesAchievement"/></returns>
        public abstract Task<IReadOnlyList<GamesAchievement>> GetAchievementsAsync();

        /// <summary>
        /// Asynchronously reveals a hidden achievement to the currently signed in player. If the achievement is already visible, 
        /// this will have no effect. This form of the API will attempt to update the user's achievement on the server immediately. 
        /// </summary>
        /// <param name="achievementid">The id for the achievement.</param>
        /// <returns>A Task that will complete successfully when the server has been updated.</returns>
        public abstract Task RevealAchievementAsync(string achievementid);

        /// <summary>
        /// Asynchronously sets an achievement to have at least the given number of steps completed. Calling this method while the 
        /// achievement already has more steps or completion percentage than the provided value is a no-op. On Android: once the achievement reaches the maximum number of 
        /// steps, the achievement will automatically be unlocked, and any further mutation operations will be ignored. On apple: once the achievement 
        /// reaches 100% completion, the achievement will automatically be unlocked, and any further mutation operations will be ignored.
        /// </summary>
        /// <param name="achievementid">The id for the achievement.</param>
        /// <param name="amount">
        /// On Android, this is the number of steps to set the achievement's <see cref="GamesAchievementAndroidExtras.CurrentSteps"/>.
        /// With Apple, this is the <see cref="GamesAchievementAppleExtras.PercentComplete"/> to set the achievement to.
        /// </param>
        /// <returns>
        /// Returns a Task which asynchronously sets an achievement's current steps by the given number of steps. 
        /// The Boolean in a successful response indicates whether the achievement is now unlocked.
        /// </returns>
        public abstract Task<bool> SetAchievementProgressAsync(string achievementid, int amount);

        /// <summary>
        /// Asynchronously unlocks a hidden achievement to the currently signed in player. If the achievement is hidden, this will reveal it to the player.
        /// This form of the API will attempt to update the user's achievement on the server immediately. 
        /// </summary>
        /// <param name="achievementid">The id for the achievement.</param>
        /// <returns>A Task that will complete successfully when the server has been updated.</returns>
        public abstract Task UnlockAchievementAsync(string achievementid);

        #endregion

        #region Leaderboards

        /// <summary>
        /// Displays the games SDK user interface for displaying the games leaderboard.
        /// </summary>
        /// <remarks>
        /// On Android, this opens the full Google Play Games leaderboard UI.
        /// On Apple platforms, this triggers the Game Center access point, which
        /// allows the player to navigate available leaderboard sets and leaderboards.
        /// </remarks>
        /// <returns>A Task that completes when the leaderboard is displayed.</returns>
        public abstract Task ShowLeaderBoardsAsync();

        /// <summary>
        /// Asynchronously shows a leaderboard (Android) or leaderboard set (Apple) for a game specified by a leaderboardId (Android) or by a leaderboardSetId (Apple).
        /// </summary>
        /// <param name="identifier"><para>(Android) The ID of the leaderboard to view.</para><para>(Apple) The leaderboard set ID to view.</para></param>
        /// <param name="timeSpan">(Android Only) The <see cref="GamesLeaderboardTimeSpan"/> to retrieve data for. Defaults to <see cref="GamesLeaderboardTimeSpan.Unknown"/>.</param>
        /// <param name="collection">(Android Only) The <see cref="GamesLeaderboardCollection"/> to retrieve data for. Defaults to <see cref="GamesLeaderboardCollection.Unknown"/>.</param>
        /// <returns>A Task that completes when the leaderboard or set is displayed.</returns>
        public abstract Task ShowLeaderBoardsAsync(string identifier, GamesLeaderboardTimeSpan timeSpan = GamesLeaderboardTimeSpan.Unknown, 
                GamesLeaderboardCollection collection = GamesLeaderboardCollection.Unknown);

        /// <summary>
        /// Asynchronously gets a list of leaderboard metadata.
        /// </summary>
        /// <returns>A Task that returns a <see cref="IReadOnlyList{T}"/> of <see cref="GamesLeaderboard"/></returns>
        public abstract Task<IReadOnlyList<GamesLeaderboard>> GetLeaderBoardDataAsync();

        /// <summary>
        /// Asynchronously gets a single leaderboard metadata.
        /// </summary>
        /// <param name="leaderBoardId">ID of the leaderboard to load metadata for.</param>
        /// <returns>A Task that returns a <see cref="GamesLeaderboard"/></returns>
        public abstract Task<GamesLeaderboard> GetLeaderBoardDataAsync(string leaderBoardId);
        
        /// <summary>
        /// Asynchronously loads <see cref="GamesLeaderboardScore"/>'s that represents an additional page of score data.
        /// </summary>
        /// <param name="maxResults">
        /// The maximum number of scores to fetch per page. Must be between 1 and 25. Note that the 
        /// number of scores returned here may be greater than this value, depending on how much data is cached on the device.
        /// </param>
        /// <param name="direction">
        /// The direction to expand score data. Values are defined in <see cref="GamesPageDirection"/>.
        /// </param>
        /// <returns>A Task that returns a <see cref="IReadOnlyList{T}"/> of <see cref="GamesLeaderboardScore"/></returns>
        public abstract Task<IReadOnlyList<GamesLeaderboardScore>> GetMoreScoresAsync(int maxResults, GamesPageDirection direction);
        
        /// <summary>
        /// Asynchronously loads the <see cref="GamesLeaderboardScore"/>'s that represents the player-centered page of scores 
        /// for the leaderboard specified by leaderboardId. If the player does not have a score on this leaderboard, this call will 
        /// return the top page instead.
        /// </summary>
        /// <param name="leaderBoardId">The ID of the leaderboard to view.</param>
        /// <param name="timeSpan"> to retrieve data for. Valid values are <see cref="GamesLeaderboardTimeSpan.Daily"/>, 
        /// <see cref="GamesLeaderboardTimeSpan.AllTime"/>, and <see cref="GamesLeaderboardTimeSpan.Weekly"/>. On Apple, this parameter is 
        /// applicable to nonrecurring leaderboards only. For recurring leaderboards, pass <see cref="GamesLeaderboardTimeSpan.AllTime"/> 
        /// for this parameter.</param>
        /// <param name="collection"> to retrieve data for. Valid values are <see cref="GamesLeaderboardCollection.Friends"/> and <see cref="GamesLeaderboardCollection.Public"/>.</param>
        /// <param name="maxResults">
        /// On Android, this is the maximum number of scores to fetch per page. Must be between 1 and 25. Note that the 
        /// number of scores returned here may be greater than this value, depending on how much data is cached on the device.
        /// With Apple, this specifies the range of ranks to use for getting the scores. The difference between the minimum 
        /// rank and maximum rank must not exceed 100.
        /// </param>
        /// <returns>A Task that returns a <see cref="IReadOnlyList{T}"/> of <see cref="GamesLeaderboardScore"/></returns>
        public abstract Task<IReadOnlyList<GamesLeaderboardScore>> GetPlayerCenteredScoresAsync(string leaderBoardId, GamesLeaderboardTimeSpan timeSpan, GamesLeaderboardCollection collection, int maxResults);

        /// <summary>
        /// Asynchronously loads the <see cref="GamesLeaderboardScore"/>'s that represents the top page of scores for the leaderboard specified by leaderboardId.
        /// </summary>
        /// <param name="leaderBoardId">The ID of the leaderboard to view.</param>
        /// <param name="timeSpan"> to retrieve data for. Valid values are <see cref="GamesLeaderboardTimeSpan.Daily"/>, 
        /// <see cref="GamesLeaderboardTimeSpan.AllTime"/>, and <see cref="GamesLeaderboardTimeSpan.Weekly"/>. On Apple, this parameter is 
        /// applicable to nonrecurring leaderboards only. For recurring leaderboards, pass <see cref="GamesLeaderboardTimeSpan.AllTime"/> 
        /// for this parameter.</param>
        /// <param name="collection"> to retrieve data for. Valid values are <see cref="GamesLeaderboardCollection.Friends"/> and <see cref="GamesLeaderboardCollection.Public"/>.</param>
        /// <param name="maxResults">
        /// On Android, this is the maximum number of scores to fetch per page. Must be between 1 and 25. Note that the 
        /// number of scores returned here may be greater than this value, depending on how much data is cached on the device.
        /// With Apple, this specifies the range of ranks to use for getting the scores. The difference between the minimum 
        /// rank and maximum rank must not exceed 100.
        /// </param>
        /// <returns>A Task that returns a <see cref="IReadOnlyList{T}"/> of <see cref="GamesLeaderboardScore"/></returns>
        public abstract Task<IReadOnlyList<GamesLeaderboardScore>> GetTopScoresAsync(string leaderBoardId, GamesLeaderboardTimeSpan timeSpan, GamesLeaderboardCollection collection, int maxResults);

        /// <summary>
        /// Asynchronously submits a score to a leaderboard for the currently signed-in player. The score is ignored if it is worse 
        /// (as defined by the leaderboard configuration) than a previously submitted score for the same player. 
        /// This form of the API is a fire-and-forget form.
        /// The meaning of the score value depends on the formatting of the leaderboard established in the developer console. 
        /// Leaderboards support the following score formats:
        /// <list type="bullet">
        /// <item> 
        /// Fixed-point: score represents a raw value, and will be formatted based on the number of decimal places configured.
        /// A score of 1000 would be formatted as 1000, 100.0, or 10.00 for 0, 1, or 2 decimal places. 
        /// </item>
        /// <item>
        /// Time: score represents an elapsed time in milliseconds.The value will be formatted as an appropriate time value. 
        /// </item>
        /// <item>
        /// Currency: score represents a value in micro units. For example, in USD, a score of 100 would display as $0.0001, 
        /// while a score of 1000000 would display as $1.00.
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="leaderBoardId">The leaderboard to submit the score to.</param>
        /// <param name="score">The raw score value.</param>
        /// <param name="metaData">
        /// Optional <see cref="GamesScoreMetadata"/> about this score.
        /// For Apple, use the <see cref="GamesScoreMetadata.AppleContext"/>.
        /// For Android, use the <see cref="GamesScoreMetadata.GoogleScoreTag"/>.
        /// </param>
        /// <returns>A Task that returns a <see cref="GamesScoreSubmission"/></returns>
        public abstract Task<GamesScoreSubmission> SubmitScoreAsync(string leaderBoardId, long score, GamesScoreMetadata metaData = null);

        #endregion

        #region SignIn

        /// <summary>
        /// Attempts to sign in to the games SDK for the current platform automatically at app startup.
        /// This method may complete silently or may trigger a platform-controlled sign-in user interface,
        /// depending on platform rules and current authentication state.
        /// </summary>
        /// <remarks>
        /// On iOS (Game Center), the system decides if and when a sign-in prompt is shown and this
        /// typically occurs only once per app launch or when the authentication state changes.
        /// On Android, this method may perform a one-time automatic sign-in prompt if the user
        /// is not already authenticated by calling <see cref="SignInAsync()"/>.
        /// </remarks>
        /// <returns>
        /// A <see cref="Task"/> that completes with <c>true</c> if the user is authenticated after
        /// the attempt; otherwise <c>false</c>.
        /// </returns>
        public abstract Task<bool> SignInSilentlyAsync();

        /// <summary>
        /// Initiates a user-driven sign-in flow for the games SDK on the current platform.
        /// This method should be called only in response to explicit user intent (for example,
        /// after tapping a "Sign in" button).
        /// </summary>
        /// <remarks>
        /// On Android, this method presents an interactive sign-in user interface.
        /// On iOS (Game Center), presentation of a sign-in prompt is best-effort and remains
        /// under system control; calling this method does not guarantee that a user interface
        /// will be shown.
        /// </remarks>
        /// <returns>
        /// A <see cref="Task"/> that completes with <c>true</c> if the user is authenticated after
        /// the attempt; otherwise <c>false</c>.
        /// </returns>
        public abstract Task<bool> SignInAsync();

        /// <summary>
        /// Raised when the underlying Apple platform reports that a game capability is unavailable.
        /// Not all platforms provide this information. This event is specific to Apple Game Center.
        /// </summary>
        public event EventHandler<GamesCapability> OnCapabilityUnavailable;

        internal void RaiseCapabilityUnavailable(GamesCapability capability)
            => OnCapabilityUnavailable?.Invoke(this, capability);

        /// <summary>
        /// Requests backend server sign-in access to the games SDK for the current platform.
        /// </summary>    
        /// <param name="serverClientId">
        /// (Android Only) The client ID of the server that will perform the authorization code flow exchange.
        /// </param>
        /// <param name="authScopes">
        /// (Android Only) OAuth 2.0 permissions being requested, such as Email or Profile.
        /// </param>
        /// <returns>
        /// A task that completes with a <see cref="GamesAuthResponse"/> containing platform-specific server authorization data.
        /// </returns>
        public abstract Task<GamesAuthResponse> RequestServerSideAccessAsync(string serverClientId, List<GamesAuthScope> authScopes);
        
        #endregion
    }
}
