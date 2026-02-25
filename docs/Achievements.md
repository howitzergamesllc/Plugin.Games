# Achievements

Google Play and Apple Game Center achievements are mostly abstracted to single classes with a few extras from each platform. 

- The first difference is how each platform makes progress towards unlocking the achievement. On android, a certain number of
steps need to be reached to unlock the achievement where with Apple, the achievement progress is a percentage and the achievement 
unlocks when the achievement reaches 100%. 

- The second difference is that Google provides method helpers to "reveal" and "unlock" achievemnents where Apple doesn't provide these helper methods. These helper methods have been abstracted for Apple to produce similar behavior but their is no garuntee they'll behave the same way.


## Google Achievement Progress

Google Play automatically caps progress once the current steps reaches the total steps and the achievement is unlocked. For instance,
if calling increment with 10 steps causes the CurrentSteps to increase past the TotalSteps then the CurrentSteps is set to TotalSteps.


## Apple Achievement Progress

For Apple, both progress calls "IncrementAchievementAsync" and "SetAchievementProgressAsync" have been abstracted to behave similar to Android in that calls to 110% progress either through incrementing or setting will cap the progress to the achievements maximum progress (100%) and unlock the achievement.


## Shared

- Achievements can be displayed from Google Play Games and Apple Game Center user interfaces by awaiting "ShowAchievementsAsync":
```C#
await CrossGames.Current.ShowAchievementsAsync();
```

- Available achievement information get be retrieved by awaiting "GetAchievementsAsync":
```C#
List<GamesAchievement> await CrossGames.Current.GetAchievementsAsync();
```

- Hidden achievements can be revealed to players by awaiting "RevealAchievementsAsync(achievementId)". On google this method is provided explicitly. With Apple, the achievements "hidden" property can't and shouldn't be manually manipulated but achievements are automatically revealed when any progress is made. The abstraction is a workaround by calling "GKAchievement.Report" and setting the progress to 0% to
report on the achievement and trigger its reveal.
```C#
await RevealAchievementAsync(achievementId)
```

- Await "IncrementAchievementAsync(achievementId, amount)" to progress an achievement toward unlocking it. On Google, the input is the steps you would like to increase current steps by. With Apple, this the percentage amount you would like to increase the achievement by.
Awaiting the call returns true if the achievement has been unlocked or with an GamesException if the process failed.
```C#
bool unlocked = await IncrementAchievementAsync(achievementid, amount)
```
Note: Inputing a negative value will through an "ArgumentOutOfRange" exception.

- Await "SetAchievementAsync(achievementId, amount)" to set the achievements total progress. On Google, this is the CurrentSteps property. With Apple, this sets the PercentComplete property. 
Awaiting the call returns true if the achievement has been unlocked or with an GamesException if the process failed.
```C#
bool unlocked = await SetAchievementAsync(achievementid, amount)
```
Note: Inputing a negative value will through an "ArgumentOutOfRange" exception.

- Unlocking achievements can be achieved by awaiting "UnlockAchievementAsync(achievementId)". On google this method reveals the achievment if hidden and unlocks it for the player automatically. With Apple, the abstraction awaits "SetAchievementProgressAsync"
and sets the progress to 100 percent to automatically unlock the achievement.
```C#
await UnlockAchievementAsync(achievementId)
```