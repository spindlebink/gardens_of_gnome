using Godot;
using System;

public class Rock : ItemType
{
    public override Vector2[] GetFootprint()
    {
        return new Vector2[]{
            new Vector2(0, 0)
        };
    }

    public override int HowMuchDoILike(ItemType item, GameBoard board)
    {
        if (item is BigRock)
        {
            return LikeALot;
        }
        return 0;
    }
}
