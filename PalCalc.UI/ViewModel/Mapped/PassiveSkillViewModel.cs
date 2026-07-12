using PalCalc.Model;
using PalCalc.UI.Localization;
using PalCalc.UI.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using Windows.UI.Notifications;

namespace PalCalc.UI.ViewModel.Mapped
{
    public class RankColorConverter : IValueConverter
    {
        private Dictionary<Color, SolidColorBrush> brushes = new Dictionary<Color, SolidColorBrush>();

        private SolidColorBrush MakeBrush(Color color) => brushes.ContainsKey(color) ? brushes[color] : brushes[color] = new SolidColorBrush(color) { Opacity = 1 };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return MakeBrush((int)value switch
            {
                < 0 => new Color() { R = 247, G = 63, B = 63, A = 255 },
                4 => new Color() { R = 104, G = 255, B = 216, A = 255 },
                > 1 => new Color() { R = 255, G = 221, B = 0, A = 255 },
                _ => new Color() { R = 230, G = 231, B = 223, A = 255 },
            });
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Muted per-category chip background so stat tags are colour-coded (works on the dark theme).
    // Brushes are cached + frozen (6-value enum domain, bound from every chip in a virtualized list).
    public class StatCategoryColorConverter : IValueConverter
    {
        private static readonly Dictionary<StatCategory, SolidColorBrush> brushes = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var category = (StatCategory)value;
            if (brushes.TryGetValue(category, out var cached)) return cached;

            var color = category switch
            {
                StatCategory.Combat => Color.FromRgb(0x6E, 0x3B, 0x3B),
                StatCategory.Work => Color.FromRgb(0x3B, 0x56, 0x6E),
                StatCategory.ElementBoost => Color.FromRgb(0x6E, 0x54, 0x3B),
                StatCategory.ElementResist => Color.FromRgb(0x3B, 0x6E, 0x68),
                StatCategory.Status => Color.FromRgb(0x57, 0x3B, 0x6E),
                _ => Color.FromRgb(0x45, 0x4B, 0x52),
            };
            var brush = new SolidColorBrush(color);
            brush.Freeze();
            return brushes[category] = brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class PassiveSkillViewModel
    {
        private static readonly DerivedLocalizableText<PassiveSkill> NameLocalizer = new DerivedLocalizableText<PassiveSkill>(
            (locale, passive) => passive switch
            {
                RandomPassiveSkill => Translator.Localizations[locale][LocalizationCodes.LC_RANDOM_TRAIT],
                _ => passive.LocalizedNames?.GetValueOrElse(locale.ToFormalName(), passive.Name) ?? passive.InternalName
            }
        );

        private static readonly DerivedLocalizableText<PassiveSkill> DescriptionLocalizer = new DerivedLocalizableText<PassiveSkill>(
            (locale, passive) => passive switch
            {
                RandomPassiveSkill => null,
                _ => passive.LocalizedDescriptions?.GetValueOrElse(locale.ToFormalName(), passive.Description)
            }
        );

        private static Dictionary<string, PassiveSkillViewModel> unrecognizedInstances = new();
        private static Dictionary<PassiveSkill, PassiveSkillViewModel> instances;
        private static ILocalizedText randomPassiveLabel;
        public static PassiveSkillViewModel Make(PassiveSkill passive)
        {
            if (passive == null) return null;

            if (instances == null)
            {
                instances = PalDB.LoadEmbedded().StandardPassiveSkills.ToDictionary(
                    t => t,
                    t => new PassiveSkillViewModel(t, NameLocalizer.Bind(t), DescriptionLocalizer.Bind(t))
                );
            }

            if (passive is RandomPassiveSkill)
            {
                randomPassiveLabel ??= NameLocalizer.Bind(passive);

                return new PassiveSkillViewModel(passive, randomPassiveLabel, null);
            }
            else if (passive is UnrecognizedPassiveSkill)
            {
                if (!unrecognizedInstances.TryGetValue(passive.InternalName, out PassiveSkillViewModel value))
                {
                    var name = LocalizationCodes.LC_TRAIT_LABEL_UNRECOGNIZED.Bind(passive.InternalName);
                    value = new PassiveSkillViewModel(passive, name, null);
                    unrecognizedInstances.Add(passive.InternalName, value);
                    allPassives.Add(value);
                }

                return value;
            }
            else
            {
                return instances[passive];
            }
        }

        static PassiveSkillViewModel()
        {
            allPassives = new ObservableCollection<PassiveSkillViewModel>(PalDB.LoadEmbedded().StandardPassiveSkills.Select(Make).OrderBy(p => p.Name.Value));

            All = new ReadOnlyObservableCollection<PassiveSkillViewModel>(allPassives);
        }

        private static ObservableCollection<PassiveSkillViewModel> allPassives;
        public static ReadOnlyObservableCollection<PassiveSkillViewModel> All { get; }

        // for XAML designer view
        public PassiveSkillViewModel() : this(new PassiveSkill("Runner", "runner", 2), new HardCodedText("Runner"), new HardCodedText("Runner description"))
        {
        }

        private int hash;
        private static Random random = new Random();
        private PassiveSkillViewModel(PassiveSkill passive, ILocalizedText name, ILocalizedText description)
        {
            ModelObject = passive;
            Name = name;
            Description = description;

            var effects = passive.Effects ?? new List<PassiveSkillEffect>();
            StatBreakdown = effects
                .Select(e =>
                {
                    var (label, category) = PassiveStatTaxonomy.Describe(e.InternalName);
                    return new PassiveStatViewModel(label, e.EffectStrength, category);
                })
                .ToList();

            // primary matrix columns + element damage — the pal's own-stat values (self-target only).
            // Selection logic lives in PalCalc.Model (PassiveStatSelection) so it's unit-tested.
            AttackValue = PassiveStatSelection.SelfValue(effects, "ShotAttack");
            DefenseValue = PassiveStatSelection.SelfValue(effects, "Defense");
            WorkSpeedValue = PassiveStatSelection.SelfValue(effects, "CraftSpeed");
            MoveSpeedValue = PassiveStatSelection.SelfValue(effects, "MoveSpeed");
            (ElementValue, ElementDisplay) = PassiveStatSelection.ElementDamage(effects);

            if (passive is RandomPassiveSkill) hash = random.Next();
            else hash = passive.GetHashCode();
        }

        public PassiveSkill ModelObject { get; }

        // Full per-stat effect breakdown (all effects), and the primary-column values (nullable = no effect
        // on that stat) for the stat matrix.
        public IReadOnlyList<PassiveStatViewModel> StatBreakdown { get; }
        public float? AttackValue { get; }
        public float? DefenseValue { get; }
        public float? WorkSpeedValue { get; }
        public float? MoveSpeedValue { get; }

        // element damage: sortable numeric value + a "{Element} +N" label for display
        public float? ElementValue { get; }
        public string ElementDisplay { get; }

        // World Tree passives (1.0) use Rank 5, which has no dedicated icon — clamp to the nearest known
        // rank so they still get a badge instead of rendering blank.
        public ImageSource RankIcon => PassiveSkillIcon.Images.TryGetValue(ModelObject.Rank, out var img)
            ? img
            : PassiveSkillIcon.Images[Math.Clamp(ModelObject.Rank, -3, 4)];

        public int Rank => ModelObject.Rank;

        public ILocalizedText Name { get; }

        public ILocalizedText Description { get; }

        public override bool Equals(object obj) => ModelObject is RandomPassiveSkill ? ReferenceEquals(this, obj) : ModelObject.Equals((obj as PassiveSkillViewModel)?.ModelObject);

        public override int GetHashCode() => hash;
    }
}
