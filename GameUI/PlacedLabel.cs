using Godot;
using System;

public class PlacedLabel : Label
{
    private bool _huhh;
    public bool HuhhMode
    {
        set
        {
            _huhh = value;
            if (value)
            {
                this.Text = "Rolling....";
            }
            else
            {
                UpdateText();
            }
        }
    }
    
    private int _numberOfRequiredPlacements = 0;
    private int _numberOfPlacedObjects = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var game = this.GetNode("/root/Game");
        game.Connect(nameof(Game.NumberOfRequiredPlacementsChanged), 
            this, nameof(OnNumberOfRequiredPlacementsChanged));
        game.Connect(nameof(Game.NumberOfPlacedObjectsChanged), 
            this, nameof(OnNumberOfPlacedObjectsChanged));
    }
    
    private void OnNumberOfRequiredPlacementsChanged(int newValue)
    {
        _numberOfRequiredPlacements = newValue;
        UpdateText();
    }
    
    private void OnNumberOfPlacedObjectsChanged(int newValue)
    {
        _numberOfPlacedObjects = newValue;
        UpdateText();
    }
    
    private void UpdateText()
    {
        this.Text = "Placed " + _numberOfPlacedObjects.ToString() + "/" +
            _numberOfRequiredPlacements.ToString();
    }
}
