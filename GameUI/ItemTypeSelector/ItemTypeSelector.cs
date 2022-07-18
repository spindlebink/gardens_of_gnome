using Godot;
using System;

public class ItemTypeSelector : Control
{
    [Export]
    public int RandomizationChance = 1;
    [Export]
    public int MaxPlacementsPerChoice = 6;
    [Export]
    public string TypeName;
    [Export(PropertyHint.MultilineText)]
    public string PlacementDescription;
    [Export(PropertyHint.MultilineText)]
    public string ExtendedDescription;

    [Signal]
    public delegate void ItemTypeSelected(ItemType type, ItemTypeSelector selector);

    // Initialized on ready if there's a node named ItemType as a child
    public ItemType Item;
    // Flag used from SelectorArea
    public bool IsPickedThisTurn;
    
    private FootprintIndicator _footprintIndicator;
    private AudioStreamPlayer _click;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ((RichTextLabel) FindNode("TypeLabel")).BbcodeText = "[b]" + TypeName + "[/b]: " + PlacementDescription;
        
        _footprintIndicator = FindNode("FootprintIndicator") as FootprintIndicator;
        _click = FindNode("HoverClick") as AudioStreamPlayer;

        Connect("mouse_entered", this, nameof(OnMouseEntered));
        Connect("mouse_exited", this, nameof(OnMouseExited));
        
        if (HasNode("ItemType"))
        {
            Item = GetNode<ItemType>("ItemType");
            Item.Visible = false;
            if (Item.HasNode("Sprite"))
            {
                ((TextureRect) FindNode("Icon")).Texture = Item.GetNode<Sprite>("Sprite").Texture;
            }
            _footprintIndicator.FootprintToIndicate = Item.GetFootprint();
        }
        else
        {
            GD.Print("Warning: No item type child on " + Name);
        }
        
        if (ExtendedDescription == null)
        {
            ExtendedDescription = PlacementDescription;
        }
        ExtendedDescription = "[b]" + TypeName + "[/b]: " + ExtendedDescription;
    }
    
    public virtual void IGotPicked()
    {
    }
    
    // Called when the node receives a GUI input event.
    public override void _GuiInput(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseButton)
        {
            var mouseEvent = inputEvent as InputEventMouseButton;
            if (mouseEvent.Pressed)
            {
                if ((ButtonList) mouseEvent.ButtonIndex == ButtonList.Left)
                {
                    if (Item != null)
                    {
                        _footprintIndicator.Visible = false;
                        IGotPicked();
                        EmitSignal(nameof(ItemTypeSelected), Item, this);
                    }
                    else
                    {
                        GD.PushError("No item type child on " + Name);
                    }
                }
            }
        }
    }
    
    private void OnMouseEntered()
    {
        _footprintIndicator.Visible = true;
        _click.Play();
    }
    
    private void OnMouseExited()
    {
        _footprintIndicator.Visible = false;
    }
}
