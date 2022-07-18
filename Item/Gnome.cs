using Godot;
using System;

public class Gnome : ItemType
{
    // Default GetFootprint() is for a 1x1, so no need to override

    public override int HowMuchDoILike(ItemType item, GameBoard board)
    {
        if (item is Pond4 || item is PondS)
        {
            return HateIt;
        }
        else if (item is Rock)
        {
            return LikeALittle;
        }
        else if (item is BigRock)
        {
            return LikeALot;
        }
        else if (item is Clover)
        {
            return OhMyGodILoveIt;
        }
        return 0;
    }
}
