# RF.SaveProfileManager
 A Rhythm Festival mod to manage save files and mod save data.
  
  <a href="https://shorturl.at/f8vW7"> <img src="Resources/InstallButton.png" alt="One-click Install using the Taiko Mod Manager" width="256"/> </a>

# Usage
 When first installed and ran, it will create a SaveProfileDefinitions.json file in the SaveProfileDefinitionsPath entry set in the config file.\
 This is set to BepInEx\data\SaveProfileManager\SaveProfileDefinitions.json by default.\

The file will look like this as an example
```json
[
  {
    "ProfileName": "Default",
    "ProfileColor": "#FFFFFF",
    "ModsEnabledByDefault": true,
    "Mods": [
      {
        "ModGuid": "com.DB.RF.DisplayCurrentCrownStatus",
        "Enabled": false
      }
    ]
  },
  {
    "ProfileName": "Drum",
    "ProfileColor": "#000000",
    "ModsEnabledByDefault": true,
    "Mods": [
      {
        "ModGuid": "com.DB.RF.SwapSongLanguages",
        "Enabled": false
      }
    ]
  }
]
```
The "Default" Profile will use your official save file based on your steam account ID, so it's recommended to keep that. Any other profiles can just be added on, with no realistic limit.\
In-game, you can switch between these by hitting left/right on the title screen. You'll see the current profile at the top right.\
\
This mod is capable of loading, unloading, and reloading compatible mods, allowing you to set whether specific mods are enabled or disabled on specific profiles. You can set this as shown in the json example above.\
\
Each profile also has its own config file that you can modify as well.\
The location of these will be defined in the SaveProfileManager.cfg BepInEx config file, under the ModDataFolderPath entry.\
From that location, select the mod folder you want to modify, then the profile folder, and the config file will be there. You will need to launch the game and the profile first in order to generate these config files. 


  
# Requirements
 Visual Studio 2022 or newer\
 Taiko no Tatsujin: Rhythm Festival
 
 
# Build
 Install [BepInEx 6.0.0-pre.2](https://github.com/BepInEx/BepInEx/releases/tag/v6.0.0-pre.2) into your Rhythm Festival directory and launch the game.\
 This will generate all the dummy dlls in the interop folder that will be used as references.\
 Make sure you install the Unity.IL2CPP-win-x64 version.\
 Newer versions of BepInEx could have breaking API changes until the first stable v6 release, so those are not recommended at this time.
 
 Attempt to build the project, or copy the .csproj.user file from the Resources file to the same directory as the .csproj file.\
 Edit the .csproj.user file and place your Rhythm Festival file location in the "GameDir" variable.

Add BepInEx as a nuget package source (https://nuget.bepinex.dev/v3/index.json)

# Links 
 [My Other Rhythm Festival Mods](https://docs.google.com/spreadsheets/d/1xY_WANKpkE-bKQwPG4UApcrJUG5trrNrbycJQSOia0c)\
 [My Taiko Discord Server](https://discord.gg/6Bjf2xP)
 
