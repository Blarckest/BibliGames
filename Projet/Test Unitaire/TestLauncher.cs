using Modele;
using System;
using Xunit;

namespace TestUnitaire
{
    public class TestLauncher
    {
        readonly Launcher launcher = new Launcher();

        [Theory]
        [InlineData(13)]
        [InlineData(-2)]
        public void TestNombreLauncher(int v)
        {
            launcher.NbJeux = v;
            if(v<0)
            {
                Assert.Equal(0, launcher.NbJeux);
            } else
            {
                Assert.Equal(v, launcher.NbJeux);
            }
        }

        [Theory]
        [InlineData(LauncherName.Uplay)]
        [InlineData(LauncherName.EpicGames)]
        public void TestNomLauncher(LauncherName v)
        {
            launcher.Nom = v.ToString();
            Assert.Equal(v.ToString(), launcher.Nom);
        }
    }
}
