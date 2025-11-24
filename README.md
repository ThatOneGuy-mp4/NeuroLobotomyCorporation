# Neuro Corporation
## Face the Stream, Build the Content

This mod, when fully completed, will allow Neuro-sama to communicate with Lobotomy Corporation through a secondary program that sends and receives information from the game itself using web requests. This is done because Lobotomy Corporation runs on a framework that is too old to support the Neuro SDK by itself. 

## Installation

I would not recommend installing this mod currently because it is very almost finished and you'd have a better time if you waited a bit. However, if you would like to install it anyways: 
1. If you do not have the [Lobotomy Mod Manager](https://github.com/LobotomyBaseMod/LMM/releases) downloaded already, download and install it (I would recommend following [this guide](https://lobotomycorporationmodded.wiki.gg/wiki/Lobotomy_Mod_Manager) on the Lobotomy Corporation Modding Wiki to do this).
2. Download the source code from this repository and any required dependencies, and compile it. The compiled files will be in two different projects, the NeuroLobotomyCorporation project, which is the mod the game will run, and the NeuroLCConnector project, which is the secondary program which will connect to the game.
3. Create a folder called "ThatOneGuy_NeuroLobotomyCorporation" in the "BaseMods" folder as described in the Lobotomy Mod Manager guide from before. Copy the "NeuroLobotomyCorporation.dll" and "ModConfig.xml" files, and the "Info" folder, located in the compiled files for the NeuroLobotomyCorporation project, into this folder.
4. Inside the previously created folder, create another folder called "Connector". Then, copy the all the compiled files located in the NeuroLCConnector project into this folder.
5. Optional - install the following quality of life mods (others may work, but I am only testing with the following):
    - [Mindedness's Reticle Memory Leak Fix](https://www.nexusmods.com/lobotomycorporation/mods/586) (fixes a memory leak, obviously. you do not want to play with the memory leaks.)
    - [Pcix's Optimization Mod](https://www.nexusmods.com/lobotomycorporation/mods/57) (fixes another memory leak. again you do not want memory leaks. they're bad. and cause l*tency.)
    - [Is It Chris's Project Nugway](https://www.nexusmods.com/lobotomycorporation/mods/609) (allows free customization, and recustomization, of your agents. this is important because Neuro will need to refer to agents by name to command them and thus should be able to name and customize them to better remember them. also, it's fun.)
6. Open the Connector folder's config file. Set the "NeuroURI" key's value to whatever websocket server you're using. Alternatively, set the "NEURO_SDK_WS_URL" environment variable instead.
7. The game-to-server requests and server-to-game requests run on http://localhost:8080 and http://localhost:8081 respectively by default. Either ensure nothing else is running on those servers, or, change the "GameToServerURI" and "ServerToGameURI" keys in both the Mod and Connector config files. These must be identical in both config files, and both be http or https URIs.
8. Launch the game through the Lobotomy Mod Manager. This will automatically open the secondary program which will connect to whatever Neuro SDK program you're using.
Once the game has been opened, you can also start or kill the secondary program or the game-side SDK handler using the following console commands (press the "`" button on the keyboard to access):
    - "neurosdk start {connector/handler/all}"; starts the specified of the last three options, if they are not already started
    - "neurosdk kill {connector/handler/all}"; kills the specified of the last three options, if they are alive
    - "neurosdk restart {connector/handler/all}"; kills, then starts the specified of the last three options. uniquely, also attempts to give all actions back that the AI should have had, if the connector was restarted.

And voila, you're all set to let a human-like AI play Lobotomy Corporation! Just make sure the Head doesn't find out...

## Credits 
- Vedal, Alex, and all the other Contributors for the [Neuro SDK](https://github.com/VedalAI/neuro-sdk/tree/main)
- pandapanda135, for the [C# Implementation of the Neuro SDK](https://github.com/pandapanda135/CSharp-Neuro-SDK) that is being used in the secondary program
- The developers of the [Lobotomy Mod Manager](https://github.com/LobotomyBaseMod/LMM/releases) for developing the Lobotomy Mod Manager
- Pasu4, for making [Tony](https://github.com/Pasu4/neuro-api-tony) for testing
- Project Moon, for creating Lobotomy Corporation. Glory to Project Moon. 