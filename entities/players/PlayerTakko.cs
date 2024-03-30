using Godot;
using System;

// Needed for funtion inlining
using System.Runtime.CompilerServices;


public partial class PlayerTakko : PlatformerPlayerBase
{
	
	// Things we can spawn
	PackedScene particleScene = GD.Load<PackedScene>("res://entities/particles/Particle.tscn");
	PackedScene shotScene = GD.Load<PackedScene>("res://entities/players/shots/Shot.tscn");
	
	
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
		
	const float cTakkoGravity = 0.25f;
	const float cTakkoTerminalFallSpeed = 20.75f;
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
	
	
	//! Melee constants
	//public int cMaxAerialAttacks = 1;
	
	
	//! Shot constants
	// Takes one second and a half to reach max charge shot
	// Can shoot at half that time for a weaker shot
	// Weaker shot should not have half the damage of the max one
	const int cMaxChargeShotLength = 180;
	// Timer has to be at least this value to shoot a weak shot
	const int cWeakChargeShot = 150;
	
	
	
	//======================
	//! Melee functions
	//======================
	
	public void melee_takko(double delta) {
		
		
		// Handle Aerial attaks
		
		// Only do a slam if we are in the air
		if ((this.flags & PLAYER_BITFLAG_GROUNDED) != PLAYER_BITFLAG_GROUNDED) {
			
			// If player pressed melee
			if ((this.keystates & PLAYER_INPUTFLAG_MELEE) == PLAYER_INPUTFLAG_MELEE) {
				
				// Perform slam
				// If player is holding down
				if ((this.keystates & PLAYER_INPUTFLAG_DOWN) == PLAYER_INPUTFLAG_DOWN) {
					
					// Cancel out of dashing
					this.flags = this.flags & ~PLAYER_BITFLAG_DASHING;
					
					// Do a slam
					// Add donwards velocity
					this.velocity.Y = this.velocity.Y + 15.0f; 
					
				}
				
				// Perform a bike kick
				if ((this.keystates & PLAYER_INPUTFLAG_UP) == PLAYER_INPUTFLAG_UP) {
					
					if (this.canDoBikeKick == true) {
						
						
						this.canDoBikeKick = false;
						
						// Cancel out of dashing
						this.flags = this.flags & ~PLAYER_BITFLAG_DASHING;
					
						// Add small upwards velocity
						this.velocity.Y = this.velocity.Y - 4.0f; 
						
						
					}

				}
				
				
				
			}
				
			
		}
		
		
		// Handle ground attacks
		if ((this.flags & PLAYER_BITFLAG_GROUNDED) == PLAYER_BITFLAG_GROUNDED) {
			
			
			// If player pressed melee
			if ((this.keystates & PLAYER_INPUTFLAG_MELEE) == PLAYER_INPUTFLAG_MELEE) {
			
				// Perform Cyclone
				// If player is holding up
				if ((this.keystates & PLAYER_INPUTFLAG_UP) == PLAYER_INPUTFLAG_UP) {
					
					// Cancel out of dashing
					this.flags = this.flags & ~PLAYER_BITFLAG_DASHING;
					// We are not grounded
					this.flags = this.flags & ~PLAYER_BITFLAG_GROUNDED;
					// We are not jumping
					this.flags = this.flags & ~PLAYER_BITFLAG_JUMPING;
					// And we cannot jump
					this.jumpsLeft = 0;
				
					// We can graze through bullets or else this move would be really bad
					this.flags = this.flags & ~PLAYER_BITFLAG_JUMPING;
				
					// Add upwards velocity
					this.velocity.Y = -15.0f; 
					
				}
				
				
				// Perform a bike kick
				if ((this.keystates & PLAYER_INPUTFLAG_DOWN) == PLAYER_INPUTFLAG_DOWN) {
					
					if (this.canDoBikeKick == true) {
						
						
						this.canDoBikeKick = false;
						
						// Cancel out of dashing
						this.flags = this.flags & ~PLAYER_BITFLAG_DASHING;
						// We are not grounded
						this.flags = this.flags & ~PLAYER_BITFLAG_GROUNDED;
						// We are not jumping
						this.flags = this.flags & ~PLAYER_BITFLAG_JUMPING;
						// And we cannot jump
						this.jumpsLeft = 0;
					
						// Add small upwards velocity
						this.velocity.Y = this.velocity.Y - 4.0f; 
						
						
					}

				}
				
			
				
			}
			
			
			
		
			
		}
		
		
		
	}
	
	
	
