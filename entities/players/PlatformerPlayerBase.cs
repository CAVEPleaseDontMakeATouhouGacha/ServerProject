using Godot;
using System;


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
	
	// General bitflags 
	
	// When this state flag is 1 the player is grounded, if 0 the player is airborne
	public const int PLAYER_STATEFLAG_GROUNDED = 1;
	// When this state flag is 1 the player is jumping
	public const int PLAYER_STATEFLAG_JUMPING = 2;
	// When this state flag is 1 the player is OmniDashing
	public const int PLAYER_STATEFLAG_DASHING = 4;
	// When this state flag is 1 the player is Grazing bullets, some melee attacks need this
	// or thewy will be underwhelming
	public const int PLAYER_STATEFLAG_GRAZING = 8;
	// When this state flag is 1 the player cancels bullets by touching them
	public const int PLAYER_STATEFLAG_CANCELING = 16;
	// When this flag is 1 the player is invincible
	public const int PLAYER_STATEFLAG_INVINCIBLE = 32;
	
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
	public Vector2 direction;
	
	
	//! Jump members
	
	// Checks if character can jump, reset upon touching the gound
	public int jumpsLeft;
	
	
	
	//! Dash members
	
	
	// The time in ticks of how much longer the dash will last
	// Reset upon touching the ground
	public int dashStaminaTime;
	
	//! DEPRECATED? Just makes dashing feel worse 
	// The resting time between dashes
	//int dashRestTime;
	
	// The time the player will retain the dash speed after dashing
	public int retainDashSpeedTime;
	
	// The amount of air dashes the player can do it the air
	public int airDashesLeft;
	
	
	// Shooting
	
	// The time the player has been charging to fire a shot, starts at chargeShotTimeLength
	// and goes down every tick until it reaches 0, once player releases the button goes back to chargeShotTimeLength
	public int chargeShotTime;
	
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
	public void buildKeystates() {
		
		/*
		
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
	//! Tileset Collision Functions
	//======================
	
	
	public int tileCollision_groundCeiling(float sensorOffsetY) {
		
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
			if (tile == null) {
				return Int32.MaxValue;
				GD.Print("No tile found");
			}
			Variant tileType = tile.GetCustomDataByLayerId(TILEDATA_TYPE);
			
			GD.Print("This type is " + tileType.GetType());
			GD.Print("Value " + tileType);
			Variant temp = 1;
			return Int32.MaxValue;
			
			//if (tileType == temp) {
				
				// There was collision
				//return tileSensor.Y;
				
			//}
			
			
			// Go to the next tile 
			tileSensor.Y = tileSensor.Y + 1;
		}
		
		
		return Int32.MaxValue;
		
		
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
	
	
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	
	
	
	
	
	
	
	
}
