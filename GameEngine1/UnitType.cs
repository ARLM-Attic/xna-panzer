using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine1
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

        #endregion Static Variables

        #region Static Properties

        public static int ImageHeight { get; set; }

        public static int ImageWidth { get; set; }

        public static SpriteBatch SpriteBatch { get; set; }

        public static int SpritesPerRow { get; set; }

        public static Texture2D SpriteSheet { get; set; }

        #endregion Static Properties

        #region Member Variables
// anti-aircraft combat strength
// combat strength vs attacking aircraft
// ammo supply points
// last availability date
// first availability date
// e.g. Bridging, air transportable
// close-combat strength vs. infantry
// 1 for most units
// prestige cost to buy
// e.g. inf types are better at entrenching than tanks
// fuel supply points
// defense rating
// combat strength vs. armored ground units
// unique ID number
// first to shoot, wins
// e.g. Wheeled, Tracked
// no. movement points
// e.g. Psw 233-8r
// 0=German, 1=Italian, etc
// combat strength vs unarmoed ground units
// hex range for spotting enemy units
// pixel X index within spritesheet
// pixel Y index within spritesheet

        #endregion Member Variables

        #region Auto Properties

        public int AirAttack { get; set; }

        public int AirDefense { get; set; }

        public int Ammo { get; set; }

        public DateTime AvailabilityEnd { get; set; }

        public DateTime AvailabilityStart { get; set; }

        public ulong Characteristics { get; set; }

        public int CloseDefense { get; set; }

        public int CombatRange { get; set; }

        public int Cost { get; set; }

        public int EntrenchmentRate { get; set; }

        public int Fuel { get; set; }

        public int GroundDefense { get; set; }

        public int HardAttack { get; set; }

        public int ID { get; set; }

        public int Initiative { get; set; }

        public GroundMovementClass MovementClass { get; set; }

        public int Moves { get; set; }

        public string Name { get; set; }

        public int Nationality { get; set; }

        public int SoftAttack { get; set; }

        public int SpottingRange { get; set; }

        public int SpritesheetX { get; set; }

        public int SpritesheetY { get; set; }

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

} // GameEngine1 namespace
