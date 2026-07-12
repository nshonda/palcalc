using PalCalc.Model;

namespace PalCalc.Solver.Tests
{
    [TestClass]
    public class PassiveStatTaxonomyTests
    {
        [TestMethod]
        public void Describe_MapsPrimaryCombatAndWork()
        {
            var atk = PassiveStatTaxonomy.Describe("ShotAttack");
            Assert.AreEqual("Attack", atk.Label);
            Assert.AreEqual(StatCategory.Combat, atk.Category);

            var work = PassiveStatTaxonomy.Describe("CraftSpeed");
            Assert.AreEqual("Work Speed", work.Label);
            Assert.AreEqual(StatCategory.Work, work.Category);
        }

        [TestMethod]
        public void Describe_FormatsElementLabels()
        {
            var boost = PassiveStatTaxonomy.Describe("ElementBoost_Fire");
            Assert.AreEqual("Fire Damage", boost.Label);
            Assert.AreEqual(StatCategory.ElementBoost, boost.Category);

            var resist = PassiveStatTaxonomy.Describe("ElementResist_Ice");
            Assert.AreEqual("Ice Resist", resist.Label);
            Assert.AreEqual(StatCategory.ElementResist, resist.Category);
        }

        [TestMethod]
        public void Describe_CategorizesWorkSuitabilitiesAndStatus()
        {
            Assert.AreEqual(StatCategory.Work, PassiveStatTaxonomy.Describe("Mining").Category);

            // ItemWeightReduction is in both WorkSuitabilities and FriendlyLabels — the Work check runs first.
            var weight = PassiveStatTaxonomy.Describe("ItemWeightReduction");
            Assert.AreEqual("Item Weight", weight.Label);
            Assert.AreEqual(StatCategory.Work, weight.Category);

            Assert.AreEqual(StatCategory.Status, PassiveStatTaxonomy.Describe("AdditionalEffect_Burn").Category);

            var hunger = PassiveStatTaxonomy.Describe("FullStomatch_Decrease");
            Assert.AreEqual("Hunger", hunger.Label);
            Assert.AreEqual(StatCategory.Status, hunger.Category);
        }

        [TestMethod]
        public void Describe_UnknownEffect_PrettifiesAndFallsToOther()
        {
            var u = PassiveStatTaxonomy.Describe("Some_WeirdThing");
            Assert.AreEqual("Some Weird Thing", u.Label); // underscore + camelCase split
            Assert.AreEqual(StatCategory.Other, u.Category);
        }

        // Guard against silent miscategorization: the combat/work/element stats that drive the matrix and
        // filter must NOT fall into the grey "Other" bucket.
        [TestMethod]
        public void Describe_KnownStats_AreNotOther()
        {
            foreach (var name in new[] { "ShotAttack", "Defense", "CraftSpeed", "MoveSpeed",
                                         "ElementBoost_Fire", "ElementResist_Ice", "Mining", "Logging" })
                Assert.AreNotEqual(StatCategory.Other, PassiveStatTaxonomy.Describe(name).Category, $"{name} fell to Other");
        }
    }
}
