using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using Microsoft.Xna.Framework.Content;


namespace LBR;

public class Game1 : Game
{
    Texture2D wallTexture;
    Texture2D floorTexture;
    Texture2D heroTexture;
    Texture2D coinTexture;
    Texture2D endTexture;
    SpriteFont tittleTexture;

    
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    public Player _player;
    
    private Level _level;

    public Game1(Player player)
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _player = player;
    }

    protected override void Initialize()
    {
        this.Window.Title = "Labrintico Alpha.0.1";
        _graphics.IsFullScreen = false;
        _graphics.PreferredBackBufferWidth = 1200;
        _graphics.PreferredBackBufferHeight = 800;
        _graphics.ApplyChanges();
        _level = new Level();
        // TODO: Add your initialization logic here
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        wallTexture = Content.Load<Texture2D>("wall");
        floorTexture = Content.Load<Texture2D>("floor");
        heroTexture = Content.Load<Texture2D>("hero");
        coinTexture = Content.Load<Texture2D>("coin");
        endTexture = Content.Load<Texture2D>("end");
        tittleTexture = Content.Load<SpriteFont>("Monocraft");

        var level = new Level();

    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        // TODO: Add your update logic here
        var kstate = Keyboard.GetState();
        int dX = 0;
        int dY = 0;
        if (kstate.IsKeyDown(Keys.W)) dY -= 1;
        if (kstate.IsKeyDown(Keys.S)) dY += 1;
        if (kstate.IsKeyDown(Keys.A)) dX -= 1;
        if (kstate.IsKeyDown(Keys.D)) dX += 1;
        if (_level.LevelData[((int)_level.HeroPosition.Y + dY - 1) / 50, ((int)_level.HeroPosition.X + dX - 1) / 50] != 1 &&
            _level.LevelData[((int)_level.HeroPosition.Y + dY + 33) / 50, ((int)_level.HeroPosition.X + dX + 29) / 50] != 1)
        {
            _level.HeroPosition.X += dX;
            _level.HeroPosition.Y += dY;
            
            if (_level.LevelData[((int)_level.HeroPosition.Y + dY - 1) / 50, ((int)_level.HeroPosition.X + dX - 1) / 50] != 1 &&
                _level.LevelData[((int)_level.HeroPosition.Y + dY + 33) / 50, ((int)_level.HeroPosition.X + dX + 29) / 50] != 1)
            {
                _level.HeroPosition.X += dX;
                _level.HeroPosition.Y += dY;


                if (_level.Coins[((int)_level.HeroPosition.Y + dY - 1) / 50,
                        ((int)_level.HeroPosition.X + dX - 1) / 50] != default
                    && _level.Coins[((int)_level.HeroPosition.Y + dY - 1) / 50,
                        ((int)_level.HeroPosition.X + dX - 1) / 50] != new Vector2(1000, 1000))
                {
                    _level.Coins[((int)_level.HeroPosition.Y + dY - 1) / 50,
                        ((int)_level.HeroPosition.X + dX - 1) / 50] = new Vector2(1000, 1000);
                    this._player.Coins++;
                    Console.WriteLine($"Собрано {this._player.Coins} монетs");
                }
                
                if (_level.Coins[((int)_level.HeroPosition.Y + dY + 33) / 50,
                        ((int)_level.HeroPosition.X + dX + 29) / 50] != default 
                    && _level.Coins[((int)_level.HeroPosition.Y + dY + 33) / 50,
                        ((int)_level.HeroPosition.X + dX + 29) / 50] != new Vector2(1000, 1000))
                {
                    _level.Coins[((int)_level.HeroPosition.Y + dY + 33) / 50,
                        ((int)_level.HeroPosition.X + dX + 29) / 50] = new Vector2(1000, 1000);
                    this._player.Coins++;
                    Console.WriteLine($"Собрано {this._player.Coins} монетs");
                }
            }
        }

        if (_level.LevelData[((int)_level.HeroPosition.Y + dY - 1) / 50, ((int)_level.HeroPosition.X + dX - 1) / 50] == 2 ||
            _level.LevelData[((int)_level.HeroPosition.Y + dY + 33) / 50, ((int)_level.HeroPosition.X + dX + 29) / 50] == 2)
        {
            _player.Levels++;
            Console.WriteLine("Еще один уровень");
            Exit();
        }
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();
        
        for (int i = 0; i < _level.LevelData.GetLength(0); i++)
        {
            for (int j = 0; j < _level.LevelData.GetLength(1); j++)
            {
                if (_level.LevelData[i, j] == 1) _spriteBatch.Draw(wallTexture, new Vector2(j * 50, i * 50), Color.White);
                else if (_level.LevelData[i, j] == default) _spriteBatch.Draw(floorTexture, new Vector2(j * 50, i * 50), Color.White);
                else if (_level.LevelData[i, j] == 2) _spriteBatch.Draw(endTexture, new Vector2(j * 50, i * 50), Color.White);
            }
        }

        foreach (var coin in _level.Coins)
        {
            if (coin!= default) _spriteBatch.Draw(coinTexture, coin, Color.White);
        }

        _spriteBatch.Draw(heroTexture, _level.HeroPosition, Color.White);
        _spriteBatch.DrawString(tittleTexture, "Monets:", new Vector2(100, 700), Color.White);
        _spriteBatch.DrawString(tittleTexture, _player.Coins.ToString(), new Vector2(320, 700), Color.White);
        
        _spriteBatch.DrawString(tittleTexture, "Levels:", new Vector2(500, 700), Color.White);
        _spriteBatch.DrawString(tittleTexture, _player.Levels.ToString(), new Vector2(720, 700), Color.White);
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}

