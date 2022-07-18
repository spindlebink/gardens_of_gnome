using Godot;
using System;

public class FootprintIndicator : Control
{
    [Export]
    public StyleBox IndicatorStylebox;
    [Export]
    public float IndicatorMargin = 0.0f;
    
    private Vector2 _indicatorCellSize = new Vector2(0.1f, 0.1f);
    private Vector2[] _footprint = new Vector2[0];
    public Vector2[] FootprintToIndicate
    {
        set
        {
            _footprint = value;
            var extents = ItemType.GetFootprintExtents(_footprint);
            _indicatorCellSize = GetRect().Size / extents.Size;
            if (extents.Size.y == 1)
            {
                _indicatorCellSize.y = _indicatorCellSize.x;
            }
            else if (extents.Size.x == 1)
            {
                _indicatorCellSize.x = _indicatorCellSize.y;
            }
            Update();
        }
    }
    
    public override void _Draw()
    {
        foreach (var cell in _footprint)
        {
            DrawStyleBox(
                IndicatorStylebox,
                new Rect2(
                    cell * _indicatorCellSize,
                    _indicatorCellSize
                ).Grow(-IndicatorMargin)
            );
        }
    }
}
