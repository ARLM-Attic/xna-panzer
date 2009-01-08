using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine1
{
    public class Map
    {
        private string m_name;

        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        public Map() : this("unknown")
        {
        }

        public Map(string _name)
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
    public class MapContentTypeReader : ContentTypeReader<Map>
    {
        protected override Map Read(ContentReader _input, Map _map)
        {
            Map map = new Map();

            map.Name = _input.ReadString();

            return map;
        }
    } // MapContentTypeReader class

} // namespace
