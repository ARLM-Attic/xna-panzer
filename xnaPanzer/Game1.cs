/**********************************************************************************************************************
 * 
 * DISCLAIMER: this file contains PROTOTYPE code to be used for proof-of-concept development only.
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
        SpriteFont m_font1;

        Texture2D m_BevelLeftHook;
        Texture2D m_BevelRightHook;
        Texture2D m_BevelStraightLine;
        Texture2D m_DefaultGameScreenMask;

        List<MapNode> m_ClosedList = new List<MapNode>();
        private int[] m_DeltaX = new int[6] { 0, 1, 1, 0, -1, -1 };
        //private int[] m_DeltaYForEvenColumn = new int[6] { -1, -1, 0, 1, 0, -1 };
        //private int[] m_DeltaYForOddColumn = new int[6] { -1, 0, 1, 1, 1, 0 };
        private int[] m_DeltaYForEvenColumn = new int[6] { -1, 0, 1, 1, 1, 0 };
        private int[] m_DeltaYForOddColumn = new int[6] { -1, -1, 0, 1, 0, -1 };

        string[] m_TerrainNames = new string[10] { "Clear", "Mtn", "Airport", "Fort", "Woods", "swamp", "Improved", "Water", "Rough", "City" };

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
        const int m_VIEWPORT_HEX_WIDTH = 16;
        const int m_VIEWPORT_MIN_X_COORD_DRAW = 0;
        const int m_VIEWPORT_MIN_X_COORD_VISIBLE = 45;
        const int m_VIEWPORT_MAX_X_COORD_DRAW = m_VIEWPORT_MIN_X_COORD_DRAW + (m_VIEWPORT_HEX_WIDTH * m_HEXPART_LENGTH_BBA) + m_HEXPART_LENGTH_A;
        const int m_VIEWPORT_MAX_X_COORD_VISIBLE = m_VIEWPORT_MIN_X_COORD_DRAW + (m_VIEWPORT_HEX_WIDTH * m_HEXPART_LENGTH_BBA) + m_HEXPART_LENGTH_A - 45;
        const int m_VIEWPORT_MIN_Y_COORD_DRAW = 25;
        const int m_VIEWPORT_MIN_Y_COORD_VISIBLE = 75;
        const int m_VIEWPORT_MAX_Y_COORD_DRAW = m_VIEWPORT_MIN_Y_COORD_DRAW + (m_VIEWPORT_HEX_HEIGHT * m_HEXPART_FULL_HEIGHT) + m_HEXPART_LENGTH_C;
        const int m_VIEWPORT_MAX_Y_COORD_VISIBLE = m_VIEWPORT_MIN_Y_COORD_DRAW + (m_VIEWPORT_HEX_HEIGHT * m_HEXPART_FULL_HEIGHT) + m_HEXPART_LENGTH_C - 25;

        const int m_PreferredBackBufferWidth = 800;
        const int m_PreferredBackBufferHeight = 600;

        const int m_MOUSE_SCROLL_MIN_X = 5;
        const int m_MOUSE_SCROLL_MAX_X = m_PreferredBackBufferWidth - 5;
        const int m_MOUSE_SCROLL_MIN_Y = 5;
        const int m_MOUSE_SCROLL_MAX_Y = m_PreferredBackBufferHeight - 5;

        const int m_MAP_HEX_WIDTH = 25;
        const int m_MAP_HEX_HEIGHT = 20;
        const int m_MAP_MAX_X = 24;
        const int m_MAP_MAX_Y = 19;

        bool m_LeftButtonPressed = false;
        Nullable<MapLocation> m_HexSelected = null;
        int[,] m_AllowableMoves = new int[m_MAP_MAX_X + 1, m_MAP_MAX_Y + 1];    // to be used for pathfinding algorithm

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
            this.m_MapSpriteSheet = this.Content.Load<Texture2D>("tacmap_terrain_xnaPanzer");
            this.m_DeltaTextures = this.Content.Load<Texture2D>("DeltaTextures");
            this.m_UnitSpriteSheet = this.Content.Load<Texture2D>("tacicons_start_at_0");

            this.m_font1 = Content.Load<SpriteFont>("Fonts/SpriteFont1");

            this.m_BevelLeftHook = Content.Load<Texture2D>("GUI/Bevel_Left_Hook");
            this.m_BevelRightHook = Content.Load<Texture2D>("GUI/Bevel_Right_Hook");
            this.m_BevelStraightLine = Content.Load<Texture2D>("GUI/Bevel_Straight_Line");
            this.m_DefaultGameScreenMask = Content.Load<Texture2D>("GUI/Default_Game_Screen_Mask");
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
            // mouse cursor is located at an edge of the app window.  Currently scrolls by 2 hexes at a time
            // (eventually will need to scroll a few pixels at a time for smooth scrolling).
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

            // see if user left clicks a hex to select it--if so, we need to calculate pathfinding
            if (this.IsMouseWithinViewport() && Mouse.GetState().LeftButton == ButtonState.Pressed && !this.m_LeftButtonPressed) {
                this.m_LeftButtonPressed = true;
                this.m_HexSelected = new MapLocation(this.m_MouseHexX, this.m_MouseHexY);
                this.SetAllowableMoves(new Unit(this.m_MouseHexX, this.m_MouseHexY));
            }

            // deselect current hex/unit so allowable moves are no longer displayed
            if (this.m_LeftButtonPressed && Mouse.GetState().RightButton == ButtonState.Pressed) {
                this.m_LeftButtonPressed = false;
                this.m_HexSelected = null;
            }

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
                    Rectangle destRect = new Rectangle(m_VIEWPORT_MIN_X_COORD_DRAW + (columnNumber * m_HEXPART_LENGTH_BBA),
                            m_VIEWPORT_MIN_Y_COORD_DRAW + (rowNumber * m_HEXPART_FULL_HEIGHT), m_HEXPART_FULL_WIDTH, m_HEXPART_FULL_HEIGHT);

                    if (this.IsEvenNumber(columnNumber)) {             // shift odd-numbered columns down half a hex
                        destRect.Y += m_HEXPART_LENGTH_C;
                    }

                    if (this.m_HexSelected != null && this.m_AllowableMoves[x,y] == 0) {
                        this.m_spriteBatch.Draw(this.m_MapSpriteSheet,
                            destRect,                                       // destination rectangle
                            sourceRect,                                     // source rectangle
                            Color.DarkGray);                                   // white = don't apply tinting
                    } else {
                        this.m_spriteBatch.Draw(this.m_MapSpriteSheet,
                            destRect,                                       // destination rectangle
                            sourceRect,                                     // source rectangle
                            Color.White);                                   // white = don't apply tinting
                    }
                    ++rowNumber;
                }
                ++columnNumber;
            }

            // mask out viewport border
            this.m_spriteBatch.Draw(this.m_DefaultGameScreenMask, new Rectangle(0, 0, 800, 600), Color.White);

            // 85,75 straight down
            //this.m_spriteBatch.Draw(this.m_BevelLeftHook, new Rectangle(85,75,8, 120), Color.White);

            // display info text at top
            this.m_spriteBatch.DrawString(this.m_font1, "Scroll map with arrow keys and mouse.  Select/deselect a hex with left/right clicks." +
                "\r\nNon-shaded hexes show where fictitious unit could NOT move.  Does not account\r\nfor terrain, enemy units, etc.",
                new Vector2(10, 3), Color.White);

            if (this.IsMouseWithinViewport()) {
                this.m_spriteBatch.DrawString(this.m_font1,
                    "m_ViewportLeftHexX = " + this.m_ViewportLeftHexX.ToString() +
                    ", m_ViewportTopHexY = " + this.m_ViewportTopHexY.ToString() +
                    ", Mouse hex X,Y = " + this.m_MouseHexX.ToString() + ", " + this.m_MouseHexY.ToString()
                    , new Vector2(10, 550), Color.White);
                this.m_spriteBatch.DrawString(this.m_font1,
                    "Mouse coord X,Y = " + Mouse.GetState().X.ToString() + ", " + Mouse.GetState().Y.ToString()
                    , new Vector2(10, 570), Color.White);
            } else {
                MouseState ms = new MouseState();
                ms = Mouse.GetState();
                this.m_spriteBatch.DrawString(this.m_font1,
                    "Mouse coord X,Y = " + ms.X.ToString() + ", " + ms.Y.ToString()
                    , new Vector2(10, 550), Color.White);
            }
            this.m_spriteBatch.End();

            base.Draw(gameTime);
        }

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
        protected Point CalculateSpritesheetCoordinates(int _spriteNumber)
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
        /// Converts mouse x,y coordinates to map hex x,y location.
        /// </summary>
        /// <param name="_mouseX">integer contains mouse cursor X coordinate</param>
        /// <param name="_mouseY">integer contains mouse cursor Y coordinate</param>
        /// <returns>MapLocation (x,y point) of hex map x,y (or -1,-1 if off map)</returns>
        public MapLocation ConvertMousePositionToMapLocation(int _mouseX, int _mouseY)
        {
            // abort if mouse is not within the viewport (the map portion of the screen)
            if (_mouseX < m_VIEWPORT_MIN_X_COORD_VISIBLE || _mouseX > m_VIEWPORT_MAX_X_COORD_VISIBLE ||
                _mouseY < m_VIEWPORT_MIN_Y_COORD_VISIBLE || _mouseY > m_VIEWPORT_MAX_Y_COORD_VISIBLE) {
                return new MapLocation(-1, -1);
            }

            // adjust mouse coords for viewport position relative to top-left of screen
            _mouseX = _mouseX - m_VIEWPORT_MIN_X_COORD_VISIBLE;
            _mouseY = _mouseY - m_VIEWPORT_MIN_Y_COORD_VISIBLE;

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

            // Drill into the appropriate mask square to see what x,y deltas (-1, 0 or +1 values) need to be added
            // to the basic square x,y to yield map hex x,y.  There are two mask squares: one for when square x is
            // odd and one for even.  Assume no delta adjustments (thus the initialization to zero below).
            // The color within the mask square for the cursor location will be used to determine the delta values.
            int deltaX = 0; // note: this is original method -> m_DeltaX[isXOdd, mouseXWithinSquare, mouseYWithinSquare];
            int deltaY = 0; // note: this is original method -> m_DeltaY[isXOdd, mouseXWithinSquare, mouseYWithinSquare];
            Color[] deltaColor = new Color[1];

            // e.g. if even-numbered sqare x and cursor is within Red portion of mask square then subtract 1 from 
            // square x and 0 (the default value) from square y to yield the map hex x,y location
            if ((squareHexX & 1) == 0) {                                // even-numbered square?
                this.m_DeltaTextures.GetData(0, new Rectangle(mouseXWithinSquare, mouseYWithinSquare, 1, 1), deltaColor, 0, 1);
                if (deltaColor[0].R == 255) {         
                } else if (deltaColor[0].B == 255) {
                    deltaY = 1;
                } else { // green
                    deltaX = 1;
                    deltaY = 1;
                }
            } else {                                                    // odd-numbered square
                this.m_DeltaTextures.GetData(0, new Rectangle(mouseXWithinSquare + 45, mouseYWithinSquare, 1, 1), deltaColor, 0, 1);
                if (deltaColor[0].R == 255) {
                    deltaY = 1;
                } else if (deltaColor[0].B == 255) {
                    deltaX = 1;
                    deltaY = 1;
                } else { // green
                    deltaX = 1;
                }
            }

            // now calculate the actual map hex x,y by adding basic square x,y + delta x,y adjustments + upper left
            // hex x,y of viewport 
            int hexX = squareHexX + deltaX + this.m_ViewportLeftHexX;
            int hexY = squareHexY + deltaY + this.m_ViewportTopHexY;
            
            // uncomment for debugging info in debugger's Output window
            //Console.WriteLine("square: {0},{1}   mouse Relative: {2},{3}   delta: {4},{5}   hex: {6},{7}",
            //    squareHexX, squareHexY, mouseXWithinSquare, mouseYWithinSquare, deltaX, deltaY, hexX, hexY);

            return new MapLocation(hexX, hexY);
        }

        /// <summary>
        /// Returns true if mouse is currently within the Viewport, false if not
        /// </summary>
        /// <returns>
        /// Returns true if mouse is currently within the Viewport, false if not
        /// </returns>
        private bool IsMouseWithinViewport()
        {
            return (Mouse.GetState().X >= m_VIEWPORT_MIN_X_COORD_VISIBLE && Mouse.GetState().X <= m_VIEWPORT_MAX_X_COORD_VISIBLE &&
                Mouse.GetState().Y >= m_VIEWPORT_MIN_Y_COORD_VISIBLE && Mouse.GetState().Y <= m_VIEWPORT_MAX_Y_COORD_VISIBLE);
        }

        /// <summary>
        /// Returns true if the mouse cursor is currently within the application window, false if not
        /// </summary>
        /// <remarks>
        /// Not sure if using this.m_graphics.PreferredBackBufferXXX is the proper way to get max x,y coordinate
        /// values for the app window
        /// </remarks>
        /// <returns>
        /// Returns true if the mouse cursor is currently within the application window, false if not
        /// </returns>
        private bool IsMouseWithinAppWindow()
        {
            return (Mouse.GetState().X >= 0 && Mouse.GetState().X < this.m_graphics.PreferredBackBufferWidth &&
                Mouse.GetState().Y >= 0 && Mouse.GetState().Y < this.m_graphics.PreferredBackBufferHeight);
        }


        /*
        1. Create an open list and a closed list that are both empty. Put the start node in the open list.
        2. Loop this until the goal is found or the open list is empty:
              a. Find the node with the lowest F cost in the open list and place it in the closed list.
              b. Expand this node and for the adjacent nodes to this node:
                    i. If they are on the closed list, ignore.
                    ii. If not on the open list, add to open list, store the current node as the parent for this adjacent node, and calculate the             F,G, H costs of the adjacent node.
                    iii. If on the open list, compare the G costs of this path to the node and the old path to the node. If the G cost of using the             current node to get to the node is the lower cost, change the parent node of the adjacent node to the current node.             Recalculate F,G,H costs of the node.
        3. If open list is empty, fail.
        */
 
        public void SetAllowableMoves(Unit _unit)
        {
            // clear out the array that contains whether the selected unit can move to a given hex
            for (int x = 0; x <= m_MAP_MAX_X; x++) {
                for (int y = 0; y <= m_MAP_MAX_Y; y++) {
                    this.m_AllowableMoves[x, y] = 0;
                }
            }
 
            // init lists
            List<MapNode> openList = new List<MapNode>();
            //   List<MapNode> closedList = new List<MapNode>();
            this.m_ClosedList.Clear();
 
            // add starting location to open list
            MapNode nodeStart = new MapNode(_unit.x, _unit.y);
            nodeStart.cost = 0;
            openList.Add(nodeStart);
 
            // evaluate open list until empty
            while (openList.Count > 0) {
                // pop the top node
                MapNode nodeCurrent = openList[0];
                openList.Remove(nodeCurrent);
                this.m_ClosedList.Add(nodeCurrent);
 
                // evaluate all 6 adjacent nodes
                for (int dir = 0; dir < 6; dir++) {
                    // get node for current direction
                    MapNode nodeDir = this.GetMapNodeForDirection(nodeCurrent, dir);
 
                    // evaluate node if it is valid (within playable portion of the map) AND it is not on the closed list
                    if (nodeDir.isValid && !this.IsNodeInClosedList(nodeDir.x, nodeDir.y)) { //closedList.Contains(nodeDir)) {
                        // get total cost to enter adjacent node (i.e. cost to enter current node + cost to enter new node)
                        int totalCost = nodeCurrent.cost + CalculateCostToEnterNode(nodeDir, dir, MoveType.Land);
 
                        // if unit has enough moves to enter new node AND cost to enter new node by this route is less than
                        // cost to enter it by other current routes then make current node its parent and add it to open
                        // list
                        if (_unit.movementPoints >= totalCost && nodeDir.cost > totalCost) {
                            nodeDir.cost = totalCost;
                            openList.Add(nodeDir);
                            this.m_AllowableMoves[nodeDir.x, nodeDir.y] = 1;
                        }
                    }
                }
            }
 
            // write array grid to console window
            //Console.WriteLine("X = 0  1  2  3  4  5  6  7  8  9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26");
            //for (int y = 0; y <= m_MAP_MAX_Y; y++) {
            //    Console.Write(y.ToString() + " ");
            //    for (int x = 0; x <= m_MAP_MAX_X; x++) {
            //        Console.Write(this.m_AllowableMoves[x, y].ToString() + " ");
            //    }
            //    Console.WriteLine();
            //}
 
        }
 
        public bool IsNodeInClosedList(int _x, int _y)
        {
            foreach (MapNode node in this.m_ClosedList) {
                if (node.x == _x && node.y == _y) {
                    return true;
                }
            }
            return false;
        }
 
        public int CalculateCostToEnterNode(MapNode _destinationNode, int _direction, MoveType _moveType)
        {
            return 1;
        }
 
        public enum MoveType
        {
            Air = 0,
            Land = 1,
            Sea = 2
        }
 
        public MapNode GetMapNodeForDirection(MapNode _fromNode, int _direction)
        {
            int deltaX = this.m_DeltaX[_direction];
            int deltaY = IsEvenNumber(_fromNode.x) ? this.m_DeltaYForEvenColumn[_direction] : this.m_DeltaYForOddColumn[_direction];
            return new MapNode(_fromNode.x + deltaX, _fromNode.y + deltaY);
        }
 
        public bool IsEvenNumber(int _number)
        {
            return ((_number & 1) == 0);
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

    public struct MapNode
    {
        public int x, y, parentX, parentY, cost;
        public bool isValid;

        public MapNode(int _x, int _y)
        {
            x = _x;
            y = _y;
            cost = 9999;
            parentX = -1;
            parentY = -1;

            // map node is valid if it is within playable portion of the map
            // TODO: replace hard-coded boundaries
            if (x > 0 && x < 24 && y > 0 && y < 19) {
                isValid = true;
            } else {
                isValid = false;
            }
        }
    }

        public struct Unit
        {
            public int x, y;
            public int movementPoints;
 
            public Unit(int _x, int _y)
            {
                x = _x;
                y = _y;
                movementPoints = 2;
            }
        }

} // namespace xnaPanzer
