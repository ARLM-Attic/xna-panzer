/**********************************************************************************************************************
 * 
 * DISCLAIMER: this file contains PROTOTYPE code to be used for proof-of-concept development only.  It is in no way
 * representative of the proper way to do ANYTHING.  REPEAT: this is merely proof-of-concept code--it's ugly, it's a
 * HACK, don't use anything like it in the real world!
 * 
 * Game1.cs
 * 
 * Contains prototyping code to experiment with creating a hex-based 2D map using Panzer General graphics.  This code
 * is not expected to be used in the actual development of the xnaPanzer game but is merely a proof-of-concept
 * experiment.
 * 
 * Copyright 2008 by Troy Scheffel (Rockton, Illinois, USA)
 * 
 * Created using Visual Studio 2008 Professional and XNA Game Studio 3.0 (beta)
 * 
 * All rights not forfeited by the designated license are hereby reserved by Troy Scheffel
 * 
 * Error Log Reference #108a56b5-f77f-4ca8-81cc-ea551e823b98 (when trying to add new component to Issue Tracker)
 *
 * xmas: Zuzu's hand-painted beer/wine glasses; W.S. pro. multichopper, xmas cookie cutters, spa
 * Tutorials:
 * http://www.ziggyware.com/readarticle.php?article_id=160
 * http://msdn.microsoft.com/en-us/library/bb203924(MSDN.9).aspx
 * http://blogs.msdn.com/shawnhar/archive/2008/08/12/teaching-a-man-to-fish.aspx
 * http://jamesewelch.wordpress.com/2008/04/17/how-to-use-xnacontent-xml-files/
 * http://www.ziggyware.com/readarticle.php?article_id=150 (Sprite, 2.0)
 * http://www.rrstar.com/homepage/x1091751971 (sledding)
 * 
 *********************************************************************************************************************/
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Xml;
using GameEngine1;

namespace xnaPanzer1
{
    /// <summary>
    /// This is the prototype development class for xnaPanzer
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Member variables

        GraphicsDeviceManager m_graphics;
        SpriteBatch m_spriteBatch;
        Texture2D m_MapSpriteSheet;
        Texture2D m_UnitSpriteSheet;
        Texture2D m_DeltaTextures;
        SpriteFont m_font1;
        SpriteFont m_UnispaceFont;

        Texture2D m_BevelLeftHook;
        Texture2D m_BevelRightHook;
        Texture2D m_BevelStraightLine;
        Texture2D m_DefaultGameScreenMask;

        Texture2D m_HexGridParts;

        List<MapNode> m_OpenList = new List<MapNode>();
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

        // this is the minimum number of full hexes that should be visible around a selected hex.  if the number is
        // less than this then the map should auto-scroll as necessary.  this applies only when a hex/unit is SELECTED.
        const int m_VIEWPORT_MIN_VISIBLE_RADIUS_FOR_SELECTED_HEX = 3;

        const int m_PreferredBackBufferWidth = 1024; //800;
        const int m_PreferredBackBufferHeight = 768; //600;

        const int m_MOUSE_SCROLL_MIN_X = 5;
        const int m_MOUSE_SCROLL_MAX_X = m_PreferredBackBufferWidth - 5;
        const int m_MOUSE_SCROLL_MIN_Y = 5;
        const int m_MOUSE_SCROLL_MAX_Y = m_PreferredBackBufferHeight - 5;

        const int m_MAP_HEX_WIDTH = 25;
        const int m_MAP_HEX_HEIGHT = 20;
        const int m_MAP_MAX_X = 24;
        const int m_MAP_MAX_Y = 19;

        bool m_LeftButtonPressed = false;
        //MapLocation m_HexSelected = new MapLocation(-1, -1);
        Pathfinding[,] m_AllowableMoves = new Pathfinding[m_MAP_MAX_X + 1, m_MAP_MAX_Y + 1];    // to be used for pathfinding algorithm
        Color[] m_HexTintForPathfinding = new Color[3] { Color.White, Color.DarkGray, Color.Gold }; // allowed, prohibited, start hex

