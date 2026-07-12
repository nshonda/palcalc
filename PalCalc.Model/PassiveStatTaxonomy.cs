using System.Collections.Generic;
using System.Linq;

namespace PalCalc.Model
{
    public enum StatCategory { Combat, Work, ElementBoost, ElementResist, Status, Other }

    // Maps a game passive-effect internal name (e.g. "ShotAttack", "ElementBoost_Fire", "Mining") to a
    // display label, a category for filter grouping, and whether it's one of the primary matrix columns.
    public static class PassiveStatTaxonomy
    {
        // The primary matrix columns, in display order.
        public static readonly IReadOnlyList<string> PrimaryColumns =
            new[] { "ShotAttack", "Defense", "CraftSpeed", "MoveSpeed", "MaxInventoryWeight" };

        private static readonly Dictionary<string, (string Label, StatCategory Category)> Primary = new()
        {
            ["ShotAttack"] = ("Attack", StatCategory.Combat),
            ["Defense"] = ("Defense", StatCategory.Combat),
            ["CraftSpeed"] = ("Work Speed", StatCategory.Work),
            ["MoveSpeed"] = ("Move Speed", StatCategory.Combat),
            ["MaxInventoryWeight"] = ("Weight", StatCategory.Work),
        };

        // Work-suitability effect types (map to the Work category, non-primary).
        private static readonly HashSet<string> WorkSuitabilities = new()
        {
            "Mining", "Logging", "Watering", "Seeding", "Handcraft", "Collection", "Deforest",
            "GenerateElectricity", "Cool", "Transport", "MonsterFarm", "ProductMedicine", "EmitFlame",
            "ItemWeightReduction", "CraftSpeed_Product",
        };

        public static (string Label, StatCategory Category, bool IsPrimaryColumn) Describe(string effectInternalName)
        {
            var name = effectInternalName ?? "";

            if (Primary.TryGetValue(name, out var p))
                return (p.Label, p.Category, true);

            if (name.StartsWith("ElementBoost_"))
                return ($"{name.Substring("ElementBoost_".Length)} Damage", StatCategory.ElementBoost, false);

            if (name.StartsWith("ElementResist_"))
                return ($"{name.Substring("ElementResist_".Length)} Resist", StatCategory.ElementResist, false);

            if (WorkSuitabilities.Contains(name))
                return (name, StatCategory.Work, false);

            if (name.Contains("AdditionalEffect") || name.StartsWith("Sanity") || name.StartsWith("FullStomatch")
                || name.StartsWith("Fishing") || name.StartsWith("PalExp"))
                return (name, StatCategory.Status, false);

            return (name, StatCategory.Other, false);
        }
    }
}
