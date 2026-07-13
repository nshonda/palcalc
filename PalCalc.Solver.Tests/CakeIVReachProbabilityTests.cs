using PalCalc.Model;

namespace PalCalc.Solver.Tests
{
    [TestClass]
    public class CakeIVReachProbabilityTests
    {
        // floor=1, ceil=5 (Mushroom / Deluxe Vegetable cake)
        [TestMethod]
        public void Gap1_Guaranteed()
        {
            var (iv, p) = Probabilities.IVs.ApplyCakeBonusToTarget(new IV_Value(true, 99, 99), target: 100, floor: 1, ceil: 5);
            Assert.AreEqual(100, iv.Min);
            Assert.AreEqual(1.0f, p, 1e-5f);
        }

        [TestMethod]
        public void Gap2_FourFifths()
        {
            var (iv, p) = Probabilities.IVs.ApplyCakeBonusToTarget(new IV_Value(true, 98, 98), target: 100, floor: 1, ceil: 5);
            Assert.AreEqual(100, iv.Min);          // lifted to target (satisfies), uncertainty is in p
            Assert.AreEqual(0.8f, p, 1e-5f);       // (5-2+1)/5
        }

        [TestMethod]
        public void Gap5_OneFifth()
        {
            var (iv, p) = Probabilities.IVs.ApplyCakeBonusToTarget(new IV_Value(true, 95, 95), target: 100, floor: 1, ceil: 5);
            Assert.AreEqual(100, iv.Min);
            Assert.AreEqual(0.2f, p, 1e-5f);       // (5-5+1)/5
        }

        [TestMethod]
        public void Gap6_Unreachable_FallsToGuaranteeFloor()
        {
            var (iv, p) = Probabilities.IVs.ApplyCakeBonusToTarget(new IV_Value(true, 94, 94), target: 100, floor: 1, ceil: 5);
            Assert.AreEqual(95, iv.Min);           // 94 + floor(1); does NOT reach 100
            Assert.AreEqual(1.0f, p, 1e-5f);       // no false probability; path just won't satisfy this IV
        }

        [TestMethod]
        public void AlreadyMeetsTarget_NoDiscount()
        {
            var (iv, p) = Probabilities.IVs.ApplyCakeBonusToTarget(new IV_Value(true, 100, 100), target: 100, floor: 1, ceil: 5);
            Assert.AreEqual(100, iv.Min);          // 100 + floor clamped to 100
            Assert.AreEqual(1.0f, p, 1e-5f);
        }

        [TestMethod]
        public void UntargetedIV_JustExtendsRange()
        {
            var (iv, p) = Probabilities.IVs.ApplyCakeBonusToTarget(new IV_Value(true, 50, 50), target: 0, floor: 1, ceil: 5);
            Assert.AreEqual(51, iv.Min);
            Assert.AreEqual(55, iv.Max);
            Assert.AreEqual(1.0f, p, 1e-5f);
        }

        [TestMethod]
        public void NoCake_IsIdentity()
        {
            var merged = new IV_Value(true, 98, 98);
            var (iv, p) = Probabilities.IVs.ApplyCakeBonusToTarget(merged, target: 100, floor: 0, ceil: 0);
            Assert.AreEqual(merged, iv);
            Assert.AreEqual(1.0f, p, 1e-5f);
        }

        [TestMethod]
        public void RandomIV_Untouched()
        {
            var (iv, p) = Probabilities.IVs.ApplyCakeBonusToTarget(IV_Value.Random, target: 100, floor: 1, ceil: 5);
            Assert.AreEqual(IV_Value.Random, iv);
            Assert.AreEqual(1.0f, p, 1e-5f);
        }

        [TestMethod]
        public void CeilClampsMaxAt100()
        {
            var (iv, _) = Probabilities.IVs.ApplyCakeBonusToTarget(new IV_Value(true, 98, 98), target: 100, floor: 1, ceil: 5);
            Assert.AreEqual(100, iv.Max);          // 98 + 5 = 103 -> clamped to 100
        }
    }
}
