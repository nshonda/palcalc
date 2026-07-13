using PalCalc.Model;

namespace PalCalc.Solver.Tests
{
    [TestClass]
    public class CakeIVBonusTests
    {
        // A relevant IV of exactly 99 with a +1 floor becomes a guaranteed 100.
        [TestMethod]
        public void ApplyCakeBonus_LiftsFloorAndCeil()
        {
            var iv = new IV_Value(IsRelevant: true, Min: 99, Max: 99);

            var bumped = Probabilities.IVs.ApplyCakeBonus(iv, floor: 1, ceil: 5);

            Assert.AreEqual(100, bumped.Min); // 99 + 1 floor
            Assert.AreEqual(100, bumped.Max); // 99 + 5 = 104, clamped to 100
            Assert.IsTrue(bumped.IsRelevant);
        }

        // Below the cap the ceil is not clamped.
        [TestMethod]
        public void ApplyCakeBonus_RangeBelowCapIsNotClamped()
        {
            var iv = new IV_Value(IsRelevant: true, Min: 90, Max: 90);

            var bumped = Probabilities.IVs.ApplyCakeBonus(iv, floor: 1, ceil: 5);

            Assert.AreEqual(91, bumped.Min);
            Assert.AreEqual(95, bumped.Max);
        }

        // No cake (floor 0) is a no-op.
        [TestMethod]
        public void ApplyCakeBonus_ZeroFloorIsNoOp()
        {
            var iv = new IV_Value(IsRelevant: true, Min: 99, Max: 99);

            var bumped = Probabilities.IVs.ApplyCakeBonus(iv, floor: 0, ceil: 0);

            Assert.AreEqual(iv, bumped);
        }

        // A random (untargeted) IV is left untouched.
        [TestMethod]
        public void ApplyCakeBonus_RandomIvIsUntouched()
        {
            var bumped = Probabilities.IVs.ApplyCakeBonus(IV_Value.Random, floor: 1, ceil: 5);

            Assert.AreEqual(IV_Value.Random, bumped);
        }
    }
}
