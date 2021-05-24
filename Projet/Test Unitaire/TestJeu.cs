using Modele;
using System;
using Xunit;

namespace TestUnitaire
{
    public class TestJeu
    {
        readonly Jeu Jeu = new Jeu("Valorant", "", "Valorant.exe", LauncherName.Riot);

        [Theory]
        [InlineData("Rocket League")]
        [InlineData("League of Legends")]
        [InlineData("Valorant")]
        public void TestNomJeu(string nom)
        {
            Jeu.Nom = nom;
            Assert.Equal(nom,Jeu.Nom);
        }

        [Theory]
        [InlineData("//programme/JEUX")]
        [InlineData("/bin/temp")]
        public void TestDossierJeu(string v)
        {
            Jeu.Dossier = v;
            Assert.Equal(v, Jeu.Dossier);
        }

        [Theory]
        [InlineData("Jeux super cool")]
        [InlineData("Il y'a un '")]
        public void TestDescriptionJeu(string desc)
        {
            Jeu.Description = desc;
            Assert.Equal(desc, Jeu.Description);
        }

        [Theory]
        [InlineData("Truc.png")]
        [InlineData("Jeu.png")]
        public void TestImageJeu(string img)
        {
            Jeu.Image = img;
            Assert.Equal(img, Jeu.Image);
        }

        [Theory]
        [InlineData("Truc.exe")]
        [InlineData("Jeu.exe")]
        public void TestExecutableJeu(string v)
        {
            Jeu.Exec = v;
            Assert.Equal(v, Jeu.Exec);
        }

        [Theory]
        [InlineData(LauncherName.Autre)]
        [InlineData(LauncherName.EpicGames)]
        public void TestLauncherJeu(LauncherName v)
        {
            Jeu.Launcher = v;
            Assert.Equal(v, Jeu.Launcher);
        }

        [Theory]
        [InlineData("Truc.png")]
        [InlineData("Jeu.png")]
        public void TestIconeJeu(string v)
        {
            Jeu.Icone = v;
            Assert.Equal(v, Jeu.Icone);
        }

        [Theory]
        [InlineData("monter gold")]
        [InlineData("faire un top 1")]
        public void TestNoteJeu(string v)
        {
            Jeu.Note = v;
            Assert.Equal(v, Jeu.Note);
        }
    }
}
