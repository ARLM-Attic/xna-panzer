using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace xnaPanzer
{
    public struct Offset
    {
        public int x;
        public int y;

        public Offset(Int16 _x, Int16 _y)
        {
            this.x = _x;
            this.y = _y;
        }
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager m_graphics;
        SpriteBatch m_spriteBatch;
        Texture2D m_MapSpriteSheet;
        Texture2D m_UnitSpriteSheet;
        Texture2D m_DeltaTextures;
        SpriteFont Font1;

        int[,] m_AllowableMoves = new int[5, 5];
        string[] terrainNames = new string[10] { "Clear", "Mtn", "Airport", "Fort", "Woods", "swamp", "Improved", "Water", "Rough", "City" };

        // constants for calculating combat unit offset into sprite sheet
        const int m_NUM_UNITS_IN_SPRITE_SHEET = 90;
        const int m_UNIT_IMAGE_HEIGHT = 50;
        const int m_UNIT_IMAGE_WIDTH = 60;
        const int m_UNIT_BORDER_HEIGHT = 1;
        const int m_UNIT_BORDER_WIDTH = 1;

        // constants for calculating terrain offset into sprite sheet
        const int m_NUM_TERRAIN_HEXES_IN_SPRITE_SHEET = 10;
        const int m_TERRAIN_IMAGE_HEIGHT = 50;
        const int m_TERRAIN_IMAGE_WIDTH = 60;
        const int m_TERRAIN_BORDER_HEIGHT = 1;
        const int m_TERRAIN_BORDER_WIDTH = 1;

        // 2xA + 2xB = 2x15 + 2x15 = 30 + 30 = 60 (width of each hex in pixels)
        // 2xC = 2x25 = 50 (height of each hex in pixels)
        const int m_HEXPART_LENGTH_A = 15;                              // angle width
        const int m_HEXPART_LENGTH_B = 15;                              // half of square width
        const int m_HEXPART_LENGTH_C = 25;                              // half of hex height
        const int m_HEXPART_LENGTH_BBA = 45;                            // 3/4 width of hex
        const int m_HEXPART_FULL_HEIGHT = 50;                           // full height of hex
        const int m_HEXPART_FULL_WIDTH = 60;                            // full width of hex

        const int m_VIEWPORT_HEX_HEIGHT = 10;
        const int m_VIEWPORT_HEX_WIDTH = 10;
        const int m_VIEWPORT_MIN_X_COORD = 50;
        const int m_VIEWPORT_MAX_X_COORD = m_VIEWPORT_MIN_X_COORD + (m_VIEWPORT_HEX_WIDTH * m_HEXPART_LENGTH_BBA) + m_HEXPART_LENGTH_A;
        const int m_VIEWPORT_MIN_Y_COORD = 50;
        const int m_VIEWPORT_MAX_Y_COORD = m_VIEWPORT_MIN_Y_COORD + (m_VIEWPORT_HEX_HEIGHT * m_HEXPART_FULL_HEIGHT) + m_HEXPART_LENGTH_C;

        const int m_PreferredBackBufferWidth = 800;
        const int m_PreferredBackBufferHeight = 600;

        const int m_MOUSE_SCROLL_MIN_X = 5;
        const int m_MOUSE_SCROLL_MAX_X = m_PreferredBackBufferWidth - 5;
        const int m_MOUSE_SCROLL_MIN_Y = 5;
        const int m_MOUSE_SCROLL_MAX_Y = m_PreferredBackBufferHeight - 5;

        const int m_MAP_HEX_WIDTH = 25;
        const int m_MAP_HEX_HEIGHT = 20;

        int m_ViewportLeftHexX = 0;
        int m_ViewportTopHexY = 0;
        int m_MouseHexX = -1;
        int m_MouseHexY = -1;
        
        int[,] m_map;

        UInt16[] m_DELTA_X_FOR_EVEN_SQUARE_X = new UInt16[50] {
            0x0000, // 0 <-- y coord
            0x0001, // 1
            0x0001, // 2
            0x0003, // 3
            0x0007, // 4
            0x0007, // 5
            0x000F, // 6
            0x000F, // 7
            0x001F, // 8
            0x003F, // 9 (6 bit on)
            0x003F, // 10
            0x007F, // 11
            0x00FF, // 12
            0x00FF, // 13
            0x01FF, // 14
            0x01FF, // 15
            0x03FF, // 16
            0x07FF, // 17
            0x07FF, // 18
            0x0FFF, // 19
            0x1FFF, // 20
            0x1FFF, // 21
            0x3FFF, // 22
            0x3FFF, // 23
            0x7FFF, // 24

            0xFFFF, // 25
            0x7FFF,
            0x7FFF,
            0x3FFF,
            0x3FFF,
            0x1FFF, // 30
            0x0fff,
            0x0fff,
            0x07ff,
            0x03ff,
            0x03ff,
            0x01ff,
            0x01ff,
            0x00ff,
            0x007f,
            0x007f, // 40
            0x003f,
            0x001f,
            0x001f,
            0x000f,
            0x000f,
            0x0007,
            0x0003,
            0x0003,
            0x0001  // 49 (line 50)
        };

        UInt16[] m_DELTA_X_FOR_ODD_SQUARE_X = new UInt16[50] {  // TODO: NOT ACTUAL VALUE!!!
            0x0000, // 0 <-- y coord
            0x0001, // 1
            0x0001, // 2
            0x0003, // 3
            0x0007, // 4
            0x0007, // 5
            0x000F, // 6
            0x000F, // 7
            0x001F, // 8
            0x003F, // 9 (6 bit on)
            0x003F, // 10
            0x007F, // 11
            0x00FF, // 12
            0x00FF, // 13
            0x01FF, // 14
            0x01FF, // 15
            0x03FF, // 16
            0x07FF, // 17
            0x07FF, // 18
            0x0FFF, // 19
            0x1FFF, // 20
            0x1FFF, // 21
            0x3FFF, // 22
            0x3FFF, // 23
            0x7FFF, // 24

            0xFFFF, // 25
            0x7FFF,
            0x7FFF,
            0x3FFF,
            0x3FFF,
            0x1FFF, // 30
            0x0fff,
            0x0fff,
            0x07ff,
            0x03ff,
            0x03ff,
            0x01ff,
            0x01ff,
            0x00ff,
            0x007f,
            0x007f, // 40
            0x003f,
            0x001f,
            0x001f,
            0x000f,
            0x000f,
            0x0007,
            0x0003,
            0x0003,
            0x0001  // 49 (line 50)
        };

        UInt16[] m_DELTA_Y_FOR_EVEN_SQUARE_X = new UInt16[50] {
            0x0000, // 0 <-- y coord
            0x0000, // 1
            0x0000, // 2
            0x0000, // 3
            0x0000, // 4
            0x0000, // 5
            0x0000, // 6
            0x0000, // 7
            0x0000, // 8
            0x0000, // 9 (6 bit on)
            0x0000, // 10
            0x0000, // 11
            0x0000, // 12
            0x0000, // 13
            0x0000, // 14
            0x0000, // 15
            0x0000, // 16
            0x0000, // 17
            0x0000, // 18
            0x0000, // 19
            0x0000, // 20
            0x0000, // 21
            0x0000, // 22
            0x0000, // 23
            0x0000, // 24

            0xFFFF, // 25
            0x7FFF,
            0x7FFF,
            0x3FFF,
            0x3FFF,
            0x1FFF, // 30
            0x0fff,
            0x0fff,
            0x07ff,
            0x03ff,
            0x03ff,
            0x01ff,
            0x01ff,
            0x00ff,
            0x007f,
            0x007f, // 40
            0x003f,
            0x001f,
            0x001f,
            0x000f,
            0x000f,
            0x0007,
            0x0003,
            0x0003,
            0x0001  // 49 (line 50)
        };

        UInt16[] m_DELTA_Y_FOR_ODD_SQUARE_X = new UInt16[50] {  // TODO: not actual values!!!
            0x0000, // 0 <-- y coord
            0x0000, // 1
            0x0000, // 2
            0x0000, // 3
            0x0000, // 4
            0x0000, // 5
            0x0000, // 6
            0x0000, // 7
            0x0000, // 8
            0x0000, // 9 (6 bit on)
            0x0000, // 10
            0x0000, // 11
            0x0000, // 12
            0x0000, // 13
            0x0000, // 14
            0x0000, // 15
            0x0000, // 16
            0x0000, // 17
            0x0000, // 18
            0x0000, // 19
            0x0000, // 20
            0x0000, // 21
            0x0000, // 22
            0x0000, // 23
            0x0000, // 24

            0x0001, // line 25
            0x0001,
            0x0001,
            0x0001,
            0x0001,
            0x0001, // 30
            0x0001,
            0x0001,
            0x0001,
            0x0001,
            0x0001,
            0x0001,
            0x0001,
            0x0001,
            0x0001,
            0x0001, // 40
            0x0001,
            0x0001,
            0x0001,
            0x0001,
            0x0001,
            0x0001,
            0x0001,
            0x0001,
            0x0001  // 49 (line 50)
        };

        int[, ,] m_DeltaY = new int[2, 6, 5];

        KeyboardState keyboardState;
        KeyboardState previousKeyboardState;
        GamePadState gamepadState;
        GamePadState previousGamepadState;

        public Game1()
        {
            m_graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.m_graphics.PreferredBackBufferWidth = 800;
            this.m_graphics.PreferredBackBufferHeight = 600;
            //this.m_graphics.IsFullScreen = true;
            this.m_graphics.ApplyChanges();
            this.IsMouseVisible = true;

            //this.m_map = new int[m_MAP_HEX_WIDTH, m_MAP_HEX_HEIGHT];
            Random random = new Random(unchecked((int) (DateTime.Now.Ticks)));
            this.m_map = new int[m_MAP_HEX_WIDTH, m_MAP_HEX_HEIGHT];
            for (int x = 0; x < m_MAP_HEX_WIDTH; x++) {
                //this.m_map[x][ = new int[20];
                for (int y = 0; y < m_MAP_HEX_HEIGHT; y++) {
                    this.m_map[x, y] = random.Next(10); // (int)(y % 10); // random.Next(10);
                    //Console.WriteLine("m_map[{0},{1}] = {2}", x.ToString(), y.ToString(), this.terrainNames[this.m_map[x,y]]);
                }
            }

            for (int i = 0; i < 2; i++) {
                for (int x = 0; x < 6; x++) {
                    for (int y = 0; y < 5; y++) {
                        this.m_DeltaY[i, x, y] = 0;
                    }
                }
            }

            // init keyboard & gamepad states (the previous ones are used to help detect keypresses)
            KeyboardState keyboardState = Keyboard.GetState();
            KeyboardState previousKeyboardState = Keyboard.GetState();
            GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);
            GamePadState previousGamepadState = GamePad.GetState(PlayerIndex.One);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.m_spriteBatch = new SpriteBatch(GraphicsDevice);

            //Load the images for map terrain and combat units from bitmap files
            m_MapSpriteSheet = this.Content.Load<Texture2D>("tacmap_terrain_xnaPanzer");
            this.m_DeltaTextures = this.Content.Load<Texture2D>("DeltaTextures");
            m_UnitSpriteSheet = this.Content.Load<Texture2D>("tacicons_start_at_0");

            Font1 = Content.Load<SpriteFont>("Fonts/SpriteFont1");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // get keyboard & gamepad state
            keyboardState = Keyboard.GetState();
            gamepadState = GamePad.GetState(PlayerIndex.One);

            //Check to see if the game should be exited
            if (gamepadState.Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape) == true)
            {
                this.Exit();
            }

            // scroll the map in the appropriate direction(s) if the arrows keys were just pressed

            if ((keyboardState.IsKeyDown(Keys.Down) && !this.previousKeyboardState.IsKeyDown(Keys.Down)) ||
                (Mouse.GetState().Y >= m_MOUSE_SCROLL_MAX_Y)) {
                if (this.m_ViewportTopHexY + m_VIEWPORT_HEX_HEIGHT + 2 < m_MAP_HEX_HEIGHT) {
                    this.m_ViewportTopHexY += 2;
                }
            }

            if ((keyboardState.IsKeyDown(Keys.Up) && !this.previousKeyboardState.IsKeyDown(Keys.Up)) ||
                (Mouse.GetState().Y <= m_MOUSE_SCROLL_MIN_Y)) {
                if (this.m_ViewportTopHexY >= 2) {
                    this.m_ViewportTopHexY -= 2;
                }
            }

            if ((keyboardState.IsKeyDown(Keys.Left) && !this.previousKeyboardState.IsKeyDown(Keys.Left)) ||
                (Mouse.GetState().X <= m_MOUSE_SCROLL_MIN_X)) {
                if (this.m_ViewportLeftHexX  >= 2) {
                    this.m_ViewportLeftHexX -= 2;
                }
            }

            if ((keyboardState.IsKeyDown(Keys.Right) && !this.previousKeyboardState.IsKeyDown(Keys.Right)) ||
                (Mouse.GetState().X >= m_MOUSE_SCROLL_MAX_X)) {
                if (this.m_ViewportLeftHexX + m_VIEWPORT_HEX_WIDTH + 2 < m_MAP_HEX_WIDTH) {
                    this.m_ViewportLeftHexX += 2;
                }
            }

            MouseState ms = new MouseState();
            ms = Mouse.GetState();

            MapLocation ml = this.ConvertMousePositionToMapLocation(ms.X, ms.Y);
            this.m_MouseHexX = ml.x;
            this.m_MouseHexY = ml.y;

            previousKeyboardState = keyboardState;
            previousGamepadState = gamepadState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            m_graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            this.m_spriteBatch.Begin();

            //this.m_spriteBatch.Draw(this.m_UnitSpriteSheet, new Rectangle(0, 0, this.m_UnitSpriteSheet.Width, this.m_UnitSpriteSheet.Height), Color.White);
            Offset offset = new Offset() ; // = this.CalculateSpritesheetCoordinates(this.m_UnitCounter);

            // draw all the hexes that are fully or partially visible within the viewport
            // note: the game GUI will mask out partial hexes around the edges of the viewport when it is drawn
            // in a later step
            int rightmostHexX = this.m_ViewportLeftHexX + m_VIEWPORT_HEX_WIDTH;
            int bottommostHexY = this.m_ViewportTopHexY + m_VIEWPORT_HEX_HEIGHT;
            Rectangle sourceRect = new Rectangle(0, 0, m_HEXPART_FULL_WIDTH, m_HEXPART_FULL_HEIGHT);

            int relativeY = -99;
            int columnNumber = 0;
            int rowNumber;
            for (int x = this.m_ViewportLeftHexX; x < rightmostHexX; x++) {
                rowNumber = 0;
                for (int y = this.m_ViewportTopHexY; y < bottommostHexY; y++) {
                    offset = this.CalculateSpritesheetCoordinates(this.m_map[x, y]);
                    sourceRect.X = offset.x;
                    sourceRect.Y = offset.y;

                    // calculate where the hex should be drawn on the viewport
                    relativeY = (y % m_VIEWPORT_HEX_HEIGHT) - 1;
                    Rectangle destRect = new Rectangle(m_VIEWPORT_MIN_X_COORD + (columnNumber * m_HEXPART_LENGTH_BBA),
                            m_VIEWPORT_MIN_Y_COORD + (rowNumber * m_HEXPART_FULL_HEIGHT), m_HEXPART_FULL_WIDTH, m_HEXPART_FULL_HEIGHT);

                    if ((columnNumber % 2) == 1) {                                 // if remainder = 1 then odd-numbered column
                        destRect.Y += m_HEXPART_LENGTH_C;
                    }

                    this.m_spriteBatch.Draw(this.m_MapSpriteSheet,
                        destRect,                                       // destination rectangle
                        sourceRect,                                     // source rectangle
                        Color.White);                                   // white = don't apply tinting

                    ++rowNumber;
                }
                //Console.WriteLine("x = " + x.ToString() +
                //    ", map[x,m_viewportTopHexY] = " + this.m_map[x, this.m_ViewportTopHexY].ToString() +
                //    ", offset = " + offset.x.ToString() + "," + offset.y.ToString() +
                //    ", relativeY = " + relativeY.ToString());
                ++columnNumber;
            }

            if (this.IsMouseInViewport()) {
                this.m_spriteBatch.DrawString(Font1,
                    "m_ViewportLeftHexX = " + this.m_ViewportLeftHexX.ToString() +
                    ", m_ViewportTopHexY = " + this.m_ViewportTopHexY.ToString() +
                    ", Mouse hex X,Y = " + this.m_MouseHexX.ToString() + ", " + this.m_MouseHexY.ToString()
                    , new Vector2(10, 550), Color.White);
            } else {
                MouseState ms = new MouseState();
                ms = Mouse.GetState();
                this.m_spriteBatch.DrawString(Font1,
                    "Mouse coord X,Y = " + ms.X.ToString() + ", " + ms.Y.ToString()
                    , new Vector2(10, 550), Color.White);
            }
            this.m_spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Calculates x & y coordinate offsets for the upper left corner of the unit sprite sheet for the specified
        /// sprite number.
        /// </summary>
        /// <remarks>
        /// Sample X, Y offsets into the unit sprite sheet for a given unit number:
        /// Unit  0 offsets = 0,   0    Unit 01 offsets = 61,   0   Unit 02 offsets = 122,   0
        /// Unit 10 offsets = 0,  51    Unit 11 offsets = 61,  51   Unit 12 offsets = 122,  51
        /// Unit 20 offsets = 0, 102    Unit 21 offsets = 61, 102   Unit 22 offsets = 122, 102
        /// Bottom line: each unit measures 60W x 50H plus a 1W x 1H border, thus each unit block is 61x51 pixels
        /// </remarks>
        /// <param name="_spriteNumber">Sprite number within the sprite sheet, 0..# of sprites</param>
        /// <returns>An Offset object containing the x & y coords.</returns>
        protected Offset CalculateSpritesheetCoordinates(int _spriteNumber)
        {
            // formula for calculating a unit's x,y coords within the source SHP bitmap:
            // e.g. sprite number = 125 --> tens = 12, ones = 5.  e.g. sprite number 9 --> tens = 0, ones = 9.
            // offset x = (ones * unit_bitmap_width) + (ones * unit_border_width)
            // offset y = (tens * unit_bitmap_height) + (tens * unit_border_height)

            int tens = _spriteNumber / 10;
            int ones = _spriteNumber - (tens * 10);

            Offset offset = new Offset();
            offset.x = (ones * 60) + (ones * 1);
            offset.y = (tens * 50) + (tens * 1);

            return offset;
        }
        
        public MapLocation ConvertMousePositionToMapLocation(int _mouseX, int _mouseY)
        {
            // abort if mouse is not within the viewport (the map portion of the screen)
            if (_mouseX < m_VIEWPORT_MIN_X_COORD || _mouseX > m_VIEWPORT_MAX_X_COORD ||
                _mouseY < m_VIEWPORT_MIN_Y_COORD || _mouseY > m_VIEWPORT_MAX_Y_COORD) {
                return new MapLocation(-1, -1);
            }

            // adjust mouse coords for viewport position relative to top-left of screen
            _mouseX = _mouseX - m_VIEWPORT_MIN_X_COORD;
            _mouseY = _mouseY - m_VIEWPORT_MIN_Y_COORD;

            // calculate which map square mouse cursor is in (each square is composed of 3 partial hexes)
            // there are 2 types of map squares, one for hex X being even and one for odd
            int squareHexX = (int)(_mouseX / m_HEXPART_LENGTH_BBA);
            int squareHexY = (int)(_mouseY / m_HEXPART_FULL_HEIGHT);

            // calculate relative mouse position within that square
            // e.g. if mouse x,y = 390,300 (within the map viewport) and hex width,height = 60,50 then
            //      : mouse is (390 % 60) = 30 pixels from left edge of that square
            //      : mouse is (300 % 50) = 0 pixels from top edge of that square
            //      : thus, the mouse cursor is at the very top-center of that map square
            int mouseXWithinSquare = _mouseX % m_HEXPART_LENGTH_BBA; ;
            int mouseYWithinSquare = _mouseY % m_HEXPART_FULL_HEIGHT;

            // drill into mask square to see what x,y values (deltas) need to be added to square x,y to yield map hex x,y
            // note: there are two mask squares: one where square x is odd and one for even
            int deltaX = 0; // m_DeltaX[isXOdd, mouseXWithinSquare, mouseYWithinSquare];
            int deltaY = 0; // m_DeltaX[isXOdd, mouseXWithinSquare, mouseYWithinSquare];
            Color[] deltaColor = new Color[1];
            if ((squareHexX & 1) == 0) {                                // even-numbered square?
                this.m_DeltaTextures.GetData(0, new Rectangle(mouseXWithinSquare, mouseYWithinSquare, 1, 1), deltaColor, 0, 1);
                if (deltaColor[0].R == 255) {
                    deltaX = -1;
                } else if (deltaColor[0].B == 255) {
                    deltaY = 1;
                } else {
                }
  //              Console.WriteLine("Even X: Pixel Color for " + mouseXWithinSquare + "," + mouseYWithinSquare + " = " + deltaColor[0].ToString());
            } else {                                                    // odd-numbered square
                this.m_DeltaTextures.GetData(0, new Rectangle(mouseXWithinSquare + 45, mouseYWithinSquare, 1, 1), deltaColor, 0, 1);
                if (deltaColor[0].R == 255) {
                    deltaX = -1;
                } else if (deltaColor[0].B == 255) {
                    deltaY = 1;
                } else {
                }
//                Console.WriteLine("Odd X: Pixel Color for " + mouseXWithinSquare + "," + mouseYWithinSquare + " = " + deltaColor[0].ToString());
            } //((squareHexX % 1) == 0));                      // 1 = even, 0 = odd
//            Console.WriteLine("Pixel Color = " + deltaColor[0].ToString());

   ////declare a uint array to hold the pixel data of our tilemap     backBufferData.GetData<Color>(
        //0,
        //sourceRectangle,
        //retrievedColor,
        //0,
        //1);

   //uint[] tilesource = new uint[tileMap.Width * tileMap.Height]; 
   ////populate the array 
   //tileMap.GetData(tilesource, 0, tileMap.Width * tileMap.Height);

            // now calculate the actual map hex x,y
            int hexX = squareHexX + deltaX + this.m_ViewportLeftHexX;
            int hexY = squareHexY + deltaY + this.m_ViewportTopHexY;
            Console.WriteLine("square: {0},{1}   mouse Relative: {2},{3}   delta: {4},{5}   hex: {6},{7}",
                squareHexX, squareHexY, mouseXWithinSquare, mouseYWithinSquare, deltaX, deltaY, hexX, hexY);
            return new MapLocation(hexX, hexY);
        }

        private bool IsMouseInViewport()
        {
            int mx = Mouse.GetState().X;
            int my = Mouse.GetState().Y;

            return (mx >= m_VIEWPORT_MIN_X_COORD && mx <= m_VIEWPORT_MAX_X_COORD &&
                my >= m_VIEWPORT_MIN_Y_COORD && my <= m_VIEWPORT_MAX_Y_COORD);
        }

        private float CalculateEvenHexSlopeForUpperHalf(int _x, int _y)
        {
            return (float) (_y - 24) / (_x - 0);
        }


} // class Game1

    public struct MapLocation
    {
        public int x;
        public int y;

        public MapLocation(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
    }

    public struct CoOrds
    {
        public int x, y;

        public CoOrds(int p1, int p2) {
            x = p1;
            y = p2;
        }
    }

}

    
    /*
    

    }
 
    private int[,] m_AllowableMoves;
 
    private void PathFinding(int _hexX, int _hexY)
    {
            for (int x = 0; x < m_MAP_HEX_WIDTH; x++) {
                //this.m_map[x][ = new int[20];
                for (int y = 0; y < m_MAP_HEX_HEIGHT; y++) {
                    this.m_map[x,y] = random.Next(10);
                }
            }

 * // clear out the array that contains whether the selected unit can move to a given hex
        for (int x = 0; x < 5; x++) {
            for (int y = 0; y < 5; y++) {
                this.m_AllowableMoves[x, y] = 0;
            }
        }
    }
*/