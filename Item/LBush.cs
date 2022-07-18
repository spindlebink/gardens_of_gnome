using Godot;
using System;

public class LBush : ItemType
{
    public override Vector2[] GetFootprint()
    {
        return new Vector2[]{
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1)
        };
    }
    
    public override int BaseScore()
    {
        return HighBaseScore;
    }

    public override int HowMuchDoILike(ItemType item, GameBoard board)
    {
        if (item is Tree3 || item is Tree2 || item is LBush)
        {
            return NoThankYou;
        }
        else if (item is PondS || item is Pond4)
        {
            return LikeALittle;
        }
        return 0;
    }
}
