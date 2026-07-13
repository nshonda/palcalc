using PalCalc.Model;
using System;
using System.Globalization;
using System.Windows.Data;

namespace PalCalc.UI.ViewModel.Converters
{
    // Renders a BreedingCake enum value as a friendly display name for the settings combo box
    // (e.g. DeluxeVegetableCake -> "Deluxe Vegetable Cake") instead of the raw enum identifier.
    internal class BreedingCakeDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not BreedingCake cake) return value?.ToString() ?? string.Empty;

            return cake switch
            {
                BreedingCake.None => "None",
                BreedingCake.MushroomCake => "Mushroom Cake",
                BreedingCake.VegetableCake => "Vegetable Cake",
                BreedingCake.DeluxeVegetableCake => "Deluxe Vegetable Cake",
                BreedingCake.SpecialCake => "Special Cake",
                _ => cake.ToString(),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
