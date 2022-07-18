using Godot;
using System;

public class Clover : ItemType
{
    public override int HowMuchDoILike(ItemType item, GameBoard board)
    {
        if (item is Tree2 || item is Tree3)
        {
            return LikeALittle;
        }
        else if (item is Pond4 || item is PondS)
        {
            return LikeALittle;
        }
        return 0;
    }
}
