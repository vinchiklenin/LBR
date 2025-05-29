using System;
using System.Collections.Generic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LBR;

public class Game1 : Game
{
    Texture2D wallTexture;
    Texture2D floorTexture;
    Texture2D heroTexture;
    
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        this.Window.Title = "Labrintico Alpha.0.1";
        _graphics.IsFullScreen = false;
        _graphics.PreferredBackBufferWidth = 1200;
        _graphics.PreferredBackBufferHeight = 800;
        _graphics.ApplyChanges();
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        wallTexture = Content.Load<Texture2D>("wall");
        floorTexture = Content.Load<Texture2D>("floor");
        heroTexture = Content.Load<Texture2D>("hero");

        var level = new Level(new int[16,24], new Vector2(100, 100), new Vector2(1000, 1000), new List<(Vector2, bool)>());

    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();
        var level = new Level(new int[16,24], new Vector2(100, 100), new Vector2(1000, 1000), new List<(Vector2, bool)>());
        level.LevelData[3, 7] = 1;

        for (int i = 0; i < level.LevelData.GetLength(0); i++)
        {
            for (int j = 0; j < level.LevelData.GetLength(1); j++)
            {
                if (level.LevelData[i, j] == default) _spriteBatch.Draw(wallTexture, new Vector2(j * 50, i * 50), Color.White);
                else if (level.LevelData[i, j] == 1) _spriteBatch.Draw(floorTexture, new Vector2(j * 50, i * 50), Color.White);

            }
        }

        _spriteBatch.Draw(heroTexture, new Vector2(200, 200), Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}

class Level
{
    public string levelName;
    public int[,] LevelData;
    public Vector2 HeroPosition;
    public Vector2 EndPosition;
    public List<(Vector2, bool)> Coins;
    private bool IsInitializedBool = false;

    public Level(int[,] LevelData, Vector2 HeroPosition,Vector2 EndPosition , List<(Vector2, bool)> Coins)
    {
        this.levelName = "typical level";
        if (LevelData.GetLength(0) == 16 && LevelData.GetLength(1) == 24) this.LevelData = LevelData;
        else throw new IncompleteInitialization();
        this.HeroPosition = HeroPosition;
        this.EndPosition = EndPosition;
        this.Coins = Coins;
        this.IsInitializedBool = true;
    }

    public bool IsInitialized()
    {
        return IsInitializedBool;
    }
}