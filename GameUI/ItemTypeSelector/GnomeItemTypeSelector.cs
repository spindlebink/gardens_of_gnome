using Godot;
using System;

public class GnomeItemTypeSelector : ItemTypeSelector
{
    private string _originalExtDesc;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        _originalExtDesc = ExtendedDescription;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
