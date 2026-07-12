using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PalCalc.Model
{
    public enum StatCategory { Combat, Work, ElementBoost, ElementResist, Status, Other }

    // Maps a game passive-effect internal name (e.g. "ShotAttack", "ElementBoost_Fire", "Mining") to a
    // display label and a category for filter grouping.
    public static class PassiveStatTaxonomy
    {
        // The primary matrix columns (self-target combat/work stats that actually occur on standard passives),
        // in display order. NOTE: raw-attack element damage lives in its own column (see PassiveStatSelection).
        public static readonly IReadOnlyDictionary<string, string> PrimaryColumns = new Dictionary<string, string>
        {
            ["ShotAttack"] = "Attack",
            ["Defense"] = "Defense",
            ["CraftSpeed"] = "Work Speed",
            ["MoveSpeed"] = "Move Speed",
        };

        private static readonly Dictionary<string, StatCategory> PrimaryCategory = new()
        {
            ["ShotAttack"] = StatCategory.Combat,
            ["Defense"] = StatCategory.Combat,
            ["CraftSpeed"] = StatCategory.Work,
            ["MoveSpeed"] = StatCategory.Combat,
        };

        // Work-suitability effect types (map to the Work category).
        private static readonly HashSet<string> WorkSuitabilities = new()
        {
            "Mining", "Logging", "Watering", "Seeding", "Handcraft", "Collection", "Deforest",
            "GenerateElectricity", "Cool", "Transport", "MonsterFarm", "ProductMedicine", "EmitFlame",
            "ItemWeightReduction",
        };

        // Friendly labels for common non-primary effects so chips read cleanly.
        private static readonly Dictionary<string, string> FriendlyLabels = new()
        {
            ["FullStomatch_Decrease"] = "Hunger",
            ["Sanity_Decrease"] = "SAN",
            ["PalExp_Increase"] = "Pal EXP",
            ["ItemWeightReduction"] = "Item Weight",
            ["CollectItemDrop"] = "Gather Drop",
            ["GainItemDrop"] = "Item Drop",
            ["EquipmentDurabilityRate"] = "Durability",
            ["BodyPartsWeakDamage"] = "Weak-point Dmg",
            ["DamageRateByEquippedWeapon"] = "Weapon Dmg",
        };

        // Turn a raw effect name into something readable: known effect -> friendly, else split "_" and camelCase.
        private static string Prettify(string name)
        {
            if (FriendlyLabels.TryGetValue(name, out var friendly)) return friendly;
            var spaced = name.Replace('_', ' ');
            return Regex.Replace(spaced, "([a-z])([A-Z])", "$1 $2");
        }

        public static (string Label, StatCategory Category) Describe(string effectInternalName)
        {
            var name = effectInternalName ?? "";

            if (PrimaryColumns.TryGetValue(name, out var primaryLabel))
                return (primaryLabel, PrimaryCategory[name]);

            if (name.StartsWith("ElementBoost_"))
                return ($"{name.Substring("ElementBoost_".Length)} Damage", StatCategory.ElementBoost);

            if (name.StartsWith("ElementResist_"))
                return ($"{name.Substring("ElementResist_".Length)} Resist", StatCategory.ElementResist);

            if (WorkSuitabilities.Contains(name))
                return (Prettify(name), StatCategory.Work);

            if (name.Contains("AdditionalEffect") || name.StartsWith("Sanity") || name.StartsWith("FullStomatch")
                || name.StartsWith("Fishing") || name.StartsWith("PalExp"))
                return (Prettify(name), StatCategory.Status);

            return (Prettify(name), StatCategory.Other);
        }
    }
}
