-- hello.lua by anotak
-- this is the welcome message script that runs at first start

UI.LogLine("Hi,")
UI.LogLine("Welcome to the Lua scripting plugin for Doom Builder!")
UI.LogLine("")
UI.LogLine("To get this to do useful things, you need to load a script file.")
UI.LogLine("To do that, you can use the Lua menu up in the menu bar.")

UI.LogLine("There are several example scripts in the Lua subfolder in your Doom Builder folder.")
UI.LogLine("You can get at those by using the Scripts submenu in the Lua menu")
UI.LogLine("Or you can browse for them manually.")

UI.LogLine("")
UI.LogLine("For more information on the Lua API for writing your own scripts, check https://github.com/anotak/doombuilderx/wiki")

UI.DebugLogLine("This line only shows up if an error or a warning occurs, otherwise it's invisible.")