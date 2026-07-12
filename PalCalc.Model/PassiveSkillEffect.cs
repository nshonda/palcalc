using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalCalc.Model
{
    public class PassiveSkillEffect
    {
        public string InternalName { get; set; }
        public float EffectStrength { get; set; }

        // Which entity the effect applies to (e.g. "self", party, enemy). Populated for the full
        // structured-effects list on PassiveSkill; may be null for legacy solver-tracked effects.
        public string TargetType { get; set; }

        public static string BreedSpeed => "BreedSpeed"; // (e.g. Philanthropist standard passive)
        public static string SyncCapturedPassives => "SyncroPassiveWhenCapture"; // (e.g. Birds of a Feather partner skill)

        public static readonly IEnumerable<string> TrackedEffects = [BreedSpeed, SyncCapturedPassives];
    }
}
