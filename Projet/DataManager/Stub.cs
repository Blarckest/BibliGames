using Modele;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DataManager
{
    public class Stub :Loader
    {
        public List<Element> Jeux { get; set; } = new List<Element>();
        public Stub(string Path=""):base(Path)
        {
            Directory.CreateDirectory("./Stub/Game1/Infos");
            Directory.CreateDirectory("./Stub/Game2/Infos");
            Directory.CreateDirectory("./Stub/Game3/Infos");
            Directory.CreateDirectory("./Stub/Game4/Infos");
            Directory.CreateDirectory("./Stub/Game5/Infos");
            Directory.CreateDirectory("./Stub/Game6/Infos");
            Directory.CreateDirectory("./Stub/Game7/Infos");
            Directory.CreateDirectory("./Stub/Game8/Infos");
            Directory.CreateDirectory("./Stub/Game9/Infos");
            Directory.CreateDirectory("./Stub/Game10/Infos");
            Directory.CreateDirectory("./Stub/Game11/Infos");
            Directory.CreateDirectory("./Stub/Game1/Exec");
            Directory.CreateDirectory("./Stub/Game2/Exec");
            Directory.CreateDirectory("./Stub/Game3/Exec");
            Directory.CreateDirectory("./Stub/Game4/Exec");
            Directory.CreateDirectory("./Stub/Game5/Exec");
            Directory.CreateDirectory("./Stub/Game6/Exec");
            Directory.CreateDirectory("./Stub/Game7/Exec");
            Directory.CreateDirectory("./Stub/Game8/Exec");
            Directory.CreateDirectory("./Stub/Game9/Exec");
            Directory.CreateDirectory("./Stub/Game10/Exec");
            Directory.CreateDirectory("./Stub/Game11/Exec");
            File.Create("./Stub/Game1/Exec/game.exe");
            File.Create("./Stub/Game2/Exec/game.exe");
            File.Create("./Stub/Game3/Exec/game.exe");
            File.Create("./Stub/Game4/Exec/game.exe");
            File.Create("./Stub/Game5/Exec/game.exe");
            File.Create("./Stub/Game6/Exec/game.exe");
            File.Create("./Stub/Game7/Exec/game.exe");
            File.Create("./Stub/Game8/Exec/game.exe");
            File.Create("./Stub/Game9/Exec/game.exe");
            File.Create("./Stub/Game10/Exec/game.exe");
            File.Create("./Stub/Game11/Exec/game.exe");
            Jeux.Add(new Launcher(LauncherName.EpicGames));
            Jeux.Add(new Jeu("Game1", "./Stub/Game1/Exec/", "./Stub/Game1/Exec/game.exe", "./Stub/Game1/Infos/image.jpg", "./Stub/Game1/Infos/icon.jpg", "./Stub/Game1/Infos/note.txt", "./Stub/Game1/Infos/text.txt", LauncherName.EpicGames));
            Jeux.Add(new Jeu("Game2", "./Stub/Game2/Exec/", "./Stub/Game2/Exec/game.exe", "./Stub/Game2/Infos/image.jpg", "./Stub/Game2/Infos/icon.jpg", "./Stub/Game2/Infos/note.txt", "./Stub/Game2/Infos/text.txt", LauncherName.EpicGames));
            Jeux.Add(new Jeu("Game3", "./Stub/Game3/Exec/", "./Stub/Game3/Exec/game.exe", "./Stub/Game3/Infos/image.jpg", "./Stub/Game3/Infos/icon.jpg", "./Stub/Game3/Infos/note.txt", "./Stub/Game3/Infos/text.txt", LauncherName.EpicGames));
            Jeux.Add(new Launcher(LauncherName.Origin));
            Jeux.Add(new Jeu("Game4", "./Stub/Game4/Exec/", "./Stub/Game4/Exec/game.exe", "./Stub/Game4/Infos/image.jpg", "./Stub/Game4/Infos/icon.jpg", "./Stub/Game4/Infos/note.txt", "./Stub/Game4/Infos/text.txt", LauncherName.Origin));
            Jeux.Add(new Jeu("Game5", "./Stub/Game5/Exec/", "./Stub/Game5/Exec/game.exe", "./Stub/Game5/Infos/image.jpg", "./Stub/Game5/Infos/icon.jpg", "./Stub/Game5/Infos/note.txt", "./Stub/Game5/Infos/text.txt", LauncherName.Origin));
            Jeux.Add(new Jeu("Game6", "./Stub/Game6/Exec/", "./Stub/Game6/Exec/game.exe", "./Stub/Game6/Infos/image.jpg", "./Stub/Game6/Infos/icon.jpg", "./Stub/Game6/Infos/note.txt", "./Stub/Game6/Infos/text.txt", LauncherName.Origin));
            Jeux.Add(new Launcher(LauncherName.Riot));
            Jeux.Add(new Jeu("Game7", "./Stub/Game7/Exec/", "./Stub/Game7/Exec/game.exe", "./Stub/Game7/Infos/image.jpg", "./Stub/Game7/Infos/icon.jpg", "./Stub/Game7/Infos/note.txt", "./Stub/Game7/Infos/text.txt", LauncherName.Riot));
            Jeux.Add(new Jeu("Game8", "./Stub/Game8/Exec/", "./Stub/Game8/Exec/game.exe", "./Stub/Game8/Infos/image.jpg", "./Stub/Game8/Infos/icon.jpg", "./Stub/Game8/Infos/note.txt", "./Stub/Game8/Infos/text.txt", LauncherName.Riot));
            Jeux.Add(new Launcher(LauncherName.Steam));
            Jeux.Add(new Jeu("Game9", "./Stub/Game9/Exec/", "./Stub/Game9/Exec/game.exe", "./Stub/Game9/Infos/image.jpg", "./Stub/Game9/Infos/icon.jpg", "./Stub/Game9/Infos/note.txt", "./Stub/Game9/Infos/text.txt", LauncherName.Steam));
            Jeux.Add(new Launcher(LauncherName.Uplay));
            Jeux.Add(new Jeu("Game10", "./Stub/Game10/Exec/", "./Stub/Game10/Exec/game.exe", "./Stub/Game10/Infos/image.jpg", "./Stub/Game10/Infos/icon.jpg", "./Stub/Game10/Infos/note.txt", "./Stub/Game10/Infos/text.txt", LauncherName.Uplay));
            Jeux.Add(new Launcher(LauncherName.Autre));
            Jeux.Add(new Jeu("Game11", "./Stub/Game11/Exec/", "./Stub/Game11/Exec/game.exe", "./Stub/Game11/Infos/image.jpg", "./Stub/Game11/Infos/icon.jpg", "./Stub/Game11/Infos/note.txt", "./Stub/Game11/Infos/text.txt", LauncherName.Autre));
        }

        public override IList<Element> Load()
        {
            return Jeux;
        }
    }
}
