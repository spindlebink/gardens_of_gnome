using Godot;
using System;

public class SpriteRandomizer : Node
{
    [Export]
    public bool Flippable = true;
    
    public override void _Ready()
    {
        foreach (Sprite child in GetChildren())
        {
            child.Visible = false;
        }
        var spriteIndex = Mathf.Abs((int) GD.Randi() % GetChildCount());
        (GetParent() as Sprite).Texture = (GetChild(spriteIndex) as Sprite).Texture;
        if (Flippable)
        {
            (GetParent() as Sprite).Scale = new Vector2((float)(1.0 - Mathf.Round(GD.Randf()) * 2.0), 1f) * (GetParent() as Sprite).Scale;
        }
    }
}
