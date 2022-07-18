using Godot;
using System;
using System.Collections;

public class GameBoard : TileMap
{    
    [Export]
    public float PanSpeed = 720.0f;
    [Export]
    public float PanAccel = 26.0f;
    [Export]
    public float ScrollClickVolume = -2.0f;

    [Signal]
    public delegate void ScoreChanged(int amount);

    private int _score = 0;
    public int Score 
    {
        get {return _score;} 
        private set 
        {
            _score = value;
            EmitSignal(nameof(ScoreChanged), value);
        }
    }
    
    public int NumPlacedItems
    {
        get { return _placedItems.GetChildCount(); }
    }

    public Rect2 CellGlobalBounds;
    
    private ItemType[,] _itemGrid;
    private Vector2 _currentPanRate;
    private CellValidityIndicator _validityIndicator;
    private Node2D _placedItems;
    private PackedScene _scoreLabelScene = GD.Load("res://GameBoard/ScoreLabel/ScoreLabel.tscn") as PackedScene;
    private AudioStreamPlayer2D _scrollClick;

    private ItemType _lastPostedScoreFrom;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ClampPositionToWindow();
        _placedItems = GetNode<Node2D>("PlacedItems");
        _validityIndicator = GetNode<CellValidityIndicator>("CellValidityIndicator");
        _validityIndicator.CellSize = CellSize;
        _validityIndicator.Position = GetUsedRect().Position * CellSize;
        _scrollClick = GetNode<AudioStreamPlayer2D>("ScrollClick");
        
        var ur = GetUsedRect();
        GetNode<GridLines>("GridLines").WidthInCells = (int) ur.Size.x - 2;
        GetNode<GridLines>("GridLines").HeightInCells = (int) ur.Size.y - 2;
        
        _itemGrid = new ItemType[(int) ur.Size.x + (int) ur.Position.x, (int) ur.Size.y + (int) ur.Position.y];
        
