
using System;
using System.Threading.Tasks;
using GameKit;
using Microsoft.Maui.ApplicationModel;

namespace Plugin.Games
{
    /// <summary>
    /// Implementation for Apple Games Center
    /// </summary>
    [Preserve(AllMembers = true)]
    public partial class GamesImplementation : BaseGames
    {
        /// <summary>
        /// Default Constructor for GamesImplementation on Android
        /// </summary>
        public GamesImplementation()
        {
        }

#if __IOS__ || __TVOS__
        internal static bool HasRarityPercent => UIKit.UIDevice.CurrentDevice.CheckSystemVersion(17, 0);
        internal static bool HasLeaderBoardEntry => UIKit.UIDevice.CurrentDevice.CheckSystemVersion(17, 4);
        internal static bool HasGameCenterViewController => UIKit.UIDevice.CurrentDevice.CheckSystemVersion(18, 0);
        internal static bool HasTriggerAccessPointWithInfo => UIKit.UIDevice.CurrentDevice.CheckSystemVersion(18, 0);
        internal static bool HasLeaderBoardExtras => UIKit.UIDevice.CurrentDevice.CheckSystemVersion(26, 0);
        internal static bool HasUIScene => UIKit.UIDevice.CurrentDevice.CheckSystemVersion(17, 0);
        internal static bool HasUIViewController => UIKit.UIDevice.CurrentDevice.CheckSystemVersion(18, 2);
#else
		static bool initRarityPercent, hasRarityPercent, initLeaderBoardEntry, hasLeaderBoardEntry, initGameCenterViewController, hasGameCenterViewController, initTriggerAccessPointWithInfo, hasTriggerAccessPointWithInfo, initLeaderBoardExtras, hasLeaderBoardExtras, initUIScene, hasUIScene, initUIViewController, hasUIViewController;
        
        internal static bool HasRarityPercent
        {
			get
            {
				if (initRarityPercent)
					return hasRarityPercent;

				initRarityPercent = true;

				using var info = new NSProcessInfo();
				hasRarityPercent = info.IsOperatingSystemAtLeastVersion(new NSOperatingSystemVersion(17, 0, 0));
				return hasRarityPercent;
			}
        }
        internal static bool HasLeaderBoardEntry
        {
			get
            {
				if (initLeaderBoardEntry)
					return hasLeaderBoardEntry;

				initLeaderBoardEntry = true;

				using var info = new NSProcessInfo();
				hasLeaderBoardEntry = info.IsOperatingSystemAtLeastVersion(new NSOperatingSystemVersion(17, 4, 0));
				return hasLeaderBoardEntry;
			}
        }
        internal static bool HasGameCenterViewController
        {
            get
            {
                if (initGameCenterViewController)
                    return hasGameCenterViewController;

                initGameCenterViewController = true;

                using var info = new NSProcessInfo();
                hasGameCenterViewController = info.IsOperatingSystemAtLeastVersion(new NSOperatingSystemVersion(18, 0, 0));
                return hasGameCenterViewController;
            }
        }
        internal static bool HasTriggerAccessPointWithInfo
        {
			get
            {
				if (initTriggerAccessPointWithInfo)
					return hasTriggerAccessPointWithInfo;

				initTriggerAccessPointWithInfo = true;

				using var info = new NSProcessInfo();
				hasTriggerAccessPointWithInfo = info.IsOperatingSystemAtLeastVersion(new NSOperatingSystemVersion(18, 0, 0));
				return hasTriggerAccessPointWithInfo;
			}
        }
        internal static bool HasLeaderBoardExtras
        {
			get
            {
				if (initLeaderBoardExtras)
					return hasLeaderBoardExtras;

				initLeaderBoardExtras = true;

				using var info = new NSProcessInfo();
				hasLeaderBoardExtras = info.IsOperatingSystemAtLeastVersion(new NSOperatingSystemVersion(18, 0, 0));
				return hasLeaderBoardExtras;
			}
        }
        internal static bool HasUIScene
        {
            get
            {
                if (initUIScene) return hasUIScene;
                initUIScene = true;
                using var info = new NSProcessInfo();
                hasUIScene = info.IsOperatingSystemAtLeastVersion(new NSOperatingSystemVersion(18, 2, 0));
                return hasUIScene;
            }
        }
        internal static bool HasUIViewController
        {
            get
            {
                if (initUIViewController) return hasUIViewController;
                initUIViewController = true;
                using var info = new NSProcessInfo();
                hasUIViewController = info.IsOperatingSystemAtLeastVersion(new NSOperatingSystemVersion(18, 2, 0));
                return hasUIViewController;
            }
        }
#endif

        private Task ShowAccessPoint()
        {
            return MainThread.InvokeOnMainThreadAsync(() =>
            {
                GKAccessPoint.Shared.TriggerAccessPoint(() => { });
            });
        }
    }
}