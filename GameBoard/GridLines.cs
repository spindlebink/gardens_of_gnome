using Godot;
using System;

public class GridLines : Node2D
{
    [Export]
    private int CellSize = 64;
    [Export]
    private Color Color;

    private int _cellsW = 0;
    public int WidthInCells
    {
        set
        {
            _cellsW = value;
            Update();
        }
    }
    
    private int _cellsH = 0;
    public int HeightInCells
    {
        set
        {
            _cellsH = value;
            Update();
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }
    
    public override void _Draw()
    {
        //draw columns
        for(float x = 0; x < _cellsW; x++)
        {
            Vector2 from = new Vector2(x * CellSize, 0);
            Vector2 to = new Vector2(x * CellSize, _cellsH * CellSize);
            DrawLine(from, to, Color);
        }
        
        //draw rows
        for(float y = 0; y <= _cellsH; y++)
        {
            Vector2 from = new Vector2(0, y * CellSize);
            Vector2 to = new Vector2(_cellsW * CellSize, y * CellSize);
            DrawLine(from, to, Color);
        }
    }
}
