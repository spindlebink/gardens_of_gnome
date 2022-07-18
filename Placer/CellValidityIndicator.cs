using Godot;
using System;
using System.Collections;

public class CellValidityIndicator : Node2D
{
    [Export]
    public StyleBox ValidCellIndicator;
    [Export]
    public StyleBox InvalidCellIndicator;
    [Export]
    public StyleBox PlacedCellIndicator;
    [Export]
    public float CellIndicatorMargin = 2.0f;

    public Vector2 CellSize = new Vector2(64.0f, 64.0f);

    public ArrayList ValidCells = new ArrayList();
    public ArrayList InvalidCells = new ArrayList();
    public ArrayList PlacedCells = new ArrayList();

    // Called when the node has requested a redraw using `.Update()`.
    public override void _Draw()
    {
        foreach (Vector2 cellPosition in ValidCells)
        {
            var cellBounds = new Rect2(cellPosition * CellSize, CellSize).Grow(-CellIndicatorMargin);
            DrawStyleBox(ValidCellIndicator, cellBounds);
        }
        
        foreach (Vector2 cellPosition in InvalidCells)
        {
            var cellBounds = new Rect2(cellPosition * CellSize, CellSize).Grow(-CellIndicatorMargin);
            DrawStyleBox(InvalidCellIndicator, cellBounds);
        }
        
        foreach (Vector2 cellPosition in PlacedCells)
        {
            var cellBounds = new Rect2(cellPosition * CellSize, CellSize).Grow(-CellIndicatorMargin);
            DrawStyleBox(PlacedCellIndicator, cellBounds);
        }
    }
}
