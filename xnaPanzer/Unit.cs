using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace xnaPanzer
{

/// <summary>
/// Super simple representation of a combat unit.
/// </summary>
public class Unit
{
    #region Static Variables

    public static int PixelWidth
    {
        get { return Unit.pixelWidth; }
        set { Unit.pixelWidth = value; }
    }
    private static int pixelWidth;

    public static int PixelHeight
    {
        get { return Unit.pixelHeight; }
        set { Unit.pixelHeight = value; }
    }
    private static int pixelHeight;

    public static SpriteBatch SpriteBatch
    {
        get { return Unit.spriteBatch; }
        set { Unit.spriteBatch = value; }
    }
    private static SpriteBatch spriteBatch;

    public static Texture2D SpriteSheet
    {
        get { return Unit.spriteSheet; }
        set { Unit.spriteSheet = value; }
    }
    private static Texture2D spriteSheet;

    #endregion Static Variables

    #region Instance Variables (non-boolean)

    /// <summary>
    /// Unit's unique ID (used for array index)
    /// </summary>
    public int ID
    {
        get { return this.m_ID; }
        set { this.m_ID = value; }
    }
    private int m_ID;

    /// <summary>
    /// Area from the spritesheet that contains this unit's icon
    /// </summary>
    public Rectangle SpriteRectangle
    {
        get { return this.m_SpriteRectangle; }
        set { this.m_SpriteRectangle = value; }
    }
    private Rectangle m_SpriteRectangle;

    public int Moves
    {
        get { return this.m_Moves; }
        set { this.m_Moves = value; }
    }
    private int m_Moves;

    public int Owner
    {
        get { return this.m_Owner; }
        set { this.m_Owner = value; }
    }
    private int m_Owner;

    public int StartingX
    {
        get { return m_StartingX; }
        set { m_StartingX = value; }
    }
    private int m_StartingX;

    public int StartingY
    {
        get { return m_StartingY; }
        set { m_StartingY = value; }
    }
    private int m_StartingY;

    public int Strength
    {
        get { return this.m_Strength; }
        set { this.m_Strength = value; }
    }
    private int m_Strength;

    public UnitType UnitType
    {
        get { return this.m_UnitType; }
        set { this.m_UnitType = value; }
    }
    private UnitType m_UnitType;

    public string TypeName
    {
        get { return this.m_TypeName; }
        set { this.m_TypeName = value; }
    }
    private string m_TypeName;

    public int X
    {
        get { return this.m_X; }
        set { this.m_X = value; }
    }
    private int m_X;

    public int Y
    {
        get { return this.m_Y; }
        set { this.m_Y = value; }
    }
    private int m_Y;

    #endregion Instance Variables (non-boolean)

    #region Boolean Instance Variables

    public bool HasMoved
    {
        get { return this.m_HasMoved; }
        set { this.m_HasMoved = value; }
    }
    private bool m_HasMoved;

    public bool IsVisible
    {
        get { return this.m_IsVisible; }
        set { this.m_IsVisible = value; }
    }
    private bool m_IsVisible;

    #endregion Boolean Instance Variables

    /// <summary>
    /// Default constructor (be sure to initialize the Unit via properties).
    /// </summary>
    public Unit() : this(-1, -1, -1, -1, -1, -1, UnitType.Pioneere)
    {
    }

    /// <summary>
    /// Primary constructor.
    /// </summary>
    /// <param name="_id">unique ID number</param>
    /// <param name="_x">Current map X location (hexagon)</param>
    /// <param name="_y">Current map Y location (hexagon)</param>
    /// <param name="_moves">Number of movement points each turn</param>
    /// <param name="_owner">Controlling player number</param>
    /// <param name="_strength">Current strength points</param>
    /// <param name="_type">Unit's type</param>
    public Unit(int _id, int _x, int _y, int _moves, int _owner, int _strength, UnitType _type)
    {
        this.ID = _id;
        this.X = _x;
        this.Y = _y;
        this.Moves = _moves;
        this.Owner = _owner;
        this.Strength = _strength;
        this.UnitType = _type;
        this.TypeName = Enum.GetName(typeof(UnitType), _type);
        // TEST: string s = UnitType.Infantry.ToString();
        this.HasMoved = false;
        Point offset = Util.CalculateSpritesheetCoordinates((int)this.UnitType);
        this.SpriteRectangle = new Rectangle(offset.X, offset.Y, Unit.PixelWidth, Unit.PixelHeight);
    }

    public void Move(int _x, int _y)
    {
        this.X = _x;
        this.Y = _y;
        this.HasMoved = true;
    }

    public void UndoMove()
    {
        this.X = this.StartingX;
        this.Y = this.StartingY;
        this.HasMoved = false;
    }

    public void EndMove()
    {
        this.StartingX = this.X;
        this.StartingY = this.Y;
    }

    public void Draw(SpriteBatch _spriteBatch, int _screenLocationX, int _screenLocationY)
    {
        // TODO: need a way to calculate new destination rectangle OR do we use DrawLocation()??? use GameServices??
        if (!this.IsVisible) {
            Rectangle destinationRectangle = new Rectangle(_screenLocationX, _screenLocationY, Unit.PixelWidth, Unit.PixelHeight);
            _spriteBatch.Draw(Unit.SpriteSheet,						// source bitmap image
                            destinationRectangle,                       // where to draw the unit on the screen
                            this.SpriteRectangle,                       // what portion of the source image to draw
                            Color.White);                               // White = magenta color will be transparent
        }
    }


}

// using the following hack for now...need to figure out what we really need
public enum MovementClass
{
    Static, Towed, Leg, Truck, Wheeled, Tracked
}

//public class UnitType
//{
//    private string m_name;
//    public string Name { get; set; }


//}

} // namespace