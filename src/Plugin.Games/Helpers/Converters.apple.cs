
using System.Collections.Generic;
using Foundation;
using UIKit;
using System;
using GameKit;

namespace Plugin.Games
{
    internal static class Converters
    {
        #region Achievements

        internal static GamesAchievement ToGamesAchievement(this GKAchievement native, GKAchievementDescription description)
        {
            return new GamesAchievement()
            {
                AchievementId = native.Identifier,
                Apple = new GamesAchievementAppleExtras()
                {
                    PlayerID = native.Player.TeamPlayerId,
                    AchievedDescription = description.AchievedDescription,
                    UnachievedDescription = description.UnachievedDescription,
                    PercentComplete = (int)(native.PercentComplete * 100),
                    ShowCompletionBanner = native.ShowsCompletionBanner,
                    Rarity = GamesImplementation.HasRarityPercent ? description.RarityPercent.DoubleValue : 0.0D
                },
                Description = native.Description,
                State = ToGamesAchievementState(native.Completed, description.Hidden),
                LastUpdated = native.LastReportedDate != null ? native.LastReportedDate.ToDateTime() : null,
                XpValue = description.MaximumPoints,
                Name = description.Title,
            };
        }

        private static GamesAchievementState ToGamesAchievementState(this bool completed, bool hidden)
        {
            if (hidden)
                return GamesAchievementState.Hidden;
            else if (completed)
                return GamesAchievementState.Unlocked;
            else
                return GamesAchievementState.Revealed;
        }

        public static DateTime ToDateTime(this NSDate date)
        {
            // NSDate is seconds since 2001-01-01 00:00:00 UTC
            var reference = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return reference.AddSeconds(date.SecondsSinceReferenceDate).ToLocalTime();
        }

        public static Dictionary<string, GKAchievementDescription> ToDictionaryByAchievementDescriptionId(this GKAchievementDescription[] descriptions)
        {
            var dict = new Dictionary<string, GKAchievementDescription>(descriptions.Length);

            if (descriptions == null)
                return dict;

            foreach (var desc in descriptions)
            {
                if (desc?.Identifier != null && !dict.ContainsKey(desc.Identifier))
                {
                    dict[desc.Identifier] = desc;
                }
            }

            return dict;
        }

        #endregion

        #region Leaderboard

        internal static GamesLeaderboardScore ToGamesLeaderboardScore(this GKLeaderboardEntry native, UIImage largePhoto, UIImage smallPhoto)
        {    
            return new GamesLeaderboardScore()
            {
                DisplayName = native.Player.DisplayName,
                RawScore = native.Score,
                DisplayScore = native.Score.ToString(),
                Rank = native.Rank,
                DisplayRank = native.Rank.ToString(),
                DateAndTime = native.Date.ToDateTime(),
                Metadata = new GamesScoreMetadata() { AppleContext = (long)native.Context },
                Apple = new GamesScoreAppleExtras()
                {
                    GamePlayerId = native.Player.GamePlayerId,
                    TeamPlayerId = native.Player.TeamPlayerId,
                    PlayerLargeImage = largePhoto.ToByteArray(),
                    PlayerIconImage = smallPhoto.ToByteArray(),
                    Alias = native.Player.Alias,
                    IsInvitable = native.Player.IsInvitable
                    
                }
            };
        }

        public static byte[] ToByteArray(this UIImage uiImage)
        {
            if (uiImage == null)
                return null;

            return uiImage.AsPNG().ToArray();
        }

        internal static void ToNUInt(this long value, out nuint nuintValue)
        {
            long clamped = Math.Clamp(value, 0, (long)nuint.MaxValue);

            nuintValue = (nuint)clamped;
        }

