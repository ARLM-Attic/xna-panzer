using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using XmlContentShared;

namespace ContentPipelineExtension
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentTypeWriter]
    public class UnitContentTypeWriter : ContentTypeWriter<XmlContentShared.Unit>
    {
        protected override void Write(ContentWriter _output, XmlContentShared.Unit _value)
        {
            _output.Write(_value.ID);
            _output.Write(_value.Experience);
            //_output.WriteObject<Boolean>(_value.HasMoved);
            _output.Write(_value.Moves);
            _output.Write(_value.Name);
            _output.Write(_value.Owner);
            _output.Write(_value.StartingX);
            _output.Write(_value.StartingY);
            _output.Write(_value.Strength);
            _output.Write(_value.UnitTypeID);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(XmlContentShared.UnitContentTypeReader).AssemblyQualifiedName;
        }
    }
}
