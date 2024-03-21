using Godot;
using System;

public partial class PlayerSonic : PlatformerPlayerBase
{
	
	
	
	//======================
	//! Movement Functions
	//======================
	
	public void movement_sonic(double delta) {
		
		// OG Sonic constants
		const float accel = 0.046875f;
		const float decel = 0.5f;
		const float friction = 0.046875f;
		const float topSpeed = 6f;
		
		
		
		// Get position
		this.position = Position;
		
		// Reset momentum
		//momentum.Y = 0;
		//momentum.X = 0;
		
		// Reset velocity
		//velocity.Y = 0;
		//velocity.X = 0;
		
		
		/*
		// Handle jumping, yes for one tick the jump receives no gravity
		//! TODO: Remake this maybe with more Megaman like physics
		if ((this.keystates & PLAYER_INPUTFLAG_JUMP) == PLAYER_INPUTFLAG_JUMP) {
			
			// If there are jumps left
			if (this.jumpsLeft > 0) {
				
				// If grounded 
				if ((this.flags & PLAYER_STATEFLAG_GROUNDED) == PLAYER_STATEFLAG_GROUNDED) {
					
					// No longer grounded
					this.flags = flags & ~PLAYER_STATEFLAG_GROUNDED;
					this.flags = flags | PLAYER_STATEFLAG_JUMPING;
					
					this.jumpsLeft = this.jumpsLeft - 1;
					
					// Give full jump force and later check if player stopped holding down button
					this.velocity.Y = -jumpForce;
					
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
				if (this.velocity.Y < -4) {
					// Clamp it down to four
					this.velocity.Y = -4;
				
				}
			
			}
		
		}
		*/
		
		
		//!TODO: Below code only happens when grounded
		
		// If player is pressing left
		if ((keystates & PLAYER_INPUTFLAG_LEFT) == PLAYER_INPUTFLAG_LEFT) {
		
			// If moving to the right
			if (scalarSpeed > 0) {
				// Decelerate
				scalarSpeed = scalarSpeed - decel;  
				
				// Emulate deceleration quirk
				if (scalarSpeed <= 0) {
					scalarSpeed = -0.5f;
				}
					
			// If moving to the left
			} else if (scalarSpeed > -topSpeed) {
				
				// Accelerate
				scalarSpeed = scalarSpeed - accel;
				
				if (scalarSpeed <= -topSpeed) {
					// Reached top speed
					scalarSpeed = -topSpeed;
				}
				
			}
		}
	   
	
		// If player is pressing right
		if ((keystates & PLAYER_INPUTFLAG_RIGHT) == PLAYER_INPUTFLAG_RIGHT) {
		
			// If moving to the left
			if (scalarSpeed < 0) {
				
				// Going in the wrong direction, time decelerate
				scalarSpeed = scalarSpeed + decel;
				
				// Emulate deceleration quirk
				if (scalarSpeed >= 0) {
					scalarSpeed = 0.5f;
				}
				
			} else if (scalarSpeed < topSpeed) {
				
				// If we are here we are moving to the right
				// Accelerate
				scalarSpeed = scalarSpeed + accel;
				
				// Clamp speed
				if (scalarSpeed >= topSpeed) {
					scalarSpeed = topSpeed;
				}
				
			}
		
		} 
			
			

		// If not pressing left or right
		// Let the friction take care of it
		if ((keystates & PLAYER_INPUTFLAG_LEFTRIGHT) != PLAYER_INPUTFLAG_LEFTRIGHT) { 
			
			float scalarSpeedSign = Math.Sign(scalarSpeed);
			float minSpeed = Math.Min(Math.Abs(scalarSpeed), friction);
			
			scalarSpeed = scalarSpeed - minSpeed * scalarSpeedSign;
			
		}
		
		this.velocity.X = scalarSpeed;
		
		// Add momentum to position
		position.Y = position.Y + velocity.Y;
		position.X = position.X + velocity.X; 
		
		
		// Finally set position back
		Position = position;
		
		
	}
	
	
	
	
	
}
