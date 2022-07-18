using Godot;
using System;

public class Pond4 : ItemType
{
    public override Vector2[] GetFootprint()
    {
        return new Vector2[]{
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(2, 0),
            new Vector2(3, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(2, 1),
            new Vector2(3, 1),
            new Vector2(0, 2),
            new Vector2(1, 2),
            new Vector2(2, 2),
            new Vector2(3, 2),
            new Vector2(0, 3),
            new Vector2(1, 3),
            new Vector2(2, 3),
            new Vector2(3, 3),
        };
    }
    
    public override int BaseScore()
    {
        return HighBaseScore;
    }
    
    public override int HowMuchDoILike(ItemType item, GameBoard board)
    {
        if (item is Pond4 || item is PondS)
        {
            return LikeALot;
        }
        return 0;
    }
}
