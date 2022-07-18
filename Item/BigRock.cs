using Godot;
using System;

public class BigRock : ItemType
{
    public override Vector2[] GetFootprint()
    {
        return new Vector2[]{
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(2, 1),
            new Vector2(1, 2),
        };
    }
    
    public override int BaseScore()
    {
        return HighBaseScore;
    }

    public override int HowMuchDoILike(ItemType item, GameBoard board)
    {
        if (item is BigRock)
        {
            return HateIt;
        }
        else if (item is Rock)
        {
            return LikeALittle;
        }
        return 0;
    }
}
