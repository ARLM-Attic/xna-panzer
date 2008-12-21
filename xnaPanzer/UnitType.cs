using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnaPanzer
{
    public enum GroundMovementClass
    {
        Static, Towed, Leg, Truck, Wheeled, Tracked
    }

    public enum ServiceBranch
    {
        Army, Navy, AirForce
    }

    public enum UnitCharacteristics : ulong 
    {
        // basic types (1..16)
        IsAirTransport = 1L << 1,
        IsAircraft = 1L << 2,
        IsArtillery = 1L << 3,                                          // defensive fire for adjacent units
        IsInfantry = 1L << 4,                                           // soft target

        // special abilities (17..48)
        CanBridgeRivers = 1L << 17,
        CanProtectBombers = 1L << 18,
        CanTransportGroundUnits = 1L << 19,
        CanUtilizeInterceptors = 1L << 20,
        CausesTurnSuppression = 1L << 21,
        IgnoresEnrenchment = 1L << 22,
        IsAirtransportable = 1L << 23,                                  // can be transported from/to an airport
        IsParatrooper = 1L << 24,                                       // can drop on any ground (not just airport)

        // restrictions (49.64)
        MustAttackBeforeMoving = 1L << 49
    };

    public class UnitType
    {
        #region Member Variables

        private int m_AirAttack;
        private int m_AirDefense;
        private int m_Ammo;
        private int m_CloseDefense;
        private int m_CombatRange; // 1 for most units
        private int m_Fuel;
        private int m_HardAttack;
        private GroundMovementClass m_MovementClass;  // Wheeled
        private ServiceBranch m_ServiceBranch; // Army (ground unit)
        private int m_Moves;
        private string m_Name;  // Psw 233-8r
        private long m_UnitCharacteristics;                             // e.g. Bridging, air transportable
        private int m_Nationality; // 1 = Italian
        private int m_SoftAttack;
        private int m_SpottingRange; // 1-5
        private int m_SpritesheetX;
        private int m_SpritesheetY;

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

        public int Fuel
        {
            get { return m_Fuel; }
            set { m_Fuel = value; }
        }

        public int HardAttack
        {
            get { return m_HardAttack; }
            set { m_HardAttack = value; }
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

        public ServiceBranch ServiceBranch
        {
            get { return m_ServiceBranch; }
            set { m_ServiceBranch = value; }
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

        public UnitType()
        {
        }

        public UnitType(int _airAttack, int _airDefense, int _ammo, int _closeDefense, int _combatRange, int _fuel,
            int _hardAttack, GroundMovementClass _movementClass, ServiceBranch _serviceBranch, int _moves, 
            string _name, int _nationality, int _softAttack, int _spottingRange, int _spritesheetX, int _spritesheetY)
        {
            this.AirAttack = _airAttack;
            this.AirDefense = _airDefense;
            this.Ammo = _ammo;
            this.CloseDefense = _closeDefense;
            this.CombatRange = _combatRange;
            this.Fuel = _fuel;
            this.HardAttack = _hardAttack;
            this.MovementClass = _movementClass;
            this.Moves = _moves;
            this.Name = _name;
            this.Nationality = _nationality;
            this.ServiceBranch = _serviceBranch;
            this.SoftAttack = _softAttack;
            this.SpottingRange = _spottingRange;
            this.SpritesheetX = _spritesheetX;
            this.SpritesheetY = _spritesheetY;
        }

        #endregion Constructors()
    }
}
