# Plugin.Games

## Todo:
## Bugs:

## Project Directories
- Repo
```zsh
- cd "$HOME/path/to/your/projects/Plugin.Games"
```
- Source
```zsh
cd "$HOME/path/to/your/projects/Plugin.Games/src/Plugin.Games"
```
- Test
```zsh
cd "$HOME/path/to/your/projects/Plugin.Games/src/GamesTests/GamesMauiTest"
```

## Build
Restore
```zsh
dotnet restore "src"
```
Clean
```zsh
dotnet clean "src"
```
Build
```zsh
dotnet build "src"
```
Build Nuget
```zsh
dotnet build "src/Plugin.Games/Plugin.Games.csproj"
```
Build Nuget Release
```zsh
dotnet build "src/Plugin.Games/Plugin.Games.csproj" -c release
```
Build Test
```zsh
dotnet build "src/GamesTests/GamesMauiTest/GamesTest.csproj"
```
Build Test Release
```zsh
dotnet build "src/GamesTests/GamesMauiTest/GamesTest.csproj" -c release
```

## GitHub Publishing Commands
- Navigate to the directory
```zsh
cd "$HOME/Visual Studio Projects/Plugin.Games"
```
- Check the remote for changes.
```zsh
git fetch
git status
```
- Pull any pending commits if behind
```zsh
git pull
```
- Verify local repo is up to date against remote repo.
```zsh
git status
```
- Add Changes to the local repo.
```zsh
git add .
```
- Commit changes
```zsh
git commit -m "Initial commit with project files"
```
- Verify on main branch
```zsh
git branch -m main
 ```
- Push commit to the remote repo
```zsh
git push -u origin main
```

## Push package for private publishing.
```zsh
dotnet nuget push "$HOME/path/to/your/projects/Plugin.Games/Plugin.Games/src/Plugin.Games/bin/release/Plugin.Games.10.0.30.nupkg" --source "https://nuget.pkg.github.com/yourgithubusername/index.json" --api-key $GITHUBTOKEN
```
