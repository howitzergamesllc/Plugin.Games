using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameKit;
using Microsoft.Maui.ApplicationModel;

namespace Plugin.Games
{
    /// <summary>
    /// Implementation for Apple Games Center
    /// </summary>
    public partial class GamesImplementation : BaseGames
    {
        /// <summary>
        /// (Apple Only) If this property is <c>true</c>, GameKit displays a notification banner to inform the player that they completed the achievement. 
        /// If you want to present your own interface, set this property to false. The default value is true.
        /// </summary>
        public static bool ShouldShowCompletionBanners { get; set; } = true;

        /// <inheritdoc />
        public override Task ShowAchievementsAsync()
        {
            try
            {
                return ShowAccessPoint();
            }
            catch (Exception ex)
            {
                throw new GamesException(GamesError.FailedToShowAchievements, ex);
            }
        }

        /// <inheritdoc />
        public override async Task<bool> IncrementAchievementAsync(string achievementId, int amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            return await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                try
                {
                    // Load current progress
                    var achievements = await GKAchievement.LoadAchievementsAsync();
                    var existing = Array.Find(achievements, a => a.Identifier == achievementId);

                    var current = existing?.PercentComplete ?? 0.0;
                    var updated = Math.Min(100.0, current + amount);

                    var achievement = new GKAchievement(achievementId)
                    {
                        PercentComplete = updated,
                        ShowsCompletionBanner = ShouldShowCompletionBanners
                    };

                    await GKAchievement.ReportAchievementsAsync(new[] { achievement });

                    return updated >= 100.0;
                }
                catch (Exception ex)
                {
                    throw new GamesException(GamesError.FailedToIncrementAchievement, ex);
                }
            });
        }

        /// <inheritdoc />
        public override async Task<IReadOnlyList<GamesAchievement>> GetAchievementsAsync()
        {
            return await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                try
                {
                    var playerAchievements = await GKAchievement.LoadAchievementsAsync();
                    var descriptions = await GKAchievementDescription.LoadAchievementDescriptionsAsync();

                    var progressById = playerAchievements.ToDictionary(a => a.Identifier, a => a);

                    var results = new List<GamesAchievement>(descriptions.Length);

                    foreach (var description in descriptions)
                    {
                        progressById.TryGetValue(description.Identifier, out var progress);

                        GamesAchievementState state;

                        if (progress?.PercentComplete >= 100.0)
                        {
                            state = GamesAchievementState.Unlocked;
                        }
                        else if (progress != null)
                        {
                            state = GamesAchievementState.Revealed;
                        }
                        else
                        {
                            state = description.Hidden ? GamesAchievementState.Hidden : GamesAchievementState.Revealed;
                        }

                        results.Add(new GamesAchievement
                        {
                            AchievementId = description.Identifier,
                            Name = description.Title,
                            Description = progress?.PercentComplete >= 100.0 ? description.AchievedDescription : description.UnachievedDescription,
                            State = state,
                            LastUpdated = progress?.LastReportedDate?.ToDateTime(),
                            XpValue = description.MaximumPoints,
                            Apple = new GamesAchievementAppleExtras
                            {
                                PlayerID = progress?.Player?.GamePlayerId,
                                PercentComplete = (int)(progress?.PercentComplete ?? 0),
                                ShowCompletionBanner = progress?.ShowsCompletionBanner ?? true,
                                IsReplayable = description.Replayable,
                                AchievedDescription = description.AchievedDescription,
                                UnachievedDescription = description.UnachievedDescription
                            }
                        });
                    }

                    return results.AsReadOnly();
                }
                catch (Exception ex)
                {
                    throw new GamesException(GamesError.FailedToGetAchievements, ex);
                }
            });
        }

        /// <inheritdoc />
        public override Task RevealAchievementAsync(string achievementid) 
            => SetAchievementProgressAsync(achievementid, 0);

        /// <inheritdoc />
        public override async Task<bool> SetAchievementProgressAsync(string achievementId, int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            return await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                try
                {
                    var clamped = Math.Min(100.0, amount);

                    var achievement = new GKAchievement(achievementId)
                    {
                        PercentComplete = clamped,
                        ShowsCompletionBanner = ShouldShowCompletionBanners
                    };

                    await GKAchievement.ReportAchievementsAsync(new[] { achievement });

                    return clamped >= 100.0;
                }
                catch (Exception ex)
                {
                    throw new GamesException(GamesError.FailedToSetAchievementProgress, ex);
                }
            });
        }

        /// <inheritdoc />
        public override Task UnlockAchievementAsync(string achievementid) => SetAchievementProgressAsync(achievementid, 100);
    }
}