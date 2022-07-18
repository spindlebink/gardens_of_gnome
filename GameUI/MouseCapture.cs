using Godot;
using System;

public class MouseCapture : Control
{
    [Signal]
    public delegate void Clicked();
    
    public override void _GuiInput(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseButton)
        {
            var mouseEvent = inputEvent as InputEventMouseButton;
            if (mouseEvent.Pressed)
            {
                EmitSignal(nameof(Clicked));
            }
        }
    }
}
