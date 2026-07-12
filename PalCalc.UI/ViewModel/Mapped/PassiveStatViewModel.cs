using PalCalc.Model;

namespace PalCalc.UI.ViewModel.Mapped
{
    // A single structured passive effect for display (e.g. "Attack +20%"), used by the stat breakdown
    // and the stat matrix in the passive picker.
    public class PassiveStatViewModel
    {
        public string Label { get; }
        public float Value { get; }
        public StatCategory Category { get; }

        public PassiveStatViewModel(string label, float value, StatCategory category)
        {
            Label = label;
            Value = value;
            Category = category;
        }

        public override string ToString()
        {
            // "Other" effects are typically flags/immunities where a "%" is meaningless — show just the label.
            if (Category == StatCategory.Other) return Label;
            return $"{Label} {(Value >= 0 ? "+" : "")}{Value:0.#}%";
        }
    }
}
