using Godot;
using System;

public class TotalScoreLabel : Label
{
    [Export]
    public float TransitionTime = 0.65f;
    
    private float _displayedScore;
    public float DisplayedScore
    {
        get { return _displayedScore; }
        set
        {
            _displayedScore = value;
            Text = "Score: " + Mathf.Floor(_displayedScore);
        }
    }

    private float _score;
    public float Score
    {
        get { return _score; }
        set
        {
            _score = value;
            _tween.InterpolateProperty(
                 this,
                 "DisplayedScore",
                 _displayedScore,
                 _score,
                TransitionTime,
                 Tween.TransitionType.Quad,
                 Tween.EaseType.Out
            );
            _tween.Start();
        }
    }

    private Tween _tween;
    
    public override void _Ready()
    {
        _tween = GetNode<Tween>("Tween");
    }
}
