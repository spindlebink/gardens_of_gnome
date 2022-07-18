using Godot;
using System;

public class Die : Node2D
{
    [Export]
    public float RollStopThreshold = 0.5f;
    [Export]
    public float RollSlowdownSpeed = 0.06f;
    [Export]
    public float RollClickAttenuationRate = 0.35f;
    [Signal]
    public delegate void RollCompleted(int result);
    
    private int _numFaces;
    public int NumFaces
    {
        set
        {
            _numFaces = value;
            if (_sprite.Frame >= _numFaces)
            {
                _sprite.Frame = _numFaces - 1;
            }
        }
    }
    
    private float SpeedyFastRollSpeedDontchaKnow = 0.0005f;
    
    private AnimatedSprite _sprite;
    private AudioStreamPlayer _dieSound;
    private AudioStreamPlayer _dieClick;

    private float _rollSpeed;
    private bool _rolling = false;
    private float _rollAccum = 0.0f;
    private float _initialVolume;
    
    public override void _Ready()
    {
        _sprite = GetNode<AnimatedSprite>("Sprite");
        _numFaces = _sprite.Frames.GetFrameCount("default");
        _dieSound = GetNode<AudioStreamPlayer>("DieSound");
        _dieClick = GetNode<AudioStreamPlayer>("DieClick");

        _initialVolume = _dieClick.VolumeDb;
    }

    public override void _Process(float delta)
    {
        if (_rolling)
        {
            _rollAccum += delta;
            if (_rollAccum >= _rollSpeed)
            {
                _dieClick.Play();
                _dieClick.VolumeDb -= RollClickAttenuationRate;
                _rollAccum = 0.0f;
                _rollSpeed += Mathf.Clamp(GD.Randf(), 0.5f, 1.0f) * RollSlowdownSpeed;
                var prevFrame = _sprite.Frame;
                while (_sprite.Frame == prevFrame)
                {
                    _sprite.Frame = (int) GD.Randi() % _numFaces;
                }
                if (_rollSpeed > RollStopThreshold)
                {
                    _rolling = false;
                    EmitSignal(nameof(RollCompleted), _sprite.Frame);
                }
            }
        }
    }

    // Rolls the die.
    public void Roll()
    {
        _dieClick.VolumeDb = _initialVolume;
        _dieSound.Play();
        _rolling = true;
        _rollSpeed = SpeedyFastRollSpeedDontchaKnow;
    }
}
