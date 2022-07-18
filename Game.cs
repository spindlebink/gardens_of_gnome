using Godot;
using System;

public class Game : Control
{
    [Signal]
    public delegate void NumberOfRequiredPlacementsChanged(int newValue);
    [Signal]
    public delegate void NumberOfPlacedObjectsChanged(int newValue);
    
    public enum Phase
    {
        TurnWillStart,
        ItemTypeSelection,
        ItemCountChoosing,
        ItemPlacement,
        Tossed,
        AllPlaced,
        GameEnd
    }

    private int _livesLeft;
    private GameBoard _gameBoard;
    private SelectorArea _selectorArea;
    private Placer _placer;
    private TotalScoreLabel _scoreLabel;

    private Phase _gamePhase;
    public Phase GamePhase
    {
        get { return _gamePhase; }
        private set
        {
            Phase oldPhase = _gamePhase;
            _gamePhase = value;
            TransitionPhases(oldPhase, value);
        }
    }
    
    private int _numberOfRequiredPlacements = 0;
    public int NumberOfRequiredPlacements
    {
        get { return _numberOfRequiredPlacements; }
        private set
        {  
            _numberOfRequiredPlacements = value;
            EmitSignal(nameof(NumberOfRequiredPlacementsChanged), value);
        }
    }
    
