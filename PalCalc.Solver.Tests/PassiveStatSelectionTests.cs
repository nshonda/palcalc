using PalCalc.Model;
using System.Collections.Generic;

namespace PalCalc.Solver.Tests
{
    [TestClass]
    public class PassiveStatSelectionTests
    {
        private static PassiveSkillEffect Fx(string name, string target, float value) =>
            new() { InternalName = name, TargetType = target, EffectStrength = value };

        [TestMethod]
        public void IsSelfTarget_IncludesSelfVariants_ExcludesOthers()
        {
            Assert.IsTrue(PassiveStatSelection.IsSelfTarget("ToSelf"));
            Assert.IsTrue(PassiveStatSelection.IsSelfTarget("ToSelfAndTrainer")); // partial-match subtlety
            Assert.IsTrue(PassiveStatSelection.IsSelfTarget(null));               // null treated as self
            Assert.IsFalse(PassiveStatSelection.IsSelfTarget("ToTrainer"));
            Assert.IsFalse(PassiveStatSelection.IsSelfTarget("ToOtomo"));
            Assert.IsFalse(PassiveStatSelection.IsSelfTarget("ToBaseCampPal"));
        }

        [TestMethod]
        public void SelfValue_ReturnsSelfTargetEffect_IgnoresPartyTargeted()
        {
            var effects = new List<PassiveSkillEffect>
            {
                Fx("ShotAttack", "ToSelf", 30),
                Fx("Defense", "ToOtomo", 20), // buffs the OTHER pal, not this one
            };

            Assert.AreEqual(30f, PassiveStatSelection.SelfValue(effects, "ShotAttack"));
            Assert.IsNull(PassiveStatSelection.SelfValue(effects, "Defense"));    // party-targeted -> not own stat
            Assert.IsNull(PassiveStatSelection.SelfValue(effects, "CraftSpeed")); // absent
            Assert.IsNull(PassiveStatSelection.SelfValue(null, "ShotAttack"));    // null-safe
        }

        [TestMethod]
        public void ElementDamage_ReturnsSelfElementBoost_WithLabel()
        {
            var effects = new List<PassiveSkillEffect>
            {
                Fx("ElementBoost_Ice", "ToSelf", 10),
                Fx("ShotAttack", "ToSelf", 20),
            };

            var (value, display) = PassiveStatSelection.ElementDamage(effects);
            Assert.AreEqual(10f, value);
            Assert.AreEqual("Ice +10", display);

            // party-targeted element boost (the majority in the real data) is not the pal's own stat
            var (v2, d2) = PassiveStatSelection.ElementDamage(new List<PassiveSkillEffect> { Fx("ElementBoost_Fire", "ToOtomo", 15) });
            Assert.IsNull(v2);
            Assert.IsNull(d2);
        }
    }
}
