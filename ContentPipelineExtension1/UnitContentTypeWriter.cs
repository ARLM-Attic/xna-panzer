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

using TWrite = GameEngine1.Unit;

namespace ContentPipelineExtension1
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentTypeWriter]
    public class UnitContentTypeWriter : ContentTypeWriter<TWrite>
    {
        protected override void Write(ContentWriter _output, TWrite _value)
        {
            _output.Write(_value.ID);
            _output.Write(_value.Name);
            _output.Write(_value.Owner);
            _output.Write(_value.StartingX);
            _output.Write(_value.StartingY);
            _output.Write(_value.Strength);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(GameEngine1.UnitContentTypeReader).AssemblyQualifiedName;
        }
    }
}
