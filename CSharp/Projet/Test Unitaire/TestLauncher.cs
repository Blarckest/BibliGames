using Modele;
using System;
using Xunit;

namespace TestUnitaire
{
    public class TestLauncher
    {
        readonly Launcher Launcher = new Launcher();

        [Theory]
        [InlineData(13)]
        [InlineData(-2)]
        public void TestNombreLauncher(int v)
        {
            Launcher.NbJeux = v;
            if(v<0)
            {
                Assert.Equal(0, Launcher.NbJeux);
            } else
            {
                Assert.Equal(v, Launcher.NbJeux);
            }
        }

        [Theory]
        [InlineData(LauncherName.Uplay)]
        [InlineData(LauncherName.EpicGames)]
        public void TestNomLauncher(LauncherName v)
        {
            Launcher.Nom = v.ToString();
            Assert.Equal(v.ToString(), Launcher.Nom);
        }
    }
}
