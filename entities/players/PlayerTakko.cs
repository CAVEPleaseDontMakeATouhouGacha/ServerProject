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
	public const float cWalkSpeed = 3.5f;
	public const float cDashSpeed = 8f;
	public const float cSuperSpeed = 10f;
		
	// Dash can last up to 1.5 second
	public const int cDashDurationTimeLength = 180;
	//!DEPRECATED
	//const int cRetainDashSpeedTimeLength = 120;
	public const int cMaxAerialDashes = 2;
	
	public const float cTakkoGravity = 0.25f;
	public const float cTakkoTerminalFallSpeed = 20.75f;
	public const float cJumpForce = 10.0f;
	public const int cMaxJumps = 1;
		
	// Value to normlize ordinal movement
	public const float cOrdinalNormalizer = 0.7071067f;
	public const float cCardinalDashSpeed = cDashSpeed;
	public const float cOrdinalDashSpeed = cDashSpeed * cOrdinalNormalizer;
		
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
	public const int cMaxChargeShotLength = 180;
	// Timer has to be at least this value to shoot a weak shot
	public const int cWeakChargeShot = 150;
	
	
	
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
				level.AddChild(smallShot);
				// Only after do we set members
				smallShot.spawnSmall(this.position.X, this.position.Y, this.lookDirection);
			
				
	
				
				
			} else if (this.chargeShotTimer <= 0) {
				
				Shot bigShot = shotScene.Instantiate<Shot>();
				
				// Spawn the Shot by adding it to the Main scene.
				level.AddChild(bigShot);
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
	public bool movement_takko_startDash(double delta) {
		
		
		// Just started dashing
		if ((this.keystates & PLAYER_INPUTFLAG_DASH) == PLAYER_INPUTFLAG_DASH) {
			
			// And if we have stamina left
			if (this.dashStaminaTimer > 0) {
				
				//!TODO: Move this test out of here?
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
	
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool movement_takko_stopDash(double delta) {
		
		
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
			this.scalarSpeed = this.currDashSpeed;
			//this.scalarSpeed = cDashSpeed;
				
			// No order quirks here, gravity gets applied after dashing correctly
			// Garvity applied on the next tick
			
			return true;
				
		}
		
		return false;
	
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool movement_takko_startDashJump(double delta) {
		
		// Handling jumping out of a ground dash
		if ((this.keystates & PLAYER_INPUTFLAG_JUMP) == PLAYER_INPUTFLAG_JUMP) {
						
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

			return true;
		}
		
		return false;
		
	}
	
	
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void movement_takko_dashMovement(double delta) {
		
			
		float isCardinal = this.moveDirection.Y * this.moveDirection.X;
		this.currDashSpeed = 0.0f;
			
		if (isCardinal != 0) {
				
			// Ordinal Dash
			this.currDashSpeed = cOrdinalDashSpeed;
				
		} else {
				
			// Cardinal Dash
			this.currDashSpeed = cCardinalDashSpeed;
					
		}
			
		// Apply omnidirectional movement
		this.velocity.Y = this.moveDirection.Y * this.currDashSpeed;
		this.velocity.X = this.moveDirection.X * this.currDashSpeed;
			

	}
	
	
	
	
	
	
	public void movement_takko(double delta) {
		
	

		switch (this.state) {
			
			
			case (PLAYER_STATE_GROUNDED): {
				
				this.movementGeneral_calculateDirections(delta);
				
				bool startedGroundDashing = this.movement_takko_startDash(delta);
				if (startedGroundDashing == true) {

					// Begin the state of dashing in this tick
					this.state = PLAYER_STATE_DASHINGGROUND;
					goto case(PLAYER_STATE_DASHINGGROUND);
				}
				
				
				
				// Calculate normal movement speed
				this.velocity.X = this.moveDirection.X * this.scalarSpeed;
				
				bool bWasJumpStarted = this.movementGeneral_startJump(delta);
				if (bWasJumpStarted == true) {
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
				
				
				bool startedAirDashing = this.movement_takko_startDash(delta);
				if (startedAirDashing == true) {
					// Take away one airDash if we are in the air
					this.airDashesLeft = this.airDashesLeft - 1;
					// Begin the state of dashing in this tick
					this.state = PLAYER_STATE_DASHINGAIR;
					goto case(PLAYER_STATE_DASHINGAIR);
				}
				
				
				
				this.movementGeneral_applyGravity(delta);
				
				this.movementGeneral_updatePositionWithVelocity(delta);
				
				
				break;
			}
			
			case (PLAYER_STATE_JUMPING): {
				
				this.movementGeneral_calculateDirections(delta);
				
				bool startedAirDashing = this.movement_takko_startDash(delta);
				if (startedAirDashing == true) {
					// Take away one airDash if we are in the air
					this.airDashesLeft = this.airDashesLeft - 1;
					// Begin the state of dashing in this tick
					this.state = PLAYER_STATE_DASHINGAIR;
					goto case(PLAYER_STATE_DASHINGAIR);
				}
				
				
				// Calculate normal movement speed		
				this.velocity.X = this.moveDirection.X * this.scalarSpeed;
				
				this.movementGeneral_applyGravity(delta);
				
				this.movementGeneral_stopJump(delta);
				
				this.movementGeneral_updatePositionWithVelocity(delta);
				
				
				break;
			}
			
			//! DEPRECATED Use air or ground dash instead
			/*
			case (PLAYER_STATE_DASHING): {
				
				
				
				
				
				break;
			}
			*/
			case (PLAYER_STATE_DASHINGGROUND): {
				
				
				bool stoppedDash = this.movement_takko_stopDash(delta);
				if (stoppedDash == true) {
					// If we stopped dashing in the ground we are now grounded
					this.state = PLAYER_STATE_GROUNDED;
					// Give the normal walking speed back
					this.scalarSpeed = cWalkSpeed;
					goto case(PLAYER_STATE_GROUNDED);
				}
				
				this.movementGeneral_calculateDirections(delta);
				
				//bool hasDashJumped = movement_takko_startDashJump(delta);
				
				
				this.movement_takko_dashMovement(delta);
				
				this.movementGeneral_updatePositionWithVelocity(delta);
				
				// If we are dashing upwards or in a ordinal direction
				// That means we have started air dashing
				if (this.velocity.Y < 0) {

					// Decrease dash stamina timer when dashing in the air
					this.dashStaminaTimer = this.dashStaminaTimer - 1;
					this.state = PLAYER_STATE_DASHINGAIR;
				}
				
				break;
			}
			
			case (PLAYER_STATE_DASHINGAIR): {
				
				// Only check if we ran out of Dash stamina here so the player has full Dash tick duration
				// and also early exit in case of not wanting to dash anymore
				bool stoppedDash = this.movement_takko_stopDash(delta);
				if (stoppedDash == true) {
					// If we stopped dashing in the air we are now airborne
					this.state = PLAYER_STATE_AIRBORNE;
					goto case(PLAYER_STATE_AIRBORNE);
				}
				
			
				this.movementGeneral_calculateDirections(delta);
				
				
				this.movement_takko_dashMovement(delta);
				
				// Decrease dash stamina timer when dashing in the air
				this.dashStaminaTimer = this.dashStaminaTimer - 1;
				
				
				this.movementGeneral_updatePositionWithVelocity(delta);
				
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
		
		
		int lineStartPosY = (int)this.prevPosition.Y;
		int lineEndPosY = (int)this.position.Y;
		int lineStartPosX = (int)this.prevPosition.X;
		int lineEndPosX= (int)this.position.X;
		
		
		tileCollision_lineY(lineStartPosY, lineEndPosY, lineStartPosX);
		
		
		if (this.position.Y > 900) {
			
			//this.position.Y = (float)bOnGround;
			this.position.Y = 900;
			
			// We are now on ground
			this.state = PLAYER_STATE_GROUNDED;
			
			
			// Reset vertical momentum
			this.velocity.Y = 0.0f;
			
			// Reset jumps
			this.jumpsLeft = cMaxJumps;
			
			// Reset dash
			this.dashStaminaTimer = cDashDurationTimeLength;
			this.scalarSpeed = cWalkSpeed;
			this.airDashesLeft = cMaxAerialDashes;
			
			
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
		
		
		switch (this.state) {
			
			
			case (PLAYER_STATE_GROUNDED): {
				
				this.animatedSprite2D.Play("Idle");
				
				break;
			}
			
			case (PLAYER_STATE_AIRBORNE): {
				
				this.animatedSprite2D.Play("Idle");
				
				break;
			}
			
			case (PLAYER_STATE_JUMPING): {
				
				this.animatedSprite2D.Play("Idle");
				
				break;
			}
			

			case (PLAYER_STATE_DASHINGGROUND):
			case (PLAYER_STATE_DASHINGAIR): {
				
				// If we are dashing, play the dashing animation
				this.animatedSprite2D.Play("Dashing");
				
				break;
			}
			
			

			
			case (PLAYER_STATE_ATTACKGROUND): {
				
				
				break;
			}
			
			
			case (PLAYER_STATE_ATTACKAIR): {
				
				
				
				break;
			}
			
			
		};
		
		
		

		
		// If we are moving fast we let out after images
		if (this.scalarSpeed > cWalkSpeed) {
			
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
		this.movement_takko(delta);
		
		//! Handle bullets
		// Graze bullets that are grazeable and cancel those that are cancelable
		// and of course collide with them
		
		
		// Shoot charged Shots
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
		this.airDashesLeft = cMaxAerialDashes;
		this.dashStaminaTimer = cDashDurationTimeLength;
		
		
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
