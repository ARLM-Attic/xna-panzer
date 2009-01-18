using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine1
{
    public class Mapport
    {
        public int MinXCoordDrawable { get; private set; }
        public int MaxXCoordDrawable { get; private set; }
        public int MinYCoordDrawable { get; private set; }
        public int MaxYCoordDrawable { get; private set; }

        public int MinXCoordVisible { get; private set; }
        public int MaxXCoordVisible { get; private set; }
        public int MinYCoordVisible { get; private set; }
        public int MaxYCoordVisible { get; private set; }

        public Mapport()
        {
        }

        public Mapport(int _minXCoordDrawable, int _maxXCoordDrawable, int _minYCoordDrawable, int _maxYCoordDrawable,
            int _minXCoordVisible, int _maxXCoordVisible, int _minYCoordVisible, int _maxYCoordVisible)
        {
            this.MinXCoordDrawable = _minXCoordDrawable;
            this.MaxXCoordDrawable = _maxXCoordDrawable;
            this.MinYCoordDrawable = _minYCoordDrawable;
            this.MaxYCoordDrawable = _maxYCoordDrawable;

            this.MinXCoordVisible = _minXCoordVisible;
            this.MaxXCoordVisible = _maxXCoordVisible;
            this.MinYCoordVisible = _minYCoordVisible;
            this.MaxYCoordVisible = _maxYCoordVisible;
        }

        /// <summary>
        /// Returns true if x,y coords are within the Mapport region
        /// </summary>
        /// <param name="_x">integer containing X coordinate</param>
        /// <param name="_y">integer containing Y coordinate</param>
        /// <returns>Returns true if x,y coords are within the Mapport region, otherwise false</returns>
        public bool IsMouseWithinMapport(int _x, int _y)
        {
            return (_x >= this.MinXCoordDrawable && _x <= this.MaxXCoordDrawable &&
                _y >= this.MinYCoordDrawable && _y <= this.MaxYCoordDrawable);
        }
    }
}
