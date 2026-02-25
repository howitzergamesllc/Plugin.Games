
using System;
using System.Collections.Generic;
using Android.Gms.Games.Achievement;
using Android.Gms.Games.Leaderboard;
using Google.Android.Gms.Games.Gamessignin;

namespace Plugin.Games
{
    internal static class Converters
    {
        #region Achievements

        internal static GamesAchievement ToGamesAchievement(this AchievementRef native)
        {
            return new GamesAchievement()
            {
                AchievementId = native.AchievementId,
                Android = new GamesAchievementAndroidExtras()
                {
                    CurrentSteps = native.CurrentSteps,
                    Type = native.Type.ToGamesAchievementType(),
                    TotalSteps = native.TotalSteps,
                    RevealedImageUri = native.RevealedImageUri.ToUri(),
                    RevealedImageUrl = native.RevealedImageUrl,
                    UnlockedImageUri = native.UnlockedImageUri.ToUri(),
                    UnlockedImageUrl = native.UnlockedImageUrl,
                },
                Description = native.Description,
                State = native.State.ToGamesAchievementState(),
                LastUpdated = native.LastUpdatedTimestamp > 0 ? native.LastUpdatedTimestamp.FromUnixMilliseconds() : null,
                XpValue = native.XpValue,
                Name = native.Name,
            };
        }

        private static GamesAchievementState ToGamesAchievementState(this int state)
        {
            return state switch
            {
                2 => GamesAchievementState.Hidden,
                1 => GamesAchievementState.Revealed,
                0 => GamesAchievementState.Unlocked,
                _ => GamesAchievementState.Unknown,
            };
        }

        private static GamesAchievementType ToGamesAchievementType(this int type)
        {
            return type switch
            {
                1 => GamesAchievementType.Standard,
                0 => GamesAchievementType.Incremental,
                _ => GamesAchievementType.Unknown,
            };
        }

        public static DateTime FromUnixMilliseconds(this long unixMillis)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(unixMillis).DateTime;
        }

        #endregion

        #region Leaderboard

        internal static GamesLeaderboardScore ToGamesLeaderboardScoreAsync(this LeaderboardScoreRef native)
        {    
            return new GamesLeaderboardScore()
            {
                DisplayName = native.ScoreHolderDisplayName,
                RawScore = native.RawScore,
                DisplayScore = native.DisplayScore,
                Rank = native.Rank,
                DisplayRank = native.DisplayRank,
                DateAndTime = native.TimestampMillis.FromUnixMilliseconds(),
                Android = new GamesScoreAndroidExtras()
                {
                    PlayerLargeImageUri = native.ScoreHolderHiResImageUri.ToUri(),
                    PlayerLargeImageUrl = native.ScoreHolderHiResImageUrl,
                    PlayerIconImageUri = native.ScoreHolderIconImageUri.ToUri(),
                    PlayerIconImageUrl = native.ScoreHolderIconImageUrl,
                },
                Metadata = new GamesScoreMetadata(){GoogleScoreTag = native.ScoreTag,}
            };
        }

        internal static GamesLeaderboard ToGamesLeaderboard(this LeaderboardRef native)
        {
            return new GamesLeaderboard()
            {
                LeaderboardId = native.LeaderboardId,
                DisplayName = native.DisplayName,
                Android = new GamesLeaderboardAndroidExtras()
                {
                    IconImageUri = native.IconImageUri.ToUri(),
                    IconImageUrl = native.IconImageUrl,
                    SortOrder = native.ScoreOrder.ToGamesLeaderboardSortOrder()
                }
            };
        }

        private static GamesLeaderboardSortOrder ToGamesLeaderboardSortOrder(this int order)
        {
            return order switch
            {
                1 => GamesLeaderboardSortOrder.Descending,
                0 => GamesLeaderboardSortOrder.Ascending,
                _ => GamesLeaderboardSortOrder.Unknown,
            };
        }

        internal static int ToNativePageDirection(this GamesPageDirection direction)
        {
            return direction switch
            {
                GamesPageDirection.Next => 0,
                GamesPageDirection.None => -1,
                GamesPageDirection.Prev => 1,
                _ => throw new GamesException(GamesError.InvalidGamesPageDireciton)
            };
        }

