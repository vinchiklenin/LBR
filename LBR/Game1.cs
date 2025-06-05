using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace LBR;

public class Game1 : Game
{
    public Scene _activeScene;
    
    public Scene _nextScene;

    public GraphicsDevice _device;
    
    private GraphicsDeviceManager _graphics;
    public SpriteBatch _spriteBatch;
    public Player _player;
    public Texture2D wallTexture;
    public Texture2D floorTexture;
    public Texture2D heroTexture;
    public Texture2D coinTexture;
    public Texture2D endTexture;
    public Texture2D thornsTexture;
    public Texture2D monsterTexture;
    public SpriteFont tittleTexture;
    private Song song;

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
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        base.Initialize();
        Console.WriteLine("Game Init");
    }

    protected override void LoadContent()
    {
        wallTexture = Content.Load<Texture2D>("wall");
        floorTexture = Content.Load<Texture2D>("floor");
        heroTexture = Content.Load<Texture2D>("hero");
        coinTexture = Content.Load<Texture2D>("coin");
        endTexture = Content.Load<Texture2D>("end");
        thornsTexture = Content.Load<Texture2D>("thorns");
        monsterTexture = Content.Load<Texture2D>("monster");
        tittleTexture = Content.Load<SpriteFont>("Monocraft");
        song = Content.Load<Song>("sound");
        _activeScene = new Scene(this, _player);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }
        if (_player.Lives < 1) Exit();
        if (_nextScene != null) TransitionScene();
        if(_activeScene != null) _activeScene.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        MediaPlayer.Volume = 1f;
        MediaPlayer.Play(song);
        
        if(_activeScene != null)
        {
            _activeScene.Draw(_spriteBatch);
        }
        base.Draw(gameTime);
    }
    
    public void ChangeScene(Scene next)
    {

        if(_activeScene != next)
        {
            _nextScene = next;
        }
    }
    
    private void TransitionScene()
    {
        if(_activeScene != null)
        {
            _activeScene.UnloadContent();
        }
        GC.Collect();
        _activeScene = _nextScene;
        _nextScene = null;
        if(_activeScene != null) _activeScene.Initialize();
    }
}

public class Scene
{
    protected Game1 _game;
    
    protected ContentManager _content;
    public Level _level;
    public Player _player;
    Texture2D wallTexture;
    Texture2D floorTexture;
    Texture2D heroTexture;
    Texture2D coinTexture;
    Texture2D endTexture;
    Texture2D thornsTexture;
    Texture2D monsterTexture;
    SpriteFont tittleTexture;
    private SpriteBatch _spriteBatch;
    
    public Scene(Game1 game, Player _player)
    {

        if (game == null)
        {
            throw new ArgumentNullException(nameof(game), "Game cannot be null!");
        }
        this._game = game;
        this._level = new Level();
        this._player = _player;
        this._spriteBatch = _game._spriteBatch;
        wallTexture = _game.wallTexture;
        floorTexture = _game.floorTexture;
        heroTexture = _game.heroTexture;
        coinTexture = _game.coinTexture;
        endTexture = _game.endTexture;
        thornsTexture = _game.thornsTexture;
        monsterTexture = _game.monsterTexture;
        tittleTexture = _game.tittleTexture;
    }
    
    public void Initialize()
    {
        _content = new ContentManager(_game.Services);
        _content.RootDirectory = _game.Content.RootDirectory;
    }
    
    
    public void UnloadContent() { }

