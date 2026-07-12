using AdonisUI.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using FuzzySharp;
using PalCalc.Model;
using PalCalc.UI.ViewModel.Mapped;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PalCalc.UI.View
{
    [ObservableObject]
    public partial class PassivesSearchWindow : AdonisWindow
    {
        [NotifyPropertyChangedFor(nameof(DisplayedOptions))]
        [ObservableProperty]
        private string searchText;

        // The stat filter — when set, only passives that affect the chosen stat are shown.
        [NotifyPropertyChangedFor(nameof(DisplayedOptions))]
        [ObservableProperty]
        private StatFilterOption selectedStatFilter;

        // Every effect type present across passives, grouped by category, with an "All stats" entry first.
        public List<StatFilterOption> StatFilterOptions { get; } = BuildStatFilterOptions();

        private static List<StatFilterOption> BuildStatFilterOptions()
        {
            var opts = PassiveSkillViewModel.All
                .SelectMany(p => p.ModelObject.Effects)
                .Select(e => e.InternalName)
                .Distinct()
                .Select(n =>
                {
                    var (label, category, _) = PassiveStatTaxonomy.Describe(n);
                    return new StatFilterOption(n, label, category);
                })
                .OrderBy(o => o.Category)
                .ThenBy(o => o.Label)
                .ToList();

            opts.Insert(0, new StatFilterOption(null, "All stats", StatCategory.Other));
            return opts;
        }

        private bool Matches(string text) => text.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || Fuzz.WeightedRatio(SearchText.ToLower(), text.ToLower()) > 70;

        public List<PassiveSkillViewModel> DisplayedOptions =>
            PassiveSkillViewModel.All.Where(p =>
                (string.IsNullOrEmpty(SearchText) ||
                 Matches(p.Name.Value) ||
                 Matches(p.Description.Value))
                &&
                (SelectedStatFilter?.InternalName == null ||
                 p.ModelObject.Effects.Any(e => e.InternalName == SelectedStatFilter.InternalName))
            ).ToList();

        public PassivesSearchWindow()
        {
            InitializeComponent();

            Loaded += (_, _) => m_TextBox.Focus();
        }
    }

    // A selectable stat in the passive-search filter. InternalName == null means "All stats".
    public class StatFilterOption
    {
        public string InternalName { get; }
        public string Label { get; }
        public StatCategory Category { get; }

        public StatFilterOption(string internalName, string label, StatCategory category)
        {
            InternalName = internalName;
            Label = label;
            Category = category;
        }

        public string DisplayLabel => InternalName == null ? Label : $"{Category}: {Label}";
    }
}
