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
	
	// A slow modifier for when the Bullet is in the Delay or Decay state 
	public const float cSlowModifier = 0.5f;
	
	// The original speed of the Bullet
	public float originalSpeed;
	// The Delay/Decay speed of the Bullet
	public float slowSpeed;
	
	// Movement angle of the Bullet
	public float movementAngle;
	
	
	// Direction Y, -1 for up, +1 for down, 0 for no movement
	// Direction X, -1 for left, +1 for right, 0 for no movement
	public Vector2 direction;
	
	// A timer that when it reaches 0 kills the Bullet
	public int timeoutTimer;
	
	// The number of ticks the Bullet stays in the Delay and Decay state
	public int delayDecayTimer;
	
	// Stay in the Delay and Decay state for 16 ticks or 0.1333 seconds
	// 32 ticks is 0.266 seconds
	public const int cDelayDecayLength = 16;
	
	
	public AnimatedSprite2D animatedSprite2D;
	
		
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Bullets do not collide with terrain and stay on top of it
		this.TopLevel = true;
		AnimatedSprite2D animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	
	}
	
	
	public void spawn(Vector2 spawnPos, Vector2 dir, float scalarSpeed, int timeout) {
		
		this.state = BULLET_STATE_DELAY;
		
		this.position.Y = spawnPos.Y;
		this.position.X = spawnPos.X;
		this.prevPosition.Y = spawnPos.Y;
		this.prevPosition.X = spawnPos.X;
		
		//Position = spawnPos;
		this.GlobalPosition = this.position;

		
		
		

		this.originalSpeed = scalarSpeed;
		this.slowSpeed = scalarSpeed * cSlowModifier;
		this.scalarSpeed = this.slowSpeed;
		
		this.velocity.Y = dir.Y * scalarSpeed;
		this.velocity.X = dir.X * scalarSpeed;
		
		this.timeoutTimer = timeout;
		
		// Set Delay
		this.delayDecayTimer = cDelayDecayLength;
			
		
		
	}
	
	
	
	
	public override void _PhysicsProcess(double delta) {
		
		// Get position
		this.position = this.GlobalPosition;
		// Record previous position
		this.prevPosition = this.position;
		
		switch (this.state) {
			
			case(BULLET_STATE_DELAY): {
				
				
				// Update Delay Timer
				this.delayDecayTimer = this.delayDecayTimer - 1;
		
				if (this.delayDecayTimer <= 0) {
					// The bullet is now airborne
					this.state = BULLET_STATE_AIRBORNE;
					
					// Go into intended speed
					this.scalarSpeed = this.originalSpeed;
					this.velocity.Y = direction.Y * scalarSpeed;
					this.velocity.X = direction.X * scalarSpeed;
					
					// Do collision detection
					
				}
				
				
				break;
			}
			
			case(BULLET_STATE_AIRBORNE): {
				
				
				// Update timeout
				this.timeoutTimer = this.timeoutTimer - 1;
		
				if (this.timeoutTimer <= 0) {
					// If we timed out we go to Decay
					this.state = BULLET_STATE_DECAY;
					
					// Slow down Bullet
					this.scalarSpeed = this.slowSpeed;
					this.velocity.Y = direction.Y * this.scalarSpeed;
					this.velocity.X = direction.X * this.scalarSpeed;
					
					// Set Decay timer
					this.delayDecayTimer = cDelayDecayLength;
				}
				
				
				break;
			}
			
			case(BULLET_STATE_DECAY): {
				
				
				
				// Update Decay Timer
				this.delayDecayTimer = this.delayDecayTimer - 1;
		
				if (this.delayDecayTimer <= 0) {
					// Finally free the bullet for real
					QueueFree();
			
				}
				
				
				break;
			}
			
			
			
			
			
		};
		
		// Update position no matter the state
		this.position.Y = this.position.Y + this.velocity.Y;
		this.position.X = this.position.X + this.velocity.X; 
		
		
		
		// Write back Global position
		this.GlobalPosition = this.position;
	}
		
	
	
	
}
