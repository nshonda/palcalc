using PalCalc.Model;

namespace PalCalc.Solver.Tests
{
    [TestClass]
    public class BreedingCakeSettingsTests
    {
        // Cakes are mutually exclusive (one per breeding farm) — modeled as a single ActiveCake choice.
        // The Vegetable Cake is the only cake that changes eggs-per-breeding (to 2).
        [TestMethod]
        public void EggsPerBreeding_DerivesFromActiveCake()
        {
            Assert.AreEqual(1, new GameSettings { ActiveCake = BreedingCake.None }.EggsPerBreeding);
            Assert.AreEqual(2, new GameSettings { ActiveCake = BreedingCake.VegetableCake }.EggsPerBreeding);
            Assert.AreEqual(1, new GameSettings { ActiveCake = BreedingCake.SpecialCake }.EggsPerBreeding);
        }

        [TestMethod]
        public void IvBonus_DerivesFromActiveCake()
        {
            Assert.AreEqual(0, new GameSettings { ActiveCake = BreedingCake.None }.IvBonusFloor);
            Assert.AreEqual(0, new GameSettings { ActiveCake = BreedingCake.None }.IvBonusCeil);

            Assert.AreEqual(1, new GameSettings { ActiveCake = BreedingCake.MushroomCake }.IvBonusFloor);
            Assert.AreEqual(5, new GameSettings { ActiveCake = BreedingCake.MushroomCake }.IvBonusCeil);

            Assert.AreEqual(1, new GameSettings { ActiveCake = BreedingCake.DeluxeVegetableCake }.IvBonusFloor);
            Assert.AreEqual(5, new GameSettings { ActiveCake = BreedingCake.DeluxeVegetableCake }.IvBonusCeil);

            // passive/egg cakes give no IV bonus
            Assert.AreEqual(0, new GameSettings { ActiveCake = BreedingCake.VegetableCake }.IvBonusFloor);
            Assert.AreEqual(0, new GameSettings { ActiveCake = BreedingCake.SpecialCake }.IvBonusFloor);
        }
    }
}
