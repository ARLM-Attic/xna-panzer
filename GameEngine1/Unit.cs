using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine1
{
    public class Unit
    {
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

        //public int Experience
        //{
        //    get { return this.m_Experience; }
        //    set { this.m_Experience = value; }
        //}
        //private int m_Experience;

        //public int Moves
        //{
        //    get { return this.m_Moves; }
        //    set { this.m_Moves = value; }
        //}
        //private int m_Moves;

        /// <summary>
        /// Unit's unique name, e.g. 27th Manstein's Tiger II.  Defaults to unit ID + UnitType name, e.g. 27th Tiger II
        /// </summary>
        public string Name
        {
            get { return this.m_Name; }
            set { this.m_Name = value; }
        }
        private string m_Name;

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

        //public UnitType UnitType
        //{
        //    get { return this.m_UnitType; }
        //    set { this.m_UnitType = value; }
        //}
        //private UnitType m_UnitType;

        //public int UnitTypeID
        //{
        //    get { return this.m_UnitTypeID; }
        //    set { this.m_UnitTypeID = value; }
        //}
        //private int m_UnitTypeID;

        //public int X
        //{
        //    get { return this.m_X; }
        //    set { this.m_X = value; }
        //}
        //private int m_X;

        //public int Y
        //{
        //    get { return this.m_Y; }
        //    set { this.m_Y = value; }
        //}
        //private int m_Y;

        #endregion Instance Variables (non-boolean)

        #region Boolean Instance Variables

        ///// <summary>
        ///// True if unit has attacked this turn, false if not.
        ///// </summary>
        //public bool HasAttacked
        //{
        //    get { return this.m_HasAttacked; }
        //    set { this.m_HasAttacked = value; }
        //}
        //private bool m_HasAttacked;

        ///// <summary>
        ///// True if unit has physically moved this turn, false if not.
        ///// </summary>
        //public bool HasMoved
        //{
        //    get { return this.m_HasMoved; }
        //    set { this.m_HasMoved = value; }
        //}
        //private bool m_HasMoved;

        ///// <summary>
        ///// True if unit is visible to the enemy and therefore should be drawn on the map, false if not.
        ///// </summary>
        //public bool IsVisible
        //{
        //    get { return this.m_IsVisible; }
        //    set { this.m_IsVisible = value; }
        //}
        //private bool m_IsVisible;

        #endregion Boolean Instance Variables

        public Unit() : this("unknown")
        {
        }

        public Unit(string _name)
        {
            this.Name = _name;
        }
    }

    /// <summary>
    /// This class will be instantiated by the XNA Framework Content
    /// Pipeline to read the specified data type from binary .xnb format.
    /// 
    /// Unlike the other Content Pipeline support classes, this should
    /// be a part of your main game project, and not the Content Pipeline
    /// Extension Library project.
    /// </summary>
    public class UnitContentTypeReader : ContentTypeReader<Unit>
    {
        protected override Unit Read(ContentReader _input, Unit _unit)
        {
            Unit unit = new Unit();

            unit.ID = _input.ReadInt32();
//            unit.Experience = _input.ReadInt32();
            //unit.HasMoved = _input.ReadBoolean();

  //          unit.Moves = _input.ReadInt32();
            unit.Owner = _input.ReadInt32();
            unit.StartingX = _input.ReadInt32();
            unit.StartingY = _input.ReadInt32();
            unit.Strength = _input.ReadInt32();
            //unit.UnitTypeID = _input.ReadInt32();
            unit.Name = _input.ReadString();

            return unit;
        }
    } // UnitContentTypeReader class

} // namespace
