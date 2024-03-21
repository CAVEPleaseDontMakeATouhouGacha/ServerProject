using Godot;
using System;

public partial class PlayerTakko : PlatformerPlayerBase
{
	
	
	
	
	// OG Megaman X constants
	//const float walkSpeed = 1.5f;
	//const float dashSpeed = 3.5f;
		
	//const float gravity = 0.25f;
	//const float terminalFallSpeed = 5.75f;
	//const float jumpForce = 5.0f;
		
		
	// Changed for this game's resolution
	const float cWalkSpeed = 3.5f;
	const float cDashSpeed = 8f;
	const float cSuperSpeed = 10f;
		
	// Dash can last up to 1.5 second
	const int cDashDurationTimeLength = 180;
	const int cRetainDashSpeedTimeLength = 120;
		
	const float cGravity = 0.25f;
	const float cTerminalFallSpeed = 20.75f;
	const float cJumpForce = 10.0f;
		
	// Value to normlize ordinal movement
	const float cOrdinalNormalizer = 0.7071067f;
	const float cCardinalDashSpeed = cDashSpeed;
	const float cOrdinalDashSpeed = cDashSpeed * cOrdinalNormalizer;
		
	// Using delta time
	//float walkSpeed = 420f * delta;
	//float dashSpeed = 840.f * delta;
	
	//float cardinalDashSpeed = dashSpeed;
	//float ordinalDashSpeed = dashSpeed * 0.7071067f;
	
	
	
	//======================
	//! Melee functions
	//======================
	
	public void melee_takko(double delta) {
		
		
		// Slam attack
		
		// Only do a slam if we are in the air
		if ((this.flags & PLAYER_STATEFLAG_GROUNDED) != PLAYER_STATEFLAG_GROUNDED) {
			
			// If player pressed melee
			if ((this.keystates & PLAYER_INPUTFLAG_MELEE) == PLAYER_INPUTFLAG_MELEE) {
				
				// If player is holding down
				if ((this.keystates & PLAYER_INPUTFLAG_DOWN) == PLAYER_INPUTFLAG_DOWN) {
					
					// Add donwards velocity
					this.velocity.Y = this.velocity.Y + 15.0f; 
					
				}
				
			}
				
			
		}
		
		
		
	}
	
	
	
	//======================
	//! Shot functions
	//======================
	
	public void shooting_takko(double delta) {
		
		// Takes one second and a half to reach max charge shot
		// Can shoot at half that time for a weaker shot
		// Weaker shot should not have half the damage of the max one
		const int maxChargeShotLength = 180;
		// Timer has to be at least this value to shoot a weak shot
		const int weakChargeShot = 150;
		
		
		if ((this.keystates & PLAYER_INPUTFLAG_SHOT) != PLAYER_INPUTFLAG_SHOT) {
		
			// Fire the shot, if there is one
			if (this.chargeShotTime <= weakChargeShot) {
				
				
				
			} else if (this.chargeShotTime <= 0) {
				
				
				
			}
			
			this.chargeShotTime = maxChargeShotLength;
			
		}
		
		
		
		if ((this.keystates & PLAYER_INPUTFLAG_SHOT) == PLAYER_INPUTFLAG_SHOT) {
		
			this.chargeShotTime = this.chargeShotTime - 1;
		
		}
		
	}
	
	
	//======================
	//! Movement Functions
	//======================
	
	
	//! Megaman Z Styled movement
	
	
	public void movement_takko_handleJump(double delta) {
		
		
	}
	
	public void movement_takko_handleDash(double delta) {
		
		
	}
	
