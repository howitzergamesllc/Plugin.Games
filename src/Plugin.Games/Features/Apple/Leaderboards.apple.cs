using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameKit;
using Foundation;
using System.Linq;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui;
using UIKit;

namespace Plugin.Games
{
    /// <summary>
    /// Implementation for Apple Games Center
    /// </summary>
    public partial class GamesImplementation : BaseGames
    {
        private GKLeaderboard currentLeaderboard;
        private NSRange currentRange;
        private GKLeaderboardTimeScope currentTimeScope;
        private GKLeaderboardPlayerScope currentPlayerScope;
        private IReadOnlyList<GamesLeaderboardScore> currentScores;

        /// <inheritdoc />
        public override async Task ShowLeaderBoardsAsync()
        {
            try
            {
                await ShowAccessPoint();
            }
            catch (Exception ex)
            {
                throw new GamesException(GamesError.FailedToShowLeaderboard, ex);
            }
        }

        /// <inheritdoc />
        public override Task ShowLeaderBoardsAsync(string identifier, GamesLeaderboardTimeSpan timeSpan = GamesLeaderboardTimeSpan.Unknown, 
                GamesLeaderboardCollection collection = GamesLeaderboardCollection.Unknown)
        {
            if(!HasTriggerAccessPointWithInfo)
                return Task.CompletedTask;

            return MainThread.InvokeOnMainThreadAsync(() =>
            {
                try
                {
                    GKAccessPoint.Shared.TriggerAccessPointWithLeaderboardSetId(identifier, () => {});
                }
                catch (Exception ex)
                {
                    throw new GamesException(GamesError.FailedToShowLeaderboard, ex);
                }
            });
        }

        /// <inheritdoc />
        public override async Task<IReadOnlyList<GamesLeaderboard>> GetLeaderBoardDataAsync()
        {
            return await MainThread.InvokeOnMainThreadAsync(async() =>
            {
                try
                {
                    var leaderboards = new List<GamesLeaderboard>();
                    var nativeLeaderBoardSets = await GKLeaderboardSet.LoadLeaderboardSetsAsync();
                    foreach(var nativeSet in nativeLeaderBoardSets)
                    {
                        var leaderboardImage = await nativeSet.LoadImageAsync();
                        var nativeLeaderBoards = await nativeSet.LoadLeaderboardsWithCompletionHandlerAsync();
                        foreach(var nativeLeaderBoard in nativeLeaderBoards)
                        {
                            leaderboards.Add(nativeLeaderBoard.ToGamesLeaderboard(leaderboardImage));
                        }
                    }
                    return leaderboards;
                }
                catch(Exception ex)
                {
                    throw new GamesException(GamesError.FailedToLoadLeaderboard, ex);
                }
            });
        }

        /// <inheritdoc />
        public override async Task<GamesLeaderboard> GetLeaderBoardDataAsync(string leaderBoardId) 
        {
            try
            {
                var lb = await GetNativeLeaderBoardData(leaderBoardId);

                if (lb == null)
                    return null;

                var image = await lb.LoadImageAsync();

                return lb.ToGamesLeaderboard(image);
            }
            catch(Exception ex)
            {
                throw new GamesException(GamesError.FailedToLoadLeaderboard, ex);
            }
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
        public override async Task<IReadOnlyList<GamesLeaderboardScore>> GetMoreScoresAsync(int maxResults, GamesPageDirection direction)
        {
            if (maxResults < 1) throw new GamesException(GamesError.LeaderboardMaxResultsOutofBounds,
                    "Max results must be positive");
            maxResults = Math.Min(maxResults, 100);

            var lb = currentLeaderboard ?? throw new GamesException(GamesError.LeaderboardScoresNotInitialized, 
                $"Call {nameof(GetTopScoresAsync)} or {nameof(GetPlayerCenteredScoresAsync)} first.");
            var range = currentRange.Length == 0 ? throw new GamesException(GamesError.LeaderboardScoresNotInitialized, 
                $"Call {nameof(GetTopScoresAsync)} or {nameof(GetPlayerCenteredScoresAsync)} first.") : currentRange;

            var nextRange = direction switch
            {
                GamesPageDirection.Next => new NSRange(range.Location + range.Length, maxResults),
                GamesPageDirection.Prev => new NSRange(Math.Max(1, range.Location - maxResults), maxResults),
                GamesPageDirection.None => new NSRange(range.Location, maxResults),
                _ => range
            };

            try
            {
                currentScores = await BuildScoreDataAsync(lb, nextRange, currentTimeScope, currentPlayerScope);

                return currentScores;
            }
            catch (Exception ex)
            {
                throw new GamesException(GamesError.FailedToLoadLeaderboardScore, ex);
            }
        }

        /// <inheritdoc />
        public override async Task<IReadOnlyList<GamesLeaderboardScore>> GetPlayerCenteredScoresAsync(string leaderBoardId, GamesLeaderboardTimeSpan timeSpan, GamesLeaderboardCollection collection, int maxResults)
        {
            ClearPaginationState();

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
                int startRank = Math.Max(1, rank - maxResults / 2);
                var range = new NSRange(startRank, maxResults);
                
                currentPlayerScope = collection.ToNativeCollection();
                currentTimeScope = timeSpan.ToNativeTimeScope();
                currentScores = await BuildScoreDataAsync(lb, range, currentTimeScope, currentPlayerScope);

                return currentScores;
            }
            catch (Exception ex)
            {
                throw new GamesException(GamesError.FailedToLoadLeaderboardScore, ex);
            }
        }

