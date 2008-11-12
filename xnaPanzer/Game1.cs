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

        int m_UnitCounter = 0;
        int m_TerrainCounter = 0;

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
            this.m_graphics.PreferredBackBufferWidth = 1024;
            this.m_graphics.PreferredBackBufferHeight = 768;
            this.m_graphics.ApplyChanges();

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
            m_UnitSpriteSheet = this.Content.Load<Texture2D>("tacicons_start_at_0");
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
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);

            //Check to see if the game should be exited
            if (gamepadState.Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape) == true)
            {
                this.Exit();
            }

            // TODO: Add your update logic here
            m_UnitCounter = ++m_UnitCounter % m_NUM_UNITS_IN_SPRITE_SHEET;
            m_TerrainCounter = ++m_TerrainCounter % m_NUM_TERRAIN_HEXES_IN_SPRITE_SHEET;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            m_graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            this.m_spriteBatch.Begin();

            //this.m_spriteBatch.Draw(this.m_UnitSpriteSheet, new Rectangle(0, 0, this.m_UnitSpriteSheet.Width, this.m_UnitSpriteSheet.Height), Color.White);
            Offset offset; // = this.CalculateSpritesheetCoordinates(this.m_UnitCounter);

            // draw the next terrain image
            offset = this.CalculateSpritesheetCoordinates(this.m_TerrainCounter);
            this.m_spriteBatch.Draw(this.m_MapSpriteSheet, new Rectangle(60, 50, m_TERRAIN_IMAGE_WIDTH, m_TERRAIN_IMAGE_HEIGHT),
                new Rectangle(offset.x, offset.y, m_TERRAIN_IMAGE_WIDTH, m_TERRAIN_IMAGE_HEIGHT), Color.White);

            // draw the next unit
            offset = this.CalculateSpritesheetCoordinates(this.m_UnitCounter);
            this.m_spriteBatch.Draw(this.m_UnitSpriteSheet, new Rectangle(0, 0, m_UNIT_IMAGE_WIDTH, m_UNIT_IMAGE_HEIGHT),
                new Rectangle(offset.x, offset.y, m_UNIT_IMAGE_WIDTH, m_UNIT_IMAGE_HEIGHT), Color.White);

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
    }
}
