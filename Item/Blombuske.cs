using Godot;
using System;

public class Blombuske : ItemType
{
    public override int HowMuchDoILike(ItemType item, GameBoard board)
    {
        return OhMyGodILoveIt;
    }
}