    public void Update(GameTime gameTime)
    {
        var kstate = Keyboard.GetState();
        int dX = 0;
        int dY = 0;
        if (kstate.IsKeyDown(Keys.W)) dY -= 1;
        if (kstate.IsKeyDown(Keys.S)) dY += 1;
        if (kstate.IsKeyDown(Keys.A)) dX -= 1;
        if (kstate.IsKeyDown(Keys.D)) dX += 1;
        if (_level.LevelData[((int)_level.HeroPosition.Y + dY - 1) / 50, ((int)_level.HeroPosition.X + dX - 1) / 50] == 3 ||
            _level.LevelData[((int)_level.HeroPosition.Y + dY + 33) / 50, ((int)_level.HeroPosition.X + dX + 29) / 50] == 3)
        {
            _player.Lives -= 5;
        }
        
        else if (_level.LevelData[((int)_level.HeroPosition.Y + dY - 1) / 50, ((int)_level.HeroPosition.X + dX - 1) / 50] != 1 &&
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
                }
                
                if (_level.Coins[((int)_level.HeroPosition.Y + dY + 33) / 50,
                        ((int)_level.HeroPosition.X + dX + 29) / 50] != default 
                    && _level.Coins[((int)_level.HeroPosition.Y + dY + 33) / 50,
                        ((int)_level.HeroPosition.X + dX + 29) / 50] != new Vector2(1000, 1000))
                {
                    _level.Coins[((int)_level.HeroPosition.Y + dY + 33) / 50,
                        ((int)_level.HeroPosition.X + dX + 29) / 50] = new Vector2(1000, 1000);
                    this._player.Coins++;
                }
            }
        }

        
        
        if (Math.Abs(_level.HeroPosition.X - _level.MonsterPosition.X) < 35 &&
            Math.Abs(_level.HeroPosition.Y - _level.MonsterPosition.Y) < 35)
        {
            _player.Lives -= 1;
        }
        if (Math.Abs(_level.HeroPosition.X - _level.MonsterPosition.X) < 40 &&
            Math.Abs(_level.HeroPosition.Y - _level.MonsterPosition.Y) < 40 &&
            kstate.IsKeyDown(Keys.E))
        {
            _level.MonsterSpeed.X = 0;
            _level.MonsterSpeed.Y = 0;
            _level.MonsterPosition = new Vector2(1150, 200);
        }

        //if (Math.Abs(_level.HeroPosition.X - _level.MonsterPosition.X) < 49 &&
        //    Math.Abs(_level.HeroPosition.Y - _level.MonsterPosition.Y) < 49)
        //{
        //    _level.MonsterSpeed.X = _level.HeroPosition.X - _level.MonsterPosition.X / 40;
        //    _level.MonsterSpeed.Y = _level.HeroPosition.Y - _level.MonsterPosition.Y / 40;
        //}
        _level.MonsterPosition += _level.MonsterSpeed;
        if (_level.LevelData[((int)_level.MonsterPosition.Y + 0) / 50, ((int)_level.MonsterPosition.X - 1) / 50] != 0
            && _level.LevelData[((int)_level.MonsterPosition.Y + 33) / 50, ((int)_level.MonsterPosition.X - 1) / 50] != 0)
        {
            _level.MonsterPosition += new Vector2(1, 0);
            _level.MonsterSpeed = new Vector2(0, 1);
        }
        
        if (_level.LevelData[((int)_level.MonsterPosition.Y + 34) / 50, ((int)_level.MonsterPosition.X + 0) / 50] != 0
            && _level.LevelData[((int)_level.MonsterPosition.Y + 34) / 50, ((int)_level.MonsterPosition.X + 29 ) / 50] != 0)
        {
            _level.MonsterPosition += new Vector2(0, -1);
            _level.MonsterSpeed = new Vector2(1, 0);
        }
        
        if (_level.LevelData[((int)_level.MonsterPosition.Y + 0) / 50, ((int)_level.MonsterPosition.X + 30) / 50] != 0
                 && _level.LevelData[((int)_level.MonsterPosition.Y + 33) / 50, ((int)_level.MonsterPosition.X + 30) / 50] != 0)
        {
            _level.MonsterPosition += new Vector2(-1, 0);
            _level.MonsterSpeed = new Vector2(0, -1);
        }
        
        if (_level.LevelData[((int)_level.MonsterPosition.Y - 1) / 50, ((int)_level.MonsterPosition.X + 0) / 50] != 0
                 && _level.LevelData[((int)_level.MonsterPosition.Y - 1) / 50, ((int)_level.MonsterPosition.X + 30) / 50] != 0)
        {
            _level.MonsterPosition += new Vector2(0, 1);
            _level.MonsterSpeed = new Vector2(-1, 0);
        }
      
        
        
        
        if (_level.LevelData[((int)_level.HeroPosition.Y + dY - 1) / 50, ((int)_level.HeroPosition.X + dX - 1) / 50] == 2 ||
            _level.LevelData[((int)_level.HeroPosition.Y + dY + 33) / 50, ((int)_level.HeroPosition.X + dX + 29) / 50] == 2)
        {
            _player.Levels++;
            _game._nextScene = new Scene(_game, _player);
        }
    }
    
    
    public void Draw(SpriteBatch _spriteBatch)
    {
        _game.GraphicsDevice.Clear(Color.Indigo);
        _spriteBatch.Begin();
        for (int i = 0; i < _level.LevelData.GetLength(0); i++)
        {
            for (int j = 0; j < _level.LevelData.GetLength(1); j++)
            {
                if (_level.LevelData[i, j] == 1) _spriteBatch.Draw(wallTexture, new Vector2(j * 50, i * 50), Color.White);
                else if (_level.LevelData[i, j] == default) _spriteBatch.Draw(floorTexture, new Vector2(j * 50, i * 50), Color.White);
                else if (_level.LevelData[i, j] == 2) _spriteBatch.Draw(endTexture, new Vector2(j * 50, i * 50), Color.White);
                else if (_level.LevelData[i, j] == 3) _spriteBatch.Draw(thornsTexture, new Vector2(j * 50, i * 50), Color.White);
            }
        }

        foreach (var coin in _level.Coins)
        {
            if (coin!= default) _spriteBatch.Draw(coinTexture, coin, Color.White);
        }

        _spriteBatch.Draw(heroTexture, _level.HeroPosition, Color.White);
        _spriteBatch.Draw(monsterTexture, _level.MonsterPosition, Color.White);
        
        _spriteBatch.DrawString(tittleTexture, "Monets:", new Vector2(100, 700), Color.White);
        _spriteBatch.DrawString(tittleTexture, _player.Coins.ToString(), new Vector2(320, 700), Color.White);
        
        _spriteBatch.DrawString(tittleTexture, "Levels:", new Vector2(500, 700), Color.White);
        _spriteBatch.DrawString(tittleTexture, _player.Levels.ToString(), new Vector2(720, 700), Color.White);
        
        _spriteBatch.DrawString(tittleTexture, "HP:", new Vector2(900, 700), Color.White);
        _spriteBatch.DrawString(tittleTexture, _player.Lives.ToString(), new Vector2(990, 700), Color.White);
        _spriteBatch.End();
    }
}

