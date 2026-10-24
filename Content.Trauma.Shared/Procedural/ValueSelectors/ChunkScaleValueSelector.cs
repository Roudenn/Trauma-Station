using Content.Shared.ValueSelectors;
using Content.Shared.ValueSelectors.Numbers;
using Content.Trauma.Shared.Procedural.Regions;
using Robust.Shared.Random;

namespace Content.Trauma.Shared.Procedural.ValueSelectors;

public sealed partial class ChunkScaleValueSelector : NumberSelector, IContextValueSelector<int, float, ChunkValueContext>
{
    [DataField(required: true)]
    public NumberSelector Selector;

    [DataField]
    public float Scale = 1f;

    public override int Get(IRobustRandom rand)
    {
        return Selector.Get(rand);
    }

    public override float Odds()
    {
        return Selector.Odds();
    }

    public override float Average()
    {
        return Selector.Average();
    }

    public int Get(ChunkValueContext context, IRobustRandom rand)
    {
        return (int) MathF.Round(Selector.Get(rand) * context.Chunk.Bounds.Width * context.Chunk.Bounds.Height * Scale);
    }
}

public record struct ChunkValueContext(Chunk Chunk) : IValueSelectorContext;
