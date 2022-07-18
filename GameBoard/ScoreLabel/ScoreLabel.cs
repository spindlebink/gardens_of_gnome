using Godot;
using System;

public class ScoreLabel : Node2D
{    
    [Export]
    public float OffsetRange = 10.0f;
    [Export]
    public Color PositiveColor = new Color(0.45f, 1.0f, 0.65f);
    [Export]
    public Color NegativeColor = new Color(0.96f, 0.45f, 0.45f);

    public int DisplayedScore
    {
        set
        {
            if (value > 0)
            {
                _label.Text = "+" + value;
                Modulate = PositiveColor;
            }
            else
            {
                _label.Text = value.ToString();
                Modulate = NegativeColor;
            }
        }
    }

    private Label _label;
    private AnimationPlayer _animPlayer;

    public override void _Ready()
    {
        _label = GetNode<Label>("Label");
        _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _animPlayer.Connect("animation_finished", this, nameof(OnAnimationFinished));
    }

    public void FloatUp()
    {
        Position += new Vector2(
            (float) GD.Randf() * OffsetRange * 2.0f - OffsetRange,
            (float) GD.Randf() * OffsetRange * 2.0f - OffsetRange
        );
        _animPlayer.Play("Float");
    }
    
    private void OnAnimationFinished(string animName)
    {
        this.QueueFree();
    }
}
