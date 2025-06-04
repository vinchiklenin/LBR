using System;
using LBR;

var player = new Player();

LBR.Game1 game;
//using var game = new LBR.Game1(new Player());

while (true)
{
    game = new Game1(player);
    game.RunOneFrame();
    player = game._player;
    
    Console.WriteLine(player.Levels);
    //game.Exit();


}


