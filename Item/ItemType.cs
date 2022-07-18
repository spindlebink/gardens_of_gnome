using Godot;
using System;
using System.Collections;

public class ItemType : Node2D
{
    public static int GlobalScoreMultiplier = 4;
    
    public static int LowBaseScore = 1;
    public static int MidBaseScore = 3;
    public static int HighBaseScore = 5;

    // Constants for pre-defined how-much-I-like-it values
    public static int HateIt = -6;
    public static int NoThankYou = -3;
    public static int LikeALittle = 1;
    public static int LikeALot = 2;
    public static int OhMyGodILoveIt = 3;

    public Vector2 PlacedPosition = new Vector2(0f, 0f);
    
    private bool _displayAsValid = true;
    public bool DisplayAsValid
    {
        set
        {
            if (value == _displayAsValid)
            {
                return;
            }

            if (value)
            {
                Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
            else
            {
                Modulate = new Color(1.0f, 1.0f, 1.0f, 0.25f);
            }
            _displayAsValid = value;
        }
    }

    public virtual void PlayPlacementSound()
    {
        if (HasNode("Audio"))
        {
            GetNode<AudioStreamPlayer2D>("Audio").Play();
        }
        if (HasNode("AudioSustained"))
        {
            GetNode<AudioStreamPlayer2D>("AudioSustained").Play();
        }
    }

    public virtual void IJustGotPlaced(GameBoard board, Vector2 corner)
    {
        // Override in subclasses if you need anything special
    }

    public virtual void ResolveScore(GameBoard board)
    {
        var gooditude = 0;
        foreach (ItemType neighbor in board.GetUniqueItems(ItemType.GetFootprintNeighbors(GetFootprint()), PlacedPosition))
        {
            gooditude += HowMuchDoILike(neighbor, board);
            var neighborBonus = neighbor.HowMuchDoILike(this, board);
            board.PostScore(neighbor, neighborBonus * GlobalScoreMultiplier);
        }
        if (BaseScore() > 0 && gooditude > 0)
        {
            board.PostScore(this, BaseScore() * GlobalScoreMultiplier);
            board.PostScore(this, gooditude * GlobalScoreMultiplier);
        }
        else
        {
            // combine them so we don't get +1/-2, which is a lil confusing
            board.PostScore(this, (BaseScore() + gooditude) * GlobalScoreMultiplier);
        }
    }
    
    public virtual int BaseScore()
    {
        return LowBaseScore;
    }
    
    public virtual int HowMuchDoILike(ItemType item, GameBoard board)
    {
        return 0;
    }
    
    // Iterates through each cell position in the item type's footprint, marking
    // that cell as valid or not depending on whatever requirements the type has.
    //
    // By default, it just checks each point on the footprint using
    // `board.IsCellEmpty()`. This is probably enough for most purposes.
    public virtual bool TestPlacement(GameBoard board, Vector2 topLeft)
    {
        var footprint = GetFootprint();
        var ok = true;
        for (var i = 0; i < footprint.Length; i += 1) {
            if (board.IsCellEmpty(footprint[i] + topLeft))
            {
                board.MarkValid(footprint[i] + topLeft);
            }
            else
            {
                board.MarkInvalid(footprint[i] + topLeft);
                ok = false;
            }
        }
        return ok;
    }

    // Returns the item type's board footprint.
    public virtual Vector2[] GetFootprint()
    {
        return new Vector2[]{
            new Vector2(0, 0)
        };
    }
    
    // Called when the item has been placed. Fill at the end of the jam with
    // juice of some sort, idk.
    public virtual void AnimatePlaced()
    {
        if (HasNode("AnimationPlayer"))
        {
            GetNode<AnimationPlayer>("AnimationPlayer").Play("Placed");
        }
        else if (HasNode("Tween"))
        {
            GetNode<Tween>("Tween").InterpolateProperty(
                this,
                "position:y",
                Position.y - 25.0f,
                Position.y,
                0.35f,
                Tween.TransitionType.Bounce,
                Tween.EaseType.Out
            );
            GetNode<Tween>("Tween").Start();
        }
        
        if (HasNode("Particles2D"))
        {
            GetNode<Particles2D>("Particles2D").Emitting = true;
        }
    }
    
    public static ArrayList GetFootprintNeighbors(Vector2[] footprint)
    {
        Vector2[] offsets = new Vector2[]{
            new Vector2(-1, 0),
            new Vector2(0, -1),
            new Vector2(1, 0),
            new Vector2(0, 1)
        };
        ArrayList neighbors = new ArrayList();
        foreach (var cell in footprint)
        {
            foreach (var offset in offsets)
            {
                var n = cell + offset;
                if (!Array.Exists(footprint, cl => cl == n) && !neighbors.Contains(n))
                {
                    neighbors.Add(n);
                }
            }
        }
        return neighbors;
    }

    // Returns the *technical* center of a footprint extent. I'm very tired and
    // I couldn't think of a better name or better way to explain it. It's diff
    // from the other one because this one's for calculating the actual cell
    // positions and the other one's for calculating the visual center.
    public static Vector2 GetActualCenterOfFootprintExtents(Rect2 extents, Vector2 cellSize)
    {
        return (extents.Size - new Vector2(1.0f, 1.0f)) * 0.5f * cellSize;
    }
    
    // Returns the visual center of a footprint extent. Put the item sprite here.
    public static Vector2 GetVisualCenterOfFootprintExtents(Rect2 extents, Vector2 cellSize)
    {
        return extents.Size * 0.5f * cellSize;
    }

    // Returns the bounding extents in cells of the current item type's footprint.
    public static Rect2 GetFootprintExtents(Vector2[] footprint)
    {
        var min = footprint[0];
        var max = footprint[0];
        foreach (Vector2 cell in footprint)
        {
            min.x = Mathf.Min(min.x, cell.x);
            min.y = Mathf.Min(min.y, cell.y);
            max.x = Mathf.Max(max.x, cell.x);
            max.y = Mathf.Max(max.y, cell.y);
        }
        max.x += 1f;
        max.y += 1f;
        return new Rect2(min, max - min);
    }
}
