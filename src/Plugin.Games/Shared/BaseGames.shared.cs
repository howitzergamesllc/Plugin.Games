
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
        /// Asynchronously shows a leaderboard for a game specified by a leaderboardId.
        /// </summary>
        /// <param name="leaderBoardId">
        /// The identifier used by the underlying platform to present leaderboard UI.
        /// On Android, this is the leaderboard ID.
        /// On Apple platforms, this must be a leaderboard <b>set</b> identifier,
        /// as Game Center does not support directly presenting individual leaderboards.
        /// </param>
        /// <remarks>
        /// On Apple platforms, the system UI determines available time spans and
        /// player scopes (such as friends or global). These options cannot be
        /// preselected by the application.
        /// </remarks>
        /// <returns>A Task that completes when the leaderboard is displayed.</returns>
        public abstract Task ShowLeaderBoardsAsync(string leaderBoardId);
#if ANDROID
        /// <summary>
        /// Asynchronously shows a leaderboard for a game specified by a leaderboardId, time span, and collection.
        /// </summary>
        /// <param name="leaderBoardId">The ID of the leaderboard to view.</param>
        /// <param name="timeSpan"> to retrieve data for. Valid values are <see cref="GamesLeaderBoardTimeSpan.Daily"/>, <see cref="GamesLeaderBoardTimeSpan.AllTime"/>, and <see cref="GamesLeaderBoardTimeSpan.Weekly"/>.</param>
        /// <param name="collection"> to retrieve data for. Valid values are <see cref="GamesLeaderboardCollection.Friends"/> and <see cref="GamesLeaderboardCollection.Public"/>.</param>
        /// <returns>A Task that completes when the leaderboard is displayed.</returns>
        public abstract Task ShowLeaderBoardsAsync(string leaderBoardId, GamesLeaderBoardTimeSpan timeSpan, GamesLeaderboardCollection collection);

        /// <summary>
        /// Asynchronously shows a leaderboard for a game specified by a leaderboardId and time span.
        /// </summary>
        /// <param name="leaderBoardId">The ID of the leaderboard to view.</param>
        /// <param name="timeSpan"> to retrieve data for. Valid values are <see cref="GamesLeaderBoardTimeSpan.Daily"/>, <see cref="GamesLeaderBoardTimeSpan.AllTime"/>, and <see cref="GamesLeaderBoardTimeSpan.Weekly"/></param>
        /// <returns>A Task that completes when the leaderboard is displayed.</returns>
        public abstract Task ShowLeaderBoardsAsync(string leaderBoardId, GamesLeaderBoardTimeSpan timeSpan);

        /// <summary>
        /// Asynchronously gets a list of leaderboard metadata.
        /// </summary>
        /// <returns>A Task that returns a <see cref="IReadOnlyList{T}"/> of <see cref="GamesLeaderboard"/></returns>
        public abstract Task<IReadOnlyList<GamesLeaderboard>> GetLeaderBoardDataAsync();
#endif

        /// <summary>
        /// Asynchronously gets a single leaderboard metadata.
        /// </summary>
        /// <param name="leaderBoardId">ID of the leaderboard to load metadata for.</param>
        /// <returns>A Task that returns a <see cref="GamesLeaderboard"/></returns>
        public abstract Task<GamesLeaderboard> GetLeaderBoardDataAsync(string leaderBoardId);
