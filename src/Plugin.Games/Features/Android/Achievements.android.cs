using Android.Content;
using Android.Gms.Games;
using Android.Gms.Games.Achievement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plugin.Games
{
    /// <summary>
    /// Implementation for Google Play Games
    /// </summary>
    public partial class GamesImplementation : BaseGames
    {
        /// <summary>
        /// A code for showing achievements from the MainActivity by overriding OnActivityResult.
        /// </summary>
        public static int ShowAchievementsCode { get; set; } = 3003;

        private static IAchievementsClient AchievementsClient
        {
            get
            {
                EnsureInitialized();
                return PlayGames.GetAchievementsClient(Activity);
            }
        }

        public override async Task ShowAchievementsAsync()
        {
            var client = AchievementsClient;
            
            try
            {
                var intent = await client
                    .GetAchievementsIntent()
                    .AsTask<Intent>();
                Activity.StartActivityForResult(intent, ShowAchievementsCode);
            }
            catch (Exception ex)
            {
                throw new GamesException(GamesError.FailedToShowAchievements, ex);
            }
        }

        public override async Task<bool> IncrementAchievementAsync(string achievementid, int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Increment amount must be positive.");

            var client = AchievementsClient;

            try
            {
                var unlocked = await client.IncrementImmediate(achievementid, amount).AsTask<Java.Lang.Boolean>();

                return unlocked.BooleanValue();
            }
            catch (Exception ex)
            {
                throw new GamesException(GamesError.FailedToIncrementAchievement, ex);
            }
        }

        public override async Task<IReadOnlyList<GamesAchievement>> GetAchievementsAsync()
        {
            var client = AchievementsClient;

            try
            {
                var data = await client.Load(ShouldForceReload).AsTask<AnnotatedData>();

                var buffer = (AchievementBuffer)data.Get();

                var results = new List<GamesAchievement>(buffer.Count);

                for (int i = 0; i < buffer.Count; i++)
                {
                    using var achievementRef = (AchievementRef)buffer.Get(i);
                    results.Add(achievementRef.ToGamesAchievement());
                }

                buffer.Release();

                return results.AsReadOnly();
            }
            catch (Exception ex)
            {
                throw new GamesException(GamesError.FailedToGetAchievements, ex);
            }
        }

        public override Task RevealAchievementAsync(string achievementid)
        {
            var client = AchievementsClient;

            try
            {
                return client.RevealImmediate(achievementid).AsTask<Java.Lang.Void>();
            }
            catch (Exception ex)
            {
                throw new GamesException(GamesError.FailedToRevealAchievement, ex);
            }
        }

        public override async Task<bool> SetAchievementProgressAsync(string achievementid, int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "set amount must be positive.");

            var client = AchievementsClient;
            
            try
            {
                var unlocked = await client.SetStepsImmediate(achievementid, amount).AsTask<Java.Lang.Boolean>();

                return unlocked.BooleanValue();
            }
            catch (Exception ex)
            {
                throw new GamesException(GamesError.FailedToSetAchievementProgress, ex);
            }
        }

        public override Task UnlockAchievementAsync(string achievementid)
        {
            var client = AchievementsClient;

            try
            {
                return client.UnlockImmediate(achievementid).AsTask<Java.Lang.Void>();
            }
            catch (Exception ex)
            {
                throw new GamesException(GamesError.FailedToUnlockAchievement, ex);
            }
        }
    }
}