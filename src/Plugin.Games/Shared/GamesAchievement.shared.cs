
using System;

namespace Plugin.Games
{
    /// <summary>
    /// Gets the current state of an achievement
    /// </summary>
    public enum GamesAchievementState
    {
        /// <summary>
        /// Achievement is hidden.
        /// </summary>
        Hidden = 2,

        /// <summary>
        /// Achievement is revealed.
        /// </summary>
        Revealed = 1,

        /// <summary>
        /// Achievement is unlocked.
        /// </summary>
        Unlocked = 0,

        /// <summary>
        /// An unknown type, possibly recently added in a new update.
        /// </summary>
        Unknown = -1
    }

    /// <summary>
    /// (Android Only) The type of achievement
    /// </summary>
    public enum GamesAchievementType
    {
        //Android Types
        /// <summary>
        /// The achievement is incremental.
        /// </summary>
        Incremental,

        /// <summary>
        /// Standard achievement.
        /// </summary>
        Standard,

        /// <summary>
        /// An unknown type, possibly recently added in a new update.
        /// </summary>
        Unknown = -1
    }

    /// <summary>
    /// Achievement info specific to Apple Platforms
    /// </summary>
    [Preserve(AllMembers = true)]
    public class GamesAchievementAppleExtras
    {
        /// <summary>
        /// The player ID associated with the achievement.
        /// </summary>
        public string PlayerID { get; set; }

        /// <summary>
        /// The achievement description when <see cref="GamesAchievementState.Unlocked"/>.
        /// </summary>
        public string AchievedDescription { get; set; }

        /// <summary>
        /// The achievement description when revealed but not unlocked.
        /// </summary>
        public string UnachievedDescription { get; set; }

        /// <summary>
        /// The completion progress for incremental achievements, represented as a percentage (0–100).
        /// </summary>
        public int PercentComplete { get; set; }

        /// <summary>
        /// Determines whether to automatically show a banner when the achievement is unlocked.
        /// </summary>
        public bool ShowCompletionBanner { get; set; }

        /// <summary>
        /// Determines if the achievement can be replayed.
        /// </summary>
        public bool IsReplayable { get; set; }

        /// <summary>
        /// A double value representing the rarity of the achievement (0.0 to 1.0).
        /// </summary>
        public double Rarity { get; set; }

        /// <summary>
        /// Bytes for the revealed achievement image.
        /// </summary>
        public byte[] RevealedImageBytes { get; set; }

        /// <summary>
        /// Bytes for the unlocked achievement image.
        /// </summary>
        public byte[] UnlockedImageBytes { get; set; }
    }

    /// <summary>
    /// Achievement info specific to Android platform
    /// </summary>
    [Preserve(AllMembers = true)]
    public class GamesAchievementAndroidExtras
    {

        /// <summary>
        /// The type of achievement.
        /// </summary>
        public GamesAchievementType Type { get; set; }

        /// <summary>
        /// The number of steps the user has completed to unlock the achievement. Only applicable to <see cref="GamesAchievementType.Incremental"/>
        /// </summary>
        public int CurrentSteps { get; set; }

        /// <summary>
        /// The total number of steps the user needs to unlock the achievement. Only applicable to <see cref="GamesAchievementType.Incremental"/>
        /// </summary>
        public int TotalSteps { get; set; }

        /// <summary>
        /// URI to the revealed achievement image.
        /// </summary>
        public Uri RevealedImageUri { get; set; }

        /// <summary>
        /// URL to the revealed achievement image.
        /// </summary>
        public string RevealedImageUrl { get; set; }

        /// <summary>
        /// URI to the unlocked achievement image.
        /// </summary>
        public Uri UnlockedImageUri { get; set; }

        /// <summary>
        /// URL to the unlocked achievement image.
        /// </summary>
        public string UnlockedImageUrl { get; set; }
    }

	/// <summary>
	/// A in game Achievement
	/// </summary>
	[Preserve(AllMembers = true)]
	public class GamesAchievement
    {
        /// <summary>
        /// The achievement id.
        /// </summary>
        public string AchievementId { get; set; }

        /// <summary>
        /// The achievement description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The state of the achievement.
        /// </summary>
        public GamesAchievementState State { get; set; }

        /// <summary>
        /// The date and time the achievement was last updated.
        /// </summary>
        public DateTime? LastUpdated { get; set; }

        /// <summary>
        /// XP value for this achievement (if provided by platform).
        /// </summary>
        public long XpValue { get; set; }

        /// <summary>
        /// The achievement name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Extra information for apple platforms
        /// </summary>
        public GamesAchievementAppleExtras Apple { get; init; }
        /// <summary>
        /// Extra information for Android platforms
        /// </summary>
        public GamesAchievementAndroidExtras Android { get; init; }
    }
}