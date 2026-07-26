```
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
▀▄▀▄                              Endless                                 ▄▀▄▀
░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
```

# BnS Buddy

Improve your Blade & Soul experience — mods, addons, dat/bin tools, plugins, and more.

## Disclaimer

- If the game is rendered unstartable, **you** are the only one responsible. By using this tool, you agree to those risks.
- You can always use the **original NCSoft launcher** to restore files and start the game.

## Credits

| Who | For |
|-----|-----|
| **Miyako** | Inspired by BnS-Ark Tool |
| **Endless aka Kogaru** | Recoded from scratch and improved |
| **ronny1982** | bnsdat tool |
| **LokiReborn** | Porting bnsdat to C#, AuthToken, VersionCheck, BXML4 |
| **Yevvie** | Icons, splash art, BnS Buddy forum images |
| **Airix** | Battleground fix |
| **GunerX** | UPK numbers for animation toggles & community |
| **Yeti** | Finding AMD ULPS interfering with B&S performance |
| **Megai2** | DX12 proxy mod for better B&S performance |
| **Fukki** | MUI Cache method for Garena client paths |
| **Pilao and Community** | PluginLoader and more |
| **Pilao** | New and improved dat handling |
| **LEaN and Vectoriel** | Signature bypass |
| **Tonic and Hora** | Animations and effects toggles |
| **Tomato** | Info for dat files |

## Requirements

- [.NET Framework 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework/net472) (full, not Client Profile)
- A brain :P

## Postponed features

- Game Installer (NA/EU/KR/JP/TW)
- Game Updater (NA/EU/KR/JP/TW)
- Game Installation Cleaner
- Remember Me for Purple Login
- DPS Meter Toggle

## Removed features

- QoL Toggles

## Updater

**Updater 3.0.0.5**

- Reworked: Self Extractor was always breaking due to permission issues

---

## Change log

### Update 6.0.6.0

- **Added:** Full Remote Database (browse, download, preview, upload, update, delete) for addons, mods, and splashes
- **Added:** Mod zip download auto-extract into game YourMods folder
- **Added:** 32/64-bit bin support (`datafile` / `localfile` and `datafile64` / `localfile64`)
- **Added:** In-app Bin Editor for opening, editing, and saving `.bin` tables
- **Added:** Bin dump/pack from extracted dats (dat context menu unpack/repack)
- **Added:** Bin Translator for localfile-style string translation
- **Added:** Public plugin catalog on bnsbuddy.com (no account required)
- **Reworked:** Plugin Manager to use HTTPS catalog/downloads instead of Mega API
- **Fixed Bug:** Plugin Manager Old Builds button only reloaded the current catalog
- **Fixed Bug:** Remote Database login/upload sessid unreliable via WebBrowser HTML wrapping
- **Fixed Bug:** Remote Database multipart upload not landing files on the server
- **Updated:** Plugin listing/install/uninstall/loader flow unchanged for users, faster and more reliable
- **Removed:** MegaApiClient dependency, credentials prompts, and Mega-related embeds

### Update 6.0.5.1

- **Added:** Support for Steam NA/EU
- **Added:** Block/Unblock buttons for NCGTMP (`bb64.dll`) in Extras (was preventing plugins from working)
- **Added:** Addon regex for search/replace
- **Fixed Bug:** Saving via addon creator would not save in proper path
- **Fixed Bug:** Mod Preview button was not working
- **Fixed Bug:** Check Bool Maintenance for Taiwan and Japanese were inverted
- **Fixed Bug:** Login via Purple would skip return of login state causing a lock
- **Fixed Bug:** Affinity would still be set even if the toggle was off
- **Fixed Bug:** Clean Memory button would stay grayed out even if game was running
- **Fixed Bug:** Auto Memory Cleaner would not do anything
- **Fixed Bug:** Start with Windows would do nothing
- **Updated:** STServer TW/JP/KR IDs for generating game session token
- **Updated:** More languages for conversion to locale for client

### Update 6.0.4.0

- **Added:** Missing pathing for Japan Neo
- **Added:** Missing pathing for Taiwan Neo
- **Added:** Selected language `local64.dat` pathing to dat editing (UNPACK FIRST)
- **Added:** Purple Login methods for Japan and Taiwan
- **Added:** Checks for WebView2 on your system
- **Added:** New system identification to protect your login credentials
- **Added:** Credentials for Mega API for using Plugin Manager
- **Added:** Busy overlay to hide frozen GUI when logging in
- **Added:** Setting to restore window when game dies
- **Fixed Bug:** Good email on normal login with multiple dots would prevent saving the profile
- **Fixed Bug:** Auto Login was not doing anything
- **Fixed Bug:** WebView2 on start would occasionally cause an error
- **Fixed Bug:** Reopening Purple Login after closing without selecting a method would crash
- **Fixed Bug:** Faster Boot/Loading toggle would be off when it should be true on start
- **Fixed Bug:** Selecting servers would cause issues on start and when changing
- **Fixed Bug:** Plugin Loader detection broken and prevented multi-clienting on supported clients
- **Fixed Bug:** No region was passed for multi-clienting preventing new sessions
- **Fixed Bug:** Context menu for license users in dat editing could not browse to extracted bin file
- **Fixed Bug:** Killing a single client session in session manager would not do anything
- **Fixed Bug:** Detecting regions would sometimes cause a crash on start
- **Fixed Bug:** Applying addons via Addons tab would crash when unpacking dat
- **Updated:** Update prompt specifies which client
- **Updated:** Language conversion to locale for client
- **Updated:** Private keys for signature generation on Neo dat files
- **Updated:** IP for Neo on both EU and NA (DDoS-related changes)
- **Updated:** Message for missing client but registry value still exists