        internal static GamesLeaderboard ToGamesLeaderboard(this GKLeaderboard native, UIImage imageIcon)
        {
            return new GamesLeaderboard()
            {
                LeaderboardId = native.Identifier,
                DisplayName = native.Title,
                TimeSpan = native.TimeScope.ToGamesLeaderBoardTimeSpan(),
                Collection = native.PlayerScope.ToGamesLeaderBoardCollection(),
                Apple = new GamesLeaderboardAppleExtras()
                {
                    IconImageBytes = imageIcon.ToByteArray(),
                    LeaderboardType = native.Type.ToGamesLeaderboardType()
                }
            };
        }

        private static GamesLeaderboardType ToGamesLeaderboardType(this GKLeaderboardType type)
        {
            return type switch
            {
                GKLeaderboardType.Classic => GamesLeaderboardType.Classic,
                GKLeaderboardType.Recurring => GamesLeaderboardType.Recurring,
                _ => GamesLeaderboardType.Unknown
            };
        }

        private static GamesLeaderBoardTimeSpan ToGamesLeaderBoardTimeSpan(this GKLeaderboardTimeScope native)
        {
            return native switch
            {
                GKLeaderboardTimeScope.AllTime => GamesLeaderBoardTimeSpan.AllTime,
                GKLeaderboardTimeScope.Today => GamesLeaderBoardTimeSpan.Daily,
                GKLeaderboardTimeScope.Week => GamesLeaderBoardTimeSpan.Weekly,
                _ => GamesLeaderBoardTimeSpan.Unknown
            };
        }

        private static GamesLeaderboardCollection ToGamesLeaderBoardCollection(this GKLeaderboardPlayerScope native)
        {
            return native switch
            {
                GKLeaderboardPlayerScope.FriendsOnly => GamesLeaderboardCollection.Friends,
                GKLeaderboardPlayerScope.Global => GamesLeaderboardCollection.Public,
                _ => GamesLeaderboardCollection.Unknown
            };
        }

        internal static GKLeaderboardTimeScope ToNativeTimeScope(this GamesLeaderBoardTimeSpan timespan)
        {
            return timespan switch
            {
                GamesLeaderBoardTimeSpan.AllTime => GKLeaderboardTimeScope.AllTime,
                GamesLeaderBoardTimeSpan.Daily => GKLeaderboardTimeScope.Today,
                GamesLeaderBoardTimeSpan.Weekly => GKLeaderboardTimeScope.Week,
                _ => throw new GamesException(GamesError.InvalideGamesLeaderboardTimeSpan)
            };
        }

        internal static GKLeaderboardPlayerScope ToNativeCollection(this GamesLeaderboardCollection collection)
        {
            return collection switch
            {
                GamesLeaderboardCollection.Friends => GKLeaderboardPlayerScope.FriendsOnly,
                GamesLeaderboardCollection.Public => GKLeaderboardPlayerScope.Global,
                _ => throw new GamesException(GamesError.InvalideGamesLeaderboardCollection)
            };
        }

        internal static GamesScoreSubmissionResult ToGamesScoreSubmissionResult(this GKEntriesForPlayersResult native, UIImage largePhoto, UIImage smallPhoto)
        {
            return new GamesScoreSubmissionResult()
            {
                RawScore = native.LocalPlayerEntry.Score,
                FormattedScore = native.LocalPlayerEntry.FormattedScore,
                Metadata = new GamesScoreMetadata() { AppleContext = (long)native.LocalPlayerEntry.Context },
                Apple = new GamesScoreSubmissionResultAppleExtras()
                {
                    DisplayName = native.LocalPlayerEntry.Player.DisplayName,
                    RawScore = native.LocalPlayerEntry.Score,
                    DisplayScore = native.LocalPlayerEntry.FormattedScore,
                    Rank = (int)native.LocalPlayerEntry.Rank,
                    DisplayRank = native.LocalPlayerEntry.Rank.ToString(),
                    DateAndTime = native.LocalPlayerEntry.Date.ToDateTime(),
                    PlayerIconImage = smallPhoto.ToByteArray(),
                    PlayerLargeImage = largePhoto.ToByteArray()
                }
            };
        }

        #endregion
    }
}
