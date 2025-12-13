# Neuro Corporation
## Face the Stream, Build the Content

This mod allows Neuro-sama or Evil Neuro to communicate with Lobotomy Corporation through a secondary program that sends and receives information from the game itself using web requests. This is done because Lobotomy Corporation runs on a framework that is too old to support the Neuro SDK by itself. 
It also has some other small additions. For content.

## Installation
1. If you do not have the [Lobotomy Mod Manager](https://github.com/LobotomyBaseMod/LMM/releases) downloaded already, download and install it (I would recommend following [this guide](https://lobotomycorporationmodded.wiki.gg/wiki/Lobotomy_Mod_Manager) on the Lobotomy Corporation Modding Wiki to do this).
2. There are two methods of installation.
    - (Recommended) Download the latest release. Copy the folders inside into the "LobotomyCorp_Data" directory in the game's files, replacing files when asked. Skip to step 6 if you have done so.
    - Download the source code from this repository and any required dependencies, and compile it. The compiled files will be in two different projects, the NeuroLobotomyCorporation project, which is the mod the game will run, and the NeuroLCConnector project, which is the secondary program which will connect to the game.
3. Create a folder called "ThatOneGuy_NeuroLobotomyCorporation" in the "BaseMods" folder as described in the Lobotomy Mod Manager guide from before. Copy the "NeuroLobotomyCorporation.dll" and "ModConfig.xml" files, and the "Info", "Localize", "Resources", "StoryData", and "Equipment" folders, located in the compiled files for the NeuroLobotomyCorporation project, into this folder.
4. Inside the previously created folder, create another folder called "Connector". Then, copy the all the compiled files located in the NeuroLCConnector project into this folder.
5. Copy the "ExternalData" folder into the "LobotomyCorp_Data" directory the BaseMods folder resides in and replace files when asked. 
6. Optional - install the following quality of life mods (others may work, but I am only testing with the following):
    - [Mindedness's Reticle Memory Leak Fix](https://www.nexusmods.com/lobotomycorporation/mods/586) (fixes a memory leak, obviously. you do not want to play with the memory leaks.)
    - [Pcix's Optimization Mod](https://www.nexusmods.com/lobotomycorporation/mods/57) (fixes another memory leak. does come with the downside of employees popping in and out of existence when you look away from them though, so consider that before installing.)
    - [Is It Chris's Project Nugway](https://www.nexusmods.com/lobotomycorporation/mods/609) (allows free customization, and recustomization, of your agents. this is important because Neuro or Evil will need to refer to agents by name to command them and thus should be able to name and customize them to better remember them. also, it's fun.)
    - [TheNamesan's Music Loop Fix](https://www.nexusmods.com/lobotomycorporation/mods/616) (fixes a weird issue where the game doesn't loop music while paused. recommended if you don't like awkward silences.)
7. Open the Connector folder's config file. Set the "NeuroURI" key's value to whatever websocket server you're using. Alternatively, set the "NEURO_SDK_WS_URL" environment variable instead.
8. The game-to-server requests and server-to-game requests run on http://localhost:8080 and http://localhost:8081 respectively by default. Either ensure nothing else is running on those servers, or, change the "GameToServerURI" and "ServerToGameURI" keys in both the Mod and Connector config files. These must be identical in both config files, and both be http or https URIs.
9. Launch the game through the Lobotomy Mod Manager. This will automatically open the secondary program which will connect to whatever Neuro SDK program you're using.
Once the game has been opened, you can also start or kill the secondary program or the game-side SDK handler using the following console commands (press the "`" button on the keyboard to access):
    - "neurosdk start {connector/handler/all}"; starts the specified of the last three options, if they are not already started
    - "neurosdk kill {connector/handler/all}"; kills the specified of the last three options, if they are alive
    - "neurosdk restart {connector/handler/all}"; kills, then starts the specified of the last three options. uniquely, also attempts to give all actions back that the AI should have had, if the connector was restarted.

Please note that the integration is disabled during the tutorial and part of the first day. 
There are also some sections where ONLY the AI has control. If something goes wrong and the AI can't respond, use the console command "neurosdk regaincontrol" to...regain control.

And voila, you're all set to let a human-like AI play Lobotomy Corporation! Just make sure the Head doesn't find out...

## Credits 
- Vedal, Alex, and all the other Contributors for the [Neuro SDK](https://github.com/VedalAI/neuro-sdk/tree/main)
- pandapanda135, for the [C# Implementation of the Neuro SDK](https://github.com/pandapanda135/CSharp-Neuro-SDK) that is being used in the secondary program
- The developers of the [Lobotomy Mod Manager](https://github.com/LobotomyBaseMod/LMM/releases) for developing the Lobotomy Mod Manager
- Pasu4, for making [Tony](https://github.com/Pasu4/neuro-api-tony) for testing
- Project Moon, for creating the worst spaghetti coded garbage I've ever seen. Glory to Project Moon. 