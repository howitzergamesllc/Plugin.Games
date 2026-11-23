

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plugin.Games
{
    /// <summary>
    /// GamesNotImplemented for Windows, .net, and Tizen
    /// </summary>
    [Preserve(AllMembers = true)]
    internal partial class GamesNotImplementation : BaseGames
    {
        /// <summary>
        /// Default Constructor for GamesNotImplementation on Windows, .net, and Tizen.
        /// </summary>
        public GamesNotImplementation()
        {
        }

        #region Achievements
        
        public override Task ShowAchievementsAsync()=> throw CrossGames.NotImplemented();
        public override Task<bool> IncrementAchievementAsync(string achievementid, int amount)=> throw CrossGames.NotImplemented();
        public override Task<IReadOnlyList<GamesAchievement>> GetAchievementsAsync()=> throw CrossGames.NotImplemented();
        public override Task RevealAchievementAsync(string achievementid)=> throw CrossGames.NotImplemented();
        public override Task<bool> SetAchievementProgressAsync(string achievementid, int amount)=> throw CrossGames.NotImplemented();
        public override Task UnlockAchievementAsync(string achievementid)=> throw CrossGames.NotImplemented();

        #endregion

        #region Leaderboards
        public override Task ShowLeaderBoardsAsync()=> throw CrossGames.NotImplemented();
        public override Task ShowLeaderBoardsAsync(string leaderBoardId, GamesLeaderboardTimeSpan timeSpan, GamesLeaderboardCollection collection)=> throw CrossGames.NotImplemented();
        public override Task<IReadOnlyList<GamesLeaderboard>> GetLeaderBoardDataAsync() => throw CrossGames.NotImplemented();
        public override Task<GamesLeaderboard> GetLeaderBoardDataAsync(string leaderBoardId) => throw CrossGames.NotImplemented();
        public override Task<IReadOnlyList<GamesLeaderboardScore>> GetMoreScoresAsync(int maxResults, GamesPageDirection direction) => throw CrossGames.NotImplemented();
        public override Task<IReadOnlyList<GamesLeaderboardScore>> GetPlayerCenteredScoresAsync(string leaderBoardId, GamesLeaderboardTimeSpan timeSpan, GamesLeaderboardCollection collection, int maxResults) => throw CrossGames.NotImplemented();
        public override Task<IReadOnlyList<GamesLeaderboardScore>> GetTopScoresAsync(string leaderBoardId, GamesLeaderboardTimeSpan timeSpan, GamesLeaderboardCollection collection, int maxResults) => throw CrossGames.NotImplemented();
        public override Task<GamesScoreSubmission> SubmitScoreAsync(string leaderBoardId, long score, GamesScoreMetadata metaData = null) => throw CrossGames.NotImplemented();

        #endregion

        #region SignIn

        public override Task<bool> SignInSilentlyAsync() => throw CrossGames.NotImplemented();
        public override Task<bool> SignInAsync() => throw CrossGames.NotImplemented();
        public override Task<GamesAuthResponse> RequestServerSideAccessAsync(string serverClientId, List<GamesAuthScope> authScopes) => throw CrossGames.NotImplemented();

        #endregion
    }
}