using PalCalc.Model;

namespace PalCalc.Solver.Tests
{
    [TestClass]
    public class PassiveStatTaxonomyTests
    {
        [TestMethod]
        public void Describe_MapsCombatAndWorkPrimaries()
        {
            var atk = PassiveStatTaxonomy.Describe("ShotAttack");
            Assert.AreEqual("Attack", atk.Label);
            Assert.AreEqual(StatCategory.Combat, atk.Category);
            Assert.IsTrue(atk.IsPrimaryColumn);

            var work = PassiveStatTaxonomy.Describe("CraftSpeed");
            Assert.AreEqual("Work Speed", work.Label);
            Assert.AreEqual(StatCategory.Work, work.Category);
            Assert.IsTrue(work.IsPrimaryColumn);
        }

        [TestMethod]
        public void Describe_CategorizesElementsAndWorkSuitabilities()
        {
            var boost = PassiveStatTaxonomy.Describe("ElementBoost_Fire");
            Assert.AreEqual(StatCategory.ElementBoost, boost.Category);
            Assert.IsFalse(boost.IsPrimaryColumn);

            Assert.AreEqual(StatCategory.ElementResist, PassiveStatTaxonomy.Describe("ElementResist_Ice").Category);
            Assert.AreEqual(StatCategory.Work, PassiveStatTaxonomy.Describe("Mining").Category);
        }

        [TestMethod]
        public void Describe_UnknownEffectFallsBackToOther()
        {
            var unknown = PassiveStatTaxonomy.Describe("SomethingUnknown");
            Assert.AreEqual("SomethingUnknown", unknown.Label);
            Assert.AreEqual(StatCategory.Other, unknown.Category);
            Assert.IsFalse(unknown.IsPrimaryColumn);
        }

        [TestMethod]
        public void PrimaryColumns_AreTheFiveExpected()
        {
            CollectionAssert.AreEqual(
                new[] { "ShotAttack", "Defense", "CraftSpeed", "MoveSpeed", "MaxInventoryWeight" },
                PassiveStatTaxonomy.PrimaryColumns.ToArray());
        }
    }
}
