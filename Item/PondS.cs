using Godot;
using System;

public class PondS : ItemType
{
    public override Vector2[] GetFootprint()
    {
        return new Vector2[]{
            new Vector2(1, 0),
            new Vector2(2, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(2, 1),
            new Vector2(0, 2),
            new Vector2(1, 2)
        };
    }
    
    public override int BaseScore()
    {
        return HighBaseScore;
    }

    public override int HowMuchDoILike(ItemType item, GameBoard board)
    {
        if (item is PondS)
        {
            return LikeALot;
        }
        if (item is Pond4)
        {
            return LikeALittle;
        }
        return 0;
    }
}