public abstract class Scene
{
    protected Game1 _game;
    
    protected ContentManager _content;

    /// <summary>
    ///     Creates a new Scene instance.
    /// </summary>
    /// <param name="game">
    ///     A reference to our Game1 instance.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the value supplied for <paramref name="game"/> 
    ///     is null
    /// </exception>
    public Scene(Game1 game)
    {
        if (game == null)
        {
            throw new ArgumentNullException(nameof(game), "Game cannot be null!");
        }
        _game = game;
    }
    
    public virtual void Initialize()
    {
        _content = new ContentManager(_game.Services);
        _content.RootDirectory = _game.Content.RootDirectory;
        LoadContent();
    }


    public virtual void LoadContent() { }

    /// <summary>
    ///     Unloads any content that has been loaded by the scene.
    /// </summary>
    /// <remarks>
    ///     This will be called after the game switches to a new
    ///     scene.
    /// </remarks>
    public virtual void UnloadContent() 
    {
        _content.Unload();
        _content = null;
    }

    /// <summary>
    ///     Updates the Scene.
    /// </summary>
    /// <param name="gameTime">
    ///     A snapshot of the frame specific timing values.
    /// </param>
    public virtual void Update(GameTime gameTime) { }


    /// <summary>
    ///     Handles preparing the Scene to draw.
    /// </summary>
    /// <remarks>
    ///     This is called just before the main Draw method.
    /// </remarks>
    /// <param name="spriteBatch"></param>
    public virtual void BeforeDraw(SpriteBatch spriteBatch, Color clearColor)
    {
        //  Clear the backbuffer
        _game.GraphicsDevice.Clear(clearColor);

        //  Begin the spritebatch
        spriteBatch.Begin();
    }

    /// <summary>
    ///     Draws the Scene to the screen.
    /// </summary>
    /// <remarks>
    ///     This is called immediately after BeforeDraw.
    /// </remarks>
    /// <param name="spriteBatch">
    ///     The SpriteBatch instance used for rendering.
    /// </param>
    public virtual void Draw(SpriteBatch spriteBatch) { }

    /// <summary>
    ///     Handles ending any drawing the scene is performing.
    /// </summary>
    /// <remarks>
    ///     This is called immediately after Draw.
    /// </remarks>
    /// <param name="spriteBatch">
    ///     The SpriteBatch instance used for rendering.
    /// </param>
    public virtual void AfterDraw(SpriteBatch spriteBatch)
    {
        //  End the spritebatch
        spriteBatch.End();
    }
}


class Level
{
    public string levelName;
    public int[,] LevelData;
    public Vector2 HeroPosition;
    public Vector2[,] Coins;
    
    private bool IsInitializedBool = false;
    private Random random = new Random();

