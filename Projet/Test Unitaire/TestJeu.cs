using Modele;
using System;
using Xunit;

namespace TestUnitaire
{
    public class TestJeu
    {
        readonly Jeu jeu = new Jeu("Valorant", "", "Valorant.exe", LauncherName.Riot);

        [Theory]
        [InlineData("Rocket League")]
        [InlineData("League of Legends")]
        [InlineData("Valorant")]
        public void TestNomJeu(string Nom)
        {
            jeu.Nom = Nom;
            Assert.Equal(Nom,jeu.Nom);
        }

        [Theory]
        [InlineData("//programme/JEUX")]
        [InlineData("/bin/temp")]
        public void TestDossierJeu(string v)
        {
            jeu.Dossier = v;
            Assert.Equal(v, jeu.Dossier);
        }

        [Theory]
        [InlineData("Jeux super cool")]
        [InlineData("Il y'a un '")]
        public void TestDescriptionJeu(string desc)
        {
            jeu.Description = desc;
            Assert.Equal(desc, jeu.Description);
        }

        [Theory]
        [InlineData("Truc.png")]
        [InlineData("Jeu.png")]
        public void TestImageJeu(string img)
        {
            jeu.Image = img;
            Assert.Equal(img, jeu.Image);
        }

        [Theory]
        [InlineData("Truc.exe")]
        [InlineData("Jeu.exe")]
        public void TestExecutableJeu(string v)
        {
            jeu.Exec = v;
            Assert.Equal(v, jeu.Exec);
        }

        [Theory]
        [InlineData(LauncherName.Autre)]
        [InlineData(LauncherName.EpicGames)]
        public void TestLauncherJeu(LauncherName v)
        {
            jeu.Launcher = v;
            Assert.Equal(v, jeu.Launcher);
        }

        [Theory]
        [InlineData("Truc.png")]
        [InlineData("Jeu.png")]
        public void TestIconeJeu(string v)
        {
            jeu.Icone = v;
            Assert.Equal(v, jeu.Icone);
        }

        [Theory]
        [InlineData("monter gold")]
        [InlineData("faire un top 1")]
        public void TestNoteJeu(string v)
        {
            jeu.Note = v;
            Assert.Equal(v, jeu.Note);
        }
    }
}
