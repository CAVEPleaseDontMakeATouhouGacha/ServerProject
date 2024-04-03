using Godot;
using System;


// Needed for funtion inlining
using System.Runtime.CompilerServices;


public partial class Shot : Node2D
{
	
	// State for when the particle is alive
	public const int SHOT_STATE_DEAD = 0;
	
	public const int SHOT_STATE_ALIVE = 1;
	
	
	// If set to one the shot will penetrate multiple enemies
	public const int SHOT_FLAG_PENETRATE = 1;
	
	
	
	
	public const int SHOT_COLLISION_LAYER = 6;
	
	
	// Takko shot constants
	public const float cTakkoSmallShotWidth = 48.5f;
	public const float cTakkoSmallShotHeight = 16.5f;
	
	public const float cTakkoBigShotWidth = 48.5f;
	public const float cTakkoBigShotHeight = 48.5f;
	
	
	//======================
	//! Members
	//======================
	
	//!Status
	
	// The state of the shot
	public int state;
	
	// The flags of the shot 
	public int flags;
	
	
	// The ID of the Shot
	public int id;
	// The next free Shot in the Pool
	public int nextFree;
	
	
	//! Movement
	
	// A temporary position Vector2 used for calculations
	public Vector2 position;
	
	// Previous position of the Shot
	public Vector2 prevPosition;

	// The velocity of the Shot
	public Vector2 velocity;
	
	
	

	// A member that records the speed of the shot in one variable
	public float scalarSpeed;
	
	// The damage the shot deals to enemies
	public int damage;
	// The knockback of the Shot
	public float knockback;
	
	// Direction Y, -1 for up, +1 for down, 0 for no movement
	// Direction X, -1 for left, +1 for right, 0 for no movement
	public Vector2 direction;
	
	// A timer that when it reaches 0 kills the Shot
	public int timeoutTimer;
	
	
	//! Misc Members
	
	// Pool this Shot belongs to
	public EntityPooler parentPool;
	
	
	public AnimatedSprite2D animatedSprite2D;
	
	public PhysicsDirectSpaceState2D directSpaceState;
	
	public PhysicsShapeQueryParameters2D shapeCollisionQuery;
	
	
	
		
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Shot does not collide with terrain and stays on top of it
		//this.TopLevel = true;
		this.animatedSprite2D = this.GetNode<AnimatedSprite2D>("ShotAnimatedSprite2D");
		if (animatedSprite2D == null) {
			
			GD.Print("Unable to get animation node");
			
		}
		
		
		this.shapeCollisionQuery = new PhysicsShapeQueryParameters2D();
		this.directSpaceState = GetWorld2D().DirectSpaceState;

		this.SetPhysicsProcess(false);
		this.SetProcess(false);
		this.Hide();
	}
	
	
	public void spawnSmall(float spawnPosX, float spawnPosY, Vector2 dir) {
		
		this.position.Y = spawnPosY;
		this.position.X = spawnPosX;
		
		this.prevPosition.Y = spawnPosY;
		this.prevPosition.X = spawnPosX;
		
		
		this.GlobalPosition = this.position;

		
		this.scalarSpeed = 15;
		
		this.velocity.Y = 0;
		this.velocity.X = dir.X * scalarSpeed;
		
		// If moving to the left, flip sprite
		if (velocity.X < 0) {
			this.animatedSprite2D.FlipH = true;
		}
		
		
		// If not lower
		this.damage = 8;
		// Does no knockback
		this.knockback = 0.0f;
		
		// Set collision
		Rid rectRid = PhysicsServer2D.RectangleShapeCreate();
		Vector2 rectWidthHeight = new Vector2(cTakkoSmallShotWidth, cTakkoSmallShotHeight);
		PhysicsServer2D.ShapeSetData(rectRid, rectWidthHeight);
		this.shapeCollisionQuery.ShapeRid = rectRid;
		//!TODO: Maybe change this to false...
		this.shapeCollisionQuery.CollideWithBodies = true;
		this.shapeCollisionQuery.CollisionMask = SHOT_COLLISION_LAYER;
		
		
		// Time out after 3 seconds
		// So it can also be used for sniping
		this.timeoutTimer = 360;
		
		
		this.animatedSprite2D.Animation = "SmallShot";
		//this.animatedSprite2D.Play("SmallShot");
		
	}
	
	public void spawnBig(float spawnPosX, float spawnPosY, Vector2 dir) {
		
		this.position.Y = spawnPosY;
		this.position.X = spawnPosX;
		
		this.prevPosition.Y = spawnPosY;
		this.prevPosition.X = spawnPosX;
		
		
		this.GlobalPosition = this.position;
		
		this.scalarSpeed = 15;
		
		this.velocity.Y = 0;
		this.velocity.X = dir.X * scalarSpeed;
		
		// If moving to the left, flip sprite
		if (velocity.X < 0) {
			this.animatedSprite2D.FlipH = true;
		}
		
		this.damage = 50;
		
		
		// Set collision shape
		Rid rectRid = PhysicsServer2D.RectangleShapeCreate();
		Vector2 rectWidthHeight = new Vector2(cTakkoBigShotWidth, cTakkoBigShotHeight);
		PhysicsServer2D.ShapeSetData(rectRid, rectWidthHeight);
		this.shapeCollisionQuery.ShapeRid = rectRid;
		//!TODO: Maybe change this to false...
		this.shapeCollisionQuery.CollideWithBodies = true;
		this.shapeCollisionQuery.CollisionMask = SHOT_COLLISION_LAYER;
		
		
		
		// Time out after 3 seconds, allow Shot to be used for sniping
		// 3 seconds is 360
		this.timeoutTimer = 360;
		
		
		this.animatedSprite2D.Animation = "BigShot";
		//this.animatedSprite2D.Play("BigShot");
		
	}
	
	
	
	
	
	public override void _PhysicsProcess(double delta) {
		
		// Get position
		this.position = this.GlobalPosition;
		// Record previous position
		this.prevPosition = this.position;
		
		// Update position
		this.position.Y = this.position.Y + this.velocity.Y;
		this.position.X = this.position.X + this.velocity.X; 
		
		// Update timeout
		this.timeoutTimer = this.timeoutTimer - 1;
		
		if (this.timeoutTimer <= 0) {
			
			// Keep Shot in memory for later reusing, just stop processing it
			this.parentPool.freeShot(this.id);
			//QueueFree();
			
		}
		
		
		Godot.Collections.Array<Godot.Collections.Dictionary> collisionResult = this.directSpaceState.IntersectShape(this.shapeCollisionQuery, 1);
		if (collisionResult != null) {
						
			// There was a collision
						
		}
		
		
		
		this.GlobalPosition = this.position;
	}
		
	
}
