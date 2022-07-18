using Godot;
using System;
using System.Collections;

public class Placer : Node2D
{
    [Export]
    public NodePath GameBoardNodePath;

    [Export]
    public float PlacementPositionLerpSpeed = 22.0f;

    [Signal]
    public delegate void ItemPlaced(ItemType itemType, Vector2 cellPosition);

    [Signal]
    public delegate void ItemPlacedNope(Vector2 cellPosition);

    public ItemType PlacementType
    {
        set
        {
            if (_placerItem != null)
            {
                _placerItem.QueueFree();
            }
            _placerItem = (ItemType) value.Duplicate();
            _placerFootprint = _placerItem.GetFootprint();
            _placerNeighbors = ItemType.GetFootprintNeighbors(_placerFootprint);
            _placerFootprintExtents = ItemType.GetFootprintExtents(_placerFootprint);
            _placerItem.DisplayAsValid = false;
            AddChild(_placerItem);
        }
    }

    private GameBoard _gameBoard;

    private ItemType _placerItem;
    private Vector2[] _placerFootprint;
    private ArrayList _placerNeighbors;
    private Rect2 _placerFootprintExtents;
    private Vector2 _placementCorner;
    private bool _valid;
    private bool _placing;
    
    private Vector2 _placementTargetPosition = new Vector2(-1f, -1f);
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _gameBoard = GetNode<GameBoard>(GameBoardNodePath);
    }
    
    // Called each frame.
    public override void _Process(float delta)
    {
        if (_placing)
        {
            var mousePosition = GetViewport().GetMousePosition();
            if (!_gameBoard.CellGlobalBounds.HasPoint(mousePosition)) {
                _placementTargetPosition.x = -1f;
                _placementCorner.x = -1f; // set to -1 to force update of placement corner next time it's in range
                _placerItem.GlobalPosition = mousePosition;
                _placerItem.DisplayAsValid = false;
                _gameBoard.ClearValidityIndicator();
                _valid = false;
            }
            else
            {
                var mouseLocal = _gameBoard.ToLocal(mousePosition);
                var fpCenterOffset = ItemType.GetActualCenterOfFootprintExtents(_placerFootprintExtents, _gameBoard.CellSize);
                var cornerPosition = mouseLocal - fpCenterOffset;
                var clampedCorner = _gameBoard.ClampPosition(cornerPosition);
                var cellCorner = _gameBoard.WorldToMap(clampedCorner);
                
                SetPlacementCorner(cellCorner);

                _placerItem.GlobalPosition = _placerItem.GlobalPosition.LinearInterpolate(
                    _placementTargetPosition,
                    PlacementPositionLerpSpeed * delta
                );
            }
        }
    }
    
    // Activates placement mode on the Placer.
    public void BeginPlacing()
    {
        _placerItem.Visible = true;
        _placing = true;
    }

    public void EndPlacing()
    {
        _placerItem.Visible = false;
        _placing = false;
        _gameBoard.ClearValidityIndicator();
    }

    // Checks whether the current position is a valid one to put an item at, and
    // if so, emits the ItemPlaced signal.
    public void RequestPlacement()
    {
        if (_valid)
        {
            _placerItem.DisplayAsValid = true;
            EmitSignal(nameof(ItemPlaced), _placerItem, _placementCorner);
            _placerItem.DisplayAsValid = false;
            _valid = false;
            _gameBoard.ClearValidityIndicator();
        }
        else
        {
            EmitSignal(nameof(ItemPlacedNope), _placementCorner);
        }
    }
    
    // Sets the corner of the placement indicator and working item type.
    // Recalculates valid placement cells and calls into the game board's validity
    // indicator if the new corner is different from the old one, otherwise does
    // nothing.
    private void SetPlacementCorner(Vector2 corner)
    {
        var fpCenterOffset = ItemType.GetVisualCenterOfFootprintExtents(_placerFootprintExtents, _gameBoard.CellSize);
        var pxCorner = _gameBoard.MapToWorld(corner);
        var gp = _gameBoard.ToGlobal(pxCorner + fpCenterOffset);

        if (_placementTargetPosition.x < 0) {
            _placerItem.GlobalPosition = gp;
        }
        _placementTargetPosition = gp;

        if (corner != _placementCorner)
        {
            var items = _gameBoard.GetUniqueItems(_placerNeighbors, corner);
            if (items.Count == 0 && _gameBoard.NumPlacedItems > 0)
            {
                _placementCorner.x = -1f; // set to -1 to force update of placement corner next time it's in range
                _placerItem.GlobalPosition = GetViewport().GetMousePosition();
//                _placerItem.DisplayAsValid = false;
                _gameBoard.ClearValidityIndicator();
                _valid = false;
            }
            else
            {
                _placementCorner = corner;
                _gameBoard.ClearValidityIndicator();
                _valid = _placerItem.TestPlacement(_gameBoard, _placementCorner);
//                _placerItem.DisplayAsValid = _valid;
            }
        }
    }
}
