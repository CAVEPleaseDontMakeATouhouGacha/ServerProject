using Godot;
using System;

public partial class Particle : Node2D
{
	
	
	// State for when the particle is alive
	public const int PARTICLE_STATE_DEAD = 0;
	
	public const int PARTICLE_STATE_ALIVE = 1;
	
	
	
	//======================
	//! Members
	//======================
	
	//!Status
	
	// The state of the particle
	public int state;
	
	
	
	
	
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
	
	
		
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
	
	
	
	public override void _PhysicsProcess(double delta) {
		
		// Get position
		this.position = Position;
		
		// Update position
		this.position.Y = this.position.Y + this.velocity.Y;
		this.position.X = this.position.X + this.velocity.X; 
		
		// Update timeout
		this.timeoutTimer = this.timeoutTimer - 1;
		
		if (this.timeoutTimer <= 0) {
			
			
		}
		
		
		Position = this.position;
	
	}
		
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