#if ANDROID
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
#endif
        /// <summary>
        /// Asynchronously loads the <see cref="GamesLeaderboardScore"/>'s that represents the player-centered page of scores 
        /// for the leaderboard specified by leaderboardId. If the player does not have a score on this leaderboard, this call will 
        /// return the top page instead.
        /// </summary>
        /// <param name="leaderBoardId">The ID of the leaderboard to view.</param>
        /// <param name="timeSpan"> to retrieve data for. Valid values are <see cref="GamesLeaderBoardTimeSpan.Daily"/>, 
        /// <see cref="GamesLeaderBoardTimeSpan.AllTime"/>, and <see cref="GamesLeaderBoardTimeSpan.Weekly"/>. On Apple, this parameter is 
        /// applicable to nonrecurring leaderboards only. For recurring leaderboards, pass <see cref="GamesLeaderBoardTimeSpan.AllTime"/> 
        /// for this parameter.</param>
        /// <param name="collection"> to retrieve data for. Valid values are <see cref="GamesLeaderboardCollection.Friends"/> and <see cref="GamesLeaderboardCollection.Public"/>.</param>
        /// <param name="maxResults">
        /// On Android, this is the maximum number of scores to fetch per page. Must be between 1 and 25. Note that the 
        /// number of scores returned here may be greater than this value, depending on how much data is cached on the device.
        /// With Apple, this specifies the range of ranks to use for getting the scores. The difference between the minimum 
        /// rank and maximum rank must not exceed 100.
        /// </param>
        /// <returns>A Task that returns a <see cref="IReadOnlyList{T}"/> of <see cref="GamesLeaderboardScore"/></returns>
        public abstract Task<IReadOnlyList<GamesLeaderboardScore>> GetPlayerCenteredScoresAsync(string leaderBoardId, GamesLeaderBoardTimeSpan timeSpan, GamesLeaderboardCollection collection, int maxResults);

        /// <summary>
        /// Asynchronously loads the <see cref="GamesLeaderboardScore"/>'s that represents the top page of scores for the leaderboard specified by leaderboardId.
        /// </summary>
        /// <param name="leaderBoardId">The ID of the leaderboard to view.</param>
        /// <param name="timeSpan"> to retrieve data for. Valid values are <see cref="GamesLeaderBoardTimeSpan.Daily"/>, 
        /// <see cref="GamesLeaderBoardTimeSpan.AllTime"/>, and <see cref="GamesLeaderBoardTimeSpan.Weekly"/>. On Apple, this parameter is 
        /// applicable to nonrecurring leaderboards only. For recurring leaderboards, pass <see cref="GamesLeaderBoardTimeSpan.AllTime"/> 
        /// for this parameter.</param>
        /// <param name="collection"> to retrieve data for. Valid values are <see cref="GamesLeaderboardCollection.Friends"/> and <see cref="GamesLeaderboardCollection.Public"/>.</param>
        /// <param name="maxResults">
        /// On Android, this is the maximum number of scores to fetch per page. Must be between 1 and 25. Note that the 
        /// number of scores returned here may be greater than this value, depending on how much data is cached on the device.
        /// With Apple, this specifies the range of ranks to use for getting the scores. The difference between the minimum 
        /// rank and maximum rank must not exceed 100.
        /// </param>
        /// <returns>A Task that returns a <see cref="IReadOnlyList{T}"/> of <see cref="GamesLeaderboardScore"/></returns>
        public abstract Task<IReadOnlyList<GamesLeaderboardScore>> GetTopScoresAsync(string leaderBoardId, GamesLeaderBoardTimeSpan timeSpan, GamesLeaderboardCollection collection, int maxResults);

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

#if IOS || MACCATALYST
        /// <summary>
        /// Raised when the underlying Apple platform reports that a game capability is unavailable.
        /// Not all platforms provide this information. This event is specific to Apple Game Center.
        /// </summary>
        public event EventHandler<GamesCapability> OnCapabilityUnavailable;

        internal void RaiseCapabilityUnavailable(GamesCapability capability)
            => OnCapabilityUnavailable?.Invoke(this, capability);

        /// <summary>
        /// Requests backend server sign in access to the games SDK for the current platform automatically in the background at app start.
        /// </summary>
        /// <returns>
        /// A Task that completes with an <see cref="GamesAuthResponse.Apple"/> containing <see cref="GamesAuthResponseApple"/>
        /// </returns>
        public abstract Task<GamesAuthResponse> RequestServerSideAccessAsync();
#elif ANDROID
        /// <summary>
        /// Requests backend server sign in access to the games SDK for the current platform automatically in the background at app start.
        /// </summary>
        /// <param name="serverClientId">The client ID of the server that will perform the authorization code flow exchange.</param>
        /// <param name="authScopes">A list of <see cref="GamesAuthScope"/> values representing the OAuth 2.0 permissions being requested, such as <see cref="GamesAuthScope.Email"/>, <see cref="GamesAuthScope.Profile"/> and <see cref="GamesAuthScope.Email"/>.</param>
        /// <returns>A Task that completes with an <see cref="GamesAuthResponse.Android"/> containing <see cref="GamesAuthResponseAndroid.AuthorizationCode"/> and a list of the AuthScopes that were effectively granted by the user (see description for details on consent). This authorization code can be exchanged by your server for an access token (and conditionally a refresh token) that can be used to access the Play Games Services web APIs and other Google Identity APIs.</returns>
        public abstract Task<GamesAuthResponse> RequestServerSideAccessAsync(string serverClientId, List<GamesAuthScope> authScopes);
#endif

        #endregion
    }
}
