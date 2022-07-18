using Godot;
using System;

public class Bush : ItemType
{
    public override Vector2[] GetFootprint()
    {
        return new Vector2[]{
            new Vector2(0, 0),
            new Vector2(1, 0),
        };
    }

    public override int HowMuchDoILike(ItemType item, GameBoard board)
    {
        if (item is Bush)
        {
            var totalBushes = 0;
            var nb = board.GetUniqueItems(ItemType.GetFootprintNeighbors(GetFootprint()), PlacedPosition);
            foreach (var neighbor in nb)
            {
                if (neighbor is Bush && neighbor != item)
                {
                    totalBushes += 1;
                }
            }
            return totalBushes + LikeALittle;
        }
        return 0;
    }
}