        var winSize = GetViewport().GetVisibleRect().Size;
        Position = new Vector2(
            winSize.x * 0.5f - ur.Size.x * 0.5f * CellSize.x,
            winSize.y * 0.5f - ur.Size.y * 0.5f * CellSize.y
        );
    }

    // Called each frame.
    public override void _Process(float delta)
    {
        var inputX = Input.GetAxis("left", "right");
        var inputY = Input.GetAxis("up", "down");
        
        _currentPanRate = _currentPanRate.LinearInterpolate(
            new Vector2(inputX, inputY) * PanSpeed,
            PanAccel * delta
        );
        _scrollClick.VolumeDb = Mathf.Lerp(-80.0f, ScrollClickVolume, Mathf.Clamp(_currentPanRate.Length() / PanSpeed, 0.0f, 1.0f));
        
        GlobalPosition -= _currentPanRate * delta;

        ClampPositionToWindow();

        var cellExtents = GetUsedRect();
        CellGlobalBounds = new Rect2(
            GlobalPosition + cellExtents.Position * CellSize,
            cellExtents.Size * CellSize
        );
    }
    
    public bool IsCellWithinBounds(Vector2 cell)
    {
        var ur = GetUsedRect();
        return cell.x >= ur.Position.x && cell.y >= ur.Position.y && cell.x < ur.Position.x + ur.Size.x && cell.y < ur.Position.y + ur.Size.y;
    }
    
    // Returns the item, if any, at `cellPosition`.
    public ItemType ItemAtCell(Vector2 cellPosition)
    {
        if (IsCellWithinBounds(cellPosition))
        {
            var x = (int) cellPosition.x;
            var y = (int) cellPosition.y;
            return _itemGrid[x, y];
        }
        else
        {
            return null;
        }
    }
    
    // Collects an array list of unique items currently placed in cells.
    public ArrayList GetUniqueItems(ArrayList cells, Vector2 offset)
    {
        ArrayList items = new ArrayList();
        foreach (Vector2 cell in cells)
        {
            var cellOffsetted = cell + offset;
            var item = ItemAtCell(cellOffsetted);
            if (item != null && !items.Contains(item))
            {
                items.Add(item);
            }
        }
        return items;
    }

    // Floats up a score indicator from `atItem` with value `score`.
    public void PostScore(ItemType atItem, int score)
    {
        if (score == 0)
        {
            return;
        }

        ScoreLabel label = _scoreLabelScene.Instance() as ScoreLabel;
        AddChild(label);
        label.Position = atItem.Position;
        if (atItem == _lastPostedScoreFrom)
        {
            label.Position += new Vector2(0.0f, -20.0f);
        }
        
        label.DisplayedScore = score;
        label.FloatUp();
        _score += score;
        _lastPostedScoreFrom = atItem;
    }

    // Places an item at `cellPosition`.
    public void PlaceItem(ItemType item, Vector2 cellPosition)
    {
        var footprint = item.GetFootprint();
        _placedItems.AddChild(item);
        
        item.PlacedPosition = cellPosition;
        item.Position = MapToWorld(cellPosition) + ItemType.GetVisualCenterOfFootprintExtents(
            ItemType.GetFootprintExtents(footprint),
            CellSize
        );
        
        // Mark each cell in the footprint as belonging to this item.
        foreach (var fpCellPosition in footprint)
        {
            var x = (int) (fpCellPosition.x + cellPosition.x);
            var y = (int) (fpCellPosition.y + cellPosition.y);
            _itemGrid[x, y] = item;
            _validityIndicator.PlacedCells.Add(fpCellPosition + cellPosition);
        }
        
        item.AnimatePlaced();
        item.PlayPlacementSound();
        item.IJustGotPlaced(this, cellPosition);

        item.ResolveScore(this);
        Score = _score; // trigger score changed signal

        _validityIndicator.Update();
    }

    // Clears the validity indicator's displayed cells.
    public void ClearValidityIndicator()
    {
        _validityIndicator.ValidCells.Clear();
        _validityIndicator.InvalidCells.Clear();
        _validityIndicator.Update();
    }
    
    // Marks a cell as valid in the validity indicator.
    public void MarkValid(Vector2 cellPosition)
    {
        _validityIndicator.ValidCells.Add(cellPosition);
        _validityIndicator.Update();
    }
    
    // Marks a cell as invalid in the validity indicator.
    public void MarkInvalid(Vector2 cellPosition)
    {
        _validityIndicator.InvalidCells.Add(cellPosition);
        _validityIndicator.Update();
    }

    // Checks whether a cell is empty. A cell is currently defined to be empty
    // (that is, a valid place to put an item) if it is:
    // * Not an undefined tile
    // * Currently lacking an item on it
    // * Tile index 0 (grass/ground)
    public bool IsCellEmpty(Vector2 cell)
    {
        var tiles = GetUsedRect();
        if (cell.x < tiles.Position.x || cell.y < tiles.Position.y || cell.x > tiles.Position.x + tiles.Size.x || cell.y > tiles.Position.y + tiles.Size.y)
        {
            return false;
        }
        int cellValue = GetCellv(cell);
        if (cellValue != TileMap.InvalidCell)
        {
            if (_itemGrid[(int) cell.x, (int) cell.y] != null)
            {
                return false;
            }
            return cellValue == 0;
        }
        else
        {
            return false;
        }
    }

    // Clamps a local position to the boundaries of the game board's cells.
    public Vector2 ClampPosition(Vector2 localPosition)
    {
        var pos = localPosition;
        
        var cellExtents = GetUsedRect();
        var min = cellExtents.Position * CellSize;
        var max = (cellExtents.Position + cellExtents.Size) * CellSize;
        
        return new Vector2(
            Mathf.Clamp(localPosition.x, min.x, max.x),
            Mathf.Clamp(localPosition.y, min.y, max.y)
        );
    }
    
    // Clamps the game board's position to ensure that its cells are always
    // visible within the window.
    private void ClampPositionToWindow()
    {
        var cellExtents = GetUsedRect();

        var leftTopCellOffset = cellExtents.Position * CellSize;
        var cellBoundaries = cellExtents.Size * CellSize;

        var windowRect = GetViewport().GetVisibleRect();
        var windowCenter = windowRect.Position + windowRect.Size * 0.5f;
        
        var minLimit = windowCenter - leftTopCellOffset - cellBoundaries;
        var maxLimit = minLimit + cellBoundaries;
        
        var gp = GlobalPosition;
        gp.x = Mathf.Round(Mathf.Clamp(gp.x, minLimit.x, maxLimit.x));
        gp.y = Mathf.Round(Mathf.Clamp(gp.y, minLimit.y, maxLimit.y));
        GlobalPosition = gp;
    }
}
