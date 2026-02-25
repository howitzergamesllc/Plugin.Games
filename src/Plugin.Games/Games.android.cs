using Android.App;
using Android.Content;
using Android.Gms.Games;
using Microsoft.Maui.ApplicationModel;
using System;

namespace Plugin.Games
{
    /// <summary>
    /// Implementation for Google Play Games
    /// </summary>
    [Preserve(AllMembers = true)]
    public partial class GamesImplementation : BaseGames
    {
        /// <summary>
        /// If true, this call will clear any locally-cached data and attempt to fetch the latest data 
        /// from the server. This would commonly be used for something like a user-initiated refresh. Normally, this should be 
        /// set to false to gain advantages of data caching. Default is false.
        /// </summary>
        public static bool ShouldForceReload { get; set; } = false;

        /// <summary>
        /// Gets the context, aka the currently activity.
        /// This is set from the MainApplication.cs file that was laid down by the plugin
        /// </summary>
        /// <value>The context.</value>
        static Activity Activity =>
            Platform.CurrentActivity ?? throw new NullReferenceException("Current Activity is null, ensure to implement Games into your MainActivity.cs");

        static Context Context => Application.Context;

        /// <summary>
        /// Default Constructor for GamesImplementation on Android
        /// </summary>
        public GamesImplementation()
        {
        }

        internal static void Initialize(Context context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (_isInitialized)
                return;

            PlayGamesSdk.Initialize(context);

            _isInitialized = true;
        }

        private static bool _isInitialized;

        private const string initializeMessage = "Call CrossGames.Initialize(this) in the MainActivity by overriding OnCreate.";

        internal static bool EnsureInitialized()
        {
            if (!_isInitialized)
                throw new GamesException(GamesError.NotInitialized, initializeMessage);

            return _isInitialized;
        }
    }
}