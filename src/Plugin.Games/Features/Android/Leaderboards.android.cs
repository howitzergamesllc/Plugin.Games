using Android.Content;
using Android.Gms.Games;
using Android.Gms.Games.Leaderboard;
using Java.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Plugin.Games
{
    /// <summary>
    /// Implementation for Google Play Games
    /// </summary>
    public partial class GamesImplementation : BaseGames
    {
        /// <summary>
        /// A code for showing leaderboards from the MainActivity by overriding OnActivityResult.
        /// </summary>
        public static int ShowLeaderboardsCode { get; set; } = 2002;

        /// <summary>
        /// The friend reqeuest status code to resolve the onActivityResult.
        /// </summary>
        public static int FriendRequestCode { get; set; } = 1001;

        private static ILeaderboardsClient LeaderboardsClient
        {
            get
            {
                EnsureInitialized();
                return PlayGames.GetLeaderboardsClient(Activity);
            }
        }

        private static GamesLeaderboardScoreBuffer GamesLeaderboardsScoreBuffer { get; set; } = new GamesLeaderboardScoreBuffer();

        public override async Task ShowLeaderBoardsAsync()
        {
            var client = LeaderboardsClient;

            try
            {
                var intent = await client
                    .GetAllLeaderboardsIntent()
                    .AsTask<Intent>();

                Activity.StartActivityForResult(intent, ShowLeaderboardsCode);
            }
            catch (Exception ex)
            {
                throw new GamesException(GamesError.FailedToShowLeaderboard, ex);
            }
        }

        public override async Task ShowLeaderBoardsAsync(string leaderBoardId, GamesLeaderBoardTimeSpan timeSpan)
        {
            var client = LeaderboardsClient;

            try
            {
                var intent = await client
                    .GetLeaderboardIntent(leaderBoardId, timeSpan.ToNativeTimeSpan())
                    .AsTask<Intent>();

                Activity.StartActivityForResult(intent, ShowLeaderboardsCode);
            }
            catch (Exception ex)
            {
                throw new GamesException(GamesError.FailedToShowLeaderboard, ex);
            }
        }

        public override async Task ShowLeaderBoardsAsync(string leaderBoardId)
        {
            var client = LeaderboardsClient;

            try
            {
                var intent = await client
                    .GetLeaderboardIntent(leaderBoardId)
                    .AsTask<Intent>();

                Activity.StartActivityForResult(intent, ShowLeaderboardsCode);
            }
            catch (Exception ex)
            {
                throw new GamesException(GamesError.FailedToShowLeaderboard, ex);
            }
        }

        public override async Task ShowLeaderBoardsAsync(string leaderBoardId, GamesLeaderBoardTimeSpan timeSpan, GamesLeaderboardCollection collection)
        {
            var client = LeaderboardsClient;

            try
            {
                var intent = await client
                    .GetLeaderboardIntent(leaderBoardId, timeSpan.ToNativeTimeSpan(), collection.ToNativeCollection())
                    .AsTask<Intent>();

                Activity.StartActivityForResult(intent, ShowLeaderboardsCode);
            }
            catch (Exception ex)
            {
                throw new GamesException(GamesError.FailedToShowLeaderboard, ex);
            }
        }

        public override async Task<IReadOnlyList<GamesLeaderboard>> GetLeaderBoardDataAsync()
        {
            var client = LeaderboardsClient;

            try
            {
                var data = await client.LoadLeaderboardMetadata(ShouldForceReload).AsTask<AnnotatedData>();

                var buffer = (LeaderboardBuffer)data.Get();

                var results = new List<GamesLeaderboard>(buffer.Count);

                for (int i = 0; i < buffer.Count; i++)
                {
                    using var Ref = (LeaderboardRef)buffer.Get(i);
                    results.Add(Ref.ToGamesLeaderboard());
                }

                buffer.Release();

                return results.AsReadOnly();
            }
            catch (Exception ex)
            {
                throw new GamesException(GamesError.FailedToLoadLeaderboard, ex);
            }
        }

        public override async Task<GamesLeaderboard> GetLeaderBoardDataAsync(string leaderBoardId)
        {
            var client = LeaderboardsClient;

            try
            {
                var data = await client
                    .LoadLeaderboardMetadata(leaderBoardId, ShouldForceReload)
                    .AsTask<AnnotatedData>();

                var stats = data.Get().JavaAs<LeaderboardRef>();

                return stats.ToGamesLeaderboard();
            }
            catch (Exception ex)
            {
                throw new GamesException(GamesError.FailedToLoadLeaderboard, ex);
            }
        }

        internal sealed class GamesLeaderboardScoreBuffer
        {
            internal LeaderboardScoreBuffer NativeBuffer { get; }

            internal GamesLeaderboardScoreBuffer()
            {
            }

            internal GamesLeaderboardScoreBuffer(LeaderboardScoreBuffer buffer)
            {
                NativeBuffer = buffer ?? throw new NullReferenceException(nameof(buffer));
            }

            internal void Release()
            {
                NativeBuffer?.Release();
            }
        }

        public override async Task<IReadOnlyList<GamesLeaderboardScore>> GetMoreScoresAsync(int maxResults, GamesPageDirection direction)
        {
            var client = LeaderboardsClient;

            if (maxResults < 1) throw new GamesException(GamesError.LeaderboardMaxResultsOutofBounds,
                    "Max results must be positive");
            maxResults = Math.Min(maxResults, 25);

            var nativebuffer = GamesLeaderboardsScoreBuffer.NativeBuffer ?? throw new GamesException(GamesError.LeaderboardScoresNotInitialized, 
                $"Call {nameof(GetTopScoresAsync)} or {nameof(GetPlayerCenteredScoresAsync)} first.");
            
            try
            {
                var data = await client.LoadMoreScores(nativebuffer, maxResults, direction.ToNativePageDirection()).AsTask<AnnotatedData>();

                var buffer = (LeaderboardScoreBuffer)data.Get();

                var results = new List<GamesLeaderboardScore>(buffer.Count);

                for (int i = 0; i < buffer.Count; i++)
                {
                    using var Ref = (LeaderboardScoreRef)buffer.Get(i);
                    results.Add(Ref.ToGamesLeaderboardScoreAsync());
                }

                GamesLeaderboardsScoreBuffer.Release(); // release old
                GamesLeaderboardsScoreBuffer = new GamesLeaderboardScoreBuffer(buffer); // save new

                return results.AsReadOnly();
            }
            catch (FriendsResolutionRequiredException ex)
            {
                Activity.StartIntentSenderForResult(ex.Resolution.IntentSender, FriendRequestCode, null, 0, 0, 0);
                return BuildEmptyGamesLeaderBoardScoreList();
            }
            catch (Exception ex)
            {
                throw new GamesException(GamesError.FailedToLoadLeaderboardScore, ex);
            }
        }

        public override async Task<IReadOnlyList<GamesLeaderboardScore>> GetPlayerCenteredScoresAsync(string leaderBoardId, GamesLeaderBoardTimeSpan timeSpan, GamesLeaderboardCollection collection, int maxResults)
        {
            var client = LeaderboardsClient;

            if (maxResults < 1) throw new GamesException(GamesError.LeaderboardMaxResultsOutofBounds,
                    "Max results must be positive");
            maxResults = Math.Min(maxResults, 25);

            try
            {
                var data = await client.LoadPlayerCenteredScores(leaderBoardId, timeSpan.ToNativeTimeSpan(), collection.ToNativeCollection(), maxResults, ShouldForceReload).AsTask<AnnotatedData>();

                var buffer = (LeaderboardScoreBuffer)data.Get();

                var results = new List<GamesLeaderboardScore>(buffer.Count);

                for (int i = 0; i < buffer.Count; i++)
                {
                    using var Ref = (LeaderboardScoreRef)buffer.Get(i);
                    results.Add(Ref.ToGamesLeaderboardScoreAsync());
                }

                GamesLeaderboardsScoreBuffer.Release(); // release old if applicable
                GamesLeaderboardsScoreBuffer = new GamesLeaderboardScoreBuffer(buffer); // save new

                return results.AsReadOnly();
            }
            catch (FriendsResolutionRequiredException ex)
            {
                Activity.StartIntentSenderForResult(ex.Resolution.IntentSender, FriendRequestCode, null, 0, 0, 0);
                return BuildEmptyGamesLeaderBoardScoreList();
            }
            catch (Exception ex)
            {
                throw new GamesException(GamesError.FailedToLoadLeaderboardScore, ex);
            }
        }

        public override async Task<IReadOnlyList<GamesLeaderboardScore>> GetTopScoresAsync(string leaderBoardId, GamesLeaderBoardTimeSpan timeSpan, GamesLeaderboardCollection collection, int maxResults)
        {
            var client = LeaderboardsClient;

            if (maxResults < 1) throw new GamesException(GamesError.LeaderboardMaxResultsOutofBounds,
                    "Max results must be positive");
            maxResults = Math.Min(maxResults, 25);

            try
            {
                var data = await client.LoadTopScores(leaderBoardId, timeSpan.ToNativeTimeSpan(), collection.ToNativeCollection(), maxResults, ShouldForceReload).AsTask<AnnotatedData>();

                var buffer = (LeaderboardScoreBuffer)data.Get();

                var results = new List<GamesLeaderboardScore>(buffer.Count);

                for (int i = 0; i < buffer.Count; i++)
                {
                    using var Ref = (LeaderboardScoreRef)buffer.Get(i);
                    results.Add(Ref.ToGamesLeaderboardScoreAsync());
                }

                GamesLeaderboardsScoreBuffer.Release(); // release old if applicable
                GamesLeaderboardsScoreBuffer = new GamesLeaderboardScoreBuffer(buffer); // save new

                return results.AsReadOnly();
            }
            catch (FriendsResolutionRequiredException ex)
            {    
                Activity.StartIntentSenderForResult(ex.Resolution.IntentSender, FriendRequestCode, null, 0, 0, 0);
                return BuildEmptyGamesLeaderBoardScoreList();
            }
            catch (Exception ex)
            {
                throw new GamesException(GamesError.FailedToLoadLeaderboardScore, ex);
            }
        }

        public override async Task<GamesScoreSubmission> SubmitScoreAsync(string leaderBoardId, long score, GamesScoreMetadata metaData = null)
        {
            var client = LeaderboardsClient;
            ScoreSubmissionData scoreSubmission;

            try
            {
                var scoreTag = metaData?.GoogleScoreTag;

                if(!string.IsNullOrEmpty(scoreTag))
                {
                    
                    if(scoreTag.Length > 64)
                        throw new GamesException(GamesError.InvalidScoreMetadata, "GoogleScoreTag must be 64 characters or less.");

                    scoreSubmission = await client.SubmitScoreImmediate(leaderBoardId, score, scoreTag).AsTask<ScoreSubmissionData>();
                }
                else
                {
                    scoreSubmission = await client.SubmitScoreImmediate(leaderBoardId, score).AsTask<ScoreSubmissionData>();
                }
                return scoreSubmission.ToGamesScoreSubmission();
            }
            catch (Exception ex)
            {
                throw new GamesException(GamesError.FailedToSubmitScore, ex);
            }
        }

        private IReadOnlyList<GamesLeaderboardScore> BuildEmptyGamesLeaderBoardScoreList()
        {
            var empty = new List<GamesLeaderboardScore>();
            empty.Add(BuildEmptyGamesLeaderBoardScore());
            return empty.AsReadOnly();
        }

        private GamesLeaderboardScore BuildEmptyGamesLeaderBoardScore()
        {
            var metadata = new GamesScoreMetadata()
            {
                GoogleScoreTag = "Hidden"
            };
            return new GamesLeaderboardScore() { DisplayRank = "Hidden", DisplayScore = "Hidden", DisplayName = "Hidden", Rank = -1L, RawScore = -1L, Metadata = metadata };
        }
    }
}