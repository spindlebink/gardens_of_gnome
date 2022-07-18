using Godot;
using System;

public class Tree3 : ItemType
{
    public override Vector2[] GetFootprint()
    {
        return new Vector2[]{
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1)
        };
    }

    public override int HowMuchDoILike(ItemType item, GameBoard board)
    {
        if (item is Tree2)
        {
            return LikeALittle;
        }
        else if (item is Tree3)
        {
            return LikeALot;
        }
        else if (item is PondS || item is Pond4)
        {
            return LikeALittle;
        }
        return 0;
    }
}
