AI Sprite Packer 101

Launch the AISpritePacker.exe

The app should start with the following default values:
* Canvas Width: 512
* Canvas Height: 512
* Margin: 10

To Import sprites you have two options:

Option A:
	# Select File -> Import
	# Browse to your sprites
	# Select the sprites you wish to import
	# Select Open

Option B:
	# Browse to your sprites from the windows explorer
	# Drag and drop the sprites to the canvas

Sprites should appear in the canvas with a margin between them

To export your spritesheet and xml:

Option A:
	# Select File -> Export
	# Select a folder to save to

Option B:
	# Push the Export Button!
	# Select a folder to save to

Exporting will output a ".png" and ".xml" to the folder you selected

Example:
	spriteSheet1.png
	spriteSheet1.xml

XML will be exported in the following format:

Root Node: Atlas
Attributes:
	*Width - Width of the SpriteSheet
	*Height - Height of the SpriteSheet
	*Sheet - Filepath to the SpriteSheet

Filepath may need to be changed depending on your project. Defaults to the output folder.

Child Node: Sprite
Attributes:
	x0 - Refers to the X position of the sprite
	y0 - Refers to the Y position of the sprite
	width - The width of the sprite
	height - The height of the sprite

The (x,y) coordinate refers to the top-left of the sprite
