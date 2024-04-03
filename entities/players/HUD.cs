using Godot;
using System;

public partial class HUD : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
	
	
	public override void _PhysicsProcess(double delta) {
		
		
		// Draw Frames Per Second
		GetNode<Label>("FramesPerSecond").Text = Engine.GetFramesPerSecond().ToString();
		
		// Draw Ticks Per Second
		GetNode<Label>("TicksPerSecond").Text = Engine.PhysicsTicksPerSecond.ToString();
	}
	
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
