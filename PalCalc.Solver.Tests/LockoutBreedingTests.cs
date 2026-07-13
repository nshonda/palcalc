using PalCalc.Model;
using System.Linq;

namespace PalCalc.Solver.Tests
{
    [TestClass]
    public class LockoutBreedingTests
    {
        // Pals that in Palworld 1.0 breed self×self only (X = X×X) and must never be
        // produced by a non-identical parent pair. Names must match db.json display names.
        private static readonly string[] SelfOnlyPalNames = new[]
        {
            // legendaries
            "Jetragon", "Frostallion", "Necromus", "Paladius",
            // tower bosses
            "Grizzbolt", "Orserk", "Faleris", "Shadowbeak",
            // new 1.0 pals flagged as breeding-locked
            "Dandilord", "Silvance",
        };

        [TestMethod]
        public void SelfOnlyPals_AppearOnlyAsSelfChild()
        {
            var bdb = PalBreedingDB.LoadEmbedded(PalDB.LoadEmbedded());

            foreach (var name in SelfOnlyPalNames)
            {
                var childEntries = bdb.Breeding.Where(b => b.Child.Name == name).ToList();

                Assert.IsTrue(
                    childEntries.Count > 0,
                    $"'{name}' has no breeding entry at all — name is wrong or the pal is missing from db.json");

                foreach (var b in childEntries)
                {
                    Assert.AreEqual(
                        b.Parent1.Pal, b.Parent2.Pal,
                        $"'{name}' is produced by non-identical parents " +
                        $"{b.Parent1.Pal.Name} + {b.Parent2.Pal.Name} — the self×self lockout is broken");
                }
            }
        }

        // Proves the predicate above actually discriminates: a normal pal (Aegidron has
        // ~120 non-identical parent pairs) MUST have at least one non-identical parent pair,
        // so a passing SelfOnlyPals test is meaningful rather than vacuously green.
        [TestMethod]
        public void NormalBreedablePal_HasNonIdenticalParents_SanityCheck()
        {
            var bdb = PalBreedingDB.LoadEmbedded(PalDB.LoadEmbedded());

            var entries = bdb.Breeding.Where(b => b.Child.Name == "Aegidron").ToList();

            Assert.IsTrue(
                entries.Any(b => b.Parent1.Pal != b.Parent2.Pal),
                "Aegidron should be breedable from non-identical parents; if not, the graph " +
                "or the test predicate is wrong");
        }
    }
}
