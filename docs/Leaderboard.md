# Leaderboards

ShowLeaderBoardsAsync, GetLeaderBoardDataAsync, and SubmitScore have been abstracted with litte platform differences. GetTopScoresAsync and GetPlayerCenteredScoresAsync have been abstracted to make Apple behave a little more like Android. Apple doesn't provide methods for "TopScores" or "PlayerCenteredScores", they just provide "loadEntries" which gets which ever range of rank scores you ask for. These methods are essentially helper methods from Google Play Games for convenience so the abstraction choice to make these methods be convenient for Apple too.


## Google Specifics

On Android, the maximum results represents the number of results that should be returned and cannot be negative or greater than 25. An ArgumentOutOfRangeException will be thrown for Android if the value is negative. Any input greater than 25 will just be clamped to 25. 

- Call a player specific leaderboard by passing in the leaderboardId and a GamesLeaderBoardTimeSpan.
```C#
GamesLeaderboard leaderboard = await ShowLeaderBoardsAsync(leaderBoardId, timeSpan);
```

- After calling "GetTopScores" or "GetPlayerCenteredScores", Google supports a concept called pagination. Use "GetMoreScores"
passing in a maxResults
```C#
List<GamesLeaderboardScore> scores = await GetMoreScoresAsync(int maxResults, GamesPageDirection direction);
```

- FriendsResolutionRequiredException
When calling the Android specific player leaderboard ShowLeaderBoardsAsync(leaderBoardId, timespan), when calling "GetTopScores", when calling "GetPlayerCenteredScores", or when calling the Android specific "GetMoreScores", a FriendsResolutionRequiredException can be thrown if the players are not friends with the calling player. When the exception is thrown, a request will automatically prompt a friend request through Googles user interface. The pending response can be handled in the MainActivit by overriding on ActivityResult and checking if it matches the GamesImplementation.FriendRequestCode. If successful, you can automatically request the information previously requested here.
```C#
    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);

        if (requestCode == GamesImplementation.FriendRequestCode)
        {
            if (resultCode == Result.Ok)
            {
                // User accepted → retry the friends leaderboard call
            }
            else
            {
                // User declined → fall back to public leaderboard
            }
        }
    }
```

## Apple Specifics

With Apple, the maxResults represents the range of Ranks from the players rank that should be returned which cannot be negative or greater than 100. An ArgumentOutOfRangeException will be thrown for Apple if the value is nagative. Any input that results in a rank range of more than 100 is clamped or set to 100.


## Shared

- Leaderboards can be displayed from Google Play Games and Apple Game Center user interfaces by awaiting "ShowLeaderBoardsAsync"
```C#
await CrossGames.Current.ShowLeaderBoardsAsync();
```

- A specific leaderboard can be displayed from Google Play Games and Apple Game Center user interfaces by awaiting "ShowLeaderBoardsAsync" and providing the leaderboardId. On Google, this is the specific identifier for the leaderboard. With Apple,
this is the identifier associated with a leaderboard set.
```C#
await CrossGames.Current.ShowLeaderBoardsAsync(leaderBoardId);
```

- To get all leaderboard data, you can await "GetLeaderBoardData"
```C#
List<GamesLeaderboard> leaderboards = await CrossGames.Current.GetLeaderBoardDataAsync();
```

- To get a specific leaderboard, just pass the leaderboardId into "GetLeaderBoardData". This could return GamesException if the process
fails or it could return null if the leaderboardId can't be found.
```C#
GamesLeaderboard leaderboard = await CrossGames.Current.GetLeaderBoardDataAsync(leaderboardId);
```

- To get the top score data from a leaderboard, await "GetTopScoresAsync" and pass in the desired identifier, GamesLeaderBoardTimeSpan, GameLeaderboardCollection, and the maximum results integer. 
```C#
List<GamesLeaderboardScore> scores = await CrossGames.Currnt.GetTopScoresAsync(leaderBoardId, timeSpan, collection, maxResults)
```
On Android, the maximum results represents the number of results that should be returned and cannot be negative or greater than 25. An ArgumentOutOfRangeException will be thrown for Android if the value is negative. Any input greater than 25 will just be clamped to 25. 

- Scores that surround the player where the player is centered can be gathered by awaiting "GetPlayerCenteredScoresAsync"
```C#
List<GamesLeaderboardScore> scores = await CrossGames.Currnt.GetPlayerCenteredScoresAsync(leaderBoardId, timeSpan, collection, maxResults)
```

- To submit a score, await "SubmitScoreAsync" passing in the leaderboard identifier, the new total score to be applied to the leaderboard, and any (optional) metadata associated with the score. For Google, the GamesScoreMetadata.GoogleScoreTag (string) should be used. For Apple, the GamesScoreMetadata.AppleContext (int) should be used. Be sure to read the xml comments to understand the requirements for this metadata.
```C#
GamesScoreSubmission result = await SubmitScoreAsync(string leaderBoardId, long score, GamesScoreMetadata metaData);
```