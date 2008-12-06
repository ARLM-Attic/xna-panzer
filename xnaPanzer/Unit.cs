using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnaPanzer
{

/// <summary>
/// Super simple representation of a combat unit.
/// </summary>
public class Unit
{
    #region Variables

    private int m_id;
    public int ID
    {
        get { return this.m_id; }
        set { this.m_id = value; }
    }

    private int m_x;
    public int X
    {
        get { return m_x; }
        set { m_x = value; }
    }

    private int m_y;
    public int Y
    {
        get { return m_y; }
        set { m_y = value; }
    }

    private int m_moves;
    public int Moves
    {
        get { return m_moves; }
        set { m_moves = value; }
    }

    private int m_owner;

    public int Owner
    {
        get { return m_owner; }
        set { m_owner = value; }
    }

    private int m_strength;
    public int Strength
    {
        get { return m_strength; }
        set { m_strength = value; }
    }

    private bool m_hasMoved;
    public bool HasMoved
    {
        get { return m_hasMoved; }
        set { m_hasMoved = value; }
    }

    private UnitType m_unitType;
    public UnitType UnitType
    {
        get { return m_unitType; }
        set { m_unitType = value; }
    }

    private string m_typeName;
    public string TypeName
    {
        get { return m_typeName; }
        set { m_typeName = value; }
    }

    #endregion Variables
    //public Unit(int _x, int _y) // : base(_x, _y, 3, 0, 10, UnitType.Infantry)
    //{
    //    x = _x;
    //    y = _y;
    //    moves = 2;
    //    owner = 0;
    //    strength = 10;
    //    hasMoved = false;
    //    type = UnitType.Infantry;
    //}

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
    }
}

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