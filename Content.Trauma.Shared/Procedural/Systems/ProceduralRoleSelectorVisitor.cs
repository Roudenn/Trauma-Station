using Content.Shared.Random.Helpers;
using Content.Trauma.Shared.Procedural.ChunkRoles;
using Content.Trauma.Shared.Procedural.Regions;
using Robust.Shared.Random;

namespace Content.Trauma.Shared.Procedural.Systems;

public sealed class ProceduralRoleSelectorVisitor : IChunkRoleSelectorVisitor<ProceduralRoleSelectorVisitor.Args>
{
    public readonly record struct Args(Chunk Chunk, IRobustRandom Random, IPrototypeManager ProtoMan);

    private readonly Dictionary<ChunkRoleSelector, float> _groupCache = new();

    private readonly List<ProtoId<ChunkRolePrototype>> _allCache = new();

    private Vector2i[] _sizeCache = [];

    public IEnumerable<ProtoId<ChunkRolePrototype>> VisitSizeSelector(SizeChunkRoleSelector selector, Args args)
    {
        var size = args.Chunk.Bounds.Size;

        Extensions.EnsureLength(ref _sizeCache, selector.SizeDict.Count);
        selector.SizeDict.Keys.CopyTo(_sizeCache, 0);

        _sizeCache.Sort((a, b) => a.LengthSquared.CompareTo(b.LengthSquared));

        var span = _sizeCache.AsSpan(0, selector.SizeDict.Count);

        var foundSelector = selector.SizeDict[BinarySearchClosest(span, size)];
        return foundSelector.Accept(this, args);
    }

    private static Vector2i BinarySearchClosest(ReadOnlySpan<Vector2i> span, Vector2i size)
    {
        if (span.IsEmpty)
            return Vector2i.Zero;
        if (span.Length == 1)
            return span[0];

        var low = 0;
        var high = span.Length - 1;
        var targetLenSq = size.LengthSquared;

        while (low <= high)
        {
            var mid = low + (high - low) / 2;
            var midLenSq = span[mid].LengthSquared;

            if (Math.Abs(midLenSq - targetLenSq) < 0.01f)
                return span[mid];

            if (midLenSq < targetLenSq)
                low = mid + 1;
            else
                high = mid - 1;
        }

        if (low >= span.Length)
            return span[^1];
        if (high < 0)
            return span[0];

        var diffLow = Math.Abs(span[low].LengthSquared - targetLenSq);
        var diffHigh = Math.Abs(span[high].LengthSquared - targetLenSq);

        return diffLow < diffHigh ? span[low] : span[high];
    }

    public IEnumerable<ProtoId<ChunkRolePrototype>> VisitAllSelector(AllChunkRoleSelector selector, Args args)
    {
        _allCache.Clear();
        foreach (var child in selector.Children)
        {
            _allCache.AddRange(child.Accept(this, args));
        }

        return _allCache;
    }

    public IEnumerable<ProtoId<ChunkRolePrototype>> VisitGroupSelector(GroupChunkRoleSelector selector, Args args)
    {
        _groupCache.Clear();
        foreach (var child in selector.Children)
        {
            _groupCache.Add(child, child.Weight);
        }

        return args.Random.Pick(_groupCache).Accept(this, args);
    }

    public IEnumerable<ProtoId<ChunkRolePrototype>> VisitNestedSelector(NestedChunkRoleSelector selector, Args args)
    {
        return args.ProtoMan.Index(selector.Id).Selector.Accept(this, args);
    }

    public IEnumerable<ProtoId<ChunkRolePrototype>> VisitSingleSelector(SingleChunkRoleSelector selector, Args args)
    {
        yield return selector.Role;
    }

    public IEnumerable<ProtoId<ChunkRolePrototype>> VisitNoneSelector(NoneChunkRoleSelector selector, Args args)
    {
        yield break;
    }
}
