using Godot;
using System;

public class Tree4 : ItemType
{
    public override Vector2[] GetFootprint()
    {
        return new Vector2[]{
            new Vector2(0, 0),
            new Vector2(0, 1)
        };
    }

    public override int HowMuchDoILike(ItemType item, GameBoard board)
    {
        if (item is Pond4 || item is PondS)
        {
            return OhMyGodILoveIt;
        }
        else if (item is Tree4)
        {
            return HateIt;
        }
        else if (item is Tree3 || item is Tree2 || item is Tree1)
        {
            return NoThankYou;
        }
        return 0;
    }
}
