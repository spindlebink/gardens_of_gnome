using Godot;
using System;

public class Flar2 : ItemType
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
        if (item is PondS || item is Pond4)
        {
            return LikeALot;
        }
        if (item is Flar2)
        {
            var totalFlars = 0;
            var nb = board.GetUniqueItems(ItemType.GetFootprintNeighbors(GetFootprint()), PlacedPosition);
            foreach (var neighbor in nb)
            {
                if (neighbor is Flar2 && neighbor != item)
                {
                    totalFlars += 1;
                }
            }
            return totalFlars > 1 ? NoThankYou : OhMyGodILoveIt;
        }
        return 0;
    }
}
