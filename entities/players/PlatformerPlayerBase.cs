using Godot;
using System;

// Needed for funtion inlining
using System.Runtime.CompilerServices;

//[GlobalClass, Icon("res://Stats/StatsIcon.svg")]
public partial class PlatformerPlayerBase : Node2D
{
	
	
	
	//!TODO: Move this to a base class, 
	// It would be implemented like, Node2D-BasePlayer, and then a node class extends that and has the specific movement code
	
	
	// Deafault Godot variables start with Uppercase, mine start with Lowercase
	
	
	
	//!TODO: This WILL be removed from here
	//======================
	//! Tileset constants
	//======================
	
	public const int TILEDATA_TYPE = 0;
	
	public const int TILETYPE_EMPTY = 0;
	public const int TILETYPE_FULLSOLID = 1;
	public const int TILETYPE_DEATH = 2;
	
	// PLayer had no collision with a tile
	public const int TILECOLRES_NONE = 0;
	// Player touched ground tile
	public const int TILECOLRES_GROUND = 1;
	// Player touched ceiling tile
	public const int TILECOLRES_CEILING = 2;
	// Player touched left wall tile
	public const int TILECOLRES_LEFTWALL = 3;
	// Player touched right wall tile
	public const int TILECOLRES_RIGHTWALL = 4;
	
	
	
	
	//======================
	//! Constants
	//======================
	
	// Keystate Input bitflags
	public const int PLAYER_INPUTFLAG_UP = 1;
	public const int PLAYER_INPUTFLAG_DOWN = 2;
	public const int PLAYER_INPUTFLAG_LEFT = 4;
	public const int PLAYER_INPUTFLAG_RIGHT = 8;
	public const int PLAYER_INPUTFLAG_DASH = 16;
	public const int PLAYER_INPUTFLAG_JUMP = 32;
	public const int PLAYER_INPUTFLAG_MELEE = 64;
	public const int PLAYER_INPUTFLAG_SHOT = 128;
	
	// Keystate combinations
	public const int PLAYER_INPUTFLAG_UPDOWN = 3;
	public const int PLAYER_INPUTFLAG_LEFTRIGHT = 12;
	public const int PLAYER_INPUTFLAG_ALLMOVEMENT = 15;
	
	
	
	// Player States
	
	// Player is on the ground with full control
	public const int PLAYER_STATE_GROUNDED = 1;
	
	// Player is on the air with full control, but did not jump
	public const int PLAYER_STATE_AIRBORNE = 2;
	
	// Player is jumping through the air on their own volition
	public const int PLAYER_STATE_JUMPING = 3;
	
	//! TODO: Maybe add more specific states?
	// Player is dashing
	public const int PLAYER_STATE_DASHING = 4;
	public const int PLAYER_STATE_DASHINGGROUND = 5;
	public const int PLAYER_STATE_DASHINGAIR = 6;

	
	// Player is doing a melee attack
	
	public const int PLAYER_STATE_ATTACK = 7;
	public const int PLAYER_STATE_ATTACKGROUND = 8;
	public const int PLAYER_STATE_ATTACKAIR = 9;
	
	// The player is not controlling the character and has been knocked down
	public const int PLAYER_STATE_KNOCKEDDOWN = 10;
	
	// General bitflags 
	
	// When this state flag is 1 the player is grounded, if 0 the player is airborne
	public const int PLAYER_BITFLAG_GROUNDED = 1;
	// When this state flag is 1 the player is jumping
	public const int PLAYER_BITFLAG_JUMPING = 2;
	// When this state flag is 1 the player is OmniDashing
	public const int PLAYER_BITFLAG_DASHING = 4;
	// When this state flag is 1 the player is Grazing bullets, some melee attacks need this
	// or thewy will be underwhelming
	public const int PLAYER_BITFLAG_GRAZING = 8;
	// When this state flag is 1 the player cancels bullets by touching them
	public const int PLAYER_BITFLAG_CANCELING = 16;
	// When this flag is 1 the player is invincible
	public const int PLAYER_BITFLAG_INVINCIBLE = 32;
	
	
	
	
	
	//======================
	//! Members
	//======================
	
