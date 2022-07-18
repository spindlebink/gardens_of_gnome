using Godot;
using System;

public class Trawtraw : ItemType
{
    public override int HowMuchDoILike(ItemType item, GameBoard board)
    {
        if (item is PondS || item is Pond4)
        {
            return HateIt;
        }
        else if (item is Trawtraw || item is Bush)
        {
            return LikeALittle;
        }
        return 0;
    }
}