        /// <inheritdoc />
        public override async Task<IReadOnlyList<GamesLeaderboardScore>> GetTopScoresAsync(string leaderBoardId, GamesLeaderboardTimeSpan timeSpan, GamesLeaderboardCollection collection, int maxResults)
        {
            ClearPaginationState();

            if (maxResults < 1 || maxResults > 100) throw new GamesException(GamesError.LeaderboardMaxResultsOutofBounds,
                    "Max results must be between 1 and 100.");

            var lb = await GetNativeLeaderBoardData(leaderBoardId);

            if (lb == null) 
                return Array.Empty<GamesLeaderboardScore>();

            try
            {
                var range = new NSRange(1, maxResults);
                currentPlayerScope = collection.ToNativeCollection();
                currentTimeScope = timeSpan.ToNativeTimeScope();
                currentScores = await BuildScoreDataAsync(lb, range, currentTimeScope, currentPlayerScope);
                
                return currentScores;
            }
            catch (Exception ex)
            {
                throw new GamesException(GamesError.FailedToLoadLeaderboardScore, ex);
            }
        }

        private void ClearPaginationState()
        {
            currentLeaderboard = null;
            currentRange = new NSRange(0,0);
            currentTimeScope = default;
            currentPlayerScope = default;
            currentScores = null;
        }

        private async Task<List<GamesLeaderboardScore>> BuildScoreDataAsync(GKLeaderboard lb, NSRange range, GKLeaderboardTimeScope timeScope, GKLeaderboardPlayerScope playerScope)
        {
            try
            {
                var entries = await lb.LoadEntriesAsync(playerScope, timeScope, range);

                var nativeEntries = entries.Entries ?? Array.Empty<GKLeaderboardEntry>();
                var scores = new List<GamesLeaderboardScore>();

                foreach(var entry in nativeEntries)
                {
                    var large = await entry.Player.LoadPhotoAsync(GKPhotoSize.Normal).ConfigureAwait(false);
                    var small = await entry.Player.LoadPhotoAsync(GKPhotoSize.Small).ConfigureAwait(false);

                    scores.Add(entry.ToGamesLeaderboardScore(large,small));
                }

                currentLeaderboard = lb;
                currentRange = range;

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
                    GamesLeaderboardTimeSpan.Weekly.ToNativeTimeScope()
                );
                var daily = await updatedlb.LoadEntriesAsync(
                    new[] { await Extensions.LocalPlayerAsync() },
                    GamesLeaderboardTimeSpan.Daily.ToNativeTimeScope()
                );
                var alltime = await updatedlb.LoadEntriesAsync(
                    new[] { await Extensions.LocalPlayerAsync() },
                    GamesLeaderboardTimeSpan.AllTime.ToNativeTimeScope()
                );

                UIImage imageLarge = await GKLocalPlayer.LocalPlayer.LoadPhotoAsync(GKPhotoSize.Normal);
                UIImage imageSmall = await GKLocalPlayer.LocalPlayer.LoadPhotoAsync(GKPhotoSize.Small);
                            
                var results = new Dictionary<GamesLeaderboardTimeSpan, GamesScoreSubmissionResult>();

                if (alltime is not null)
                    results.Add(GamesLeaderboardTimeSpan.AllTime, alltime.ToGamesScoreSubmissionResult(imageLarge, imageSmall));
                if (daily is not null)
                    results.Add(GamesLeaderboardTimeSpan.Daily, daily.ToGamesScoreSubmissionResult(imageLarge, imageSmall));
                if (weekly is not null)
                    results.Add(GamesLeaderboardTimeSpan.Weekly, weekly.ToGamesScoreSubmissionResult(imageLarge, imageSmall));

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