    private int _numberOfPlacedObjects = 0;
    public int NumberOfPlacedObjects
    {
        get { return _numberOfPlacedObjects; }
        private set
        {  
            _numberOfPlacedObjects = value;
            EmitSignal(nameof(NumberOfPlacedObjectsChanged), value);
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Seed(OS.GetTicksUsec());
        _livesLeft = 2;
        
        _gameBoard = GetNode<GameBoard>("GameBoard");
        _selectorArea = GetNode<SelectorArea>("SelectorArea");
        _placer = GetNode<Placer>("Placer");
        
        _gameBoard.Connect(nameof(GameBoard.ScoreChanged), this, nameof(OnScoreChanged));
        _selectorArea.Connect(nameof(SelectorArea.ItemTypeSelected), this, nameof(OnItemTypeSelected));
        _selectorArea.Connect(nameof(SelectorArea.BeginTurn), this, nameof(OnBeginTurnRequested));
        _selectorArea.Connect(nameof(SelectorArea.ItemCountChosen), this, nameof(OnItemCountChosen));
        _selectorArea.Connect(nameof(SelectorArea.TossRemainder), this, nameof(OnTossRemainder));
        _placer.Connect(nameof(Placer.ItemPlaced), this, nameof(OnItemPlaced));
        _placer.Connect(nameof(Placer.ItemPlacedNope), this, nameof(OnItemPlacedNope));
        
        _scoreLabel = FindNode("ScoreLabel") as TotalScoreLabel;
        
        (FindNode("BeginAgainButton") as Button).Connect("pressed", this, nameof(OnBeginAgainButtonPressed));
        
        GetNode<Control>("MouseCapture").Connect(nameof(MouseCapture.Clicked), this, nameof(OnMouseCaptureClicked));

        _selectorArea.TossLives = _livesLeft;
        GamePhase = Phase.TurnWillStart;
        
        var eht = GetNode<Tween>("EverythingHider/Tween");
        eht.InterpolateProperty(
            GetNode<ColorRect>("EverythingHider"),
            "color:a",
            1.0f,
            0.0f,
            0.54f,
            Tween.TransitionType.Linear
        );
        eht.Connect("tween_completed", this, nameof(OnEverythingHiderTweenDone));
        eht.Start();
    }
    
    // Called when the Begin Turn button is pressed.
    void OnBeginTurnRequested()
    {
        if (GamePhase == Phase.TurnWillStart)
        {
            GamePhase = Phase.ItemTypeSelection;
        }
    }

    // Callback triggered when an item type from the selector area is selected. If
    // we're in waiting-for-item-type mode, it sets the placer's item type and
    // triggers the next phase.
    void OnItemTypeSelected(ItemType itemType, ItemTypeSelector selector)
    {
        if (GamePhase == Phase.ItemTypeSelection)
        {
            _selectorArea.ExtendedDescription = selector.ExtendedDescription;
            _placer.PlacementType = itemType;
            GamePhase = Phase.ItemCountChoosing;
        }
    }
    
    // Called when the selection area selects a number of items to be placed this
    // turn.
    void OnItemCountChosen(int count)
    {
        if (GamePhase == Phase.ItemCountChoosing)
        {
            NumberOfRequiredPlacements = count;
            GamePhase = Phase.ItemPlacement;
        }
    }
    
    // Callback triggered when we successfully place an item.
    void OnItemPlaced(ItemType itemType, Vector2 cellPosition)
    {
        _gameBoard.PlaceItem(itemType.Duplicate() as ItemType, cellPosition);
        NumberOfPlacedObjects++;
        itemType.DisplayAsValid = false;
        IfRequiredNumberOfPlacementsMadeThenTransitionToNextGamePhase();
    }

    // Callback triggered when we attempt to place an item in an invalid place.
    void OnItemPlacedNope(Vector2 cellPosition)
    {
        
    }

    // Callback triggered when the game board's score changes
    void OnScoreChanged(int newScore)
    {
        _scoreLabel.Score = (float) newScore;
    }

    // Callback triggered when the mouse capture rect gets clicked.
    void OnMouseCaptureClicked()
    {
        if (GamePhase == Phase.ItemPlacement)
        {
            _placer.RequestPlacement();
        }
    }

    void OnTossRemainder()
    {
        _placer.EndPlacing();
        if (_livesLeft > 0)
        {
            _livesLeft -= 1;
            _selectorArea.TossLives = _livesLeft;
            GamePhase = Phase.Tossed;
        }
        else
        {
            GamePhase = Phase.GameEnd;
        }
    }

    void OnBeginAgainButtonPressed()
    {
        GetNode<ColorRect>("EverythingHider").Visible = true;
        GetNode<Tween>("EverythingHider/Tween").InterpolateProperty(
            GetNode<ColorRect>("EverythingHider"),
            "color:a",
            0.0f,
            1.0f,
            0.54f,
            Tween.TransitionType.Linear
        );
        GetNode<Tween>("EverythingHider/Tween").Start();
    }

    void OnEverythingHiderTweenDone(Godot.Object obj, NodePath key)
    {
        var eh = GetNode<ColorRect>("EverythingHider");
        if (eh.Color.a == 1.0) {
            GetTree().ReloadCurrentScene();
        }
        else
        {
            eh.Visible = false;
            GamePhase = Phase.TurnWillStart;
        }
    }

    private void IfRequiredNumberOfPlacementsMadeThenTransitionToNextGamePhase()
    {
        if(NumberOfPlacedObjects >= NumberOfRequiredPlacements)
        {
            _selectorArea.HideTutorial();
            _placer.EndPlacing();
            GamePhase = Phase.AllPlaced;
        }
    }
    
    private void TransitionPhases(Phase prev, Phase phase)
    {
        switch (phase)
        {
            case Phase.TurnWillStart:
                break;
            case Phase.ItemTypeSelection:
                _selectorArea.ActivateItemTypeSelection();
                break;
            case Phase.ItemCountChoosing:
                _selectorArea.ActivateItemCountChoose();
                _selectorArea.ChooseItemCount();
                break;
            case Phase.ItemPlacement:
                NumberOfPlacedObjects = 0;
                _placer.BeginPlacing();
                break;
            case Phase.AllPlaced:
                GamePhase = Phase.ItemTypeSelection;
                break;
            case Phase.Tossed:
                GamePhase = Phase.ItemTypeSelection;
                break;
            case Phase.GameEnd:
                _selectorArea.ActivateGameEnd();
                break;
        }
    }
}
