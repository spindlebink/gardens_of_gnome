using Godot;
using System;

public class Tree1 : ItemType
{
    public override void IJustGotPlaced(GameBoard board, Vector2 corner)
    {
        var scaleOffset = (float) GD.RandRange(-0.125, 0.125);
        GetNode<Sprite>("Sprite").Scale += new Vector2(scaleOffset, scaleOffset);
    }
    
    public override Vector2[] GetFootprint()
    {
        return new Vector2[]{
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
    }

    public override int BaseScore()
    {
        return MidBaseScore;
    }
}