	public void movement_takko_handleHorizontal(double delta) {
		
		
	}
	
	
	public void movement_takko(double delta) {
		
		

		// Get position
		this.position = Position;
		
		
		// Reset velocity
		//velocity.Y = 0;
		//velocity.X = 0;
		
		
		// Calculate directions
		
		// Get horizontal input in bitflags
		int verticalInput = (this.keystates & PLAYER_INPUTFLAG_UPDOWN);
		int horizontalInput = (this.keystates & PLAYER_INPUTFLAG_LEFTRIGHT) >> 2;
		
		// 0 movement if nothing is being pressed
		// -1 if player is holding left
		// +1 if player is holding right
		// 0 if player is holding both
		float[] directionLookup = {0.0f, -1.0f, +1.0f, 0.0f};
		
		this.direction.Y = directionLookup[verticalInput];
		this.direction.X = directionLookup[horizontalInput];
		
		
		
		
		
		
		
		
			
		// Handle normal grounded and airborne horizonal velocity
			
		// Calculate normal movement speed		
		this.velocity.X = this.direction.X * this.scalarSpeed;
	
		
		
		
		
		
		
		// Apply gravity if not grounded
		
		if ((this.flags & PLAYER_STATEFLAG_GROUNDED) != PLAYER_STATEFLAG_GROUNDED) {
			
			this.velocity.Y = this.velocity.Y + cGravity;
		
		
			// If we reached terminal velocity, keep it like that
			if (this.velocity.Y >= cTerminalFallSpeed) {
				
				this.velocity.Y = cTerminalFallSpeed;
			}
			
			
		}
		

		
		// Handle jumping, yes for one tick the jump receives no gravity
		if ((this.keystates & PLAYER_INPUTFLAG_JUMP) == PLAYER_INPUTFLAG_JUMP) {
			
			// If there are jumps left
			if (this.jumpsLeft > 0) {
				
				
				// Normal jump check, on ground, not jumping, not dashing
				int bNormalGroundJump = Convert.ToInt32(((this.flags & PLAYER_STATEFLAG_GROUNDED) == PLAYER_STATEFLAG_GROUNDED));
				bNormalGroundJump = bNormalGroundJump & Convert.ToInt32(((this.flags & PLAYER_STATEFLAG_JUMPING) != PLAYER_STATEFLAG_JUMPING));
				
				// If can do normal grounded jump
				//if ((this.flags & PLAYER_STATEFLAG_GROUNDED) == PLAYER_STATEFLAG_GROUNDED) {
				if (bNormalGroundJump == 1) {
				
				
					// No longer grounded
					this.flags = flags & ~PLAYER_STATEFLAG_GROUNDED;
					this.flags = flags | PLAYER_STATEFLAG_JUMPING;
					
					this.jumpsLeft = this.jumpsLeft - 1;
					
					// Give full jump force and later check if player stopped holding down button
					this.velocity.Y = -cJumpForce;
					

					
					// Slope jumps
					//velocity.X = velocity.X - (jumpForce * sin(Ground Angle));
					//velocity.Y = velocity.Y - (jumpForce * cos(Ground Angle));
					
				} 
				
				
				
				
			}
		
		}
		
		// If player is jumping but stopped holding down the jump key
		if ((this.flags & PLAYER_STATEFLAG_JUMPING) == PLAYER_STATEFLAG_JUMPING) {
			
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
		
		
		
		
		
		//! Handle OmniDash
		
		
		int bNotDashing = ~this.flags & PLAYER_STATEFLAG_DASHING;
		bNotDashing = bNotDashing >> 2;
		
		
		int bPressingDash = this.keystates & PLAYER_INPUTFLAG_DASH;
		bPressingDash = bPressingDash >> 4;
		
		int bCanStartOmniDash = bNotDashing & bPressingDash;
		
		// Just started dashing
		if (bCanStartOmniDash == 1) {
			
			// And if we have stamina left
			if (this.dashStaminaTime > 0) {
						
				// If we have enough air dashes, ground constantly resets them so no need
				// for ground check
				if (this.airDashesLeft > 0) {
							
					this.flags = this.flags | PLAYER_STATEFLAG_DASHING;
					this.flags = this.flags | PLAYER_STATEFLAG_GRAZING;
					this.flags = this.flags | PLAYER_STATEFLAG_CANCELING;
						
					// Cancel push forces
					this.pushForce.Y = 0;
					this.pushForce.X = 0;
							
					// We need a check so we can do a upwards dash and two aerials one
					if ((this.flags & PLAYER_STATEFLAG_GROUNDED) != PLAYER_STATEFLAG_GROUNDED) {
								
						// Take away one airDash if we are in the air
						this.airDashesLeft = this.airDashesLeft - 1;
								
					}
							
							
				}
						
						
						
			}
			
			
		}
					
					
				
		
		
		
		// Begin the state of dashing in this tick
		if ((this.flags & PLAYER_STATEFLAG_DASHING) == PLAYER_STATEFLAG_DASHING) {
			
			
			float isCardinal = direction.Y * direction.X;
			float currDashSpeed = 0.0f;
			
			if (isCardinal != 0) {
				
				// Ordinal Dash
				currDashSpeed = cOrdinalDashSpeed;
				
			} else {
				
				// Cardinal Dash
				currDashSpeed = cCardinalDashSpeed;
					
			}
			
			// Apply omnidirectional movement
			this.velocity.Y = this.direction.Y * currDashSpeed;
			this.velocity.X = this.direction.X * currDashSpeed;
			
			// Decrease dash stamina timer
			this.dashStaminaTime = this.dashStaminaTime - 1;
			
			
			
			
			// Handling jumping out of a ground dash
			int bIsPLayerJumping = this.keystates & PLAYER_INPUTFLAG_JUMP;
			bIsPLayerJumping = bIsPLayerJumping >> 4;
			int bIsPlayerGrounded = this.flags & PLAYER_STATEFLAG_GROUNDED;
			
			int bOutofDashJump = bIsPLayerJumping & bIsPlayerGrounded;
			
			// If jumping ot of dash is possible do it
			if (bOutofDashJump == 1) {
				
				// Dashing has ended out of own volition
				this.flags = this.flags & ~PLAYER_STATEFLAG_DASHING;
				this.flags = this.flags & ~PLAYER_STATEFLAG_GRAZING;
				this.flags = this.flags & ~PLAYER_STATEFLAG_CANCELING;
				
				// We are not grounded but jumping
				this.flags = this.flags & ~PLAYER_STATEFLAG_GROUNDED;
				this.flags = this.flags | PLAYER_STATEFLAG_JUMPING;
				
				this.jumpsLeft = 0;
				
				// Give super speed
				//this.scalarSpeed = superSpeed;
				this.scalarSpeed = cDashSpeed;
				// Apply jump force with no gravity
				this.velocity.Y = -cJumpForce;

				
			}
			
			
			
			
			
			
			//! Handle ending the dash
			int dashInput = this.keystates & PLAYER_INPUTFLAG_DASH;
			
			// If player isn't holding the dash button anymore or timer has run out
			if ((this.dashStaminaTime <= 0) || (dashInput != PLAYER_INPUTFLAG_DASH)) {
				
				// Dashing has ended
				this.flags = this.flags & ~PLAYER_STATEFLAG_DASHING;
				this.flags = this.flags & ~PLAYER_STATEFLAG_GRAZING;
				this.flags = this.flags & ~PLAYER_STATEFLAG_CANCELING;
				
				// We are not grounded but also not jumping
				this.flags = this.flags & ~PLAYER_STATEFLAG_GROUNDED;
				this.flags = this.flags & ~PLAYER_STATEFLAG_JUMPING;
				
				
				// Don't allow to jump in the air when a dash has ended
				this.jumpsLeft = 0;
				
				// Give the player a speed bost
				this.scalarSpeed = currDashSpeed;
				//this.scalarSpeed = cDashSpeed;
				
				// No order quirks here, gravity gets applied after dashing correctly
				// Garvity applied on the next tick
				
			}
			
			
		
			// We are still in the dash...
			
			
			
		}
		
		
		
		

		
		// Add velocity to position
		this.position.Y = this.position.Y + this.velocity.Y;
		this.position.X = this.position.X + this.velocity.X; 
		
		
		
		/*
		// Is on ground?
		int bOnGround = tileCollision_groundCeiling(+65.5f);
		if (bOnGround != Int32.MaxValue) {
			this.position.Y = 0;
		}
		*/
		
		// Clamp position
		//if (bOnGround != Int32.MaxValue) {
		if (this.position.Y > 900) {
			
			//this.position.Y = (float)bOnGround;
			this.position.Y = 900;
			
			// We are now on ground
			this.flags = this.flags | PLAYER_STATEFLAG_GROUNDED;
			this.flags = this.flags & ~PLAYER_STATEFLAG_JUMPING;
			
			
			// Reset vertical momentum
			this.velocity.Y = 0.0f;
			
			// Reset jumps
			this.jumpsLeft = 1;
			
			// Reset dash
			this.dashStaminaTime = cDashDurationTimeLength;
			this.scalarSpeed = cWalkSpeed;
			this.airDashesLeft = 2;
			
		}
		
		
		// Finally set position back
		Position = this.position;
		
	}
	
	
	
	
	
	//======================
	//! Animation functions
	//======================
	
	public void animation(double delta) {
		
		if (this.direction.X > 0) {
			
			this.animatedSprite2D.FlipH = false;
			
		} else if (this.direction.X < 0) {
			
			this.animatedSprite2D.FlipH = true;
			
		}
		
		// If we are moving fast we let out after images
		if ((this.flags & PLAYER_STATEFLAG_DASHING) == PLAYER_STATEFLAG_DASHING) {
		//if ((this.scalarSpeed > cWalkSpeed) {
			
			
		}
		
		
	}
	
	
	
	
	
	//======================
	//! Physics update
	//======================
	public override void _PhysicsProcess(double delta) {
		
		
		// Build keystates
		this.buildKeystates();
		
		// Melee combat code
		this.melee_takko(delta);
		
		// Run movement code
		this.movement_takko(delta);
		//this.movement_sonic(delta);
		
		//! Handle bullets
		// Graze bullets that are grazeable and cancel those that are cancelable
		// and of course collide with them
		
		
		// Run animation
		this.animation(delta);
	}
	
	
	
	
	
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		this.initPlatformerPlayerBase();
		
		int nah = tileCollision_groundCeiling(+65.5f);
		
		//! TODO: Put constants here
		chargeShotTime = 180;
		
		
		airDashesLeft = 2;
		
	}
	
	
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
