using System.Collections.Generic;
using System.Linq;

namespace PalCalc.Model
{
    // Decides which of a passive's effects apply to the pal's OWN stats (vs. party/trainer targets), and reads
    // the primary/element values shown in the stat matrix. Extracted from the UI because it's domain logic
    // (which effect value is "the pal's own stat") — and so it can be unit-tested without the WPF layer.
    public static class PassiveStatSelection
    {
        // Effects are tagged with a target (e.g. "ToSelf", "ToSelfAndTrainer", "ToTrainer", "ToOtomo", "ToBaseCampPal").
        // Only self-target effects are the pal's own stat; a null target is treated as self.
        public static bool IsSelfTarget(string targetType) => targetType == null || targetType.Contains("Self");

        // The pal's own value for an effect type (e.g. "ShotAttack"), or null if it has no self-target effect of that type.
        public static float? SelfValue(IEnumerable<PassiveSkillEffect> effects, string internalName) =>
            (effects ?? Enumerable.Empty<PassiveSkillEffect>())
                .Where(e => e.InternalName == internalName && IsSelfTarget(e.TargetType))
                .Select(e => (float?)e.EffectStrength)
                .FirstOrDefault();

        // The pal's own offensive element damage (first self-target ElementBoost_* effect): value + a "{Element} +N" label.
        public static (float? Value, string Display) ElementDamage(IEnumerable<PassiveSkillEffect> effects)
        {
            var effect = (effects ?? Enumerable.Empty<PassiveSkillEffect>())
                .FirstOrDefault(e => e.InternalName != null && e.InternalName.StartsWith("ElementBoost_") && IsSelfTarget(e.TargetType));

            if (effect == null) return (null, null);

            var element = effect.InternalName.Substring("ElementBoost_".Length);
            var sign = effect.EffectStrength >= 0 ? "+" : "";
            return (effect.EffectStrength, $"{element} {sign}{effect.EffectStrength:0.#}");
        }
    }
}
