using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameKit;
using Foundation;
using System.Linq;
using Microsoft.Maui.ApplicationModel;

namespace Plugin.Games
{
    /// <summary>
    /// Implementation for Apple Games Center
    /// </summary>
    public partial class GamesImplementation : BaseGames
    {
        /// <inheritdoc />
        public override Task ShowLeaderBoardsAsync()
        {
            try
            {
                return ShowAccessPoint();
            }
            catch (Exception ex)
            {
                throw new GamesException(GamesError.FailedToShowLeaderboard, ex);
            }
        }

        /// <inheritdoc />
        public override Task ShowLeaderBoardsAsync(string leaderBoardId)
        {
            if(!HasTriggerAccessPointWithInfo)
                return Task.CompletedTask;

            return MainThread.InvokeOnMainThreadAsync(() =>
            {
                try
                {
                    GKAccessPoint.Shared.TriggerAccessPointWithLeaderboardSetId(leaderBoardId, () => {});
                }
                catch (Exception ex)
                {
                    throw new GamesException(GamesError.FailedToShowLeaderboard, ex);
                }
            });
        }

        /// <inheritdoc />
        public override async Task<GamesLeaderboard> GetLeaderBoardDataAsync(string leaderBoardId) 
        {
            var lb = await GetNativeLeaderBoardData(leaderBoardId);

            if (lb == null)
                return null;

            var image = await lb.LoadImageAsync();

            return lb.ToGamesLeaderboard(image);
        }

        private async Task<GKLeaderboard> GetNativeLeaderBoardData(string leaderBoardId) 
        {
            return await MainThread.InvokeOnMainThreadAsync(async() =>
            {
                try
                {
                    return (await GKLeaderboard.LoadLeaderboardsAsync(new[] { leaderBoardId })).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw new GamesException(GamesError.FailedToLoadLeaderboard, ex);
                }
            });
        }

        /// <inheritdoc />
        public override async Task<IReadOnlyList<GamesLeaderboardScore>> GetPlayerCenteredScoresAsync(string leaderBoardId, GamesLeaderBoardTimeSpan timeSpan, GamesLeaderboardCollection collection, int maxResults)
        {
            if (maxResults < 1 || maxResults > 100) throw new GamesException(GamesError.LeaderboardMaxResultsOutofBounds,
                    "Max results must be between 1 and 100.");

            var lb = await GetNativeLeaderBoardData(leaderBoardId);

            try
            {
                if (lb == null) 
                    return Array.Empty<GamesLeaderboardScore>();

                var localPlayerEntry = await lb.LoadEntriesAsync(
                    new[] {  await Extensions.LocalPlayerAsync() },
                    timeSpan.ToNativeTimeScope()
                );

                if(localPlayerEntry == null || localPlayerEntry.Entries == null || localPlayerEntry.Entries.Length == 0 || localPlayerEntry.LocalPlayerEntry == null)
                    return await GetTopScoresAsync(leaderBoardId, timeSpan, collection, maxResults);

                int rank = (int)localPlayerEntry.LocalPlayerEntry.Rank;

                int half = Math.Clamp((int)Math.Round((double)maxResults / 2D, MidpointRounding.AwayFromZero), 1, 50);
                int startRank = Math.Max(1, rank - half);
                int endRank = rank + half;
                endRank = endRank - startRank + 1 > 100 ? startRank + 99 : endRank;
                
                var range = new NSRange(startRank, endRank - startRank + 1);
                var entries = await lb.LoadEntriesAsync(collection.ToNativeCollection(),
                    timeSpan.ToNativeTimeScope(),
                    range);

                var nativeEntries = entries.Entries ?? Array.Empty<GKLeaderboardEntry>();
                var scores = new List<GamesLeaderboardScore>();

                foreach(var entry in nativeEntries)
                {
                    var large = await entry.Player.LoadPhotoAsync(GKPhotoSize.Normal).ConfigureAwait(false);
                    var small = await entry.Player.LoadPhotoAsync(GKPhotoSize.Small).ConfigureAwait(false);

                    scores.Add(entry.ToGamesLeaderboardScore(large,small));
                }

                return scores.AsReadOnly();
            }
            catch (Exception ex)
            {
                throw new GamesException(GamesError.FailedToLoadLeaderboardScore, ex);
            }
        }

