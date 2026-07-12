using PalCalc.Model;
using System.Collections.Generic;
using System.Linq;

namespace PalCalc.Solver.Tests
{
    [TestClass]
    public class PassiveEffectModelTests
    {
        // A passive carries its full structured per-stat effects (not just the 2 solver-tracked ones),
        // each with the effect type, its target, and the value.
        [TestMethod]
        public void PassiveSkill_CarriesFullStructuredEffects_WithTargetType()
        {
            var passive = new PassiveSkill("Legend", "Legend", 4)
            {
                Effects = new List<PassiveSkillEffect>
                {
                    new() { InternalName = "ShotAttack", TargetType = "self", EffectStrength = 20 },
                    new() { InternalName = "Defense",    TargetType = "self", EffectStrength = 20 },
                    new() { InternalName = "MoveSpeed",  TargetType = "self", EffectStrength = 20 },
                },
            };

            Assert.AreEqual(3, passive.Effects.Count);
            var atk = passive.Effects.Single(e => e.InternalName == "ShotAttack");
            Assert.AreEqual("self", atk.TargetType);
            Assert.AreEqual(20f, atk.EffectStrength);
        }
    }
}
