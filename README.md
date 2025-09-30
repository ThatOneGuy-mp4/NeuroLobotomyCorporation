## Neuro Corporation
# Face the Stream, Build the Content

This mod, when fully completed, will allow Neuro-sama to communicate with Lobotomy Corporation through a secondary program that sends and receives information from the game itself using web requests. This is done because Lobotomy Corporation runs on a framework that is too old to support the Neuro SDK by itself. 

# INSTALLATION

I would not recommend installing this mod currently because it is very much incomplete. However, if you would like to install it anyways: 
1. If you do not have the [Lobotomy Mod Manager](https://github.com/LobotomyBaseMod/LMM/releases) downloaded already, download and install it (I would recommend following [this guide](https://lobotomycorporationmodded.wiki.gg/wiki/Lobotomy_Mod_Manager) on the Lobotomy Corporation Modding Wiki to do this).
2. Download the source code from this repository and any required dependencies, and compile it. The compiled files will be in two different projects, the NeuroLobotomyCorporation project, which is the mod the game will run, and the NeuroLCConnector project, which is the secondary program which will connect to the game.
3. Create a folder called "ThatOneGuy_NeuroLobotomyCorporation" in the "BaseMods" folder as described in the Lobotomy Mod Manager guide from before. Copy the "NeuroLobotomyCorporation.dll" file and "Info" folder, located in the compiled files for the NeuroLobotomyCorporation project, into this folder.
4. Inside the previously created folder, create another folder called "Connector". Then, copy the all the compiled files located in the NeuroLCConnector project into this folder.
5. Optional - install the following quality of life mods (others may work, but I am only testing with the following):
    -[Mindedness's Reticle Memory Leak Fix](https://www.nexusmods.com/lobotomycorporation/mods/586) (fixes a memory leak, obviously. you do not want to play with the memory leaks.)
    -[Pcix's Optimization Mod](https://www.nexusmods.com/lobotomycorporation/mods/57) (fixes another memory leak. again you do not want memory leaks. they're bad. and cause l*tency.)
    -[Is It Chris's Project Nugway](https://www.nexusmods.com/lobotomycorporation/mods/609) (allows free customization, and recustomization, of your agents. this is important because Neuro will need to refer to agents by name to command them and thus should be able to name and customize them to better remember them. also, it's fun.)
6. Set the "NEURO_SDK_WS_URL" environment variable to whatever you're using for testing. In the future this will be settable in a config file, but currently, the environment variable is the only way to do so. 
7. The game-to-server requests and server-to-game requests run on http://localhost:8080 and http://localhost:8081 respectively, so ensure nothing else is running on those. Again these will be settable in a config file at some point.
8. Launch the game through the Lobotomy Mod Manager. This will automatically open the secondary program which will connect to whatever Neuro SDK program you're using. 
    -Note: Due to some weirdness in how I coded it, it will currently *not* automatically open if the title screen is the post-ending title screen. I plan on fixing this in the future, but for now, you can just manually launch the program using the NeuroLCConnector.exe file from earlier as long as you do so on the title screen before any requests are made. Alternatively, you can use [Hawkbar's Save Data Profiles](https://www.nexusmods.com/lobotomycorporation/mods/703) mod to start a new save file without getting rid of your old one, to get the pre-ending title screen back.

And voila, you're all set to let a human-like AI play Lobotomy Corporation! Just make sure the Head doesn't find out...

# CREDITS 
Vedal, Alex, and all the other Contributors for the [Neuro SDK](https://github.com/VedalAI/neuro-sdk/tree/main)
pandapanda135, for the [C# Implementation of the Neuro SDK](https://github.com/pandapanda135/CSharp-Neuro-SDK) that is being used in the secondary program
The developers of the [Lobotomy Mod Manager](https://github.com/LobotomyBaseMod/LMM/releases) for developing the Lobotomy Mod Manager
Pasu4, for making [Tony](https://github.com/Pasu4/neuro-api-tony) for testing
Project Moon, for creating Lobotomy Corporation. Glory to Project Moon. 