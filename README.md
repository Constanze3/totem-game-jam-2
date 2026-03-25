
# Totem Game Jam #2
This is the project of the "winning team" for second iteration of the Totem Game Jam.

## Setup
- The project is intended to be developed on Windows
- Install [Git](https://git-scm.com/install/windows)
- Register a [GitHub](https://github.com) account if you do not have one
- Generate an SSH key if you do not have one
  - Run `ssh-keygen -t ed25519`
  - This will prompt you to specify where to save the key, make sure to keep this the default `C:\Users\<UserName>\.ssh\id_ed25519`
  - It will also ask for a password (you can just keep it empty to not have a password)
- On GitHub go to `Your Profile Icon` -> `Settings` -> `SSH and GPG keys` -> `New SSH key`
  - Give a name to the key e.g. `My Favorite Laptop` and fill in the content of the file ending in `.pub` that was generated in the previous step
  - KEEP THE FILE WITHOUT THE .pub ENDING SECRET!
- Clone the repository using `git clone git@github.com:Constanze3/totem-game-jam-2.git`
 - Optional: Install [GitHub Desktop](https://desktop.github.com/download) in case you would prefer using a simple visual interface over console commands for interacting with git.
 - Configure Git
  - Open up PowerShell (or Command Prompt)
  - Run `git config --global user.email <TheEmailYouRegisteredOnGitHubWith>` (sets global git email)
  - Run `git config --global user.name <YourGitHubUsername>` (sets global git username)
  - Run `git config push.autoSetupRemote true` (this makes sure that branches on the GitHub repository have the same name as your local branches)
- Install Unity 6.4 through [Unity Hub](https://docs.unity.com/en-us/hub/install-hub)
- Install [VS Code](https://code.visualstudio.com/)
- Install VS Code extensions:
  - C# Dev Kit (code competition, navigation, refactoring and other utilities)
  - Unity (Unity specific utilities)
  - CSharpier (automatic code formatting)
- Install .NET SDK 10.0
  - Press `Ctrl + Shift + P` and then type `Install New .NET SDK` to get to a page with a button to install
- In VS Code open the folder of the Git repository using `File` -> `Open Folder`
- In case it was not automatically installed, install the Unity extension for VS Code
  - In Unity go to `Window` -> `Package Management` -> `Package Manager`
  - Search for `Visual Studio Editor` and install it
- Configure VS Code
  - Press `Ctrl + Shift + P` and type `user settings json`, this will open up a file
  - Inside the outer `{}` add the following lines to the end of the file:
    ```
    "editor.formatOnSave": true,
    "[csharp]": {
        "editor.defaultFormatter": "/csharpier.csharpier-vscode"
    },
    "files.exclude": {
        // Hide Unity .meta files in the editor
        "**/*.meta": true,
    }
    ```

## Develop
- The `Game` folder can be opened using Unity Hub as a Unity project
- Code is found under `Game/Assets/Scripts`
- The `RawAssets` folder is intended for large raw assets like Blender files, it is set up to be tracked in [Git LFS](https://git-lfs.com/) (A default Git install on Windows should already have the Git extension for this installed)