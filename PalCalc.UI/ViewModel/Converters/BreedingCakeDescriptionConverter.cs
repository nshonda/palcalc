using PalCalc.Model;
using System;
using System.Globalization;
using System.Windows.Data;

namespace PalCalc.UI.ViewModel.Converters
{
    // Renders a one-line "what it does + when to use it" description for the selected breeding cake,
    // shown under the settings combo box so the effect and recommended stage are discoverable.
    internal class BreedingCakeDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not BreedingCake cake) return string.Empty;

            return cake switch
            {
                BreedingCake.None =>
                    "Plain breeding — no bonus. The solver assumes no cake.",
                BreedingCake.MushroomCake =>
                    "Offspring gets +1–5 IV. Use while grinding IVs toward 100 — parents a few points short can now reach the target.",
                BreedingCake.VegetableCake =>
                    "2 eggs per breeding cycle (about half the breeding time). Use for bulk egg production and early breeding.",
                BreedingCake.DeluxeVegetableCake =>
                    "Offspring gets +1–5 IV plus a mutation chance. Use late-game — IV grinding while hunting mutation variants. (Solver models the IV bonus, not mutation.)",
                BreedingCake.SpecialCake =>
                    "Guarantees the offspring inherits all 4 target passives (no RNG). Use to lock in a 4-passive keeper once its IVs are finished.",
                _ => string.Empty,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
