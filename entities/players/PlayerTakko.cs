using Godot;
using System;

// Needed for funtion inlining
using System.Runtime.CompilerServices;


public partial class PlayerTakko : PlatformerPlayerBase
{
	
	// Things we can spawn
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
	
	
	public void melee_startSlam(double delta) {
		
		// Only do a slam if we are in the air
		
		// Cancel out of dashing
		this.flags = this.flags & ~PLAYER_BITFLAG_DASHING;
					
		// Do a slam
		// Add donwards velocity
		this.velocity.Y = this.velocity.Y + 15.0f; 
					
		
		
		
	}
	
	
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
		

		
		if ((this.keystates & PLAYER_INPUTFLAG_SHOOT) != PLAYER_INPUTFLAG_SHOOT) {
		
			// Fire a Big Shot if we reached the timer end
			 if (this.chargeShotTimer <= 0) {
				
				
				Shot bigShot = this.entityPooler.getShot();

				// Now set members
				bigShot.spawnBig(this.position.X, this.position.Y, this.lookDirection);
			
				
				
			} else if (this.chargeShotTimer <= cWeakChargeShot) {
				
				// If we have not reached the timer we can prpbably fire a small shot
				
				//Shot smallShot = shotScene.Instantiate<Shot>();
				
				// Spawn the Shot by adding it to the Main scene.
				//level.AddChild(smallShot);
				Shot smallShot = this.entityPooler.getShot();
				// Only after do we set members
				smallShot.spawnSmall(this.position.X, this.position.Y, this.lookDirection);
			
				
			}
				
				
			// Reset the Charge Shot Timer back to tye max value
			this.chargeShotTimer = cMaxChargeShotLength;
			
		} else {
			
			//if ((this.keystates & PLAYER_INPUTFLAG_SHOT) == PLAYER_INPUTFLAG_SHOT) {
			// The player is holding down the shoot button
			this.chargeShotTimer = this.chargeShotTimer - 1;
			
		}
	
	
		
	}
	
	
	
	//======================
	//! Tile collision functions
	//======================
	
	
	public void tileCollision_takko_responseX(TileCollisionResponse closestXTileResponse) {
		
		
		//! X Collision Response
		if (closestXTileResponse == null) {
			return;
		}
		
		
		switch (closestXTileResponse.tileData.Terrain) {
			
			case (TILETYPE_EMPTY): {
				
				// Don't o anything
				return;
			}
			
			case (TILETYPE_FULLSOLID): {
				
				// Only push the player out if the tile is in the center of the hitbox
			
				//int tileDistance = intStartLineY - closestXTileResponse.tileTopPosY;
				int tileDistance = 0;
				//GD.Print("CLosestXTilePos Y:" + (closestXTileResponse.tileTopPosY << 5) + " X:" + (closestXTileResponse.tileTopPosX << 5));
				//GD.Print("Distance " + distance + "startlinePos " + intStartLineY);
				if (Math.Abs(tileDistance) < 2) {
					
					// If we are moving to the left
					if (this.velocity.X < 0.0f) {
						
						// Make sure what we are finding is a wall and not ground
						// By seeing if there are 2 empty tiles, where the player could fit into to the right
						/*
						const int right = +1;
						if (this.tileCollision_checkForHorizontalTiles(closestXTileResponse, TILEMETA_HORITILES_PLAYERWIDTH, right) == true) {
							return;
						}
						*/
						// We have found a wall on our left
						this.position.X = (float)(closestXTileResponse.tileTopPosX << 5) + this.rectWidth + 32.0f;
					
					} else if (this.velocity.X > 0.0f) {
						
						/*
						const int left = -1;
						if (this.tileCollision_checkForHorizontalTiles(closestXTileResponse, TILEMETA_HORITILES_PLAYERWIDTH, left) == true) {
							return;
						}
						*/
						
						// We have found a wall on our right
						this.position.X = (float)(closestXTileResponse.tileTopPosX << 5) - this.rectWidth;
					
					}
					
					
				}
			
		
				break;	
			}
		
		};
	
	}
	
	
	public void tileCollision_takko_responseY(TileCollisionResponse closestYTileResponse) {
		
		//! Y Tile Collision Response
		
		
		
		
		// If there is no tile
		if (closestYTileResponse == null) {
			
			// And if we are on the ground
			if (this.state == PLAYER_STATE_GROUNDED) {
				
				// That means we should be falling then goober
				this.state = PLAYER_STATE_AIRBORNE;
			}
			
			return;
			
		} 
			
		
		
		
		switch (closestYTileResponse.tileData.Terrain) {
			
			case (TILETYPE_EMPTY): {
				
				// Don't o anything
				return;
			}
			
			case (TILETYPE_FULLSOLID): {
				
				
				//!TODO: Use distance instead of velocity so the player can't zoom through walls by holding opposite directons
				// If we are moving down put us on the top of the tile
				if (this.velocity.Y > 0.0f) {
					
				
					
					// If there was a tile found... 
					// Is this solid tile part of a wall?
					// If there is another tile on top of it, it is a wall stupid
					// We check for 4 tiles since that is the amount of tiles the player can fit in
					
					
					// If the tile we think is ground does not allow for space for the character to stand on,
					// it's probably a wall
					
					const int above = -1;
					if (this.tileCollision_checkForVericalTiles(closestYTileResponse, TILEMETA_VERTTILES_PLAYERHEIGHT, above) == true) {
						return;
					}
					
					
					
					if (this.tileCollision_verticalSnap(closestYTileResponse) == false) {
						return;
					}
					
					/*
					// Only snap to tile if we are close enough to it and we are using one of the edge sensors
					if (this.sensorUsedForVertCollision != TILESENSOR_VERT_MID) {
						
						
						
					
						float tilePosY = (float)(closestYTileResponse.tileTopPosY << 5);
						float distance = tilePosY - this.position.Y;
						GD.Print("EEE" + distance);
						
						// If the distance between player center and tile is bigger than 64(two tiles)
						// don't snapp
						// Or use 32 + 24 = 56 so then player is snapped to tiles if they are close enough
						// Or 48
						if (distance < 48.0f) {
							return;
						}
						
					}
					*/
					
					
					
					this.position.Y = (float)(closestYTileResponse.tileTopPosY << 5) - this.rectHeight;
						
					// If we are not dashing on the ground
					if (this.state == PLAYER_STATE_DASHINGGROUND) {
						
						// Keep state
						this.state = PLAYER_STATE_DASHINGGROUND;
						
					} else {
							
						// We are now on ground
						this.state = PLAYER_STATE_GROUNDED;
							
					}
						
					this.interaction_takko_startGrounded();
					// End collison response
					return;
				}
					
				// If we are moving up treat it as a ceiling (unless it's passthrough)
				if (this.velocity.Y < 0.0f) {
					
					
					int tilePlayerHeight = 4;
					int currentBelowTile = 1;
					
					
					do {
						
						Vector2I tileBelowClosest = new Vector2I(closestYTileResponse.tileTopPosX, closestYTileResponse.tileTopPosY + currentBelowTile);
						TileData belowTile = this.tilemap.GetCellTileData(0, tileBelowClosest);
					
						// Check if it's part of a wall
						if (belowTile != null) {
									
							if(belowTile.Terrain != TILETYPE_EMPTY) {
									
								// Then clostest Y full solid is part of a wall.
								// Ignore it
								return;
							}
									
						}
						
						currentBelowTile = currentBelowTile + 1;
						
					} while (currentBelowTile <= tilePlayerHeight);
					
					
					this.position.Y = (float)(closestYTileResponse.tileTopPosY << 5) + 32.0f + this.rectHeight;
					// Cancel Y velocity
					this.velocity.Y = 0.0f;
					// Don't change state
					return;
				}
				
				
				
				break;
			}
			
			
		};
		
			
		
			
			

			
	}
		
		
		
	
	
	
	
	
	
	
	public void interaction_takko_startGrounded() {
		
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
	
	
	//======================
	//! Movement Functions
	//======================
	
	
	//! Megaman Z Styled movement
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void movement_takko_startGroundDash(double delta) {
		
		
		// Just started ground dashing
		
		// Force dash repress for Dash jumping
		this.actionRepressForcer = this.actionRepressForcer | PLAYER_INPUTFLAG_DASH;
		
		
		// We are now dashing
		this.state = PLAYER_STATE_DASHING;		
		this.flags = this.flags | PLAYER_BITFLAG_DASHING;
		this.flags = this.flags | PLAYER_BITFLAG_GRAZING;
		this.flags = this.flags | PLAYER_BITFLAG_CANCELING;
					
	
		// Cancel push forces
		this.pushForce.Y = 0;
		this.pushForce.X = 0;
					

		
	}
	
	
	
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void movement_takko_startAirDash(double delta) {
		
		
		// Just started air dashing
		
		
		// Don't allow to activate another dash unless player represses the button when Air Dashing
		this.actionRepressForcer = this.actionRepressForcer | PLAYER_INPUTFLAG_DASH;
		
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
		
		// Take away one airDash if we are in the air
		this.airDashesLeft = this.airDashesLeft - 1;

		
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
	
	
	
	
	
	
	public void logicUpdate_takko(double delta) {
		
	

		switch (this.state) {
			
			
			case (PLAYER_STATE_GROUNDED): {
				
				this.movementGeneral_calculateDirections(delta);
				
				// Start ground dash if Dash Input is down 
				if ((this.keystates & PLAYER_INPUTFLAG_DASH) == PLAYER_INPUTFLAG_DASH) {
				
					this.movement_takko_startGroundDash(delta);
				
					// Begin the state of dashing in this tick
					this.state = PLAYER_STATE_DASHINGGROUND;
					goto case(PLAYER_STATE_DASHINGGROUND);
					
				}
				
				
				
				// Calculate normal movement speed
				this.velocity.X = this.moveDirection.X * this.scalarSpeed;
				
				
				// Check raw keystates so player can hold the jump button to instantly jump
				// when they touch the ground
				if ((this.keystates & PLAYER_INPUTFLAG_JUMP) == PLAYER_INPUTFLAG_JUMP) {
					
					// Force player to repress jump
					//this.actionRepressForcer = this.actionRepressForcer | PLAYER_INPUTFLAG_JUMP;
					
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
				
				/*
				if ((this.lookKeystates & PLAYER_INPUTFLAG_MELEE) == PLAYER_INPUTFLAG_MELEE) {
					
					// Perform slam
					// If player is holding down
					if ((this.keystates & PLAYER_INPUTFLAG_DOWN) == PLAYER_INPUTFLAG_DOWN) {
						
						this.melee_startSlam(delta);
						//!TODO: Go to attack state?	
						
					}
					
				}
				*/
				
				//! MAYBE: If we wanted to implement coyote time all we need to do is use a timer here
				// and check if the input is within that time, of course as long as the timer is set on the tile collision function
				
				this.movementGeneral_calculateDirections(delta);
				
				// Calculate normal movement speed		
				this.velocity.X = this.moveDirection.X * this.scalarSpeed;
				
				
				// When falling start dashing if player is holding Dash key
				if ((this.keystates & PLAYER_INPUTFLAG_DASH) == PLAYER_INPUTFLAG_DASH) {
					
					// And if we have stamina left
					if (this.dashStaminaTimer > 0) {
							
							// Can we air dash again?
							if (this.airDashesLeft > 0) {
					
								this.movement_takko_startAirDash(delta);
	

								// Begin the state of dashing in this tick
								this.state = PLAYER_STATE_DASHINGAIR;
								goto case(PLAYER_STATE_DASHINGAIR);
								
							}
							
					}
					
				}
				
				
				
				this.movementGeneral_applyGravity(delta);
				
				this.movementGeneral_updatePositionWithVelocity(delta);
				
				
				break;
			}
			
			case (PLAYER_STATE_JUMPING): {
				
				
				// Only start Dashing if the player has repressed it
				if ((this.lookKeystates & PLAYER_INPUTFLAG_DASH) == PLAYER_INPUTFLAG_DASH) {
					
					// And if we have stamina left
					if (this.dashStaminaTimer > 0) {
							
							// Can we air dash again?
							if (this.airDashesLeft > 0) {
								
								this.movement_takko_startAirDash(delta);

								// Begin the state of dashing in this tick
								this.state = PLAYER_STATE_DASHINGAIR;
								goto case(PLAYER_STATE_DASHINGAIR);
							
							}
					
					}
				
				}

				
				
				this.movementGeneral_calculateDirections(delta);
				
				
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
					// Give back the ability to jump
					this.jumpsLeft = cMaxJumps;
					goto case(PLAYER_STATE_GROUNDED);
				}
				
				this.movementGeneral_calculateDirections(delta);
				
				bool bHasDashJumped = movement_takko_startDashJump(delta);
				if (bHasDashJumped == true) {
					
					// Start handling jump on this tick
					this.state = PLAYER_STATE_JUMPING;
					goto case(PLAYER_STATE_JUMPING);
				}
				
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
		
		
		// Tile Collision 
		// Uses lines to be sure there is not tunelling
		
		float lineStartPosYPrecise = this.prevPosition.Y;
		float lineEndPosYPrecise = this.position.Y;
		float lineStartPosXPrecise = this.prevPosition.X;
		float lineEndPosXPrecise = this.position.X;
		
		
		// Extend on the Y and X axis to account for the hitbox
		
		// Extend lines horizontally based on the player width
		float lineEndPosXPreciseExtended = lineEndPosXPrecise;
		// If moving to the left, extend to the left
		if (this.velocity.X < 0.0f) {
			lineEndPosXPreciseExtended = lineEndPosXPreciseExtended - this.rectWidth;
		} else if (this.velocity.X > 0.0f) {
			lineEndPosXPreciseExtended = lineEndPosXPreciseExtended + this.rectWidth;
		}
		// Don't extend or extend to the direction we are looking
		
		//!OPTIMISE: Remove that else if
		// Extend lines vertically based on the player height
		float lineEndPosYPreciseExtended = lineEndPosYPrecise;
		// If moving going up, extend upwards
		if (this.velocity.Y < 0.0f) {
			lineEndPosYPreciseExtended = lineEndPosYPreciseExtended - this.rectHeight;
		} else if (this.velocity.Y > 0.0f) {
			// Default to extending downwards
			lineEndPosYPreciseExtended = lineEndPosYPreciseExtended + this.rectHeight;
		} else {
			//!MAYBE: When not moving just sense the feet?
			// Default to extending downwards
			lineEndPosYPreciseExtended = lineEndPosYPreciseExtended + this.rectHeight;
		}
		
		// General use line variables
		// Convert to int
		int intStartLineY = (int)lineStartPosYPrecise;
		int intStartLineX = (int)lineStartPosXPrecise;
		int intEndLineY = (int)lineEndPosYPrecise;
		int intEndLineX = (int)lineEndPosXPrecise;
		
		int intEndLineYExtended = (int)lineEndPosYPreciseExtended;
		int intEndLineXExtended = (int)lineEndPosXPreciseExtended;
		
		
		// Convert to tile units
		intStartLineY = intStartLineY >> 5;
		intStartLineX = intStartLineX >> 5;
		intEndLineY = intEndLineY >> 5;
		intEndLineX = intEndLineX >> 5;
		
		intEndLineYExtended = intEndLineYExtended >> 5;
		intEndLineXExtended = intEndLineXExtended >> 5;
		
		
		
		// X Tile Grabbing
		
		
		float horiSensorYOffset = this.rectHeight / 4;
		
		// The horizontal tile sensor are the edges of the hitbox
		// Set up the various horizontal sensors origins
		float horiSensorTopPosYStart = lineStartPosYPrecise - this.rectHeight;
		float horiSensorUpPosYStart = lineStartPosYPrecise - horiSensorYOffset;
		float horiSensorMidPosY = lineStartPosYPrecise;
		float horiSensorDownPosYStart = lineStartPosYPrecise + horiSensorYOffset;
		float horiSensorBottomPosYStart = lineStartPosYPrecise + this.rectHeight;
		
		
		float horiSensorTopPosYEnd = lineEndPosYPrecise - this.rectHeight;
		float horiSensorUpPosYEnd = lineEndPosYPrecise - horiSensorYOffset;
		float horiSensorDownPosYEnd = lineEndPosYPrecise + horiSensorYOffset;
		float horiSensorBottomPosYEnd = lineEndPosYPrecise + this.rectHeight;
		
		// Set up the tile versions
		int intHoriSensorTopPosYStart = (int)horiSensorTopPosYStart;
		int intHoriSensorUpPosYStart = (int)horiSensorUpPosYStart;
		int intHoriSensorDownPosYStart = (int)horiSensorDownPosYStart;
		int intHoriSensorBottomPosYStart = (int)horiSensorBottomPosYStart;
		
		int intHoriSensorTopPosYEnd = (int)horiSensorTopPosYEnd;
		int intHoriSensorUpPosYEnd = (int)horiSensorUpPosYEnd;
		int intHoriSensorDownPosYEnd = (int)horiSensorDownPosYEnd;
		int intHoriSensorBottomPosYEnd = (int)horiSensorBottomPosYEnd;
		
		
		intHoriSensorTopPosYStart = intHoriSensorTopPosYStart >> 5;
		intHoriSensorUpPosYStart = intHoriSensorUpPosYStart >> 5;
		intHoriSensorDownPosYStart = intHoriSensorDownPosYStart >> 5;
		intHoriSensorBottomPosYStart = intHoriSensorBottomPosYStart >> 5;
		
		
		intHoriSensorTopPosYEnd = intHoriSensorTopPosYEnd >> 5;
		intHoriSensorUpPosYEnd = intHoriSensorUpPosYEnd >> 5;
		intHoriSensorDownPosYEnd = intHoriSensorDownPosYEnd >> 5;
		intHoriSensorBottomPosYEnd = intHoriSensorBottomPosYEnd >> 5;
		
		
		
		// Grab all possible tiles on the X axis
		
		TileCollisionResponse horiTopSensorResponse = tileCollision_line(intStartLineX, intHoriSensorUpPosYStart,
													 					 intEndLineXExtended, intHoriSensorUpPosYEnd);
		
		TileCollisionResponse horiMidSensorResponse = tileCollision_line(intStartLineX, intStartLineY,
													 					 intEndLineXExtended, intEndLineY);
																		
		TileCollisionResponse horiBottomSensorResponse = tileCollision_line(intStartLineX, intHoriSensorDownPosYStart,
													 					 	intEndLineXExtended, intHoriSensorDownPosYEnd);
		
		
		GetNode<Line2D>("HoriSensorMid").ClearPoints();
		Vector2 debugPosStart = new Vector2((float)(intStartLineX<<5), (float)(intStartLineY<<5));
		Vector2 debugPosEnd = new Vector2((float)(intEndLineXExtended<<5), (float)(intEndLineY<<5));
		GetNode<Line2D>("HoriSensorMid").AddPoint(GetNode<Line2D>("HoriSensorMid").ToLocal(debugPosStart), 0);
		GetNode<Line2D>("HoriSensorMid").AddPoint(GetNode<Line2D>("HoriSensorMid").ToLocal(debugPosEnd), 1);
		
		
		
		int horiDistanceTop = 2000000000;
		int horiDistanceUp = 2000000000;
		int horiDistanceMid = 2000000000;
		int horiDistanceDown = 2000000000;
		int horiDistanceBottom = 2000000000;
		
		
		if (horiTopSensorResponse != null) {
			horiDistanceTop = intStartLineY - horiTopSensorResponse.tileTopPosY;
		}
		if (horiMidSensorResponse != null) {
			horiDistanceMid = intStartLineY - horiMidSensorResponse.tileTopPosY;
		}
		if (horiBottomSensorResponse != null) {
			horiDistanceBottom = intStartLineY - horiBottomSensorResponse.tileTopPosY;
		}
		
		
		//TileCollisionResponse closestXTileResponse = null;
		TileCollisionResponse closestXTileResponse = horiMidSensorResponse;
		
		// If moving down, up takes precedence and vice versa?
		/*
		closestXTileResponse = vertMidSensorResponse;
		if(this.velocity.X < 0.0f) {
			closestYTileResponse = vertRightSensorResponse;
		} else if (this.velocity.X > 0.0f) {
			closestYTileResponse = vertLeftSensorResponse;
		} 
		*/
	
	
		if (horiDistanceTop < horiDistanceMid && horiDistanceTop <= horiDistanceBottom) {
			closestXTileResponse = horiTopSensorResponse;
		} else if (horiDistanceMid <= horiDistanceTop && horiDistanceMid <= horiDistanceBottom) {
			closestXTileResponse = horiMidSensorResponse;
		} else if (horiDistanceBottom < horiDistanceTop && horiDistanceBottom <= horiDistanceMid){
			closestXTileResponse = horiBottomSensorResponse;
		}
		
		
		
		
		
				
		// Y axis collision
		
		
		
		// The vertical tile sensor are the edges of the hitbox
		float verticalSensorXOffset = this.rectWidth;
		//float verticalSensorXOffset = 20.0f;
		
		// Set up the various vertical sensors origins
		float verticalSensorLeftPosX = lineStartPosXPrecise - verticalSensorXOffset;
		float verticalSensorRightPosX = lineStartPosXPrecise + verticalSensorXOffset;
		float verticalSensorLeftPosXEnd = lineEndPosXPrecise - verticalSensorXOffset;
		float verticalSensorRightPosXEnd = lineEndPosXPrecise + verticalSensorXOffset;
		
		
		
		// Specific to this one
		
		int intStartLineXLeft = (int)verticalSensorLeftPosX;
		int intStartLineXRight = (int)verticalSensorRightPosX;
		int intEndLineXLeft = (int)verticalSensorLeftPosXEnd;
		int intEndLineXRight = (int)verticalSensorRightPosXEnd;
		
		
		intStartLineXLeft = intStartLineXLeft >> 5;
		intStartLineXRight = intStartLineXRight >> 5;
		intEndLineXLeft = intEndLineXLeft >> 5;
		intEndLineXRight = intEndLineXRight >> 5;
		
		
		TileCollisionResponse vertLeftSensorResponse = tileCollision_line(intStartLineXLeft, intStartLineY,
													 					  intEndLineXLeft, intEndLineYExtended);
		
		
		TileCollisionResponse vertMidSensorResponse = tileCollision_line(intStartLineX, intStartLineY,
													 					 intEndLineX, intEndLineYExtended);
		
		TileCollisionResponse vertRightSensorResponse = tileCollision_line(intStartLineXRight, intStartLineY,
													 					   intEndLineXRight, intEndLineYExtended);
			
	
		GetNode<Line2D>("VertSensorLeft").ClearPoints();
		debugPosStart = new Vector2((float)(intStartLineXLeft<<5), (float)(intStartLineY<<5));
		debugPosEnd = new Vector2((float)(intEndLineXLeft<<5), (float)(intEndLineYExtended<<5));
		GetNode<Line2D>("VertSensorLeft").AddPoint(GetNode<Line2D>("VertSensorLeft").ToLocal(debugPosStart), 0);
		GetNode<Line2D>("VertSensorLeft").AddPoint(GetNode<Line2D>("VertSensorLeft").ToLocal(debugPosEnd), 1);
		
		GetNode<Line2D>("VertSensorMid").ClearPoints();
		debugPosStart = new Vector2((float)(intStartLineX<<5), (float)(intStartLineY<<5));
		debugPosEnd = new Vector2((float)(intEndLineX<<5), (float)(intEndLineYExtended<<5));
		GetNode<Line2D>("VertSensorMid").AddPoint(GetNode<Line2D>("VertSensorMid").ToLocal(debugPosStart), 0);
		GetNode<Line2D>("VertSensorMid").AddPoint(GetNode<Line2D>("VertSensorMid").ToLocal(debugPosEnd), 1);
		
		
		GetNode<Line2D>("VertSensorRight").ClearPoints();
		debugPosStart = new Vector2((float)(intStartLineXRight<<5), (float)(intStartLineY<<5));
		debugPosEnd = new Vector2((float)(intEndLineXRight<<5), (float)(intEndLineYExtended<<5));
		GetNode<Line2D>("VertSensorRight").AddPoint(GetNode<Line2D>("VertSensorRight").ToLocal(debugPosStart), 0);
		GetNode<Line2D>("VertSensorRight").AddPoint(GetNode<Line2D>("VertSensorRight").ToLocal(debugPosEnd), 1);
		
		
		
		
		
		// Between these 3 checks now choose which one we want to pick based on distance
		// if the distance is the same they take precedence depending on the order and direction of movement
		
		int vertDistanceLeft = 2000000000;
		int vertDistanceMid = 2000000000;
		int vertDistanceRight = 2000000000;
		
		
		
		
		if (vertLeftSensorResponse != null) {
			vertDistanceLeft = intStartLineY - vertLeftSensorResponse.tileTopPosY;
		}
		if (vertMidSensorResponse != null) {
			vertDistanceMid = intStartLineY - vertMidSensorResponse.tileTopPosY;
		}
		if (vertRightSensorResponse != null) {
			vertDistanceRight = intStartLineY - vertRightSensorResponse.tileTopPosY;
		}
		
		
		GD.Print("Distance Left " + vertDistanceLeft);
		GD.Print("Distance Mid " + vertDistanceMid);
		GD.Print("Distance Right " + vertDistanceRight);
		
		TileCollisionResponse closestYTileResponse = null;
		
		
		// If moving to the left the right one takes precedence and the contrary is true
		closestYTileResponse = vertMidSensorResponse;
		this.sensorUsedForVertCollision = TILESENSOR_VERT_MID;
		if(this.velocity.X < 0.0f) {
			closestYTileResponse = vertRightSensorResponse;
			this.sensorUsedForVertCollision = TILESENSOR_VERT_RIGHT;
		} else if (this.velocity.X > 0.0f) {
			closestYTileResponse = vertLeftSensorResponse;
			this.sensorUsedForVertCollision = TILESENSOR_VERT_LEFT;
		} 
		
		/*
		TileCollisionResponse tempY = vertMidSensorResponse;
		int tempDistY = vertDistanceMid;
		if (vertDistanceLeft < vertDistanceMid) {
			tempY = vertLeftSensorResponse;
			tempDistY = vertDistanceMid;
		} 
		if (vertDistanceRight < tempDistY) {
			tempY = vertRightSensorResponse;
		}
		closestYTileResponse = tempY;
		*/
		
	
		if (vertDistanceLeft < vertDistanceMid && vertDistanceLeft <= vertDistanceRight) {
			closestYTileResponse = vertLeftSensorResponse;
			this.sensorUsedForVertCollision = TILESENSOR_VERT_LEFT;
			GD.Print("Left Sensor was choosen");
		} else if (vertDistanceMid <= vertDistanceLeft && vertDistanceMid <= vertDistanceRight) {
			closestYTileResponse = vertMidSensorResponse;
			this.sensorUsedForVertCollision = TILESENSOR_VERT_MID;
			GD.Print("Mid Sensor was choosen");
		} else if (vertDistanceRight < vertDistanceLeft && vertDistanceRight <= vertDistanceMid){
			closestYTileResponse = vertRightSensorResponse;
			this.sensorUsedForVertCollision = TILESENSOR_VERT_RIGHT;
			GD.Print("Right Sensor was choosen");
		}
		
	
		
		
		
		
		
		// X axis tile collision response
		tileCollision_takko_responseX(closestXTileResponse);
				
		// Y axis tile collision response
		tileCollision_takko_responseY(closestYTileResponse);
		
		
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
			

			case (PLAYER_STATE_DASHINGGROUND):{
				
				// If we are dashing, play the dashing animation
				this.animatedSprite2D.Play("Dashing");
				
				break;
			}
			
			case (PLAYER_STATE_DASHINGAIR): {
				
				Particle dashEffect = this.entityPooler.getParticle();


				dashEffect.spawnOnTopOfMove(this.position, this.position, 3);

			
				// Change layer
				dashEffect.ZIndex = 5;
				dashEffect.animatedSprite2D.Play("DashEffect");
				dashEffect.animatedSprite2D.FlipH = this.animatedSprite2D.FlipH;
				
				
				// Also spawn Dasf Effect after images
				Particle dashAfterImage = this.entityPooler.getParticle();
				const int dashAfterImageDuration = 15;
				dashAfterImage.spawnStayMove(this.position, dashAfterImageDuration);
				dashAfterImage.animatedSprite2D.Play("DashEffect");
				dashAfterImage.animatedSprite2D.FlipH = this.animatedSprite2D.FlipH;
				
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
		
		
		// If we are charging a Shot
		if ((this.keystates & PLAYER_INPUTFLAG_SHOOT) == PLAYER_INPUTFLAG_SHOOT) {
			
			
			Particle shotCharge = this.entityPooler.getParticle();

			shotCharge.spawnOnTopOfMove(this.position, this.position, 3);

			
			// Change layer
			shotCharge.ZIndex = 5;
			
			if (this.chargeShotTimer <= 0) {
			
				// Charge Shot is ready
				shotCharge.animatedSprite2D.Play("ChargingReady");
				
				
			} else if (this.chargeShotTimer <= cWeakChargeShot) {
				
				// We can do a weak Charge Shot
				shotCharge.animatedSprite2D.Play("ChargingMid");
			
			} else {
				
				// We juts started Charging	
				shotCharge.animatedSprite2D.Play("ChargingStart");
			}
			
			
			
			
			
		}
		
		

	
		// If we are moving fast we let out after images
		if ((this.scalarSpeed > cWalkSpeed) || (this.state == PLAYER_STATE_DASHINGAIR) || (this.state == PLAYER_STATE_DASHINGGROUND)) {
			
			Particle afterImage = this.entityPooler.getParticle();
			// Spawn the afterimage by adding it to the Main scene.
			//AddChild(afterImage);
			
			// Add after image to level
			//level.AddChild(afterImage);
			

			const int afterImageDuration = 30;
			afterImage.spawnStayMove(this.position, afterImageDuration);
			//afterImage.spawn(GlobalPosition, nomove, 0.0f, 120);
			
			
			SpriteFrames newFrames = new SpriteFrames();
			newFrames.AddAnimation("still");
			
			//newFrames.AddFrame("still", this.animatedSprite2D.GetSpriteFrames.Get, );
			//afterImage.animatedSprite2D.SetSpriteFrames(this.animatedSprite2D.GetSpriteFrames);
			//afterImage.animatedSprite2D.SpriteFrames = this.animatedSprite2D.SpriteFrames;
			//afterImage.animatedSprite2D.Play("Idle", 0.0f, false);
			
			//afterImage.animatedSprite2D.Play("default");
			afterImage.animatedSprite2D.Play("ChargingStart");
			
		}
		
		
	}
	
	
	
	
	
	//======================
	//! Physics update
	//======================
	public override void _PhysicsProcess(double delta) {
		
		// Grab Global Position
		this.position = this.GlobalPosition;
		// Record previous position before updating it
		this.prevPosition = this.position;
		
		
		// Build keystates
		this.buildKeystates();
		
		// Melee combat code
		//this.melee_takko(delta);
		
		// Run all Takko logic code,
		// this includes movement code, melee, and shooting if necessary
		this.logicUpdate_takko(delta);
		
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
		
		
		if (Input.IsActionPressed("INPUT_SLOWDEBUG") == true) {
			
			Engine.PhysicsTicksPerSecond = 1;
			
		} else {
			Engine.PhysicsTicksPerSecond = 120;
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