        /// <inheritdoc />
        public override async Task<IReadOnlyList<GamesLeaderboardScore>> GetTopScoresAsync(string leaderBoardId, GamesLeaderBoardTimeSpan timeSpan, GamesLeaderboardCollection collection, int maxResults)
        {
            if (maxResults < 1 || maxResults > 100) throw new GamesException(GamesError.LeaderboardMaxResultsOutofBounds,
                    "Max results must be between 1 and 100.");

            var lb = await GetNativeLeaderBoardData(leaderBoardId);

            if (lb == null) 
                return Array.Empty<GamesLeaderboardScore>();

            try
            {
                var range = new NSRange(1, maxResults);
                var entries = await lb.LoadEntriesAsync(collection.ToNativeCollection(),
                    timeSpan.ToNativeTimeScope(),
                    range);

                var nativeEntries = entries.Entries ?? Array.Empty<GKLeaderboardEntry>();
                var scores = new List<GamesLeaderboardScore>();

                foreach(var entry in nativeEntries)
                {
                    var large = await entry.Player.LoadPhotoAsync(GKPhotoSize.Normal).ConfigureAwait(false);
                    var small = await entry.Player.LoadPhotoAsync(GKPhotoSize.Small).ConfigureAwait(false);

                    scores.Add(entry.ToGamesLeaderboardScore(large,small));
                }
                
                return scores.ToList();
            }
            catch (Exception ex)
            {
                throw new GamesException(GamesError.FailedToLoadLeaderboardScore, ex);
            }
        }

        /// <inheritdoc />
        public override async Task<GamesScoreSubmission> SubmitScoreAsync(string leaderBoardId, long score, GamesScoreMetadata metaData = null) 
        {
            var lb = await GetNativeLeaderBoardData(leaderBoardId);

            if (lb == null) 
                return null;

            try
            {
                nint nintScore = (nint)Math.Clamp(score, nint.MinValue, nint.MaxValue);

                if(metaData?.AppleContext is long context)
                {
                    context.ToNUInt(out nuint nativeContext);
                    if(nativeContext > ulong.MaxValue)
                        throw new GamesException(GamesError.InvalidScoreMetadata, "AppleContext must be less than the maximum value for an unsigned 64-bit integer.");
                    
                    await lb.SubmitScoreAsync(nintScore, nativeContext, await Extensions.LocalPlayerAsync());
                }
                else
                {
                    await lb.SubmitScoreAsync(nintScore, 0, await Extensions.LocalPlayerAsync());
                }

                var updatedlb = await GetNativeLeaderBoardData(leaderBoardId);

                var weekly = await updatedlb.LoadEntriesAsync(
                    new[] { await Extensions.LocalPlayerAsync() },
                    GamesLeaderBoardTimeSpan.Weekly.ToNativeTimeScope()
                );
                var daily = await updatedlb.LoadEntriesAsync(
                    new[] { await Extensions.LocalPlayerAsync() },
                    GamesLeaderBoardTimeSpan.Daily.ToNativeTimeScope()
                );
                var alltime = await updatedlb.LoadEntriesAsync(
                    new[] { await Extensions.LocalPlayerAsync() },
                    GamesLeaderBoardTimeSpan.AllTime.ToNativeTimeScope()
                );

                var imageLarge = await alltime.LocalPlayerEntry.Player.LoadPhotoAsync(GKPhotoSize.Normal).ConfigureAwait(false);
                var imageSmall = await alltime.LocalPlayerEntry.Player.LoadPhotoAsync(GKPhotoSize.Small).ConfigureAwait(false);
                            
                var results = new Dictionary<GamesLeaderBoardTimeSpan, GamesScoreSubmissionResult>();

                if (alltime is not null)
                    results.Add(GamesLeaderBoardTimeSpan.AllTime, alltime.ToGamesScoreSubmissionResult(imageLarge, imageSmall));
                if (daily is not null)
                    results.Add(GamesLeaderBoardTimeSpan.Daily, daily.ToGamesScoreSubmissionResult(imageLarge, imageSmall));
                if (weekly is not null)
                    results.Add(GamesLeaderBoardTimeSpan.Weekly, weekly.ToGamesScoreSubmissionResult(imageLarge, imageSmall));

                var submission = new GamesScoreSubmission
                {
                    LeaderboardId = leaderBoardId,
                    PlayerId = (await Extensions.LocalPlayerAsync()).TeamPlayerId,
                    Results = results
                };

                return submission;
            }
            catch (Exception ex)
            {
                throw new GamesException(GamesError.FailedToSubmitScore, ex);
            }
        }
    }
}