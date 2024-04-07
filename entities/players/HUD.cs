using Godot;
using System;

public partial class HUD : CanvasLayer
{
	
	
	// The player the HUD is showing info about
	PlatformerPlayerBase player;
	
	// The various little members that make the HUD
	
	public TextureRect inputUp;
	public TextureRect inputDown;
	public TextureRect inputLeft;
	public TextureRect inputRight;
	public TextureRect inputDash;
	public TextureRect inputShoot;
	public TextureRect inputMelee;
	public TextureRect inputJump;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		
		this.player = this.GetParent<Node2D>().GetNode<PlatformerPlayerBase>("PlatformerPlayer");
		
		
		this.inputUp = this.GetNode<TextureRect>("InputUp");
		this.inputDown = this.GetNode<TextureRect>("InputDown");
		this.inputLeft = this.GetNode<TextureRect>("InputLeft");
		this.inputRight = this.GetNode<TextureRect>("InputRight");
		this.inputDash = this.GetNode<TextureRect>("InputDash");
		this.inputShoot = this.GetNode<TextureRect>("InputShoot");
		this.inputMelee = this.GetNode<TextureRect>("InputMelee");
		this.inputJump = this.GetNode<TextureRect>("InputJump");
		
		
		// Set the input display
		
		Vector2 inputDisplayPos = new Vector2(300, 950);
		
		inputDisplayPos.Y = inputDisplayPos.Y - 40;	
		this.inputUp.Position = inputDisplayPos;
		inputDisplayPos.Y = inputDisplayPos.Y + 80;	
		this.inputDown.Position = inputDisplayPos;
		
		inputDisplayPos.Y = inputDisplayPos.Y - 40;	
		inputDisplayPos.X = inputDisplayPos.X - 40;	
		this.inputLeft.Position = inputDisplayPos;
		
		inputDisplayPos.X = inputDisplayPos.X + 80;	
		this.inputRight.Position = inputDisplayPos;
		
		
		inputDisplayPos.X = inputDisplayPos.X + 100;	
		this.inputDash.Position = inputDisplayPos;
		inputDisplayPos.X = inputDisplayPos.X + 40;	
		this.inputShoot.Position = inputDisplayPos;
		inputDisplayPos.X = inputDisplayPos.X + 40;	
		this.inputMelee.Position = inputDisplayPos;
		inputDisplayPos.X = inputDisplayPos.X + 40;	
		this.inputJump.Position = inputDisplayPos;
		
		
		this.inputUp.Hide();
		this.inputDown.Hide();
		this.inputLeft.Hide();
		this.inputRight.Hide();
		this.inputDash.Hide();
		this.inputShoot.Hide();
		this.inputMelee.Hide();
		this.inputJump.Hide();
		
		
	}
	
	
	public override void _PhysicsProcess(double delta) {
		
		
		// Draw Frames Per Second
		GetNode<Label>("FramesPerSecond").Text = "FPS:" + Engine.GetFramesPerSecond().ToString();
		
		// Draw Ticks Per Second
		GetNode<Label>("TicksPerSecond").Text = "TPS:" + Engine.PhysicsTicksPerSecond.ToString();
		
		
		//! TODO: Improve this
		
	
		
		
		float storeX;
		
		// Change sprite for input display
		int playerKeystates = this.player.keystates;
		
		int[] pressLookUp = {0, 32};
		
		int isUpPressed = playerKeystates &  PlatformerPlayerBase.PLAYER_INPUTFLAG_UP;
		
		this.inputUp.Hide();
		this.inputDown.Hide();
		this.inputLeft.Hide();
		this.inputRight.Hide();
		this.inputDash.Hide();
		this.inputShoot.Hide();
		this.inputMelee.Hide();
		this.inputJump.Hide();
		
		
		//if ((playerKeystates & PlatformerPlayerBase.PLAYER_INPUTFLAG_UP) == PlatformerPlayerBase.PLAYER_INPUTFLAG_UP) {
		if (Input.IsActionPressed("INPUT_UP")) {
			this.inputUp.Show();
			//this.GetNode<TextureRect>("InputUp").Show();
		}
		
		
		//if ((playerKeystates & PlatformerPlayerBase.PLAYER_INPUTFLAG_DOWN) == PlatformerPlayerBase.PLAYER_INPUTFLAG_DOWN) {
		if (Input.IsActionPressed("INPUT_DOWN")) {
			this.inputDown.Show();
		}
		
		//if ((playerKeystates & PlatformerPlayerBase.PLAYER_INPUTFLAG_LEFT) == PlatformerPlayerBase.PLAYER_INPUTFLAG_LEFT) {
		if (Input.IsActionPressed("INPUT_LEFT")) {	
			this.inputLeft.Show();
		}
		
		
		//if ((playerKeystates & PlatformerPlayerBase.PLAYER_INPUTFLAG_RIGHT) == PlatformerPlayerBase.PLAYER_INPUTFLAG_RIGHT) {
		if (Input.IsActionPressed("INPUT_RIGHT")) {	
			this.inputRight.Show();
		}
		
		
		//if ((playerKeystates & PlatformerPlayerBase.PLAYER_INPUTFLAG_DASH) == PlatformerPlayerBase.PLAYER_INPUTFLAG_DASH) {
		if (Input.IsActionPressed("INPUT_DASH")) {	
			this.inputDash.Show();
		}
		
		//if ((playerKeystates & PlatformerPlayerBase.PLAYER_INPUTFLAG_SHOOT) == PlatformerPlayerBase.PLAYER_INPUTFLAG_SHOOT) {
		if (Input.IsActionPressed("INPUT_SHOOT")) {	
			this.inputShoot.Show();
		}
		
		
		//if ((playerKeystates & PlatformerPlayerBase.PLAYER_INPUTFLAG_MELEE) == PlatformerPlayerBase.PLAYER_INPUTFLAG_MELEE) {
		if (Input.IsActionPressed("INPUT_MELEE")) {
			this.inputMelee.Show();
		}
		
		//if ((playerKeystates & PlatformerPlayerBase.PLAYER_INPUTFLAG_JUMP) == PlatformerPlayerBase.PLAYER_INPUTFLAG_JUMP) {
		if (Input.IsActionPressed("INPUT_JUMP")) {	
			this.inputJump.Show();
		}
		
		
	}
	
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
