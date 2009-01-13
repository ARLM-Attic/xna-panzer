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
        /// <summary>
        /// IMPORTANT: the order of the properties below MUST EXACTLY MATCH the order of the related properties in the XML file!!
        /// Otherwise, you'll get an error similar to the following: XML element "Experience" not found ... (in) UnitList.xml
        /// 
        /// Use [ContentSerializerIgnore] for any field/public property that is NOT contained in the source XML file.
        /// </summary>

        #region Auto Properties (non-boolean)

        public int Ammo { get; set; }

        [ContentSerializerIgnore]
        public int CurrentX { get; set; }

        [ContentSerializerIgnore]
        public int CurrentY { get; set; }

        public int Entrenchment { get; set; }

        public int Experience { get; set; }

        public int Fuel { get; set; }

        /// <summary>
        /// Unit's unique ID (used for array index)
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Number of moves (movement points) remaining (allotment - used this turn).  This will only come into play for
        /// units that can move 2+ segments in a single turn (currently none but sounds like a good ability for recon units).
        /// </summary>
        [ContentSerializerIgnore]
        public int Moves
        {
            get { return this.m_Moves; }
            set { this.m_Moves = value; }
        }
        private int m_Moves;

        /// <summary>
        /// Unit's unique name, e.g. 27th Manstein's Tiger II.  Defaults to unit ID + UnitType name, e.g. 27th Tiger II
        /// </summary>
        public string Name { get; set; }

        public int Owner { get; set; }

        public int StartingX { get; set; }

        public int StartingY { get; set; }

        public int Strength { get; set; }

        [ContentSerializerIgnore]
        public UnitType UnitType
        {
            get { return this.m_UnitType; }
            set { this.m_UnitType = value; }
        }
        private UnitType m_UnitType;

        public int UnitTypeID { get; set; }

        #endregion Instance Variables (non-boolean)

        #region Boolean Instance Variables

        /// <summary>
        /// True if unit has attacked this turn, false if not.
        /// </summary>
        [ContentSerializerIgnore]
        public bool HasAttacked
        {
            get { return this.m_HasAttacked; }
            set { this.m_HasAttacked = value; }
        }
        private bool m_HasAttacked;

        /// <summary>
        /// True if unit has physically moved this turn, false if not.
        /// </summary>
        [ContentSerializerIgnore]
        public bool HasMoved
        {
            get { return this.m_HasMoved; }
            set { this.m_HasMoved = value; }
        }
        private bool m_HasMoved;

        /// <summary>
        /// True if unit is visible to the enemy and therefore should be drawn on the map, false if not.
        /// </summary>
        [ContentSerializerIgnore]
        public bool IsVisible
        {
            get { return this.m_IsVisible; }
            set { this.m_IsVisible = value; }
        }
        private bool m_IsVisible;

        #endregion Boolean Instance Variables

        public Unit() : this("unknown")
        {
        }

        public Unit(string _name)
        {
            this.Name = _name;
        }

        /// <summary>
        /// Primary constructor.
        /// </summary>
        /// 
        /// <param name="_id">unique ID number</param>
        /// <param name="_moves">Number of movement points each turn</param>
        /// <param name="_name">Unit's unique name, e.g. 27th Manstein's Tiger II</param>
        /// <param name="_owner">Controlling player number</param>
        /// <param name="_strength">Current strength points</param>
        /// <param name="_type">Unit's type</param>
        /// <param name="_x">Map X location (hexagon)</param>
        /// <param name="_y">Map Y location (hexagon)</param>
        public Unit(int _experience, int _id, int _moves, string _name, int _owner, int _strength, int _unitTypeID, int _x, int _y)
        {
            this.CurrentX = _x;
            this.CurrentY = _y;
            this.Experience = _experience;
            this.ID = _id;
            this.Moves = _moves;
            this.Name = _name;
            this.Owner = _owner;
            this.Strength = _strength;
            this.UnitTypeID = _unitTypeID;
            //this.UnitType = _type;
            this.StartingX = _x;
            this.StartingY = _y;

            this.IsVisible = true;

            //this.TypeName = Enum.GetName(typeof(UnitType), _type);

            // TEST: string s = UnitType.Infantry.ToString();


            this.HasMoved = false;
        }

        public void Move(int _x, int _y)
        {
            this.CurrentX = _x;
            this.CurrentY = _y;
            this.HasMoved = true;
        }

        public void UndoMove()
        {
            this.CurrentX = this.StartingX;
            this.CurrentY = this.StartingY;
            this.HasMoved = false;
        }

        /// <summary>
        /// Ends a unit's move by setting starting x,y to current x,y.  If unit has not actually moved from the 
        /// starting location then HasMoved will = false and the unit will be able to move later during the turn.
        /// </summary>
        public void EndMove()
        {
            this.StartingX = this.CurrentX;
            this.StartingY = this.CurrentY;
        }

        public void Draw(Point _point)
        {
            // TODO: need a way to calculate new destination rectangle OR do we use DrawLocation()??? use GameServices??

            if (this.IsVisible && this.UnitType != null) {
                this.UnitType.Draw(_point);
                //Rectangle destinationRectangle = new Rectangle(_screenLocationX, _screenLocationY, Unit.PixelWidth, Unit.PixelHeight);
                //_spriteBatch.Draw(Unit.SpriteSheet,						// source bitmap image
                //                destinationRectangle,                       // where to draw the unit on the screen
                //                this.SpriteRectangle,                       // what portion of the source image to draw
                //                Color.White);                               // White = magenta color will be transparent
            }
        }

        public void Load(ContentManager content)
        {
            //texture = content.Load<Texture2D>(textureAsset);

        }

    } // class Unit

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

            unit.Ammo = _input.ReadInt32();
            unit.Entrenchment = _input.ReadInt32();
            unit.Experience = _input.ReadInt32();
            unit.Fuel = _input.ReadInt32();
            unit.ID = _input.ReadInt32();
            unit.Name = _input.ReadString();
            unit.Owner = _input.ReadInt32();
            unit.StartingX = _input.ReadInt32();
            unit.StartingY = _input.ReadInt32();
            unit.Strength = _input.ReadInt32();
            unit.UnitTypeID = _input.ReadInt32();

            unit.CurrentX = unit.StartingX;
            unit.CurrentY = unit.StartingY;
            unit.IsVisible = true; // HACK !!!
            unit.Moves = 5; // HACK !!!

            return unit;
        }
    } // UnitContentTypeReader class

} // namespace
