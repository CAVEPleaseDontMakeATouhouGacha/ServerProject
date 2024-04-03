using Godot;
using System;


// Needed for funtion inlining
using System.Runtime.CompilerServices;


public partial class Particle : Node2D
{
	
	
	// State for when the particle is alive
	public const int PARTICLE_STATE_DEAD = 0;
	
	public const int PARTICLE_STATE_ALIVE = 1;
	
	
	// Shrink while timing out
	public const int PARTICLE_FLAG_SHRINK = 1;
	// Grow while timing out
	public const int PARTICLE_FLAG_GROW = 2;
	// Lower opacity while timing out
	public const int PARTICLE_FLAG_TRANSPARENT = 4;	
	
	
	//======================
	//! Members
	//======================
	
	//!Status
	
	// The state of the particle
	public int state;
	
	// Flags for various other effects
	public int flags;
	
	// The ID of the Particle
	public int id;
	// The next free Particle in the Pool
	public int nextFree;
	
	//! Movement
	
	// A temporary position Vector2 used for calculations
	public Vector2 position;
	

	// The velocity of the particle
	public Vector2 velocity;
	
	// A member that records the speed of the particle in one variable
	public float scalarSpeed;
	
	
	// Direction Y, -1 for up, +1 for down, 0 for no movement
	// Direction X, -1 for left, +1 for right, 0 for no movement
	public Vector2 direction;
	
	// A timer that when it reaches 0 kills the particle
	public int timeoutTimer;
	
	// The delta of diffenrence when growing and shriking
	public float shrinkGrowDelta;
	// The delta of diffenrence when becoming transparent
	public float transparencyDelta;
	
	
	
	// Pool this Particle belongs to
	public EntityPooler parentPool;
	
	
	public AnimatedSprite2D animatedSprite2D;
	
		
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//this.TopLevel = true;
		this.animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	
	}
	
	
	public void spawn(Vector2 spawnPos, Vector2 dir, float scalarSpeed, int timeout) {
		
		this.position.Y = spawnPos.Y;
		this.position.X = spawnPos.X;
		
		//this.Position = this.position;
		this.GlobalPosition = this.position;
		
		
		this.scalarSpeed = scalarSpeed;
		
		this.velocity.Y = dir.Y * scalarSpeed;
		this.velocity.X = dir.X * scalarSpeed;
		
		this.timeoutTimer = timeout;
		
		
		
	}
	
	
	public void particle_movement(double delta) {
		
		// Update position
		this.position.Y = this.position.Y + this.velocity.Y;
		this.position.X = this.position.X + this.velocity.X; 
		
		// Update timeout
		this.timeoutTimer = this.timeoutTimer - 1;
		
		if (this.timeoutTimer <= 0) {
			
			// Keep Particle in memory for later reusing, just stop processing it
			this.parentPool.freeParticle(this.id);
			//this.SetProcess(false);
			//this.Hide();
			//QueueFree();
			
		}
		
		
	}
	
	public void particle_animation(double delta) {
		
		// Update it directly without using bitflags as a check
		
		
	}
	
	
	
	public override void _PhysicsProcess(double delta) {
		
		// Get position
		this.position = GlobalPosition;
		
		particle_movement(delta);
		
		
		//Position = this.position;
		this.GlobalPosition = this.position;
		
		particle_animation(delta);
		
	}
		
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
