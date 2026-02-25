using System;

namespace Plugin.Games
{
    /// <summary>
    /// Cross platform Games implementations
    /// </summary>
    public class CrossGames
    {
        static Lazy<IGames> implementation = new(() => CreateGames(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Gets if the plugin is supported on the current platform.
        /// </summary>
        public static bool IsSupported => implementation.Value is GamesNotImplementation ? false : true;

        /// <summary>
        /// Current plugin implementation to use
        /// </summary>
        public static IGames Current
        {
            get
            {
                var imp = implementation.Value;
                if (imp == null)
                {
                    throw NotImplemented();
                }
                return imp;
            }
        }


#if ANDROID || IOS || MACCATALYST
        static IGames CreateGames() => new GamesImplementation();
#else
        static IGames CreateGames() => new GamesNotImplementation();
#endif

#if ANDROID
        /// <summary>
        /// Initializes the PlayGamesSdk v2 with the current app context.
        /// Should be called once at app startup, typically in the <c>MainActivity</c> or application context.
        /// </summary>
        /// <param name="context">The Android <see cref="Android.Content.Context"/> used to initialize PlayGamesSdk v2.</param>
        public static void Initialize(Android.Content.Context context)
        {
            GamesImplementation.Initialize(context);
        }
#endif
        internal static Exception NotImplemented() =>
            new NotImplementedException($"This plugin has not been implemented on the calling platform. Check CrossGames.{nameof(IsSupported)} prior to calling or wrap the call in a try/catch.");

        /// <summary>
        /// Dispose of everything 
        /// </summary>
        public static void Dispose()
        {
            if (implementation != null && implementation.IsValueCreated)
            {
                implementation = new Lazy<IGames>(() => CreateGames(), System.Threading.LazyThreadSafetyMode.PublicationOnly);
            }
        }
    }
}