	//!Status
	
	// The state of the player
	public int state;
	
	// Bitfalgs of the player, if they are in the air, on groud, etc...
	public int flags;

	
	// Saves keystates of the player is pressing this tick
	// For now a byte, probably bigger later
	public int keystates;
	
	// Null-Canceling Movement keystates
	// This makes it so tha player can instantly change direction even if they are already
	// holding down a movement key
	public int lookKeystates;
	
	
	
	// The last valid vertical keystates
	// Valid keystates are the ones that make sense
	public int lastValidUpDown;
	
	
	// The last valid horizontal keystates
	// Valid keystates are the ones that make sense
	public int lastValidLeftRight;
	
	
		

	
	
	
	//! Movement
	
	// A temporary position Vector2 used for calculations
	public Vector2 position;
	
	// The previous position, used for continous collision detection
	public Vector2 prevPosition;
	
	
	// Forces outside of the player that move it around.
	// Allows for the player to be pushed around by various entities
	public Vector2 pushForce;
	
	
	// The velocity of the player
	public Vector2 velocity;
	
	// A member that records the speed of the player in one variable
	public float scalarSpeed;
	
	
	// Direction Y, -1 for up, +1 for down, 0 for no movement
	// Direction X, -1 for left, +1 for right, 0 for no movement
	public Vector2 moveDirection;
	
	// The direction the player is looking at,
	// Used as a base to spawn projectiles
	public Vector2 lookDirection;
	
	//! Jump members
	
	// Checks if character can jump, reset upon touching the gound
	public int jumpsLeft;
	
	// The force of the jump
	public float jumpForce;
	
	// Gravity of the character
	public float gravity;
	// Max velocity when falling
	public float terminalFallSpeed;
	
	
	
	// A timer that while it's biger than 0 makes it so 
	// the player cannot control the character
	// This is set by some melee moves so the player needs to take a risk and commit
	public int noControlTimer;
	
	
	//! Dash members
	
	
	// The time in ticks of how much longer the dash will last
	// Reset upon touching the ground
	public int dashStaminaTimer;
	
	
	// The time the player will retain the dash speed after dashing
	//public int retainDashSpeedTime;
	
	// The amount of air dashes the player can do it the air
	public int airDashesLeft;
	
	// Whether the speed of the dash is cardinal or ordinal
	public float currDashSpeed;
	
	
	// Shooting
	
	// The time the player has been charging to fire a shot, starts at chargeShotTimeLength
	// and goes down every tick until it reaches 0, once player releases the button goes back to chargeShotTimeLength
	public int chargeShotTimer;
	
	
	// Melee
	
	//! TODO: Maybe make this state flags?
	
	// If the player can do the bicycle kick
	public bool canDoBikeKick;

	// If the player can do the hundred kicks move
	public bool canDoHundredKicks;
	
	// Hitboxes
	
	// The timer that tells if the player is invincible and for how much
	public int invincibilityTimer;
	
	// If we are going to use a timer for this we won't need a bit flag
	public int grazingTimer;
	public int cancelingTimer;
	
	public float rectHeight = 65.0f;
	public float rectWidth = 32.0f;  
	
	public float circleRadius = 32.0f;
	public float circleRadiusSqrd = 32.0f * 32.0f;
	
	// Parents
	
	public Node2D level;
	public TileMap tilemap;
	
	// Childs
	
	public AnimatedSprite2D animatedSprite2D;
	public Camera2D camera2D;
	
	
	
