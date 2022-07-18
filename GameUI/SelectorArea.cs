using Godot;
using System;
using System.Collections;

public class SelectorArea : Control
{
    [Signal]
    public delegate void BeginTurn();
    [Signal]
    public delegate void ItemTypeSelected(ItemType itemType, ItemTypeSelector selector);
    [Signal]
    public delegate void ItemCountChosen(int count);
    [Signal]
    public delegate void TossRemainder();
    
    public int NumItemsToPickPerTurn = 2;
    
    public int TossLives
    {
        set
        {
            if (value == 0)
            {
                _tossButton.Text = "Finish";
            }
            else
            {
                _tossButton.Text = "Toss the Rest (" + value + ")";
            }
        }
    }
    
    private string _extendedDescription;
    public string ExtendedDescription
    {
        set
        {
            _extendedDescription = value;
            _extendedDescriptionLabel.BbcodeText = value;
        }
    }
    
    private Control _beginTurn;
    private Control _itemTypes;
    private Control _placingItems;
    private RichTextLabel _extendedDescriptionLabel;
    private Die _itemCountDie;
    private PlacedLabel _placedLabel;
    private Button _tossButton;
    
    private ArrayList _chances = new ArrayList();
    private AudioStreamPlayer _click;
    private AudioStreamPlayer _tossThump;
    private bool _tutorial = true;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _beginTurn = FindNode("BeginTurn") as Control;
        _itemTypes = FindNode("ItemTypes") as Control;
        _placingItems = FindNode("PlacingItems") as Control;
        _click = FindNode("SelectionClick") as AudioStreamPlayer;
        _extendedDescriptionLabel = FindNode("ExtendedDescriptionLabel") as RichTextLabel;
        _itemCountDie = FindNode("ItemCountDie") as Die;
        _placedLabel = FindNode("PlacedItemsLabel") as PlacedLabel;
        _tossThump = GetNode<AudioStreamPlayer>("TossThump");
        
        _tossButton = FindNode("TossButton") as Button;
        _tossButton.Connect("pressed", this, nameof(OnTossButtonPressed));
        
        foreach (Node selector in _itemTypes.GetChildren())
        {
            if (selector is ItemTypeSelector)
            {
                var itemSelector = selector as ItemTypeSelector;
                itemSelector.Connect(nameof(ItemTypeSelector.ItemTypeSelected), this, nameof(OnItemTypeSelected));
                for (int i = 0; i < itemSelector.RandomizationChance; i += 1)
                {
                    _chances.Add(itemSelector);
                }
            }
        }
        
        _itemCountDie.Connect(nameof(Die.RollCompleted), this, nameof(OnItemCountDieRollCompleted));
        
        // button signals are borked in C#, we need to use this name
        (FindNode("BeginTurnButton") as Button).Connect("pressed", this, nameof(OnBeginTurnButtonPressed));
    }
    
    public void HideTutorial()
    {
        _tutorial = false;
        (FindNode("ItemTypesTutorialLabel") as Label).Text = "We've got a fresh batch of things to place! Pick one:";
        (FindNode("PlacementTutorialLabel") as Control).Visible = false;
    }
    
    public void ActivateBeginTurn()
    {
        _beginTurn.Visible = true;
        _itemTypes.Visible = false;
        _placingItems.Visible = false;
    }
    
    public void ActivateItemTypeSelection()
    {
        ResetVisibleItemTypes();
        ChooseAvailableItemTypes();
        _beginTurn.Visible = false;
        _itemTypes.Visible = true;
        _placingItems.Visible = false;
    }
    
    public void ActivateItemCountChoose()
    {
        _tossButton.Disabled = true;
        _placedLabel.HuhhMode = true;
        _beginTurn.Visible = false;
        _itemTypes.Visible = false;
        _placingItems.Visible = true;
    }
    
    public void ActivateGameEnd()
    {
        _beginTurn.Visible = false;
        _itemTypes.Visible = false;
        _placingItems.Visible = false;
        (FindNode("GameEnd") as Control).Visible = true;
    }
    
    public void ChooseAvailableItemTypes()
    {
        var numPicked = 0;
        if (_tutorial)
        {
            var gnomeSelector = FindNode("Gnome") as ItemTypeSelector;
            gnomeSelector.IsPickedThisTurn = true;
            gnomeSelector.Visible = true;
            numPicked += 1;
        }
        while (numPicked < NumItemsToPickPerTurn)
        {
            var index = Mathf.Abs(((int) GD.Randi()) % _chances.Count);
            ItemTypeSelector s = _chances[index] as ItemTypeSelector;
            if (!s.IsPickedThisTurn)
            {
                s.IsPickedThisTurn = true;
                s.Visible = true;
                numPicked += 1;
            }
        }
    }
    
    public void ChooseItemCount()
    {
        // eventually: animate a dice roll/whatever visual stuff we want to do,
        // emit the signal on completion
        _itemCountDie.Roll();
    }
    
    public void OnItemCountDieRollCompleted(int result)
    {
        EmitSignal(nameof(ItemCountChosen), result + 1);
        _tossButton.Disabled = false;
        _placedLabel.HuhhMode = false;
    }
    
    public void OnBeginTurnButtonPressed()
    {
//        _click.Play();
        EmitSignal(nameof(BeginTurn));
    }
    
    public void OnTossButtonPressed()
    {
        _tossThump.Play();
        EmitSignal(nameof(TossRemainder));
    }
    
    public void OnItemTypeSelected(ItemType itemType, ItemTypeSelector selector)
    {
        _click.Play();
        _itemCountDie.NumFaces = selector.MaxPlacementsPerChoice;
        EmitSignal(nameof(ItemTypeSelected), itemType, selector);
    }

    private void ResetVisibleItemTypes()
    {
        foreach (Node selector in _itemTypes.GetChildren())
        {
            if (selector is ItemTypeSelector)
            {
                (selector as ItemTypeSelector).IsPickedThisTurn = false;
                (selector as ItemTypeSelector).Visible = false;
            }
        }
    }
}
