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

    public IEnumerable<ProtoId<ChunkRolePrototype>> VisitSizeSelector(SizeChunkRoleSelector selector, Args args)
    {
        foreach (var VARIABLE in selector.SizeDict)
        {

        }
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
