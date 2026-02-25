# Plugin.Games

## Todo:
## Bugs:

## Project Directories
- Repo
```cmd
- cd "C:\Users\path\to\your\projects\Plugin.Games"
```
- Nuget
```cmd
cd "C:\Users\path\to\your\projects\Plugin.Games\src\Plugin.Games"
```
- Test
```cmd
cd "C:\Users\path\to\your\projects\Plugin.Games\src\GamesTests\GamesMauiTest"
```

## Build
Restore
```cmd
dotnet restore "src"
```
Clean
```cmd
dotnet clean "src"
```
Build
```cmd
dotnet build "src"
```
Build Nuget
```cmd
dotnet build "src\Plugin.Games\Plugin.Games.csproj"
```
Build Nuget Release
```cmd
dotnet build "src\Plugin.Games\Plugin.Games.csproj" -c release
```
Build Test
```cmd
dotnet build "src\GamesTests\GamesMauiTest\GamesTest.csproj"
```
Build Test Release
```cmd
dotnet build "src\GamesTests\GamesMauiTest\GamesTest.csproj" -c release
```

## GitHub Publishing Commands
- Navigate to the directory
```cmd
cd "C:\Users\path\to\your\projects\Plugin.Games"
```
- Check the remote for changes.
```cmd
git fetch
git status
```
- Pull any pending commits if behind
```cmd
git pull
```
- Verify local repo is up to date against remote repo.
```cmd
git status
```
- Add Changes to the local repo.
```cmd
git add .
```
- Commit changes
```cmd
git commit -m "Initial commit with project files"
```
- Verify on main branch
```cmd
git branch -m main
 ```
- Push commit to the remote repo
```cmd
git push -u origin main
```

## Push package for private publishing.
```cmd
dotnet nuget push "C:\Users\path\to\your\projects\Plugin.Games\b\Release\Plugin.Games.10.0.20.nupkg" --source "https://nuget.pkg.github.com/yourgithubusername/index.json" --api-key $env:GITHUBTOKEN
```
