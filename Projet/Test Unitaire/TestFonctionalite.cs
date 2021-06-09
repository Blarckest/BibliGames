using Modele;
using Persistance;
using System;
using System.Linq;
using Xunit;

namespace TestUnitaire
{
    public class TestFonctionalite
    {
        private Manager Modele { get; set; } = new Manager(new Stub.Stub());
        private readonly Jeu Jeu = new Jeu("Valorant", "dossier", "Valorant.exe","image","icone","note","description", LauncherName.Riot);

        [Fact]
        public void AjoutJeu()
        {
            Modele.AjoutJeu(Jeu);
            Assert.True(Modele.Affichage.Contains(Jeu));
        }

        [Fact]
        public void ModifDetail()
        {
            var expected = "descriptionmodif";
            Modele.ElementSelected = Jeu;
            Modele.ModifDetail(Jeu.Image, Jeu.Description + "modif", Jeu.Exec, Jeu.Icone);//on test que description car les autres sont proteger contre les fichiers inexistant
            Assert.Equal(expected, Jeu.Description);
        }

        [Fact]
        public void Recherche()
        {
            Modele.Pattern = "a";
            var jeuxFiltrer=Modele.Affichage;
            Assert.True(jeuxFiltrer.All(j => j.Nom.Contains(Modele.Pattern,StringComparison.OrdinalIgnoreCase)));
        }
    }
}
