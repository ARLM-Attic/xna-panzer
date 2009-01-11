using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;

namespace GameEngine1
{
    public class Unit
    {
        /// <summary>
        /// IMPORTANT: the order of the properties below MUST EXACTLY MATCH the order of the related properties in the XML file!!
        /// Otherwise, you'll get an error similar to the following: XML element "Experience" not found ... (in) UnitList.xml
        /// </summary>

        #region Auto Properties (non-boolean)

        public int Experience { get; set; }

        /// <summary>
        /// Unit's unique ID (used for array index)
        /// </summary>
        public int ID { get; set; }

        //[XmlIgnore()]
        //public int Moves
        //{
        //    get { return this.m_Moves; }
        //    set { this.m_Moves = value; }
        //}
        private int m_Moves;

        /// <summary>
        /// Unit's unique name, e.g. 27th Manstein's Tiger II.  Defaults to unit ID + UnitType name, e.g. 27th Tiger II
        /// </summary>
        public string Name { get; set; }

        public int Owner { get; set; }

        public int StartingX { get; set; }

        public int StartingY { get; set; }

        public int Strength { get; set; }

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

            unit.Experience = _input.ReadInt32();
            unit.ID = _input.ReadInt32();
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