	//======================
	//! Misc Functions
	//======================
	
	
	// Build keystates for replays and for input to be all in one place
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void buildKeystates() {
		
		/*
		// Not worth the effort with how restrictive C# is with bit operations
		byte[,] bitflagTable = {
			{0,2},
			{0,4},
			{0,8},
			{0,16},
			{0,32},
			{0,64},
			{0,128}
		};
		
		*/
		
		
		// Grab the raw keystates from the player
		this.keystates = Convert.ToByte(Input.IsActionPressed("INPUT_UP"));
		this.keystates = this.keystates | (Convert.ToByte(Input.IsActionPressed("INPUT_DOWN")) << 1);
		this.keystates = this.keystates | (Convert.ToByte(Input.IsActionPressed("INPUT_LEFT")) << 2);
		this.keystates = this.keystates | (Convert.ToByte(Input.IsActionPressed("INPUT_RIGHT")) << 3);
		this.keystates = this.keystates | (Convert.ToByte(Input.IsActionPressed("INPUT_DASH")) << 4);
		this.keystates = this.keystates | (Convert.ToByte(Input.IsActionPressed("INPUT_JUMP")) << 5);
		this.keystates = this.keystates | (Convert.ToByte(Input.IsActionPressed("INPUT_MELEE")) << 6);
		this.keystates = this.keystates | (Convert.ToByte(Input.IsActionPressed("INPUT_SHOT")) << 7);
		
		
		
		// Null cancelling movement
		
		this.lookKeystates = this.keystates;
		
		int rawUpDown = this.keystates & PLAYER_INPUTFLAG_UPDOWN;
		// If the raw keystates are invalid
		if (rawUpDown == PLAYER_INPUTFLAG_UPDOWN) {
			
			// Get the last valid vertical movement and flip it around
			this.lookKeystates = this.lookKeystates ^ this.lastValidUpDown;
			
		} else {
			
			// If it is valid we record them
			this.lastValidUpDown = rawUpDown;
			
		}
		
		
		int rawLeftRight = this.keystates & PLAYER_INPUTFLAG_LEFTRIGHT;
		if (rawLeftRight == PLAYER_INPUTFLAG_LEFTRIGHT) {
			this.lookKeystates = this.lookKeystates ^ this.lastValidLeftRight;
		} else {
			this.lastValidLeftRight = rawLeftRight;
		}
		
		
		
	}
	
	
	
	
	//======================
	//! General Movement Functions
	//======================
	
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void movementGeneral_readGlobalPosition(double delta) {
	
		// Grab Global Position
		this.position = this.GlobalPosition;
		// Record previous position
		this.prevPosition = this.position;
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void movementGeneral_writeGlobalPosition(double delta) {
	
		// Set back Global position
		// Write Global Position back into entity
		this.GlobalPosition = this.position;
	}
	
	
	
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void movementGeneral_calculateDirections(double delta) {
		
		// Calculate directions
		
		// Get horizontal input in bitflags
		int verticalInput = (this.lookKeystates & PLAYER_INPUTFLAG_UPDOWN);
		int horizontalInput = (this.lookKeystates & PLAYER_INPUTFLAG_LEFTRIGHT) >> 2;
		
		// 0 movement if nothing is being pressed
		// -1 if player is holding left
		// +1 if player is holding right
		// 0 if player is holding both
		float[] directionLookup = {0.0f, -1.0f, +1.0f, 0.0f};
		
		this.moveDirection.Y = directionLookup[verticalInput];
		this.moveDirection.X = directionLookup[horizontalInput];
		
		if (this.moveDirection.X != 0) {
			// Record the last valid moveDirection as lookDirection
			this.lookDirection.X = this.moveDirection.X;
		}
		
		
	}
	
	
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void movementGeneral_applyGravity(double delta) {
		
		// Apply gravity to Y velocity
		this.velocity.Y = this.velocity.Y + this.gravity;
		
		//!TODO: Maybe use a temporay variable next so we can break the fall speed limit
		
		// If we reached terminal velocity, keep it like that
		if (this.velocity.Y >= this.terminalFallSpeed) {
				
			this.velocity.Y = this.terminalFallSpeed;
		}
		
	}
	
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool movementGeneral_startJump(double delta) {
		
		// Handle jumping, yes for one tick the jump receives no gravity
		if ((this.keystates & PLAYER_INPUTFLAG_JUMP) == PLAYER_INPUTFLAG_JUMP) {
			
			// If there are jumps left
			if (this.jumpsLeft > 0) {
				
				
				// No longer grounded
				this.state = PLAYER_STATE_JUMPING;
				this.flags = flags & ~PLAYER_BITFLAG_GROUNDED;
				this.flags = flags | PLAYER_BITFLAG_JUMPING;
					
				this.jumpsLeft = this.jumpsLeft - 1;
					
				// Give full jump force and later check if player stopped holding down button
				this.velocity.Y = -this.jumpForce;
					
				// Slope jumps
				//velocity.X = velocity.X - (jumpForce * sin(Ground Angle));
				//velocity.Y = velocity.Y - (jumpForce * cos(Ground Angle));
				return true;
			} 
				
				
				
				
		}
		
		return false;
		
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void movementGeneral_stopJump(double delta) {
		
		if ((this.keystates & PLAYER_INPUTFLAG_JUMP) != PLAYER_INPUTFLAG_JUMP) {
				
			// If we are moving up with a jump force bigger than 4
			// Check if we are to close to the jump
			if (this.velocity.Y < -4) {
				// Allow the player to cancel hump force any time, 
				// until a certain point in the jump of course
					
				// Clamp it down to four
				//this.velocity.Y = -4;
				// Give gravity the wheel
				this.velocity.Y = 0;
			}
			
		}
		
	}
	
	
	
	
	
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void movementGeneral_updatePositionWithVelocity(double delta) {
		
		// Add velocity to position
		this.position.Y = this.position.Y + this.velocity.Y;
		this.position.X = this.position.X + this.velocity.X; 
		
		
	}
	
	
	
	//======================
	//! Tileset Collision Functions
	//======================
	
	//! UNIMPLEMENTED
	public TileData tileCollision_lineX(int lineStartPosX, int lineEndPosX,int linePosY) {
		
		
		// Convert pixel line into a tile line
		int tileLineStartPosX = lineStartPosX >> 5;
		int tileLineEndtPosX = lineEndPosX >> 5;
		int tileLinePosY = linePosY >> 5;
		
		// Check 3 tiles bellow
		
		Vector2I tileSensor = new Vector2I();
		tileSensor.Y = tileLinePosY;
		tileSensor.X = tileLineStartPosX;
		
		
		
		// Check the tiles from up to down while looking for walls
		tileSensor.X = tileSensor.X - 2;
		
		// Tiles to check
		int tileCheckNum = 5;
		
		// Do collison checks until we have done the full X movement
		do {
			
			while (tileCheckNum > 0) {
			
				TileData tile = tilemap.GetCellTileData(0, tileSensor);
			
				// If there is no tile there move to the next one
				if (tile == null) {
				
					tileCheckNum = tileCheckNum - 1;
					tileSensor.Y = tileSensor.Y + 1;
					continue;
				}
			
			
				int tileType = tile.Terrain;
			
			
				switch (tileType) {
				
					case TILETYPE_EMPTY: {
						
						// Do nothing, move to the next tile
						break;
					}
					
					case TILETYPE_FULLSOLID: {
						
						Vector2 tilePos = tilemap.MapToLocal(tileSensor);
						tilePos = this.ToGlobal(tilePos);
						
						tilePos.Y = tilePos.Y + 16;
						
						this.position.Y = tilePos.Y;
						
						return null;
						
						break;
					}
					
				
				
				}
			
			
				// Go to the next tile 
				tileSensor.Y = tileSensor.Y + 1;
				tileCheckNum = tileCheckNum - 1;
			}
			
			// Move horizontally to the next X column
			tileSensor.X = tileSensor.X + 1;
			
		} while (tileSensor.X >= tileLineEndtPosX);
		
		
		
		return null;
		
		
		
	}
	
	//! UNIMPLEMENTED
	public TileData tileCollision_lineY(int lineStartPosY, int lineEndPosY,int linePosX) {
		
		
		// Convert pixel line into a tile line
		int tileLineStartPosY = lineStartPosY >> 5;
		int tileLineEndtPosY = lineEndPosY >> 5;
		
		// Check 3 tiles bellow
		
		Vector2I tileSensor = new Vector2I();
		tileSensor.Y = 0;
		tileSensor.X = 0;
		
		// Convert to tile index
		tileSensor.Y = tileSensor.Y >> 5;
		tileSensor.X = tileSensor.X >> 5;  
		
		// Start by checking the tiles left to right
		tileSensor.X = tileSensor.X - 1;
		
		// Tiles to check
		int tileCheckNum = 3;
		
		while (tileCheckNum > 0) {
			
			TileData tile = tilemap.GetCellTileData(0, tileSensor);
			
			// If there is no tile there move to the next one
			if (tile == null) {
				
				tileCheckNum = tileCheckNum - 1;
				tileSensor.Y = tileSensor.Y + 1;
				continue;
			}
			
			
			int tileType = tile.Terrain;
			
			
			switch (tileType) {
				
				case TILETYPE_EMPTY: {
					
					// Do nothing, move to the next tile
					break;
				}
				
				case TILETYPE_FULLSOLID: {
					
					Vector2 tilePos = tilemap.MapToLocal(tileSensor);
					tilePos = this.ToGlobal(tilePos);
					
					tilePos.Y = tilePos.Y + 16;
					
					this.position.Y = tilePos.Y;
					
					return null;
					
					break;
				}
				
				
				
			}
			
			
			// Go to the next tile 
			tileSensor.Y = tileSensor.Y + 1;
			tileCheckNum = tileCheckNum - 1;
		}
		
		
		return null;
		
		
		
	}
	
	
	// A line check with the tile map
	// Returns the first found tile data on the line path
	// It uses the Bresenham line algorithm of course
	public TileData tileCollision_line(int lineStartPosX, int lineStartPosY, 
									   int lineEndPosX, int lineEndPosY) {
		
		
		// The steep value, since we cannot move sub tiles we build up a steep value until it's
		// big enough to go to the next tile
		
		
		bool steep = Math.Abs(lineEndPosY - lineStartPosY) > Math.Abs(lineEndPosX - lineStartPosX);
		
		
		int tempSwap;
		
		// If the line is more vertical than horizontal
		if (steep) {
			// We "rotate" the horizontal line we will draw,
			// so vertical line becomes the horizontal one
			tempSwap = lineStartPosX;
			lineStartPosX = lineStartPosY;
			lineStartPosY = tempSwap;
			
			tempSwap = lineEndPosX;
			lineEndPosX = lineEndPosY;
			lineEndPosY = tempSwap;
			
		}
		
		// If the start position is more to the right than the end position
		// We swap them so the difference from start to end always remains positive
		if (lineStartPosX > lineEndPosX) {
			
			tempSwap = lineStartPosX;
			lineStartPosX = lineEndPosX;
			lineEndPosX = tempSwap;
			
			tempSwap = lineStartPosY;
			lineStartPosY = lineEndPosY;
			lineEndPosY = tempSwap;
			
			
		}
		
		// Absolute difference between the start and end position on the X axis
		// The distance we have to traverse on the X axis until we complete the line detection
		int deltaX = lineEndPosX - lineStartPosX;
		// Absolute difference between the start and end position on the Y axis
		// Not as important as X axis since Y is worked as a secondary axis we change when error accumulator
		// becomes too big
		// The distance we have to traverse on the Y axis until we complete the line detection
		int deltaY = Math.Abs(lineEndPosY - lineEndPosY);
		
		// The step amount in the Y axis
		int stepY;
		
		// The error accumlulator
		int error = 0;
		
		// Default to going up one step on the Y tile position
		stepY = -1;
		// If the start pos Y is smaller than the end one that means the start position is above the end position
		if (lineStartPosY < lineEndPosY) {
			// So we need to go down not up
			stepY = +1;
		}
		
		
		// The tile position we are currently in
		int posY = lineStartPosY;
		int posX = lineStartPosX;
		
		//! TODO: Check before moving so we don't do tile collision detection
		// Do a while since there is the possibility we are not moving
		while (posX <= lineEndPosX) {
			
			TileData tile = null;
			
			// If the line is steep we need to get the Tile using inverted positions since we inverted them at the begining
			if (steep) {
				
				// Grab terrain info
				// Point(posY,posX);
				Vector2I tilePos = new Vector2I(posY, posX);
				tile = tilemap.GetCellTileData(0, tilePos);
				
				
			} else {
				
				// If not inverted just grab them normally
				
				// Grab terrain info 
				// Point(posX,posY);
				Vector2I tilePos = new Vector2I(posX, posY);
				tile = tilemap.GetCellTileData(0, tilePos);
				
				
			}
			
			
			// If tile not null or empty return it
			if (tile != null) {
					
				if(tile.Terrain != TILETYPE_EMPTY) {
					// We found a possible tile
					return tile;
				}
					
			}
			
			
			
			// Add the deltaY to the error accumulator
			error = error + deltaY;
			
			// Since we are working with only integers we need to multiply the error accumulator by 2
			// to get a makeshift precision.
			// If the error accumulator is now outside the deltaX trajectory
			if (2 * error >= deltaX) {
				
				// We need to move the posY into the correct trajectory
				// by going one step above, or bellow
				posY = posY + stepY;
				// Error has been dealt with, we can look for a new one
				error = error - deltaX;
			}
			
			// Move to the next horizontal tile
			posX = posX + 1;
		}
		
		// No tile that could be collided found
		return null;
		
	}
	
	
	public int tileCollision_groundCeiling(float sensorOffsetY) {
		
		
		
		
		float tileHeight = -16;
		
		// If we are checking for ground we move 16 pixels up, if we are using ceilings we go down
		if (sensorOffsetY < 0.0) {
			
			tileHeight = +16;
			
		}
		
		
		// Check 3 tiles bellow
		
		Vector2I tileSensor = new Vector2I();
		tileSensor.Y = (int)(this.position.Y + sensorOffsetY);
		tileSensor.X = (int)(this.position.X - 65.5f);
		
		// Convert to tile index
		tileSensor.Y = tileSensor.Y >> 5;
		tileSensor.X = tileSensor.X >> 5;  
		
		// Start by checking the tiles left to right
		tileSensor.X = tileSensor.X - 1;
		
		// Tiles to check
		int tileCheckNum = 3;
		
		while (tileCheckNum > 0) {
			
			TileData tile = tilemap.GetCellTileData(0, tileSensor);
			
			// If there is no tile there move to the next one
			if (tile == null) {
				
				tileCheckNum = tileCheckNum - 1;
				tileSensor.Y = tileSensor.Y + 1;
				continue;
			}
			
			
			int tileType = tile.Terrain;
			
			
			switch (tileType) {
				
				case TILETYPE_EMPTY: {
					
					// Do nothing, move to the next tile
					break;
				}
				
				case TILETYPE_FULLSOLID: {
					
					Vector2 tilePos = tilemap.MapToLocal(tileSensor);
					tilePos = this.ToGlobal(tilePos);
					
					tilePos.Y = tilePos.Y + tileHeight;
					
					this.position.Y = tilePos.Y;
					
					return TILECOLRES_GROUND;
					
					break;
				}
				
				
				
			}
			
			
			// Go to the next tile 
			tileSensor.Y = tileSensor.Y + 1;
			tileCheckNum = tileCheckNum - 1;
		}
		
		
		return TILECOLRES_NONE;
		
		
	}
	
	public void tileCollision_leftSide() {
		
		
		
		
	}
	
	public void tileCollision_rightSide() {
		
		
		
		
	}
	
	
	public void tileCollision_ceiling() {
		
		
		
		
	}
	
	
	
	
	public void initPlatformerPlayerBase() {
		
		
		// Center sprites
		
		// Set up variables
		// Probably always start airborne so no jumping on air business
		GD.Print("init player");
		this.animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		if (this.animatedSprite2D == null) {
			GD.Print("Sprite loading error"); 
		}
		//camera2D = GetNode<Camera2D>("Camera2D");
		
		//this.level = GetNode<Node2D>("../../Level");
		this.level = GetParent<Node2D>();
		this.level = this.level.GetParent<Node2D>();
		//this.level = GetNode<Node2D>("Level");
		GD.Print(this.level.Name);
		if (this.level == null) {
			GD.Print("Level loading error"); 
		}
		
		//this.tilemap = GetNode<TileMap>("../../BaseTileSet"); 
		this.tilemap = level.GetNode<TileMap>("BaseTileSet"); 
		GD.Print(this.tilemap.Name);
		if (this.tilemap == null) {
			GD.Print("Tilemap loading error"); 
		}
		
		//GD.Print("End player init");
		
	}
	
	
	
	


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	
	
	
	
	
	
	
	
}
