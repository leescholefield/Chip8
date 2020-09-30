# Chip8

A Chip-8 interpreter written for C# WPF

Features:
* Select from a number of pre-loaded games or load games from an external source.
* Keyboard input.
* Fully configurable keybindings and pixel colors.

Todo:
* Convert it to MonoGame. At the moment the game rendering is achieved via the CompositionTarget.Rendering event and, although this works, it was not intended to be used to render game graphics (no matter how simple). Therefore the framerate is pretty slow in some games. 
* Sound.

<img src="https://github.com/leescholefield/Chip8/blob/master/Screenshots/pong.png" width="400" align="left">
<img src="https://github.com/leescholefield/Chip8/blob/master/Screenshots/space_invaders.png" width="400" align="left">
<img src="https://github.com/leescholefield/Chip8/blob/master/Screenshots/space_invaders_title.png" width="400" align="left">
<img src="https://github.com/leescholefield/Chip8/blob/master/Screenshots/opcode_test.png" width="400" align="left">
