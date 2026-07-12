using PalCalc.Model;
using System.Collections.Generic;

namespace PalCalc.Solver.Tests
{
    [TestClass]
    public class SpecialCakePassiveTests : PassivesProbabilitiesTestBase
    {
        // The Special Cake (Palworld 1.0) forces the number of passives inherited from the parent pool to
        // the maximum of 4 (PassiveInheritCountOverride = 4). With a 4-passive parent pool and a 4-passive
        // child, all four parent passives are inherited, so any desired subset is guaranteed present.
        [TestMethod]
        public void SpecialCake_GuaranteesDesiredPassives_FromFullParentPool()
        {
            var pool = new List<PassiveSkill> { Runner, Swift, Nimble, Lucky };
            var desired = new List<PassiveSkill> { Runner, Swift };

            var withSpecialCake = Probabilities.Passives.ProbabilityInheritedTargetPassives(
                pool, desired, numFinalPassives: 4, GameConstants.PassiveProbabilityDirectSpecialCake);

            var withoutCake = Probabilities.Passives.ProbabilityInheritedTargetPassives(
                pool, desired, numFinalPassives: 4);

            Assert.AreEqual(1.0f, withSpecialCake, 1e-5f);
            Assert.IsTrue(withoutCake < 0.2f, $"expected default probability well below 1.0 but was {withoutCake}");
        }
    }
}
