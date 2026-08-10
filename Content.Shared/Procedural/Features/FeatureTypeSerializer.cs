using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Content.Shared.Procedural.Features;

[TypeSerializer]
public sealed class FeatureTypeSerializer :
    ITypeReader<Feature, MappingDataNode>
{
    public ValidationNode Validate(ISerializationManager serializationManager,
        MappingDataNode node,
        IDependencyCollection dependencies,
        ISerializationContext? context = null)
    {
        if (node.Has(EntFeature.EntDataFieldTag))
            return serializationManager.ValidateNode<EntFeature>(node, context);

        if (node.Has(TileFeature.TileDataFieldTag))
            return serializationManager.ValidateNode<TileFeature>(node, context);

        if (node.Has(DecalFeature.DecalDataFieldTag))
            return serializationManager.ValidateNode<DecalFeature>(node, context);

        return new ErrorNode(node, "Custom validation not supported! Please specify the type manually!");
    }

    public Feature Read(ISerializationManager serializationManager,
        MappingDataNode node,
        IDependencyCollection dependencies,
        SerializationHookContext hookCtx,
        ISerializationContext? context = null,
        ISerializationManager.InstantiationDelegate<Feature>? instanceProvider = null)
    {
        var type = typeof(Feature);

        if (node.Has(EntFeature.EntDataFieldTag))
            type = typeof(EntFeature);

        if (node.Has(TileFeature.TileDataFieldTag))
            type = typeof(TileFeature);

        if (node.Has(DecalFeature.DecalDataFieldTag))
            type = typeof(DecalFeature);

        return (Feature) serializationManager.Read(type, node, context)!;
    }
}