public class Level
{
    public string levelName;
    public int[,] LevelData;
    public Vector2 HeroPosition;
    public Vector2[,] Coins;
    public Vector2 MonsterPosition;
    public Vector2 MonsterSpeed = new Vector2(0, 1);
    
    private bool IsInitializedBool = false;
    private Random random = new Random();

    public Level()
    {
        this.levelName = "Ozon-zon-zon level";
        this.LevelData = new int[16, 24];
        this.HeroPosition = default;
        this.Coins = new Vector2[16, 24];
        var maze = MazeGenarate(13, 21);
        for (var i = 0; i < maze.GetLength(0); i++)
        {
            for (var j = 0; j < maze.GetLength(1); j++)
            {
                LevelData[i, j] = maze[i, j];
                if (HeroPosition == default && LevelData[i, j] == default) this.HeroPosition = new Vector2(j * 50 + 10 ,i * 50 + 10);
            }
        }
        var coinsGenCount = 0;
        while (coinsGenCount <= 10)
        {
            for (var i = 2; i < maze.GetLength(0) - 2; i++)
            {
                for (var j = 2; j < maze.GetLength(1) - 2; j++)
                {
                    if (this.LevelData[i, j] == 0 && random.NextDouble() < 0.247)
                    {
                        this.Coins[i, j] = new Vector2(j * 50 + 12, i * 50 + 12);
                        coinsGenCount++;
                    }
                }
                if (coinsGenCount > 10) break;
            }
        }
        this.SetEndPoin();
        this.SetThorns();
        SetMonster();
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

    private void SetThorns()
    {
        for (var i = 4; i < 13; i++)
        {
            for (var j = 4; j < 20; j++)
            {
                if (this.LevelData[i, j] == 1 
                    && this.LevelData[i + 1, j] + this.LevelData[i - 1, j] + this.LevelData[i, j + 1] + this.LevelData[i, j - 1] > 2
                    && random.NextDouble() < 0.327)
                {
                    this.LevelData[i, j] = 3;

                }
            }
        }
        
    }

    private void SetMonster()
    {
        for (var i = 4; i < 13; i++)
        {
            for (var j = 4; j < 20; j++)
            {
                if (this.LevelData[i, j] == 0 && random.NextDouble() < 0.527)
                {
                    this.MonsterPosition = new Vector2(j * 50 + 10, i * 50 + 10); 
                    break;
                }
            }
            if (this.MonsterPosition != default) break;
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