    public Level()
    {
        this.levelName = "Ozon-zon-zon level";
        this.LevelData = new int[16, 24];
        this.HeroPosition = default;
        this.Coins = new Vector2[16, 24];
        var maze = MazeGenarate(13, 21);
        Console.WriteLine("Заполняем сетку");
        for (var i = 0; i < maze.GetLength(0); i++)
        {
            for (var j = 0; j < maze.GetLength(1); j++)
            {
                LevelData[i, j] = maze[i, j];
                if (HeroPosition == default && LevelData[i, j] == default) this.HeroPosition = new Vector2(j * 50 + 10 ,i * 50 + 10);
            }
        }
        
        Console.WriteLine("Сыпим монеты");

        var coinsGenCount = 0;
        while (coinsGenCount <= 10)
        {
            for (var i = 2; i < maze.GetLength(0) - 2; i++)
            {
                for (var j = 2; j < maze.GetLength(1) - 2; j++)
                {
                    if (this.LevelData[i, j] == 0 && random.NextDouble() < 0.247)
                    {
                        this.Coins[i, j] = new Vector2(j * 50 + 16, i * 50 + 16);
                        coinsGenCount++;
                    }
                }
                if (coinsGenCount > 10) break;
            }
        }
        
        Console.WriteLine("Ставим старт");
        this.SetEndPoin();
        this.IsInitializedBool = true;
        
    }

    public bool IsInitialized()
    {
        return IsInitializedBool;
    }

    private int[,] MazeGenarate(int Xwidth, int Yheight)
    {
            var random = new Random();
            if (Xwidth % 2 == 0) Xwidth += 1;
            if (Yheight % 2 == 0) Yheight += 1;
            
            int[,] maze = new int[Xwidth, Yheight];
            for (int i = 0; i < Xwidth; i++)
            {
                for (int j = 0; j < Yheight; j++)
                {
                    maze[i, j] = 1;
                }
            }
            Stack<(int, int)> stack = new Stack<(int, int)>();
            int x = random.Next(0, (Xwidth - 2) * 2) / 2;
            int y = random.Next(0, (Yheight - 2) * 2) / 2;
            
            if (x % 2 == 0) x += 1;
            if (y % 2 == 0) y += 1;
            (int, int) current_cell = (x, y);
            maze[x, y] = 0;
            stack.Push(current_cell);
            
            while (stack.Count > 0)
            {
                var directions = new[] { (0, -2), (0, 2), (-2, 0), (2, 0) };
                var neib = directions
                    .Where(dir => {
                        int dx = dir.Item1, dy = dir.Item2;
                        return 0 <= x + dx && x + dx < Xwidth && 
                               0 <= y + dy && y + dy < Yheight && 
                               maze[x + dx, y + dy] == 1;
                    })
                    .ToList();
                
                if (neib.Count > 0)
                {
                    int dx, dy;
                    if (random.NextDouble() < 0.5) 
                    {
                        for (int i = neib.Count - 1; i > 0; i--)
                        {
                            int j = random.Next(i + 1);
                            var temp = neib[i];
                            neib[i] = neib[j];
                            neib[j] = temp;
                        }
                        (dx, dy) = neib[0];
                    }
                    else
                    {
                        (dx, dy) = neib[random.Next(neib.Count)];
                    }
                    maze[x + dx / 2, y + dy / 2] = 0;
                    maze[x + dx, y + dy] = 0;
                    x = x + dx;
                    y = y + dy;
                    stack.Push((x, y));
                }
                else
                {
                    current_cell = stack.Pop();
                    (x, y) = current_cell;
                }
            }
            return maze;
        }

    private void SetEndPoin()
    {
        var isEndPointCh = false;
        for (var i = 4; i < 13; i++)
        {
            for (var j = 10; j < 20; j++)
            {
                if (this.LevelData[i, j] == 0 
                    && this.LevelData[i + 1, j] + this.LevelData[i - 1, j] + this.LevelData[i, j + 1] + this.LevelData[i, j - 1] == 3
                    && random.NextDouble() < 0.85)
                {
                    this.LevelData[i, j] = 2;
                    isEndPointCh = true;
                }
                if (isEndPointCh) break;
            }
            if (isEndPointCh) break;
        }

        if (!isEndPointCh)
        {
            for (var i = 4; i < 16; i++)
            {
                for (var j = 12; j < 20; j++)
                {
                    if (this.LevelData[i, j] == 0 
                        && this.LevelData[i + 1, j] + this.LevelData[i - 1, j] + this.LevelData[i, j + 1] + this.LevelData[i, j - 1] == 2
                        && random.NextDouble() < 0.85)
                    {
                        this.LevelData[i, j] = 2;
                        Console.WriteLine((i, j));
                        isEndPointCh = true;
                    }
                    if (isEndPointCh) break;
                }
                if (isEndPointCh) break;
            }
        }
    }
}

public class Player
{
    public int Levels;
    public int Coins;
    public int Lives;

    public Player()
    {
        this.Levels = 1;
        this.Coins = 0;
        this.Lives = 100;
    }
}