        int m_ViewportLeftHexX = 0;
        int m_ViewportTopHexY = 0;
        int m_MouseHexX = -1;
        int m_MouseHexY = -1;

        int[,] m_map;

        KeyboardState keyboardState;
        KeyboardState previousKeyboardState;
        GamePadState gamepadState;
        GamePadState previousGamepadState;

        bool m_IsHexGridDisplayed = true;
        Rectangle sourceRectangleForPartialHexGrid;
        Rectangle sourceRectangleForHexGridCursor;

        List<Unit> m_Units;
        List<UnitType> m_UnitTypes;
        int[,] m_MapUnits;
        Int16 m_CurrentPlayer;
        bool m_IsUnitSelected = false;
        int m_SelectedUnitID;

        const bool m_IS_FULL_SCREEN = false;
        readonly Color m_BACKGROUND_COLOR = new Color(128, 128, 0);     // cannot be const

        List<Unit> map;

        #endregion Member variables

        #region Constructor & Initialization

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
            // create a new SpriteBatch, which can be used to draw textures.
            this.m_spriteBatch = new SpriteBatch(GraphicsDevice);

            // init static Unit variables (must be done before initializing individual units due to needing pixel width/height)
            UnitType.SpriteBatch = this.m_spriteBatch;
            UnitType.ImageWidth = m_UNIT_IMAGE_WIDTH;
            UnitType.ImageHeight = m_UNIT_IMAGE_HEIGHT;

            this.m_graphics.PreferredBackBufferWidth = m_PreferredBackBufferWidth;
            this.m_graphics.PreferredBackBufferHeight = m_PreferredBackBufferHeight;
            this.m_graphics.IsFullScreen = m_IS_FULL_SCREEN;
            this.m_graphics.ApplyChanges();
            this.IsMouseVisible = true;

            //this.m_map = new int[m_MAP_HEX_WIDTH, m_MAP_HEX_HEIGHT];
            Random random = new Random(unchecked((int)(DateTime.Now.Ticks)));
            this.m_map = new int[m_MAP_HEX_WIDTH, m_MAP_HEX_HEIGHT];
            this.m_MapUnits = new int[m_MAP_HEX_WIDTH, m_MAP_HEX_HEIGHT];
            for (int x = 0; x < m_MAP_HEX_WIDTH; x++) {
                for (int y = 0; y < m_MAP_HEX_HEIGHT; y++) {
                    this.m_map[x, y] = random.Next(10);
                    this.m_MapUnits[x, y] = -1;                         // init map units with no units
                }
            }

