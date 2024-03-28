using Godot;
using System;


// Needed for funtion inlining
using System.Runtime.CompilerServices;


public partial class Bullet : Node2D
{
	
		
	// State for when the Bullet is free
	public const int BULLET_STATE_CANCELED = 0;
	// Bullet is in Delay Cloud state, no collision
	public const int BULLET_STATE_DELAY = 1;
	// Bullet is ariborne and can collide with player
	public const int BULLET_STATE_AIRBORNE = 2;
	// Bullet is in decay state
	public const int BULLET_STATE_DECAY = 3;
	
	
	// Bullet can be canceled by grazing, if 0 cannot be canceled
	public const int BULLET_FLAG_CANCELABLE = 1;
	// Bullet can be grazed while dashing, if 0 the player will collide with it while dashing
	public const int BULLET_FLAG_GRAZABLE = 2;
	
	
	//======================
	//! Members
	//======================
	
	//!Status
	
	// The state of the shot
	public int state;
	
	public int flags;
	
	
	
	//! Movement
	
	// A temporary position Vector2 used for calculations
	public Vector2 position;
	
	// Previous position of the Bullet
	public Vector2 prevPosition;

	// The velocity of the Bullet
	public Vector2 velocity;
	
	// A member that records the speed of the Bullet in one variable
	public float scalarSpeed;
	
	// Movement angle of the Bullet
	public float movementAngle;
	
	
	// Direction Y, -1 for up, +1 for down, 0 for no movement
	// Direction X, -1 for left, +1 for right, 0 for no movement
	public Vector2 direction;
	
	// A timer that when it reaches 0 kills the Bullet
	public int timeoutTimer;
	
	
	public AnimatedSprite2D animatedSprite2D;
	
		
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Bullets do not collide with terrain and stay on top of it
		this.TopLevel = true;
		AnimatedSprite2D animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	
	}
	
	
	public void spawn(Vector2 spawnPos, Vector2 dir, float scalarSpeed, int timeout) {
		
		this.position.Y = spawnPos.Y;
		this.position.X = spawnPos.X;
		this.prevPosition.Y = spawnPos.Y;
		this.prevPosition.X = spawnPos.X;
		
		//Position = spawnPos;
		this.GlobalPosition = this.position;

		
		this.scalarSpeed = scalarSpeed;
		
		this.velocity.Y = dir.Y * scalarSpeed;
		this.velocity.X = dir.X * scalarSpeed;
		
		this.timeoutTimer = timeout;
		
		
		
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
			
			QueueFree();
			
		}
		
		
		this.GlobalPosition = this.position;
	}
		
	
	
	
}