	//======================
	//! Shot functions
	//======================
	
	public void shooting_takko(double delta) {
		

		
		if ((this.keystates & PLAYER_INPUTFLAG_SHOT) != PLAYER_INPUTFLAG_SHOT) {
		
			// Fire a small shot
			if (this.chargeShotTimer <= cWeakChargeShot) {
				
				Shot smallShot = shotScene.Instantiate<Shot>();
				
				// Spawn the Shot by adding it to the Main scene.
				AddChild(smallShot);
				// Only after do we set members
				smallShot.spawnSmall(this.position.X, this.position.Y, this.lookDirection);
			
				
	
				
				
			} else if (this.chargeShotTimer <= 0) {
				
				Shot bigShot = shotScene.Instantiate<Shot>();
				
				// Spawn the Shot by adding it to the Main scene.
				AddChild(bigShot);
				// Now set members
				bigShot.spawnBig(this.position.X, this.position.Y, this.lookDirection);
			
				
				
			}
			
			this.chargeShotTimer = cMaxChargeShotLength;
			
		} else {
			
			//if ((this.keystates & PLAYER_INPUTFLAG_SHOT) == PLAYER_INPUTFLAG_SHOT) {
			// The player is holding down the shoot button
			this.chargeShotTimer = this.chargeShotTimer - 1;
			
		}
	
	
		
	}
	
	
	//======================
	//! Movement Functions
	//======================
	
	
	//! Megaman Z Styled movement
	

	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void movement_takko_handleJumpDEPRECATED(double delta) {
		
		// Handle jumping, yes for one tick the jump receives no gravity
		if ((this.keystates & PLAYER_INPUTFLAG_JUMP) == PLAYER_INPUTFLAG_JUMP) {
			
			// If there are jumps left
			if (this.jumpsLeft > 0) {
				
				
				// Normal jump check, on ground, not jumping, not dashing
				int bNormalGroundJump = Convert.ToInt32(((this.flags & PLAYER_BITFLAG_GROUNDED) == PLAYER_BITFLAG_GROUNDED));
				bNormalGroundJump = bNormalGroundJump & Convert.ToInt32(((this.flags & PLAYER_BITFLAG_JUMPING) != PLAYER_BITFLAG_JUMPING));
				
				// If can do normal grounded jump
				//if ((this.flags & PLAYER_STATEFLAG_GROUNDED) == PLAYER_STATEFLAG_GROUNDED) {
				if (bNormalGroundJump == 1) {
				
				
					// No longer grounded
					this.flags = flags & ~PLAYER_BITFLAG_GROUNDED;
					this.flags = flags | PLAYER_BITFLAG_JUMPING;
					
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
		if ((this.flags & PLAYER_BITFLAG_JUMPING) == PLAYER_BITFLAG_JUMPING) {
			
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
		
		
		
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void movement_takko_handleDashDEPRECATED(double delta) {
		
		// Begin the state of dashing in this tick
		if ((this.flags & PLAYER_BITFLAG_DASHING) == PLAYER_BITFLAG_DASHING) {
			
			
			float isCardinal = this.moveDirection.Y * this.moveDirection.X;
			float currDashSpeed = 0.0f;
			
			if (isCardinal != 0) {
				
				// Ordinal Dash
				currDashSpeed = cOrdinalDashSpeed;
				
			} else {
				
				// Cardinal Dash
				currDashSpeed = cCardinalDashSpeed;
					
			}
			
			// Apply omnidirectional movement
			this.velocity.Y = this.moveDirection.Y * currDashSpeed;
			this.velocity.X = this.moveDirection.X * currDashSpeed;
			
			// Decrease dash stamina timer
			this.dashStaminaTimer = this.dashStaminaTimer - 1;
			
			
			
			
			// Handling jumping out of a ground dash
			int bIsPLayerJumping = this.keystates & PLAYER_INPUTFLAG_JUMP;
			bIsPLayerJumping = bIsPLayerJumping >> 4;
			int bIsPlayerGrounded = this.flags & PLAYER_BITFLAG_GROUNDED;
			
			int bOutofDashJump = bIsPLayerJumping & bIsPlayerGrounded;
			
			// If jumping ot of dash is possible do it
			if (bOutofDashJump == 1) {
				
				// Dashing has ended out of own volition
				this.flags = this.flags & ~PLAYER_BITFLAG_DASHING;
				this.flags = this.flags & ~PLAYER_BITFLAG_GRAZING;
				this.flags = this.flags & ~PLAYER_BITFLAG_CANCELING;
				
				// We are not grounded but jumping
				this.flags = this.flags & ~PLAYER_BITFLAG_GROUNDED;
				this.flags = this.flags | PLAYER_BITFLAG_JUMPING;
				
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
			if ((this.dashStaminaTimer <= 0) || (dashInput != PLAYER_INPUTFLAG_DASH)) {
				
				// Dashing has ended
				this.flags = this.flags & ~PLAYER_BITFLAG_DASHING;
				this.flags = this.flags & ~PLAYER_BITFLAG_GRAZING;
				this.flags = this.flags & ~PLAYER_BITFLAG_CANCELING;
				
				// We are not grounded but also not jumping
				this.flags = this.flags & ~PLAYER_BITFLAG_GROUNDED;
				this.flags = this.flags & ~PLAYER_BITFLAG_JUMPING;
				
				
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
		
		
		
		
		
		
	}
	

	public bool movement_takko_startDash(double delta) {
		
		
		// Just started dashing
		if ((this.keystates & PLAYER_INPUTFLAG_DASH) == PLAYER_INPUTFLAG_DASH) {
			
			// And if we have stamina left
			if (this.dashStaminaTimer > 0) {
						
				// If we have enough air dashes, ground constantly resets them so no need
				// for ground check
				if (this.airDashesLeft > 0) {
					
					// We are now dashing
					this.state = PLAYER_STATE_DASHING;		
					this.flags = this.flags | PLAYER_BITFLAG_DASHING;
					this.flags = this.flags | PLAYER_BITFLAG_GRAZING;
					this.flags = this.flags | PLAYER_BITFLAG_CANCELING;
					
					
					// Allow the player to do aerial attacks again
					this.canDoBikeKick = true;
					this.canDoHundredKicks = true;
						
					// Cancel push forces
					this.pushForce.Y = 0;
					this.pushForce.X = 0;
					
					return true;
							
				}
						
						
						
			}
			
			
		}
		
		return false;
		
	}

	
	
	public void movement_takkoImp(double delta) {
		
	

		switch (this.state) {
			
			
			case (PLAYER_STATE_GROUNDED): {
				
				this.movementGeneral_calculateDirections(delta);
				
				// Calculate normal movement speed
				this.velocity.X = this.moveDirection.X * this.scalarSpeed;
				
				bool bWasJumpStarted = this.movementGeneral_startJump(delta);
				if (bWasJumpStarted == true) {
					// Start handling jump on this tick
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
				
				
				bool startedAirDashing = this.movement_takko_startDash(delta);
				if (startedAirDashing == true) {
					// Take away one airDash if we are in the air
					this.airDashesLeft = this.airDashesLeft - 1;
					goto case(PLAYER_STATE_DASHING);
				}
				
				
				
				this.movementGeneral_applyGravity(delta);
				
				this.movementGeneral_updatePositionWithVelocity(delta);
				
				
				break;
			}
			
			case (PLAYER_STATE_JUMPING): {
				
				this.movementGeneral_calculateDirections(delta);
				
				// Calculate normal movement speed		
				this.velocity.X = this.moveDirection.X * this.scalarSpeed;
				
				this.movementGeneral_applyGravity(delta);
				
				this.movementGeneral_stopJump(delta);
				
				this.movementGeneral_updatePositionWithVelocity(delta);
				
				
				break;
			}
			
			
			case (PLAYER_STATE_DASHING): {
				
				
				break;
			}
			
			case (PLAYER_STATE_DASHINGGROUND): {
				
				
				
				break;
			}
			
			case (PLAYER_STATE_DASHINGAIR): {
				
				
				
				break;
			}
			
			
			case (PLAYER_STATE_ATTACK): {
				
				
				break;
			}
			
			
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
			
			// Reset dash
			this.dashStaminaTimer = cDashDurationTimeLength;
			this.scalarSpeed = cWalkSpeed;
			this.airDashesLeft = 2;
			
			
			// Allow the player to do aerial attacks again
			this.canDoBikeKick = true;
			this.canDoHundredKicks = true;
			
			
		}
		
		
		
	}
	
	
	public void movement_takko(double delta) {
		
		

		
		this.movementGeneral_calculateDirections(delta);
		
		// Calculate normal movement speed		
		this.velocity.X = this.moveDirection.X * this.scalarSpeed;
		
		
		// Apply gravity if not grounded
		
		if ((this.flags & PLAYER_BITFLAG_GROUNDED) != PLAYER_BITFLAG_GROUNDED) {
			
			
			this.movementGeneral_applyGravity(delta);
			
		}
		

		
		// Handle jumping
		
		this.movement_takko_handleJumpDEPRECATED(delta);
		
		
		
		//! Handle OmniDash
		
		
		int bNotDashing = ~this.flags & PLAYER_BITFLAG_DASHING;
		bNotDashing = bNotDashing >> 2;
		
		
		int bPressingDash = this.keystates & PLAYER_INPUTFLAG_DASH;
		bPressingDash = bPressingDash >> 4;
		
		int bCanStartOmniDash = bNotDashing & bPressingDash;
		
		// Just started dashing
		if (bCanStartOmniDash == 1) {
			
			// And if we have stamina left
			if (this.dashStaminaTimer > 0) {
						
				// If we have enough air dashes, ground constantly resets them so no need
				// for ground check
				if (this.airDashesLeft > 0) {
							
					this.flags = this.flags | PLAYER_BITFLAG_DASHING;
					this.flags = this.flags | PLAYER_BITFLAG_GRAZING;
					this.flags = this.flags | PLAYER_BITFLAG_CANCELING;
					
					
					// Allow the player to do aerial attacks again
					this.canDoBikeKick = true;
					this.canDoHundredKicks = true;
						
					// Cancel push forces
					this.pushForce.Y = 0;
					this.pushForce.X = 0;
							
					// We need a check so we can do a upwards dash and two aerials ones
					if ((this.flags & PLAYER_BITFLAG_GROUNDED) != PLAYER_BITFLAG_GROUNDED) {
								
						// Take away one airDash if we are in the air
						this.airDashesLeft = this.airDashesLeft - 1;
								
					}
							
							
				}
						
						
						
			}
			
			
		}
					
					
				
		this.movement_takko_handleDashDEPRECATED(delta);
		
		
		
		

		
		// Add velocity to position
		this.position.Y = this.position.Y + this.velocity.Y;
		this.position.X = this.position.X + this.velocity.X; 
		
		
		
		
		// Is on ground?
		//int bOnGround = tileCollision_groundCeiling(+65.5f);
		
		
		
		
		
		//if (bOnGround == TILECOLRES_GROUND) {
		if (this.position.Y > 900) {
			
			//this.position.Y = (float)bOnGround;
			this.position.Y = 900;
			
			// We are now on ground
			this.flags = this.flags | PLAYER_BITFLAG_GROUNDED;
			this.flags = this.flags & ~PLAYER_BITFLAG_JUMPING;
			
			
			// Reset vertical momentum
			this.velocity.Y = 0.0f;
			
			// Reset jumps
			this.jumpsLeft = 1;
			
			// Reset dash
			this.dashStaminaTimer = cDashDurationTimeLength;
			this.scalarSpeed = cWalkSpeed;
			this.airDashesLeft = 2;
			
			
			// Allow the player to do aerial attacks again
			this.canDoBikeKick = true;
			this.canDoHundredKicks = true;
			
			
		}
		
		
	}
	
	
	
	
	
	//======================
	//! Animation functions
	//======================
	
	public void animation(double delta) {
		
		if (this.moveDirection.X > 0) {
			
			this.animatedSprite2D.FlipH = false;
			
		} else if (this.moveDirection.X < 0) {
			
			this.animatedSprite2D.FlipH = true;
			
		}
		
		// If we are moving fast we let out after images
		if ((this.flags & PLAYER_BITFLAG_DASHING) == PLAYER_BITFLAG_DASHING) {
		//if ((this.scalarSpeed > cWalkSpeed) {
			
			Particle afterImage = particleScene.Instantiate<Particle>();
			// Spawn the afterimage by adding it to the Main scene.
			//AddChild(afterImage);
			
			// Add after image to level
			GetParent<Node2D>().AddChild(afterImage);
			
			Vector2 nomove = new Vector2(0.0f, 0.0f);
			int afterImageDuration = 30;
			afterImage.spawn(this.position, nomove, 0.0f, 30);
			//afterImage.spawn(GlobalPosition, nomove, 0.0f, 120);
			
			SpriteFrames newFrames = new SpriteFrames();
			newFrames.AddAnimation("still");
			
			//newFrames.AddFrame("still", this.animatedSprite2D.GetSpriteFrames.Get, );
			//afterImage.animatedSprite2D.SetSpriteFrames(this.animatedSprite2D.GetSpriteFrames);
			//afterImage.animatedSprite2D.SpriteFrames = this.animatedSprite2D.SpriteFrames;
			//afterImage.animatedSprite2D.Play("Idle", 0.0f, false);
			
			
			
			
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
		
		// Melee combat code
		this.melee_takko(delta);
		
		// Run movement code
		//this.movement_takko(delta);
		this.movement_takkoImp(delta);
		
		//! Handle bullets
		// Graze bullets that are grazeable and cancel those that are cancelable
		// and of course collide with them
		
		this.shooting_takko(delta);
		
		
		if (Input.IsActionPressed("INPUT_DEBUG") == true) {
			
			this.position.Y = 000;
			this.position.X = 100;
			this.velocity.Y = 0;
			this.state = PLAYER_STATE_GROUNDED;
			this.scalarSpeed = cWalkSpeed;
			
		}
		
		
		// Set back Global position
		this.GlobalPosition = this.position;
		
		
		// Run animation
		this.animation(delta);
		
		
		
		
		
		
	}
	
	
	
	
	
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		this.TopLevel = true;
		this.initPlatformerPlayerBase();
		
		
		// Set movement constants
		this.gravity = cTakkoGravity;
		this.terminalFallSpeed = cTakkoTerminalFallSpeed;
		this.jumpForce = cJumpForce;
		
		
		
		// Dashing
		this.airDashesLeft = 2;
		this.dashStaminaTimer = cDashDurationTimeLength;
		
		//! TODO: Put constants here
		this.chargeShotTimer = cMaxChargeShotLength;
		
		
		
		
		this.canDoBikeKick = true;
		this.canDoHundredKicks = true;
		
		
		
		// Start on airborne state
		this.state = PLAYER_STATE_AIRBORNE;
		
	}
	
	
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