            // init keyboard & gamepad states (the previous ones are used to help detect keypresses)
            KeyboardState keyboardState = Keyboard.GetState();
            KeyboardState previousKeyboardState = Keyboard.GetState();
            GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);
            GamePadState previousGamepadState = GamePad.GetState(PlayerIndex.One);

            // init curent player
            this.m_CurrentPlayer = 0;

            base.Initialize();
        }

        #endregion Constructor & Initialization

        #region Content Load/Unload

        /// <summary>
        /// LoadContent will be called once per game and is the place to load all game content
        /// </summary>
        protected override void LoadContent()
        {
            //Load the images for map terrain and combat units from bitmap files
            this.m_MapSpriteSheet = this.Content.Load<Texture2D>("Sprites/Maps/tacmap_terrain_xnaPanzer");
            this.m_DeltaTextures = this.Content.Load<Texture2D>("Sprites/Maps/DeltaTextures");

            this.m_font1 = Content.Load<SpriteFont>("Fonts/SpriteFont1");
            this.m_UnispaceFont = Content.Load<SpriteFont>("Fonts/Unispace");

            //this.m_BevelLeftHook = Content.Load<Texture2D>("Sprites/Gui/Bevel_Left_Hook");
            //this.m_BevelRightHook = Content.Load<Texture2D>("Sprites/Gui/Bevel_Right_Hook");
            //this.m_BevelStraightLine = Content.Load<Texture2D>("Sprites/Gui/Bevel_Straight_Line");
            this.m_DefaultGameScreenMask = Content.Load<Texture2D>("Sprites/Gui/Default_Game_Screen_Mask");

            this.m_HexGridParts = Content.Load<Texture2D>("Sprites/Maps/Hex_Grid_Parts");
            this.sourceRectangleForPartialHexGrid = new Rectangle(0, 0, m_HEXPART_FULL_WIDTH, m_HEXPART_FULL_HEIGHT);
            this.sourceRectangleForHexGridCursor = new Rectangle(61, 0, m_HEXPART_FULL_WIDTH, m_HEXPART_FULL_HEIGHT);

            // load sprite sheet of unit icons (needed to initialize individual UnitType textures)
            UnitType.SpriteSheet = this.Content.Load<Texture2D>("Sprites/Units/tacicons_start_at_0");

            // load UnitTypes from XML file
            this.m_UnitTypes = new List<UnitType>();
            this.m_UnitTypes = Content.Load<List<UnitType>>(@"Xml/UnitTypeList");
            //this.map = Content.Load<List<Unit>>(@"Xml/MapList");
            //foreach (UnitType ut in this.m_UnitTypes) {
            //    Point p = Util.CalculateSpritesheetCoordinates(1);
            //    if (p != null) {
            //        this.m_UnitTypes.Texture = UnitType.SpriteSheet.GetData<
            //}

            // let's init a few test units
            this.m_Units = new List<Unit>();
            this.m_Units = Content.Load<List<Unit>>(@"Xml/UnitList");

            //foreach (Unit u in this.m_Units) {
            //    this.m_MapUnits[u.X, u.Y] = u.ID;
            //}

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload all game content
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        #endregion Content Load/Unload

        #region Update

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
                if (this.m_ViewportLeftHexX >= 2) {
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
            //if (this.IsMouseWithinViewport() && !this.m_IsUnitSelected &&
            //    Mouse.GetState().LeftButton == ButtonState.Pressed && !this.m_LeftButtonPressed &&
            //    this.GetUnitIDAtMapLocation(this.m_MouseHexX, this.m_MouseHexY) >= 0) {
            //    //this.m_HexSelected = new MapLocation(this.m_MouseHexX, this.m_MouseHexY);
            //    int id = this.GetUnitIDAtMapLocation(this.m_MouseHexX, this.m_MouseHexY);
            //    if (id > -1) {
            //        Unit unit = this.m_Units[id];
            //        if (this.m_CurrentPlayer == unit.Owner) {						// see if the current player is the unit's owner
            //            // if unit has already moved this turn then just display its stats
            //            if (unit.HasMoved) {
            //                // TODO: display unit's extended stats
            //            } else {
            //                this.SelectUnit(id);
            //                this.SetAllowableMoves(id);

            //                // ensure the selected hex has at least the minimum required number of full hexes visible all around it
            //                MapLocation origin = this.CalculateViewportOriginForSelectedUnit(unit.X, unit.Y);
            //                if (this.m_ViewportLeftHexX != origin.x || this.m_ViewportTopHexY != origin.y) {
            //                    this.m_ViewportLeftHexX = origin.x;
            //                    this.m_ViewportTopHexY = origin.y;
            //                    MapLocation unitMapLocation = new MapLocation(unit.X, unit.Y);
            //                    Point p = this.ConvertMapLocationToMousePosition(unitMapLocation);
            //                    Mouse.SetPosition(p.X, p.Y);
            //                }
            //            }
            //        } else {  // player is not the owner
            //            // TODO: display unit's public stats
            //        }
            //    }

            //}

            //// see if user wants to deselect current unit
            //if (this.m_SelectedUnitID != -1 && Mouse.GetState().RightButton == ButtonState.Pressed) {
            //    this.m_Units[this.m_SelectedUnitID].EndMove();
            //    this.m_SelectedUnitID = -1;
            //    this.m_IsUnitSelected = false;
            //}

            //// see if user wants to move the currently-selected unit
            //if (this.IsMouseWithinViewport() && this.m_IsUnitSelected &&
            //    Mouse.GetState().LeftButton == ButtonState.Pressed && !this.m_LeftButtonPressed &&
            //    this.GetUnitIDAtMapLocation(this.m_MouseHexX, this.m_MouseHexY) == -1 &&
            //    this.m_AllowableMoves[this.m_MouseHexX, this.m_MouseHexY] == Pathfinding.Allowed) {
            //    this.m_MapUnits[this.m_Units[this.m_SelectedUnitID].X, this.m_Units[this.m_SelectedUnitID].Y] = -1;
            //    this.m_Units[this.m_SelectedUnitID].Move(this.m_MouseHexX, this.m_MouseHexY);
            //    this.m_MapUnits[this.m_MouseHexX, this.m_MouseHexY] = this.m_SelectedUnitID;
            //}

            // set previous keyboard & gamepad states = to current states so we can detect new keypresses
            previousKeyboardState = keyboardState;
            previousGamepadState = gamepadState;

            base.Update(gameTime);
        }

        #endregion Update

        #region Draw

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            this.m_graphics.GraphicsDevice.Clear(m_BACKGROUND_COLOR);

            this.m_spriteBatch.Begin();

            // uncomment the following line to display all the unit sprites (collage)
            //this.m_spriteBatch.Draw(this.m_UnitSpriteSheet, new Rectangle(0, 0, this.m_UnitSpriteSheet.Width, this.m_UnitSpriteSheet.Height), Color.White);

            Point offset = new Point();                                // used to store spritesheet source coords

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
                    offset = Util.CalculateSpritesheetCoordinates(this.m_map[x, y]);
                    sourceRect.X = offset.X;
                    sourceRect.Y = offset.Y;

                    // calculate where the hex should be drawn on the viewport
                    relativeY = (y % m_VIEWPORT_HEX_HEIGHT) - 1;
                    Rectangle destRect = new Rectangle(m_VIEWPORT_MIN_X_COORD_DRAW + (columnNumber * m_HEXPART_LENGTH_BBA),
                            m_VIEWPORT_MIN_Y_COORD_DRAW + (rowNumber * m_HEXPART_FULL_HEIGHT), m_HEXPART_FULL_WIDTH, m_HEXPART_FULL_HEIGHT);

                    if (this.IsEvenNumber(columnNumber)) {             // shift odd-numbered columns down half a hex
                        destRect.Y += m_HEXPART_LENGTH_C;
                    }

                    // draw the hex
                    if (!this.m_IsUnitSelected) {  // is a hex/unit currently selected?  if not, don't apply tinting
                        this.m_spriteBatch.Draw(this.m_MapSpriteSheet,
                            destRect,                                       // destination rectangle
                            sourceRect,                                     // source rectangle
                            Color.White);                                   // white = don't apply tinting
                    } else { // if so, apply tinting to hex based on whether selected unit can move to this location
                        this.m_spriteBatch.Draw(this.m_MapSpriteSheet,
                            destRect,                                       // destination rectangle
                            sourceRect,                                     // source rectangle
                            this.m_HexTintForPathfinding[(int)this.m_AllowableMoves[x, y]]);
                    }

                    // draw units on map
                    int id = this.GetUnitIDAtMapLocation(x, y);
                    if (id >= 0) {
                        ////this.m_Units[id].Draw(new Point(destRect.X, destRect.Y));
                    }

                    // will need hex offsets for drawing hex cursor(s)
                    offset = Util.CalculateSpritesheetCoordinates(1);

                    // draw hex grid if desired
                    if (this.m_IsHexGridDisplayed) {
                        this.m_spriteBatch.Draw(this.m_HexGridParts,
                            destRect,                                       // destination rectangle
                            this.sourceRectangleForPartialHexGrid,
                            Color.White);                                   // white = don't apply tinting
                    }

                    // draw hex cursor if mouse is within viewport
                    if (this.m_MouseHexX == x && this.m_MouseHexY == y) {
                        this.m_spriteBatch.Draw(this.m_HexGridParts,
                            destRect,                                       // destination rectangle
                            this.sourceRectangleForHexGridCursor,
                            Color.White);                                   // white = don't apply tinting
                    }
                    ++rowNumber;
                }
                ++columnNumber;
            }

            // turn on mouse cursor if mouse is outside viewport; turn it off if within (we've already drawn a hexagon cursor)
            this.IsMouseVisible = !this.IsMouseWithinViewport();

            // mask out viewport border
            this.m_spriteBatch.Draw(this.m_DefaultGameScreenMask, new Rectangle(0, 0, 800, 600), Color.White);

            // 85,75 straight down
            //this.m_spriteBatch.Draw(this.m_BevelLeftHook, new Rectangle(85,75,8, 120), Color.White);

            // display info text at top
            this.m_spriteBatch.DrawString(this.m_font1, "Scroll map with arrow keys and mouse.  Select/deselect a hex with left/right clicks." +
                "\r\nNon-shaded hexes show where fictitious unit could NOT move.  Does not account\r\nfor terrain, enemy units, etc.",
                new Vector2(10, 3), Color.White);

            //if (this.IsMouseWithinViewport()) {
            //    string selectedHex = "";
            //    if (this.m_IsUnitSelected) {
            //        this.m_spriteBatch.DrawString(this.m_font1,
            //            "Selected Unit's Hex = " + this.m_Units[this.m_SelectedUnitID].X + ", " + this.m_Units[this.m_SelectedUnitID].Y
            //            , new Vector2(10, 530), Color.White);
            //    }
            //    this.m_spriteBatch.DrawString(this.m_font1,
            //        "m_ViewportLeftHexX = " + this.m_ViewportLeftHexX.ToString() +
            //        ", m_ViewportTopHexY = " + this.m_ViewportTopHexY.ToString() +
            //        ", Mouse hex X,Y = " + this.m_MouseHexX.ToString() + ", " + this.m_MouseHexY.ToString() +
            //        selectedHex
            //        , new Vector2(10, 550), Color.White);
            //    this.m_spriteBatch.DrawString(this.m_font1,
            //        "Mouse coord X,Y = " + Mouse.GetState().X.ToString() + ", " + Mouse.GetState().Y.ToString()
            //        , new Vector2(10, 570), Color.White);
            //    int id = this.GetUnitIDAtMapLocation(this.m_MouseHexX, this.m_MouseHexY);
            //    if (id >= 0) {
            //        Unit unit = this.m_Units[id];
            //        this.m_spriteBatch.DrawString(this.m_font1,
            //            (id + 1).ToString() + Util.GetOrdinalSuffix(id + 1) + " " + unit.UnitType.ToString() + "      Str: " + unit.Strength.ToString()
            //            , new Vector2(550, 720), Color.White);
            //        this.m_spriteBatch.DrawString(this.m_font1,
            //            unit.UnitType.ToString() + "            Ent: 0"
            //            , new Vector2(550, 735), Color.White);
            //        this.m_spriteBatch.DrawString(this.m_font1,
            //            "Ammo: 7     Fuel: 41   (obviously spoofed text but you get the idea)"
            //            , new Vector2(550, 750), Color.White);
            //    }
            //} else {
            //    MouseState ms = new MouseState();
            //    ms = Mouse.GetState();
            //    this.m_spriteBatch.DrawString(this.m_font1,
            //        "Mouse coord X,Y = " + ms.X.ToString() + ", " + ms.Y.ToString() + ", Map = " + this.map[0].Name
            //        , new Vector2(10, 550), Color.White);
            //}

            // TEST DRAW ONLY
            ////this.m_UnitTypes[1].Draw(new Point(100, 100));

            this.m_spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion Draw

        #region Algorithms

        /// <summary>
        /// Calculates the viewport's upper left hex location for a given selected unit to ensure there is a cetain
        /// number of full hexes visible all around the selected unit (essentially auto-scrolls the map).
        /// </summary>
        /// <param name="_x">Selected unit's X map location</param>
        /// <param name="_y">Selected unit's Y map location</param>
        /// <returns>MapLocation indicating the viewport's new upper-left origin</returns>
        protected MapLocation CalculateViewportOriginForSelectedUnit(int _x, int _y)
        {
            MapLocation origin = new MapLocation(this.m_ViewportLeftHexX, this.m_ViewportTopHexY);

            // first, check if we need to scroll the viewport left or up
            int adjustmentX = _x - origin.x - m_VIEWPORT_MIN_VISIBLE_RADIUS_FOR_SELECTED_HEX;
            int adjustmentY = _y - origin.y - m_VIEWPORT_MIN_VISIBLE_RADIUS_FOR_SELECTED_HEX;
            if (adjustmentX < 0) {
                origin.x += adjustmentX;
            }
            if (adjustmentY < 0) {
                origin.y += adjustmentY;
            }

            // now check if we need to scroll the viewport right or down
            adjustmentX = this.m_ViewportLeftHexX + m_VIEWPORT_HEX_WIDTH - _x;
            adjustmentY = this.m_ViewportTopHexY + m_VIEWPORT_HEX_HEIGHT - _y;
            if (adjustmentX < m_VIEWPORT_MIN_VISIBLE_RADIUS_FOR_SELECTED_HEX) {
                origin.x += adjustmentX;
            }
            if (adjustmentY < m_VIEWPORT_MIN_VISIBLE_RADIUS_FOR_SELECTED_HEX) {
                origin.y += m_VIEWPORT_MIN_VISIBLE_RADIUS_FOR_SELECTED_HEX - adjustmentY;
                if (!this.IsEvenNumber(origin.y)) {  // drop down 1 extra hex to compensate for half hexes
                    ++origin.y;
                }
            }

            // ensure viewport origin is within suitable boundaries
            origin.x = Math.Max(origin.x, 0);
            origin.y = Math.Max(origin.y, 0);
            origin.x = Math.Min(origin.x, m_MAP_HEX_WIDTH - m_VIEWPORT_HEX_WIDTH);
            origin.y = Math.Min(origin.y, m_MAP_HEX_HEIGHT - m_VIEWPORT_HEX_HEIGHT);

            return origin;
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

        // Calculates mouse x,y coordinates for the center of a given map hex location
        protected Point ConvertMapLocationToMousePosition(MapLocation _ml)
        {
            int xCoord = m_VIEWPORT_MIN_X_COORD_DRAW +
                ((_ml.x - this.m_ViewportLeftHexX) * m_HEXPART_LENGTH_BBA) +
                (m_HEXPART_FULL_WIDTH / 2);
            int yCoord = m_VIEWPORT_MIN_Y_COORD_DRAW +
                ((_ml.y - this.m_ViewportTopHexY) * m_HEXPART_FULL_HEIGHT) +
                (m_HEXPART_LENGTH_C);
            return new Point(xCoord, yCoord);
        }

        /// <summary>
        /// Pathfinding algorithm that determines which map hexes a unit can move to
        /// </summary>
        /// <remarks>
        /// Logic Flow:
        /// 1.  Create an open list and a closed list that are both empty
        /// 2.  Add the start node (map hex) to the open list
        /// 3.  Loop until the open list is empty
        ///     a.  Grab first node on Open list
        ///     b.  For all 6 movement directions do the following:
        ///         (1.)  Calculate adjacent map node
        ///         (2.)  If adjacent map node is within playing area AND isn't on the Closed list THEN calculate movement
        ///               cost to enter that node
        ///         (3.)  If cost is less than existing cost, set new cost and add node to Open list (if not already there)
        /// </remarks>
        /// <param name="_unit"></param>
        public void SetAllowableMoves(int _id)
        {
            Unit unit = this.m_Units[_id];
            // clear out the array that contains whether the selected unit can move to a given hex
            for (int x = 0; x <= m_MAP_MAX_X; x++) {
                for (int y = 0; y <= m_MAP_MAX_Y; y++) {
                    this.m_AllowableMoves[x, y] = Pathfinding.Prohibited;
                }
            }

            // init lists
            this.m_OpenList.Clear();
            this.m_ClosedList.Clear();

            // add starting location to open list
            //MapNode nodeStart = new MapNode(unit.X, unit.Y);
            //this.m_AllowableMoves[unit.X, unit.Y] = Pathfinding.StartHex;
            //nodeStart.cost = 0;
            //this.m_OpenList.Add(nodeStart);

            //// evaluate open list until empty
            //while (this.m_OpenList.Count > 0) {
            //    // pop the top node
            //    MapNode nodeCurrent = this.m_OpenList[0];
            //    this.m_OpenList.Remove(nodeCurrent);
            //    this.m_ClosedList.Add(nodeCurrent);

            //    // evaluate all 6 adjacent nodes
            //    for (int dir = 0; dir < 6; dir++) {
            //        // get node for current direction
            //        MapNode nodeDir = this.GetMapNodeForDirection(nodeCurrent, dir);

            //        // evaluate node if it is valid (within playable portion of the map) AND it is not on the closed list
            //        if (nodeDir.isValid && !this.IsNodeInClosedList(nodeDir.x, nodeDir.y)) { //closedList.Contains(nodeDir)) {
            //            // get total cost to enter adjacent node (i.e. cost to enter current node + cost to enter new node)
            //            int totalCost = nodeCurrent.cost + CalculateCostToEnterNode(nodeDir, dir, MoveType.Land);

            //            // if unit has enough moves to enter new node AND cost to enter new node by this route is less than
            //            // cost to enter it by other current routes then make current node its parent and add it to open
            //            // list
            //            if (unit.Moves >= totalCost && nodeDir.cost > totalCost) {
            //                nodeDir.cost = totalCost;
            //                this.m_AllowableMoves[nodeDir.x, nodeDir.y] = Pathfinding.Allowed;
            //                if (!this.IsNodeInOpenList(nodeDir.x, nodeDir.y)) {
            //                    this.m_OpenList.Add(nodeDir);
            //                }
            //            }
            //        }
            //    }
            //}

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

        /// <summary>
        /// Calculates how many movement points a unit must expend in order to enter a particular map hex
        /// (based on unit's movement type, terrain, weather, assisting unit (engineer), roads, etc).
        /// </summary>
        /// <param name="_destinationNode"></param>
        /// <param name="_direction"></param>
        /// <param name="_moveType"></param>
        /// <returns>integer containing number of movement points</returns>
        public int CalculateCostToEnterNode(MapNode _destinationNode, int _direction, MoveType _moveType)
        {
            return 1;
        }

        /// <summary>
        /// Calculates the map hex entered if you movement from a specific map hex in a given direction
        /// </summary>
        /// <param name="_fromNode"></param>
        /// <param name="_direction"></param>
        /// <returns></returns>
        public MapNode GetMapNodeForDirection(MapNode _fromNode, int _direction)
        {
            int deltaX = this.m_DeltaX[_direction];
            int deltaY = IsEvenNumber(_fromNode.x) ? this.m_DeltaYForEvenColumn[_direction] : this.m_DeltaYForOddColumn[_direction];
            return new MapNode(_fromNode.x + deltaX, _fromNode.y + deltaY);
        }

        protected int GetUnitIDAtMapLocation(int _x, int _y)
        {
            return this.m_MapUnits[_x, _y];
        }

        private void SelectUnit(int _id)
        {
            this.m_SelectedUnitID = _id;
            this.m_IsUnitSelected = true;
        }

        #endregion Algorithms

        #region Boolean evaluations

        /// <summary>
        /// Returns true if integer is an even number, otherwise returns false
        /// </summary>
        /// <param name="_number"></param>
        /// <returns></returns>
        public bool IsEvenNumber(int _number)
        {
            return ((_number & 1) == 0);
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

        /// <summary>
        /// Returns true if mouse is currently within the Viewport, false if not
        /// </summary>
        /// <returns>
        /// Returns true if mouse is currently within the Viewport, false if not
        /// </returns>
        private bool IsMouseWithinViewport()
        {
            return (this.m_MouseHexX != -1 && this.m_MouseHexY != -1);
            //return (Mouse.GetState().X >= m_VIEWPORT_MIN_X_COORD_VISIBLE && Mouse.GetState().X <= m_VIEWPORT_MAX_X_COORD_VISIBLE &&
            //    Mouse.GetState().Y >= m_VIEWPORT_MIN_Y_COORD_VISIBLE && Mouse.GetState().Y <= m_VIEWPORT_MAX_Y_COORD_VISIBLE);
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

        public bool IsNodeInOpenList(int _x, int _y)
        {
            foreach (MapNode node in this.m_OpenList) {
                if (node.x == _x && node.y == _y) {
                    return true;
                }
            }
            return false;
        }

        #endregion Boolean evaluations

    } // class Game1

    #region Enums

    /// <summary>
    /// Indicates the type of unit movement for pathfinding purposes
    /// </summary>
    public enum MoveType
    {
        Air,
        Land,
        Sea
    }

    /// <summary>
    /// Used for pathfinding algorithm plus the drawing routine (so we know what how to tint a hex)
    /// </summary>
    public enum Pathfinding
    {
        Allowed,
        Prohibited,
        StartHex
    }

    ///// <summary>
    ///// Unit classification, e.g. infanty, Tiger II
    ///// </summary>
    //public enum UnitType
    //{
    //    Pioneere,
    //    PzIIIJ,
    //    PSW2338r,
    //    Hummel,
    //    FW190a,
    //    Pz38t
    //}

    ///// <summary>
    ///// Type of movement across terrain (note: static cannot move by any means)
    ///// </summary>
    //public enum MovementClass
    //{
    //    Static = 0,
    //    Towed,
    //    Leg,
    //    Truck,
    //    Wheeled,
    //    Tracked
    //}

    #endregion Enums

    #region Structures

    /// <summary>
    /// Holds a map hex x,y location (essentially a Point but renamed to avoid confusion with mouse coordinates)
    /// </summary>
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

    /// <summary>
    /// Represents a single map hexagon; used in the pathfinding algorithm.
    /// </summary>
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

    ///// <summary>
    ///// Super simple representation of a combat unit.
    ///// </summary>
    //public struct Unit
    //{
    //    public int id;
    //    public int x, y;
    //    public int moves;
    //    public int owner;
    //    public int strength;
    //    public bool hasMoved;
    //    public UnitType type;
    //    public string typeName;

    //    //public Unit(int _x, int _y) // : base(_x, _y, 3, 0, 10, UnitType.Infantry)
    //    //{
    //    //    x = _x;
    //    //    y = _y;
    //    //    moves = 2;
    //    //    owner = 0;
    //    //    strength = 10;
    //    //    hasMoved = false;
    //    //    type = UnitType.Infantry;
    //    //}

    //    public Unit(int _id, int _x, int _y, int _moves, int _owner, int _strength, UnitType _type)
    //    {
    //        id = _id;
    //        x = _x;
    //        y = _y;
    //        moves = _moves;
    //        owner = _owner;
    //        strength = _strength;
    //        type = _type;
    //        typeName = Enum.GetName(typeof(UnitType), type);
    //        // TEST: string s = UnitType.Infantry.ToString();
    //        hasMoved = false;
    //    }
    //}

    #endregion Structures

} // namespace xnaPanzer