        internal static int ToNativeTimeSpan(this GamesLeaderBoardTimeSpan timespan)
        {
            return timespan switch
            {
                GamesLeaderBoardTimeSpan.AllTime => 2,
                GamesLeaderBoardTimeSpan.Daily => 0,
                GamesLeaderBoardTimeSpan.Weekly => 1,
                _ => throw new GamesException(GamesError.InvalideGamesLeaderboardTimeSpan)
            };
        }

        internal static int ToNativeCollection(this GamesLeaderboardCollection collection)
        {
            return collection switch
            {
                GamesLeaderboardCollection.Friends => 3,
                GamesLeaderboardCollection.Public => 0,
                _ => throw new GamesException(GamesError.InvalideGamesLeaderboardCollection)
            };
        }

        internal static GamesScoreSubmission ToGamesScoreSubmission(this ScoreSubmissionData native)
        {
            return new GamesScoreSubmission()
            {
                LeaderboardId = native.LeaderboardId,
                PlayerId = native.PlayerId,
                Results = native.ToGamesScoreSubmissionResults(),
            };
        }

        internal static IReadOnlyDictionary<GamesLeaderBoardTimeSpan, GamesScoreSubmissionResult> ToGamesScoreSubmissionResults(this ScoreSubmissionData native)
        {
            var alltime = native.GetScoreResult((int)GamesLeaderBoardTimeSpan.AllTime)?.ToGamesScoreSubmissionResult();
            var daily = native.GetScoreResult((int)GamesLeaderBoardTimeSpan.Daily)?.ToGamesScoreSubmissionResult();
            var weekly = native.GetScoreResult((int)GamesLeaderBoardTimeSpan.Weekly)?.ToGamesScoreSubmissionResult();

            var results = new Dictionary<GamesLeaderBoardTimeSpan, GamesScoreSubmissionResult>();

            if (alltime is not null)
                results.Add(GamesLeaderBoardTimeSpan.AllTime, alltime);
            if (daily is not null)
                results.Add(GamesLeaderBoardTimeSpan.Daily, daily);
            if (weekly is not null)
                results.Add(GamesLeaderBoardTimeSpan.Weekly, weekly);

            return results.AsReadOnly();
        }

        private static GamesScoreSubmissionResult ToGamesScoreSubmissionResult(this ScoreSubmissionData.Result native)
        {
            return new GamesScoreSubmissionResult()
            {
                RawScore = native.RawScore,
                FormattedScore = native.FormattedScore,
                Metadata = new GamesScoreMetadata(){GoogleScoreTag = native.ScoreTag},
                Android = new GamesScoreSubmissionResultAndroidExtras(){NewBest = native.NewBest},
            };
        }

        #endregion

        #region SignIn

        internal static GamesAuthResponse ToGamesAuthResponse(this AuthResponse response)
        {
            if(response == null)
                return null;

            return GamesAuthResponse.FromAndroid(new GamesAuthResponseAndroid(
                response.AuthCode,
                response.GrantedScopes.ToGamesScopes()));
        }

        internal static List<GamesAuthScope> ToGamesScopes(this IList<AuthScope> scopes)
        {
            var nativescopes = new List<GamesAuthScope>();
            foreach (var scope in scopes)
            {
                nativescopes.Add(scope.ToGamesScope());
            }
            return nativescopes;
        }

        private static GamesAuthScope ToGamesScope(this AuthScope nativeScope)
        {
            if (nativeScope == AuthScope.Email)
                return GamesAuthScope.Email;

            if (nativeScope == AuthScope.OpenId)
                return GamesAuthScope.OpenId;

            if (nativeScope == AuthScope.Profile)
                return GamesAuthScope.Profile;

            throw new GamesException(GamesError.InvalidAuthScope);
        }

        internal static List<AuthScope> ToNativeScopes(this List<GamesAuthScope> scopes)
        {
            var nativescopes = new List<AuthScope>();
            foreach (var scope in scopes)
            {
                nativescopes.Add(scope.ToNativeScope());
            }
            return nativescopes;
        }

        private static AuthScope ToNativeScope(this GamesAuthScope scope)
        {
            return scope switch
            {
                GamesAuthScope.Email => AuthScope.Email,
                GamesAuthScope.OpenId => AuthScope.OpenId,
                GamesAuthScope.Profile => AuthScope.Profile,
                _ => throw new GamesException(GamesError.InvalidAuthScope)
            };
        }

        #endregion
    }
}