### Update 6.0.3.0

- **Added:** All third-party logins
- **Fixed Bug:** Env vars to use WebView2 like Purple Login webbrowser
- **Fixed Bug:** Cancelling/retrying Purple Login

### Update 6.0.2.0

- **Added:** Purple Login Select window to choose login method
- **Added:** Phone login method
- **Added:** QR Code login method
- **Fixed Bug:** Kill Game would not change back to Play Game
- **Fixed Bug:** Faster Boot and Loading toggle would not detect properly on start

### Update 6.0.1.7

- **Added:** Remotely fetch addons/mods/splashes from database *[partial]*
- **Added:** Support for NEO on NA/EU region
- **Added:** NA/EU Purple Login as workaround for region locks
- **Reworked:** Login method to fetch web session token via web API
- **Reworked:** STS comms to grab STS session token for client (NA/EU only)
- **Fixed Bug:** Installing a non-existent mod folder via mod manager would cause an error
- **Fixed Bug:** Non-existent folder for INI editing would cause a crash
- **Fixed Bug:** Resizing the INI Editor window would break textbox size
- **Fixed Bug:** Left-clicking a dat in dat editor would cause an error
- **Fixed Bug:** Booting Buddy with ping checker disabled would cause an error
- **Fixed Bug:** Changing region with ping checker disabled would not show proper ping status
- **Fixed Bug:** Game live check would capture a closing game and force-close it when already closed
- **Fixed Bug:** Decompiling bin files — context menu would not work to compile
- **Fixed Bug:** Splash path check on boot would error on nulled language
- **Fixed Bug:** Checking local version was not updated for XML on UE4
- **Fixed Bug:** Fetching server info for launcher
- **Fixed Bug:** Maintenance check works again
- **Fixed Bug:** Resetting colors does not reflect changes on controls
- **Fixed Bug:** Pressing HELP in remove toggles not doing anything
- **Fixed Bug:** Detection of Purple client path on possible regions
- **Fixed Bug:** All Korean checks for LauncherInfo
- **Fixed Bug:** Changing client mode on Korean would not check client version
- **Fixed Bug:** Custom client arguments textbox would not work
- **Fixed Bug:** Custom client name textbox would not work
- **Fixed Bug:** Custom arguments did not include a space in prefix
- **Fixed Bug:** Faster Boot and Loading toggle was broken
- **Fixed Bug:** Last selected server did not support regions with more than one client
- **Fixed Bug:** Selecting even or odd cores in Affinity would do the opposite
- **Updated:** IP for NA/EU Live
- **Updated:** IP for NA/EU Neo
- **Updated:** Client starting args for Purple
- **Updated:** Error report logs to include BnS Buddy current version

### Update 6.0.1.5

- **Added:** Client INI Editor
- **Added:** Destination folder button in bnspatch addon manager (for dual Documents folders)
- **Removed:** Annoying popup when uninstalling mods (debug leftover)

### Update 6.0.1.4

- **Fixed Bug:** Wrong registry path for Russian client
- **Fixed Bug:** FastColoredTextBox would not render colors when scrolling
- **Fixed Bug:** Tray icon weird behaviour
- **Fixed Bug:** Weird behaviour when selecting an XML file from the dat editor

### Update 6.0.1.3

- **Fixed Bug:** Wrong registry path for Chinese client
- **Fixed Bug:** Refreshing mods would not show which mod is already installed
- **Fixed Bug:** Uninstalling a secondary sub-mod deletes the original mod folder

### Update 6.0.1.2

- **Added:** Home page with inbox

### Update 6.0.1.1

- **Fixed Bug:** Toggles for animations and effects were inverted

### Update 6.0.1.0

- **Added:** Class toggles in Extras are now working

### Update 6.0.0.9

- **Fixed Bug:** Changing language would not reset Splash path

### Update 6.0.0.8

- **Fixed Bug:** Registry conflicted with TW and KR (same registry path)

### Updater 3.0.0.2

- **Fixed Bug:** Missing dependencies

### Update 6.0.0.7

- **Added:** File version on unhandled error prompts
- **Fixed Bug:** Opening/saving XML from extracted bin folder would not find the path
- **Fixed Bug:** Extracting bin would not populate treeview
- **Fixed Bug:** Unpak/repak tools failed if PC username had a space

### Update 6.0.0.6

- **Added:** BnS Buddy rename detection to prevent crypto errors
- **Fixed Bug:** Toggling faster load and startup would error if one file was missing
- **Fixed Bug:** Included updater was not packed in a zip resulting in an error
- **Fixed Bug:** Game install detection would proceed even if game path did not exist

### Update 6.0.0.5

- **Fixed Bug:** Update button would stay grayed out when a new update is available

### Update 6.0.0.4

- **Reworked:** Installing mods now uses directory symlinks instead of file symlinks (overlap filename issue)

### Update 6.0.0.3

- **Fixed Bug:** Closing “add new profile” with text in textbox would freeze the app
- **Fixed Bug:** Deprecated profile check was not properly implemented

### Update 6.0.0.2

- **Fixed Bug:** Old deprecated profiles would cause issues when generating user list

### Update 6.0.0.1

- **Added:** Clicking the tray icon restores BnS Buddy UI
- **Fixed Bug:** New Instance window too small / text size mismatch
- **Fixed Bug:** Duplicate registry profiles would cause errors
- **Fixed Bug:** Directory exist check for install paths when registry is stale

### Updater 3.0.0.1

- **Fixed Bug:** Offline updater would set online version as debug text

### Update 6.0.0.0

- **Updated:** UI to MaterialSkin for a fresh modern look
- **Refactored:** Entire code & cleaned up dependencies
