using Godot;
using System;

public class Cattails : ItemType
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
        return MidBaseScore;
    }
    
    public override int HowMuchDoILike(ItemType item, GameBoard board)
    {
        if (item is Pond4 || item is PondS)
        {
            return OhMyGodILoveIt;
        }
        else if (item is Cattails)
        {
            return LikeALot;
        }
        return 0;
    }
}
