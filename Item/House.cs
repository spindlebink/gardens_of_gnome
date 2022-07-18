using Godot;
using System;

public class House : ItemType
{
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
        return HighBaseScore;
    }
    
    public override int HowMuchDoILike(ItemType item, GameBoard board)
    {
        if (item is Gnome)
        {
            return LikeALot;
        }
        return 0;
    }
}
