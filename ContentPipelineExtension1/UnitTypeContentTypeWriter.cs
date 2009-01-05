using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using GameEngine1;

namespace ContentPipelineExtension
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentTypeWriter]
    public class UnitTypeContentTypeWriter : ContentTypeWriter<GameEngine1.UnitType>
    {
        protected override void Write(ContentWriter _output, GameEngine1.UnitType _value)
        {
            _output.Write(_value.AirAttack);
            _output.Write(_value.AirDefense);
            _output.Write(_value.Ammo);
            _output.WriteObject<DateTime>(_value.AvailabilityEnd);
            _output.WriteObject<DateTime>(_value.AvailabilityStart);
            _output.Write(_value.Characteristics);
            _output.Write(_value.CloseDefense);
            _output.Write(_value.CombatRange);
            _output.Write(_value.Cost);
            _output.Write(_value.EntrenchmentRate);
            _output.Write(_value.Fuel);
            _output.Write(_value.GroundDefense);
            _output.Write(_value.HardAttack);
            _output.Write(_value.ID);
            _output.Write(_value.Initiative);
            _output.WriteObject<GameEngine1.GroundMovementClass>(_value.MovementClass);
            _output.Write(_value.Moves);
            _output.Write(_value.Name);
            _output.Write(_value.Nationality);
            _output.Write(_value.SoftAttack);
            _output.Write(_value.SpottingRange);
            _output.Write(_value.SpritesheetX);
            _output.Write(_value.SpritesheetY);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(GameEngine1.UnitTypeContentTypeReader).AssemblyQualifiedName;
        }
    }
}
