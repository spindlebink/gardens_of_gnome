using Godot;
using System;

public class Mymbleleaf : ItemType
{
    public override int BaseScore()
    {
        return HighBaseScore * 2;
    }

    public override int HowMuchDoILike(ItemType item, GameBoard board)
    {
        return 0;
    }
}
