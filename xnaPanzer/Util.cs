using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace xnaPanzer
{
    static class Util
    {
        /// <summary>
        /// Calculates x & y coordinate offsets for the upper left corner of the unit or terrain sprite sheet for 
        /// the specified sprite number.
        /// </summary>
        /// <remarks>
        /// Sample X, Y offsets into the unit sprite sheet for a given unit number:
        /// Unit  0 offsets = 0,   0    Unit 01 offsets = 61,   0   Unit 02 offsets = 122,   0
        /// Unit 10 offsets = 0,  51    Unit 11 offsets = 61,  51   Unit 12 offsets = 122,  51
        /// Unit 20 offsets = 0, 102    Unit 21 offsets = 61, 102   Unit 22 offsets = 122, 102
        /// Bottom line: each unit measures 60W x 50H plus a 1W x 1H border, thus each unit block is 61x51 pixels
        /// 
        /// Warning: no boundary checking is done for the sprite number or size of sprite sheet
        /// </remarks>
        /// <param name="_spriteNumber">Sprite number within the sprite sheet, 0..# of sprites</param>
        /// <returns>Point structure containing the x,y coordinates.</returns>
        public static Point CalculateSpritesheetCoordinates(int _spriteNumber)
        {
            // formula for calculating a unit's x,y coords within the source SHP bitmap:
            // e.g. sprite number = 125 --> tens = 12, ones = 5.  e.g. sprite number 9 --> tens = 0, ones = 9.
            // offset x = (ones * unit_bitmap_width) + (ones * unit_border_width)
            // offset y = (tens * unit_bitmap_height) + (tens * unit_border_height)

            int tens = _spriteNumber / 10;                              // calc which sprite row (0 == top row)
            int ones = _spriteNumber - (tens * 10);                     // calc which column in that row

            // now calc coords within the sprite sheet for the row/column combo
            // note: initializing Point structure here vs. in the return statement for debugging purposes
            Point offset = new Point();
            offset.X = (ones * 60) + (ones * 1);
            offset.Y = (tens * 50) + (tens * 1);

            return offset;
        }

        /// <summary>
        /// Returns the 2-letter ordinal suffix for a given number, e.g. "st" for 1, "nd" for 2, "rd" for 3, "th" for 4
        /// </summary>
        /// <param name="_number">integer for determing the suffix</param>
        /// <returns>string containing 2-letter suffix</returns>
        public static string GetOrdinalSuffix(int _number)
        {
            _number = _number % 10;                                     // we only care about the digit in the one's column

            if (_number >= 1 && _number <= 3) {
                int startIndex = (_number - 1) * 2;                     // calc index into string, 1=0, 2=2, 3=4
                return "stndrd".Substring(startIndex, 2);
            } else {
                return "th";                                            // 0 and 4..9 will return "th"
            }
        }

        public class SpriteContentReader : ContentTypeReader<Sprite>
        {
        }

} // class util

} // namespace
