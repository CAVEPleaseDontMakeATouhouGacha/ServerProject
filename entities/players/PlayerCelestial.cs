using Godot;
using System;

public partial class PlayerCelestial : PlatformerPlayerBase
{
	
	
	
	
	public void update_celestial(double delta) {
		
	

		switch (this.state) {
			
			
			case (PLAYER_STATE_GROUNDED): {
				
				this.movementGeneral_calculateDirections(delta);

				
				
				// Calculate normal movement speed
				this.velocity.X = this.moveDirection.X * this.scalarSpeed;
				
				// Check raw keystates so player can hold the jump button to instantly jump
				// when they touch the ground
				if ((this.keystates & PLAYER_INPUTFLAG_JUMP) == PLAYER_INPUTFLAG_JUMP) {
					
					this.movementGeneral_startJump(delta);
					
					// Start handling jump on this tick
					this.state = PLAYER_STATE_JUMPING;
					goto case(PLAYER_STATE_JUMPING);
				}
				
				//!TODO: Maybe move this to another place in the future
				this.movementGeneral_updatePositionWithVelocity(delta);
				
				
				break;
			}
			
			case (PLAYER_STATE_AIRBORNE): {
				
				
				
				this.movementGeneral_calculateDirections(delta);
				
				// Calculate normal movement speed		
				this.velocity.X = this.moveDirection.X * this.scalarSpeed;
				
				
				// Check look keystates so player doesn't activate double jump the moment they get off the air
				if ((this.lookKeystates & PLAYER_INPUTFLAG_JUMP) == PLAYER_INPUTFLAG_JUMP) {
					
					this.movementGeneral_startJump(delta);
					
					// Start handling jump on this tick
					this.state = PLAYER_STATE_JUMPING;
					goto case(PLAYER_STATE_JUMPING);
				}
				
				
				this.movementGeneral_applyGravity(delta);
				
				this.movementGeneral_updatePositionWithVelocity(delta);
				
				
				
				
				break;
			}
			
			case (PLAYER_STATE_JUMPING): {
				
				this.movementGeneral_calculateDirections(delta);
				

				
				// Calculate normal movement speed		
				this.velocity.X = this.moveDirection.X * this.scalarSpeed;
				
				// Check look keystates so player doesn't activate double jump the moment they get off the air
				if ((this.lookKeystates & PLAYER_INPUTFLAG_JUMP) == PLAYER_INPUTFLAG_JUMP) {
					
					this.movementGeneral_startJump(delta);
					
					// Start handling jump on this tick
					this.state = PLAYER_STATE_JUMPING;
					goto case(PLAYER_STATE_JUMPING);
				}
				
				
				this.movementGeneral_applyGravity(delta);
				
				this.movementGeneral_stopJump(delta);
				
				this.movementGeneral_updatePositionWithVelocity(delta);
				
				
				break;
			}
			

			case (PLAYER_STATE_DASHINGGROUND): {
				
				
				break;
			}
			
			case (PLAYER_STATE_DASHINGAIR): {
				

				
				break;
			}
			
			
			//! MAYBE DEPRECATED?, if dashing has both states why wouldn't this?
			/*
			case (PLAYER_STATE_ATTACK): {
				
				
				break;
			}
			*/
			
			case (PLAYER_STATE_ATTACKGROUND): {
				
				
				break;
			}
			
			
			case (PLAYER_STATE_ATTACKAIR): {
				
				
				
				break;
			}
			
			
		};
		
		
		if (this.position.Y > 900) {
			
			//this.position.Y = (float)bOnGround;
			this.position.Y = 900;
			
			// We are now on ground
			this.state = PLAYER_STATE_GROUNDED;
			
			
			// Reset vertical momentum
			this.velocity.Y = 0.0f;
			
			// Reset jumps
			this.jumpsLeft = 1;
			

			
			
		}
		
		
		
	}
	
	
	//======================
	//! Physics update
	//======================
	public override void _PhysicsProcess(double delta) {
		
		// Grab Global Position
		this.position = this.GlobalPosition;
		// Record previous position
		this.prevPosition = this.position;
		
		
		// Build keystates
		this.buildKeystates();
		
		
		// Run update code
		this.update_celestial(delta);
	
		// Shoot constant stream of shots
		//this.shooting_celestial(delta);
		
		
		if (Input.IsActionPressed("INPUT_DEBUG") == true) {
			
			this.position.Y = 000;
			this.position.X = 100;
			this.velocity.Y = 0;
			this.state = PLAYER_STATE_GROUNDED;
			this.scalarSpeed = PlayerTakko.cWalkSpeed;
			
		}
		
		
		// Set back Global position
		this.GlobalPosition = this.position;
		
		
		// Run animation
		//this.animation(delta);
		
		
	}
	
	
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		// Set movement constants
		
		this.scalarSpeed = PlayerTakko.cWalkSpeed;
		
		this.gravity = PlayerTakko.cTakkoGravity;
		this.terminalFallSpeed = PlayerTakko.cTakkoTerminalFallSpeed;
		this.jumpForce = PlayerTakko.cJumpForce;
		
		
		//! TODO: Use shot interval timer
		//this.chargeShotTimer = cMaxChargeShotLength;
		
		
		
		// Start on airborne state
		this.state = PLAYER_STATE_AIRBORNE;
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
