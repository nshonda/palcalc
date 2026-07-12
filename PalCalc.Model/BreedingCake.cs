namespace PalCalc.Model
{
    // The cake placed in the breeding farm. Cakes are mutually exclusive (one per farm) and modify
    // breeding outcomes. Magnitudes datamined from DA_BreedingItemEffectData (Palworld 1.0).
    public enum BreedingCake
    {
        None,
        MushroomCake,         // Cake02: +1..5 stat (IV) bonus
        VegetableCake,        // Cake03: 2 eggs per breeding cycle
        DeluxeVegetableCake,  // Cake04: +1..5 stat bonus and +2% mutation chance
        SpecialCake,          // Cake05: forces inheriting the max (4) passives from the parent pool
    }
}
