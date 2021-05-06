using Modele;
using System;
using Xunit;

namespace TestUnitaire
{
    public class UnitTest1
    {
        
        [Fact]
        public void TestJeu()
        {
            Jeu jeu = new Jeu("Valorant", "", "Valorant.exe", LauncherName.Riot);

            jeu.Description=$"{jeu.Nom} est un jeux de tir FPS.";
            jeu.Image = $"{jeu.Nom}.png";
            jeu.Dossier = @"/programme/Riot";
            jeu.Nom = "RocketLeague";
            jeu.Exec = "RocketLeague.exe";
            jeu.Launcher = LauncherName.EpicGames;
            jeu.Icone = "voiture.png"; 
            jeu.Note = "monter Gold";

            Jeu jeu2 = new Jeu("GTA5", "", "", "GTA5.png", "Trevor.png", "finir les braquages", "GTA5 est un jeu open world");
        }
    }
}
