using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace XmlContentShared
{
    #region enums

    /// <summary>
    /// Contains all the movement types for ground units.
    /// </summary>
    public enum GroundMovementClass
    {
        Static = 0, Towed, Leg, Truck, Wheeled, HalfTracked, Tracked
    }

    /// <summary>
    /// Contains attributes denoting characteristics, special abilities and restrictions for unit types.
    /// </summary>
    public enum UnitTypeCharacteristics : ulong
    {
        // basic types (bit positions 1..16)
        IsAircraftType = 1L << 1,                                       // can fly like a bird
        IsArtillery = 1L << 2,                                          // defensive fire for adjacent units
        IsGroundType = 1L << 3,                                         // landlubber
        IsHardTarget = 1L << 4,                                         // armored ground init
        IsNavalType = 1L << 5,                                          // can walk on water
        IsSoftTarget = 1L << 6,                                         // susceptible to small arms fire

        // special abilities (17..48)
        CanBridgeRivers = 1L << 17,                                     // bridge engineers can span rivers
        CanProtectBombers = 1L << 18,                                   // fighters can protect adjacent bombers
        CanTransportGroundUnits = 1L << 19,
        CanUtilizeInterceptors = 1L << 20,                              // bombers can utilize friendly adjacent fighters
        CausesTurnSuppression = 1L << 21,                               // level bombers
        IgnoresEnrenchment = 1L << 22,                                  // engineers are close-combat specialists
        IsAirTransport = 1L << 23,                                      // cargo aircraft : can transport infantry
        IsAirTransportable = 1L << 24,                                  // can embark/debark at an airport
        IsGroundTransport = 1L << 25,                                   // truck/half-track : can transport infantry and tow guns
        IsGroundTransportable = 1L << 26,                               // can be transported by ground transport
        IsParatrooper = 1L << 27,                                       // can debark anywhere (not just at an airport)

        // restrictions (49..64)
        MustAttackBeforeMoving = 1L << 49                               // some units lose ability to attack after moving
    };

    #endregion enums

    /// <summary>
    /// Contains info for each type of air/land/sea unit available in the game.
    /// </summary>
    [Serializable]
    public class UnitType
    {
        #region Static Variables

        private static int s_IMAGE_PIXEL_HEIGHT;
        private static int s_IMAGE_PIXEL_WIDTH;
        private static int s_SPRITES_PER_ROW;
        private static SpriteBatch s_SPRITE_BATCH;
        private static Texture2D s_SPRITE_SHEET;

        #endregion Static Variables

        #region Static Properties

        public static int ImageHeight
        {
            get { return UnitType.s_IMAGE_PIXEL_HEIGHT; }
            set { UnitType.s_IMAGE_PIXEL_HEIGHT = value; }
        }

        public static int ImageWidth
        {
            get { return UnitType.s_IMAGE_PIXEL_WIDTH; }
            set { UnitType.s_IMAGE_PIXEL_WIDTH = value; }
        }

        public static SpriteBatch SpriteBatch
        {
            get { return UnitType.s_SPRITE_BATCH; }
            set { UnitType.s_SPRITE_BATCH = value; }
        }

        public static int SpritesPerRow
        {
            get { return UnitType.s_SPRITES_PER_ROW; }
            set { UnitType.s_SPRITES_PER_ROW = value; }
        }

        public static Texture2D SpriteSheet
        {
            get { return UnitType.s_SPRITE_SHEET; }
            set { UnitType.s_SPRITE_SHEET = value; }
        }

        #endregion Static Properties

        #region Member Variables

        private int m_AirAttack;                                        // anti-aircraft combat strength
        private int m_AirDefense;                                       // combat strength vs attacking aircraft
        private int m_Ammo;                                             // ammo supply points
        private DateTime m_AvailabilityEnd;                             // last availability date
        private DateTime m_AvailabilityStart;                           // first availability date
        private ulong m_Characteristics;                                // e.g. Bridging, air transportable
        private int m_CloseDefense;                                     // close-combat strength vs. infantry
        private int m_CombatRange;                                      // 1 for most units
        private int m_Cost;                                             // prestige cost to buy
        private int m_EntrenchmentRate;                                 // e.g. inf types are better at entrenching than tanks
        private int m_Fuel;                                             // fuel supply points
        private int m_GroundDefense;                                    // defense rating
        private int m_HardAttack;                                       // combat strength vs. armored ground units
        private int m_ID;                                               // unique ID number
        private int m_Initiative;                                       // first to shoot, wins
        private GroundMovementClass m_MovementClass;                    // e.g. Wheeled, Tracked
        private int m_Moves;                                            // no. movement points
        private string m_Name;                                          // e.g. Psw 233-8r
        private int m_Nationality;                                      // 0=German, 1=Italian, etc
        private int m_SoftAttack;                                       // combat strength vs unarmoed ground units
        private int m_SpottingRange;                                    // hex range for spotting enemy units
        private int m_SpritesheetX;                                     // pixel X index within spritesheet
        private int m_SpritesheetY;                                     // pixel Y index within spritesheet

        #endregion Member Variables

        #region Properties

        public int AirAttack
        {
            get { return m_AirAttack; }
            set { m_AirAttack = value; }
        }

        public int AirDefense
        {
            get { return m_AirDefense; }
            set { m_AirDefense = value; }
        }

        public int Ammo
        {
            get { return m_Ammo; }
            set { m_Ammo = value; }
        }

        public DateTime AvailabilityEnd
        {
            get { return m_AvailabilityEnd; }
            set { m_AvailabilityEnd = value; }
        }

        public DateTime AvailabilityStart
        {
            get { return m_AvailabilityStart; }
            set { m_AvailabilityStart = value; }
        }

        public ulong Characteristics
        {
            get { return m_Characteristics; }
            set { m_Characteristics = value; }
        }

        public int CloseDefense
        {
            get { return m_CloseDefense; }
            set { m_CloseDefense = value; }
        }

        public int CombatRange
        {
            get { return m_CombatRange; }
            set { m_CombatRange = value; }
        }

        public int Cost
        {
            get { return m_Cost; }
            set { m_Cost = value; }
        }

        public int EntrenchmentRate
        {
            get { return m_EntrenchmentRate; }
            set { m_EntrenchmentRate = value; }
        }

        public int Fuel
        {
            get { return m_Fuel; }
            set { m_Fuel = value; }
        }

        public int GroundDefense
        {
            get { return m_GroundDefense; }
            set { m_GroundDefense = value; }
        }

        public int HardAttack
        {
            get { return m_HardAttack; }
            set { m_HardAttack = value; }
        }

        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        public int Initiative
        {
            get { return m_Initiative; }
            set { m_Initiative = value; }
        }

        public GroundMovementClass MovementClass
        {
            get { return m_MovementClass; }
            set { m_MovementClass = value; }
        }

        public int Moves
        {
            get { return m_Moves; }
            set { m_Moves = value; }
        }

        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public int Nationality
        {
            get { return m_Nationality; }
            set { m_Nationality = value; }
        }

        public int SoftAttack
        {
            get { return m_SoftAttack; }
            set { m_SoftAttack = value; }
        }

        public int SpottingRange
        {
            get { return m_SpottingRange; }
            set { m_SpottingRange = value; }
        }

        public int SpritesheetX
        {
            get { return m_SpritesheetX; }
            set { m_SpritesheetX = value; }
        }

        public int SpritesheetY
        {
            get { return m_SpritesheetY; }
            set { m_SpritesheetY = value; }
        }

        #endregion Properties

        #region Constructors()

        static UnitType()
        {
            UnitType.ImageHeight = 50;
            UnitType.ImageWidth = 60;
            UnitType.SpriteBatch = null;
            UnitType.SpritesPerRow = 10;
            UnitType.SpriteSheet = null;
        }

        /// <summary>
        /// Default constructor to create a new unit type, e.g. PZ IIIJ, Spitfire II.  Properties must be initialized individually.
        /// </summary>
        public UnitType()
        {
        }

        /// <summary>
        /// Primary constructor to create a new unit type, e.g. PZ IIIJ, Spitfire II.
        /// </summary>
        /// <param name="_airAttack"></param>
        /// <param name="_airDefense"></param>
        /// <param name="_ammo"></param>
        /// <param name="_availabilityStart"></param>
        /// <param name="_availabilityEnd"></param>
        /// <param name="_characteristics"></param>
        /// <param name="_closeDefense"></param>
        /// <param name="_combatRange"></param>
        /// <param name="_cost"></param>
        /// <param name="_entrenchmentRate"></param>
        /// <param name="_fuel"></param>
        /// <param name="_groundDefense"></param>
        /// <param name="_hardAttack"></param>
        /// <param name="_id"></param>
        /// <param name="_initiative"></param>
        /// <param name="_movementClass"></param>
        /// <param name="_moves"></param>
        /// <param name="_name"></param>
        /// <param name="_nationality"></param>
        /// <param name="_softAttack"></param>
        /// <param name="_spottingRange"></param>
        /// <param name="_spritesheetX"></param>
        /// <param name="_spritesheetY"></param>
        public UnitType(int _airAttack, int _airDefense, int _ammo,
            DateTime _availabilityStart, DateTime _availabilityEnd,
            ulong _characteristics, int _closeDefense, int _combatRange, int _cost,
            int _entrenchmentRate,
            int _fuel,
            int _groundDefense,
            int _hardAttack, int _id, int _initiative,
            GroundMovementClass _movementClass, int _moves,
            string _name, int _nationality,
            int _softAttack, int _spottingRange,
            int _spritesheetX, int _spritesheetY)
        {
            this.AirAttack = _airAttack;
            this.AirDefense = _airDefense;
            this.Ammo = _ammo;
            this.AvailabilityStart = _availabilityStart;
            this.AvailabilityEnd = _availabilityEnd;
            this.Characteristics = _characteristics;
            this.CloseDefense = _closeDefense;
            this.CombatRange = _combatRange;
            this.Cost = _cost;
            this.EntrenchmentRate = _entrenchmentRate;
            this.Fuel = _fuel;
            this.GroundDefense = _groundDefense;
            this.HardAttack = _hardAttack;
            this.ID = _id;
            this.Initiative = _initiative;
            this.MovementClass = _movementClass;
            this.Moves = _moves;
            this.Name = _name;
            this.Nationality = _nationality;
            this.SoftAttack = _softAttack;
            this.SpottingRange = _spottingRange;
            this.SpritesheetX = _spritesheetX;
            this.SpritesheetY = _spritesheetY;
        }

        #endregion Constructors()

        /// <summary>
        /// Draws the unit (unit type icon) at the specified pixel x,y.
        /// </summary>
        /// <param name="_point">Point structure containing pixel x,y destination.</param>
        public void Draw(Point _point)
        {
            UnitType.SpriteBatch.Draw(UnitType.SpriteSheet, new Rectangle(_point.X, _point.Y, UnitType.ImageWidth, UnitType.ImageHeight),
                new Rectangle(this.SpritesheetX, this.SpritesheetY, UnitType.ImageWidth, UnitType.ImageHeight), Color.White);
        }

        /// <summary>
        /// Draws the unit (unit type icon) at the specified rectangle.  Use this method to scale the icon.
        /// </summary>
        /// <param name="_dest">Rectangle structure containing pixel x,y destination and width/height.</param>
        public void Draw(Rectangle _dest)
        {
            UnitType.SpriteBatch.Draw(UnitType.SpriteSheet, _dest,
                new Rectangle(this.SpritesheetX, this.SpritesheetY, UnitType.ImageWidth, UnitType.ImageHeight), Color.White);
        }

        public void Load(ContentManager content)
        {
            //texture = content.Load<Texture2D>(textureAsset);
        }
    } // UnitType class

    /// <summary>
    /// This class will be instantiated by the XNA Framework Content
    /// Pipeline to read the specified data type from binary .xnb format.
    /// 
    /// Unlike the other Content Pipeline support classes, this should
    /// be a part of your main game project, and not the Content Pipeline
    /// Extension Library project.
    /// </summary>
    public class UnitTypeContentTypeReader : ContentTypeReader<UnitType>
    {
        protected override UnitType Read(ContentReader _input, UnitType _unitType)
        {
            UnitType unitType = new UnitType();

            unitType.AirAttack = _input.ReadInt32();
            unitType.AirDefense = _input.ReadInt32();
            unitType.Ammo = _input.ReadInt32();
            unitType.AvailabilityEnd = _input.ReadObject<System.DateTime>();
            unitType.AvailabilityStart = _input.ReadObject<System.DateTime>();
            unitType.Characteristics = _input.ReadUInt64();
            unitType.CloseDefense = _input.ReadInt32();
            unitType.CombatRange = _input.ReadInt32();
            unitType.Cost = _input.ReadInt32();
            unitType.EntrenchmentRate = _input.ReadInt32();
            unitType.Fuel = _input.ReadInt32();
            unitType.GroundDefense = _input.ReadInt32();
            unitType.HardAttack = _input.ReadInt32();
            unitType.ID = _input.ReadInt32();
            unitType.Initiative = _input.ReadInt32();
            unitType.MovementClass = _input.ReadObject<GroundMovementClass>();
            unitType.Moves = _input.ReadInt32();
            unitType.Name = _input.ReadString();
            unitType.Nationality = _input.ReadInt32();
            unitType.SoftAttack = _input.ReadInt32();
            unitType.SpottingRange = _input.ReadInt32();
            unitType.SpritesheetX = _input.ReadInt32();
            unitType.SpritesheetY = _input.ReadInt32();

            unitType.Load(_input.ContentManager);

            return unitType;
        }
    } // UnitTypeContentTypeReader class

} // XmlContentShared namespace
