using Godot;
using System;

public class AudioPitchVariation : AudioStreamPlayer2D
{
    public override void _Ready()
    {
        PitchScale = (float) GD.RandRange(0.95, 1.15);
    }
}
