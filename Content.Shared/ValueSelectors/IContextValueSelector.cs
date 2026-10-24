using System.Numerics;
using Robust.Shared.Random;

namespace Content.Shared.ValueSelectors;

public partial interface IContextValueSelector<out TMain, out TFrac, in TContext> : IBaseValueSelector<TMain, TFrac>
    where TMain : INumber<TMain>, IParsable<TMain>
    where TFrac : INumber<TFrac>, IParsable<TFrac>
    where TContext : IValueSelectorContext
{
    /// <summary>
    /// Returns a value for this selector given a randomizer.
    /// </summary>
    TMain Get(TContext context, IRobustRandom rand);
}
