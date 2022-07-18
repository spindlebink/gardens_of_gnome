using Godot;
using System;

public class Flar : ItemType
{
    public override int HowMuchDoILike(ItemType item, GameBoard board)
    {
        if (item is PondS || item is Pond4)
        {
            return LikeALot;
        }
        else if (item is Flar)
        {
            return NoThankYou;
        }
        else
        {
            return LikeALittle;
        }
    }
}
