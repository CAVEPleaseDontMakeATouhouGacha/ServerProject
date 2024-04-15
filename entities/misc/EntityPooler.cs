using Godot;
using System;

public partial class EntityPooler : Node
{
	
	
	// This value represents that the entity is pointing to nothing
	// Unlikely we will ever need THAT many enities in one pool...
	public const int ENTITYPOOLER_EMPTYVALUE = 0xDEAD;
	
	// Performance note
	// https://web.archive.org/web/20100709213139/http://www.vistadb.net/blog/post/2008/07/18/Strongly-type-everything!.aspx
	// https://web.archive.org/web/20101124023832/http://www.vistadb.net/blog/post/2008/08/15/Class-v-Struct.aspx
	// https://www.developer.com/microsoft/dotnet/generics-in-net-type-safety-performance-and-generality/
	
	// The level the pool belongs to
	Node2D level;
	
	// Number of Particles
	public const int particlePoolSize = 500;
	// Number of Bullets
	public const int bulletPoolSize = 1000;
	// Number of Shots
	public const int shotPoolSize = 500;
	// Number of Lasers
	
	
	// The pool of Particles
	public Particle[] particlePool;
	
	// The pool of Bullets
	public Bullet[] bulletPool;
	
	// The pool of Shots
	public Shot[] shotPool;
	
	
	
	// The next free Particle
	int nextFreeParticle;
	// The next free Bullet
	int nextFreeBullet;
	// The next free Shot
	int nextFreeShot;
	
	
	PackedScene particleScene = GD.Load<PackedScene>("res://entities/particles/Particle.tscn");
	PackedScene shotScene = GD.Load<PackedScene>("res://entities/players/shots/Shot.tscn");
	PackedScene bulletScene = GD.Load<PackedScene>("res://entities/bullets/Bullet.tscn");
	
	
	
	/*
	public void fillPool<T>(ref <T>[] pool, int poolSize, PackedScene scene, ref int nextFree) {
		
		this.level = GetParent<Node2D>();
		if (this.level == null) {
			
			GD.Print("Level was not found.");
			
		}
		
		
		pool = new <T> pool[poolSize];
		
		int currIndex = 0;
		
		// Initiate the pool
		do {
			
			T entity = scene.Instantiate<T>();
			// Set the next free Particle pointer
			entity.nextFree = (currIndex + 1);
			// Set the pool parent
			entity.parentPool = this;
			// Set index
			entity.id = currParticle;
			
			
			// Start them off
			entity.SetProcess(false);
			entity.Visible = false;
			
			pool[currIndex] = entity;
			
			
			// Add Entity to level
			this.level.CallDeferred(Node2D.MethodName.AddChild, pool[currIndex]);
			
			currIndex = currIndex + 1;
			
			
			
		} while (currIndex < poolSize);
		
		
		nextFree = 0;
		
		
		
	}
	*/
	
	public void fillParticlePool() {
		
		this.level = GetParent<Node2D>();
		if (this.level == null) {
			
			GD.Print("Level was not found.");
			
		}
		
		
		this.particlePool = new Particle[particlePoolSize];
		
		int currParticle = 0;
		
		// Initiate the Particle Pool
		do {
			
			Particle particle = particleScene.Instantiate<Particle>();
			// Set the next free Particle pointer
			particle.nextFree = (currParticle + 1);
			// Set the pool parent
			particle.parentPool = this;
			// Set index
			particle.id = currParticle;
			
			
			// Start them off
			particle.SetProcess(false);
			particle.SetPhysicsProcess(false);
			//particle.Visible = false;
			particle.Hide();
			
			particle.ProcessMode = Node.ProcessModeEnum.Pausable;
			//PROCESS_MODE_INHERIT
			this.particlePool[currParticle] = particle;
			
			
			// Add Particle to level
			this.level.CallDeferred(Node2D.MethodName.AddChild, this.particlePool[currParticle]);
			
			currParticle = currParticle + 1;
			
			
			
		} while (currParticle < particlePoolSize);
		
		// Go back to the last particle and set it as pointing to null
		int lastValid = currParticle = currParticle - 1;
		this.particlePool[lastValid].nextFree = ENTITYPOOLER_EMPTYVALUE;
		
		this.nextFreeParticle = 0;
		
		
	}
	
	
	
