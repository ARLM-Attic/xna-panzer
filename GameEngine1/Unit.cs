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
        private string m_name;

        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

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

            unit.Name = _input.ReadString();

            return unit;
        }
    } // UnitContentTypeReader class

} // namespace
