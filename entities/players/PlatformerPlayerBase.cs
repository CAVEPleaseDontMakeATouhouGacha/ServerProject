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


	public const int TILEMETA_VERTTILES_PLAYERHEIGHT = 4;
	public const int TILEMETA_HORITILES_PLAYERWIDTH = 2;
	
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
	
	
	
	public const int TILESENSOR_VERT_LEFT = 0;
	public const int TILESENSOR_VERT_MID = 1;
	public const int TILESENSOR_VERT_RIGHT = 2;
	
	//======================
	//! Constants
	//======================
	
	//! Keystate Input bitflags
	public const int PLAYER_INPUTFLAG_UP = 1;
	public const int PLAYER_INPUTFLAG_DOWN = 2;
	public const int PLAYER_INPUTFLAG_LEFT = 4;
	public const int PLAYER_INPUTFLAG_RIGHT = 8;
	public const int PLAYER_INPUTFLAG_DASH = 16;
	public const int PLAYER_INPUTFLAG_JUMP = 32;
	public const int PLAYER_INPUTFLAG_MELEE = 64;
	public const int PLAYER_INPUTFLAG_SHOOT = 128;
	
	//! Keystate combinations
	
	// When the player is pressing up and down at the same time
	public const int PLAYER_INPUTFLAG_UPDOWN = 3;
	// When the player is pressing left and right at the same time
	public const int PLAYER_INPUTFLAG_LEFTRIGHT = 12;
	// When the player is pressing up, down, left, right at the same time
	public const int PLAYER_INPUTFLAG_ALLMOVEMENT = 15;
	
	// Inputs related to actions other than movement, like dashing, shooting, melee and jumping
	public const int PLAYER_INPUTFLAG_ACTIONS = 240;
	
	
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
	// Also allows to see if the player just pressed a key instead of holding it
	public int lookKeystates;
	
	
	
	// The last valid vertical keystates
	// Valid keystates are the ones that make sense
	public int lastValidUpDown;
	
	
	// The last valid horizontal keystates
	// Valid keystates are the ones that make sense
	public int lastValidLeftRight;
	
	// A collection of keystates to know if the user should repress the jump key or other inputs
	public int lastValidPressAction;

	// Used to force the player to repress an action, this will however 
	//hold the input flag until it is used by the update code
	public int actionRepressForcer; 
	
	
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
	
	// The timer to be sure if the player is inside the Coyote Time window or not
	// Default coyote time is 0.133 seconds aka 16 ticks
	public int coyoteTimer;
	public const int cDefaultCoyoteTime = 16;
	
	
	public const float cDefaultRectHeight = 63.5f;
	public const float cDefaultRectWidth = 31.5f;
	public const float cDefaultCircleRadius = 31.5f;
	
	public float rectHeight = cDefaultRectHeight;
	public float rectWidth = cDefaultRectWidth;  
	
	public float circleRadius = cDefaultCircleRadius;
	public float circleRadiusSqrd = cDefaultCircleRadius * cDefaultCircleRadius;
	
	
	public int sensorUsedForVertCollision;
	
	
	// Parents
	
	public Node2D level;
	public TileMap tilemap;
	
	// Spawns various entities
	public EntityPooler entityPooler;
	
	// Spawns Shots
	
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
		this.keystates = this.keystates | (Convert.ToByte(Input.IsActionPressed("INPUT_SHOOT")) << 7);
		
		
		
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
		
		
		
		// Just pressed Action key
		/*
		int pressingActions = this.keystates & PLAYER_INPUTFLAG_ACTIONS;
		//this.lookKeystates = this.lookKeystates ^ pressingActions; 
		if (pressingActions == this.lastValidPressAction) {
			
			this.lookKeystates = this.lookKeystates ^ this.lastValidPressAction;
			
		} else {
			
			this.lastValidPressAction = pressingActions;
			
		}
		*/
		
		
		// Force repressing key but still hold it
		//! OPTIMIZE: There has to be a better way to do this... and no I'm not talking about clearing the bitflag
		
		int repressActions = this.keystates & PLAYER_INPUTFLAG_ACTIONS;
		
		// If player has stopped holding jump
		if ((this.keystates & PLAYER_INPUTFLAG_JUMP) != PLAYER_INPUTFLAG_JUMP) {
			// Stop jump repress forcer
			this.actionRepressForcer = this.actionRepressForcer & ~PLAYER_INPUTFLAG_JUMP;
		}
		
		// If player has stopped holding Dash
		if ((this.keystates & PLAYER_INPUTFLAG_DASH) != PLAYER_INPUTFLAG_DASH) {
			// Stop Dash repress forcer
			this.actionRepressForcer = this.actionRepressForcer & ~PLAYER_INPUTFLAG_DASH;
		}
		
		
		// Force the player to repress action keys if the forcer is 1 in one of the input flags
		this.lookKeystates = this.lookKeystates ^ this.actionRepressForcer;
		
		
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool inputflag_holding(int inputflag) {
		return ((this.keystates & inputflag) == inputflag);
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool inputflag_pressing(int inputflag) {
		return ((this.lookKeystates & inputflag) == inputflag);
	}
	
	
	//======================
	//! General Movement Functions
	//======================
	
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void movementGeneral_readGlobalPositionAndRecordPrevious(double delta) {
	
		// Grab Global Position
		this.position = this.GlobalPosition;
		// Record previous position before updating it
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
		
		// Get horizontal input in bitflags from the lookKeystates so we can cancel null movement
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
	public bool movementGeneral_startJumpHold(double delta) {
		
		// Check raw keystates so player can hold the jump button to instantly jump
		// when they touch the ground
		if ((this.keystates & PLAYER_INPUTFLAG_JUMP) == PLAYER_INPUTFLAG_JUMP) {
			
			// If there are jumps left
			return (this.jumpsLeft > 0);
		}	
	
		return false;
		
	}
	
	
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool movementGeneral_startJumpRepress(double delta) {
		
		// Only jump if player has repressed the keys
		if ((this.lookKeystates & PLAYER_INPUTFLAG_JUMP) == PLAYER_INPUTFLAG_JUMP) {
			
			// If there are jumps left
			if (this.jumpsLeft > 0) {
					
				// Force player to repress jump
				this.actionRepressForcer = this.actionRepressForcer | PLAYER_INPUTFLAG_JUMP;
				return true;
			
			}

			
		} 
		return false;
		
	}
	
	
	
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void movementGeneral_setJump(double delta) {
	

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
	public bool movementGeneral_updateCoyoteTimer(double delta) {
		
		// Decrease Coyote Timer by one
		this.coyoteTimer = this.coyoteTimer - 1;
		
		
		// If we are in the Coyote Time Window
		// We do this by checking if the timer hasn't reached 0
		return (this.coyoteTimer > 0);
		
		
		
	}
	
	
	/*
	// 8-way movement, 4 Cardinal, 4 Ordinal
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void movementGeneral_8WayMovement(double delta) {
		
			
		float isCardinal = this.moveDirection.Y * this.moveDirection.X;
		this.currDashSpeed = 0.0f;
			
		if (isCardinal != 0) {
				
			// Ordinal Dash
			this.currDashSpeed = cOrdinalDashSpeed;
				
		} else {
				
			// Cardinal Dash
			this.currDashSpeed = cCardinalDashSpeed;
					
		}
			
		// Apply 8 directional movement
		this.velocity.Y = this.moveDirection.Y * this.currDashSpeed;
		this.velocity.X = this.moveDirection.X * this.currDashSpeed;
			

	}
	*/
	
	
	
	
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
	public TileCollisionResponse tileCollision_line(int lineStartPosX, int lineStartPosY, 
									   				int lineEndPosX, int lineEndPosY) {
		
		
		
		TileCollisionResponse tileResponse = new TileCollisionResponse();
		
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
		int deltaY = Math.Abs(lineEndPosY - lineStartPosY);
		
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
				tile = this.tilemap.GetCellTileData(0, tilePos);
				
				// Record position of where we found the tile reversed
				tileResponse.tileTopPosX = posY;
				tileResponse.tileTopPosY = posX;
				
			} else {
				
				// If not inverted just grab them normally
				
				// Grab terrain info 
				// Point(posX,posY);
				Vector2I tilePos = new Vector2I(posX, posY);
				tile = this.tilemap.GetCellTileData(0, tilePos);
				
				// Record position of where we found the tile
				tileResponse.tileTopPosX = posX;
				tileResponse.tileTopPosY = posY;
				
				
			}
			
			
			// If tile not null or empty return it
			if (tile != null) {
					
				if(tile.Terrain != TILETYPE_EMPTY) {
					// We found a possible tile
					tileResponse.tileData = tile;
					return tileResponse;
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
	
	
	
	
	
	
	// Do separated vertical and horizontal look around so we don't need nested loops
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool tileCollision_checkForVericalTiles(TileCollisionResponse lookAround,
												   int numberOfTilesToLook,
												   int lookDirection) {
		
		int currentLookTile = lookDirection;
		
		do {
						
			Vector2I tileLookVec = new Vector2I(lookAround.tileTopPosX, lookAround.tileTopPosY + currentLookTile);
			TileData lookTile = this.tilemap.GetCellTileData(0, tileLookVec);
					
			// Check if it's part of a wall
			if (lookTile != null) {
									
				if(lookTile.Terrain != TILETYPE_EMPTY) {
									
					// Then clostest Y full solid is part of a wall.
					// Ignore it
					return true;
				}
									
			}
			
			currentLookTile = currentLookTile + lookDirection;
			numberOfTilesToLook = numberOfTilesToLook - 1;
				
		} while (numberOfTilesToLook > 0);
	
		return false;
	
	}
	
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool tileCollision_checkForHorizontalTiles(TileCollisionResponse lookAround,
												   	  int numberOfTilesToLook,
												   	  int lookDirection) {
		
		int currentLookTile = lookDirection;
		
		do {
						
			Vector2I tileLookVec = new Vector2I(lookAround.tileTopPosX + currentLookTile, lookAround.tileTopPosY);
			TileData lookTile = this.tilemap.GetCellTileData(0, tileLookVec);
					
			// Check if it's part of a wall
			if (lookTile != null) {
									
				if(lookTile.Terrain != TILETYPE_EMPTY) {
									
					return true;
				}
									
			}
			
			currentLookTile = currentLookTile + lookDirection;
			numberOfTilesToLook = numberOfTilesToLook - 1;
				
		} while (numberOfTilesToLook > 0);
	
		return false;
	
	}
	
	
	// Snap to a tile if the player just missed it
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool tileCollision_verticalSnap(TileCollisionResponse tileSnap) {
		
		// Only snap to tile if we are close enough to it and we are using one of the edge sensors
		if (this.sensorUsedForVertCollision != TILESENSOR_VERT_MID) {
			
			float tilePosY = (float)(tileSnap.tileTopPosY << 5);
			float distance = tilePosY - this.position.Y;
							
							
			// If the distance between player center and tile is bigger than 64(two tiles)
			// don't snapp
			// Or use 32 + 24 = 56 so then player is snapped to tiles if they are close enough
			// Or 48
			return (distance > 48.0f);
		}
		
		return true;
	}
	
	
	
	
	
	//! Init player
	
	
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
		
		// Get Particle Pool
		this.entityPooler = level.GetNode<EntityPooler>("EntityPooler");
		
		
		
	}
	
	
	
	


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	
	
	
	
	
	
	
	
}