	public void fillShotPool() {
		
		this.level = GetParent<Node2D>();
		if (this.level == null) {
			GD.Print("Level was not found.");
		}
		
		
		this.shotPool = new Shot[shotPoolSize];
		
		int currShot = 0;
		
		do {
			
			Shot shot = shotScene.Instantiate<Shot>();
			shot.nextFree = (currShot + 1);
			shot.parentPool = this;
			shot.id = currShot;
			

			shot.SetProcess(false);
			shot.SetPhysicsProcess(false);
			//shot.Visible = false;
			shot.Hide();
			
			this.shotPool[currShot] = shot;
			
			this.level.CallDeferred(Node2D.MethodName.AddChild, this.shotPool[currShot]);
			
			currShot = currShot + 1;
			
			
			
		} while (currShot < shotPoolSize);
		
		
		this.nextFreeShot = 0;
		
		
	}
	
	
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		this.fillParticlePool();
		this.fillShotPool();
			
	}
	
	
	public Particle getParticle() {
		
		
		if (this.nextFreeParticle == ENTITYPOOLER_EMPTYVALUE) {
			
			GD.Print("Reached max particle");
			
		}
		
		Particle newParticle = this.particlePool[this.nextFreeParticle];
		
		this.nextFreeParticle = newParticle.nextFree;
		
		newParticle.SetProcess(true);
		newParticle.SetPhysicsProcess(true);
		newParticle.Visible = true;
		
		return newParticle;
	}
	
	public Shot getShot() {
		
		Shot newShot = this.shotPool[this.nextFreeShot];
		
		this.nextFreeShot = newShot.nextFree;
		
		newShot.SetProcess(true);
		newShot.SetPhysicsProcess(true);
		newShot.Visible = true;
		
		return newShot;
	}
	
	
	public Bullet getBullet() {
		
		Bullet newBullet = this.bulletPool[this.nextFreeBullet];
		
		this.nextFreeBullet = newBullet.nextFree;
		
		newBullet.SetProcess(true);
		//newBullet.Visible = true;
		newBullet.Show();
		
		return newBullet;
	}
	
	
	
	
	
	public void freeParticle(int indexToFree) {
		
		// Set some members back to default
		this.particlePool[indexToFree].animatedSprite2D.FlipH = false;
		
		this.particlePool[indexToFree].SetProcess(false);
		this.particlePool[indexToFree].SetPhysicsProcess(false);
		this.particlePool[indexToFree].Hide();
		
		// The newly freed Particle now points to the next free one
		this.particlePool[indexToFree].nextFree = this.nextFreeParticle;
		
		// This Particle is now free so it's next in line 
		this.nextFreeParticle = indexToFree;
		
	}
	
	public void freeBullet(int indexToFree) {
		
		this.bulletPool[indexToFree].SetProcess(false);
		this.bulletPool[indexToFree].SetPhysicsProcess(false);
		this.bulletPool[indexToFree].Hide();
		this.bulletPool[indexToFree].nextFree = this.nextFreeBullet;
		this.nextFreeBullet= indexToFree;
		
	}
	
	
	public void freeShot(int indexToFree) {
		
		this.shotPool[indexToFree].SetProcess(false);
		this.shotPool[indexToFree].SetPhysicsProcess(false);
		this.shotPool[indexToFree].Hide();
		this.shotPool[indexToFree].nextFree = this.nextFreeShot;
		this.nextFreeShot = indexToFree;
		
	}
	
	
	
	
	
	
	//======================
	//! ECL V2 wannabe lmao
	//======================
	
	// Might be getting a bit carried away...
	
	
	
	//! Constants
	
	
	//!ZUNBloat: Why is this not a flag lmao
	// Shoot a fan pattern aimed at the player
	public const int ECL_AIMMODE_FAN_AIMED = 0;
	// Shoot a fan pattern with a certain angle 
	public const int ECL_AIMMODE_FAN_ANGLE = 1;
	
	// Two ring modes that work very similarly 
	// A ring aimed at the player
	public const int ECL_AIMMODE_RING_AIM = 2;
	public const int ECL_AIMMODE_RING_STATIC = 3;
	
	// Always aim away from the player whether odd or even
	public const int ECL_AIMMODE_RING_AIMAWAY = 4;
	// The odd version of aim mode 3 (RING_ANGLE), every angle 3 would not have a bullet
	// this one does
	public const int ECL_AIMMODE_RING_LOVESMENOT = 5;
	
	
	public const int ECL_AIMMODE_RAND_AIMEDFAN = 6;
	public const int ECL_AIMMODE_RAND_STATICRING = 7;
	// Eternal Meek
	public const int ECL_AIMMODE_RAND_MEEK = 8;
	
	
	//! Members
	
	// The aim mode of the pattern
	public int aimMode;
	
	// While these two are commonly used as layer and column count respectively
	// it can change depending on pattern 
	public int patternLayerNum;
	public int patternColumnNum;
	
	// Commonly used for max speed and min speed
	public float patternMaxSpeed;
	public float patternMinSpeed;
	
	
	public float patternMainAngle;
	public float patternSeparationAngle;
	
	
	// 600 Series
	
	//ins_600 (etSet) - Useless in our case?
	
	//ins_601 (etOn)
	public void shootPattern() {
		
		
		
	}
	
	
}
