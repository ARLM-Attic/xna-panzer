/**********************************************************************************************************************
 * 
 * Game1.cs
 * 
 * Contains prototyping code to experiment with creating a hex-based 2D map using Panzer General graphics.  This code
 * is not expected to be used in the actual development of the xnaPanzer game but is merely a proof-of-concept
 * experiment.
 * 
 * Created November 2008 by tscheffel using Visual Studio 2008 Professional and XNA Game Studio 3.0 (beta).
 * 
 * All rights not forfeited by the designated license are hereby reserved.
 * 
 *********************************************************************************************************************/
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
    /// <summary>
    /// This is the prototype development class for xnaPanzer
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager m_graphics;
        SpriteBatch m_spriteBatch;
        Texture2D m_MapSpriteSheet;
        Texture2D m_UnitSpriteSheet;
        Texture2D m_DeltaTextures;
        SpriteFont Font1;

        int[,] m_AllowableMoves = new int[5, 5];                        // to be used for pathfinding algorithm
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

        // defines the Viewpoint (viewable portion of the map)
        const int m_VIEWPORT_HEX_HEIGHT = 10;
        const int m_VIEWPORT_HEX_WIDTH = 10;
        const int m_VIEWPORT_MIN_X_COORD = 50;
        const int m_VIEWPORT_MAX_X_COORD = m_VIEWPORT_MIN_X_COORD + (m_VIEWPORT_HEX_WIDTH * m_HEXPART_LENGTH_BBA) + m_HEXPART_LENGTH_A;
        const int m_VIEWPORT_MIN_Y_COORD = 25;
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
            this.m_graphics.PreferredBackBufferWidth = m_PreferredBackBufferWidth;
            this.m_graphics.PreferredBackBufferHeight = m_PreferredBackBufferHeight;
            //this.m_graphics.IsFullScreen = true;
            this.m_graphics.ApplyChanges();
            this.IsMouseVisible = true;

            //this.m_map = new int[m_MAP_HEX_WIDTH, m_MAP_HEX_HEIGHT];
            Random random = new Random(unchecked((int) (DateTime.Now.Ticks)));
            this.m_map = new int[m_MAP_HEX_WIDTH, m_MAP_HEX_HEIGHT];
            for (int x = 0; x < m_MAP_HEX_WIDTH; x++) {
                for (int y = 0; y < m_MAP_HEX_HEIGHT; y++) {
                    this.m_map[x, y] = random.Next(10);
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
        /// LoadContent will be called once per game and is the place to load all game content
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
        /// UnloadContent will be called once per game and is the place to unload all game content
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
            // get keyboard & gamepad states
            keyboardState = Keyboard.GetState();
            gamepadState = GamePad.GetState(PlayerIndex.One);

            // see if the game should be exited
            if (gamepadState.Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape) == true) {
                this.Exit();
            }

            //
            // Scroll the map in the appropriate direction(s) if the arrows keys were just pressed OR if the
            // mouse cursor is located at an edge of the app window.  Currently scrolls by 2 hexes at a time.
            // The present algorithm does not employ smooth scrolling.
            //

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

            // calculate current hex x,y of mouse cursor (-1,-1 if off viewport)
            MapLocation ml = this.ConvertMousePositionToMapLocation(Mouse.GetState().X, Mouse.GetState().Y);
            this.m_MouseHexX = ml.x;
            this.m_MouseHexY = ml.y;

            // set previous keyboard & gamepad states = to current states so we can detect new keypresses
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

            // uncomment the following line to display all the unit sprites (collage)
            //this.m_spriteBatch.Draw(this.m_UnitSpriteSheet, new Rectangle(0, 0, this.m_UnitSpriteSheet.Width, this.m_UnitSpriteSheet.Height), Color.White);

            Point offset = new Point() ;                                // used to store spritesheet source coords

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
                    sourceRect.X = offset.X;
                    sourceRect.Y = offset.Y;

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
        protected Point CalculateSpritesheetCoordinates(int _spriteNumber)
        {
            // formula for calculating a unit's x,y coords within the source SHP bitmap:
            // e.g. sprite number = 125 --> tens = 12, ones = 5.  e.g. sprite number 9 --> tens = 0, ones = 9.
            // offset x = (ones * unit_bitmap_width) + (ones * unit_border_width)
            // offset y = (tens * unit_bitmap_height) + (tens * unit_border_height)

            int tens = _spriteNumber / 10;
            int ones = _spriteNumber - (tens * 10);

            Point offset = new Point();
            offset.X = (ones * 60) + (ones * 1);
            offset.Y = (tens * 50) + (tens * 1);

            return offset;
        }

        /// <summary>
        /// Converts mouse x,y coordinates to map hex x,y location.
        /// </summary>
        /// <param name="_mouseX">integer contains mouse cursor X coordinate</param>
        /// <param name="_mouseY">integer contains mouse cursor Y coordinate</param>
        /// <returns>MapLocation (x,y point) of hex map x,y (or -1,-1 if off map)</returns>
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

            // calculate relative mouse position within that square using the modulo operator
            // e.g. if mouse x,y = 390,300 (within the map viewport) and hex 3/4 width,height = 60,50 then
            //      : mouse is (390 % 45) = 30 pixels from left edge of that square
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
                    deltaX = -1;
                    deltaY = 1;
                } else {
                }
            } else {                                                    // odd-numbered square
                this.m_DeltaTextures.GetData(0, new Rectangle(mouseXWithinSquare + 45, mouseYWithinSquare, 1, 1), deltaColor, 0, 1);
                if (deltaColor[0].R == 255) {
                    deltaX = -1;
                } else if (deltaColor[0].B == 255) {
                    deltaY = 1;
                } else {
                }
            }

            // now calculate the actual map hex x,y
            int hexX = squareHexX + deltaX + this.m_ViewportLeftHexX;
            int hexY = squareHexY + deltaY + this.m_ViewportTopHexY;
            Console.WriteLine("square: {0},{1}   mouse Relative: {2},{3}   delta: {4},{5}   hex: {6},{7}",
                squareHexX, squareHexY, mouseXWithinSquare, mouseYWithinSquare, deltaX, deltaY, hexX, hexY);
            return new MapLocation(hexX, hexY);
        }

        /// <summary>
        /// Returns true if mouse is currently within the Viewport, false if not
        /// </summary>
        /// <returns></returns>
        private bool IsMouseInViewport()
        {
            int mx = Mouse.GetState().X;
            int my = Mouse.GetState().Y;

            return (mx >= m_VIEWPORT_MIN_X_COORD && mx <= m_VIEWPORT_MAX_X_COORD &&
                my >= m_VIEWPORT_MIN_Y_COORD && my <= m_VIEWPORT_MAX_Y_COORD);
        }

} // class Game1

    /// <summary>
    /// Holds a map hex x,y location.
    /// </summary>
    /// <remarks>
    /// This is essentially a Point structure.  I cloned it to clearly indicate it holds a map hex x,y as opposed to
    /// mouse cursor x,y coordinates.
    /// </remarks>
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

} // namespace xnaPanzer
    
/*
    The following snippet is me jotting down my thoughts for the next big challenge: finding all the hexes that a
    unit can move to.

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

        // clear out the array that contains whether the selected unit can move to a given hex
        for (int x = 0; x < 5; x++) {
            for (int y = 0; y < 5; y++) {
                this.m_AllowableMoves[x, y] = 0;
            }
        }
    }
*/