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
        if (node.Has(NestedFeature.IdDataFieldTag))
            return serializationManager.ValidateNode<NestedFeature>(node, context);

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
        if (node.Has(NestedFeature.IdDataFieldTag))
            return serializationManager.Read<NestedFeature>(node, context, notNullableOverride: true);

        if (node.Has(EntFeature.EntDataFieldTag))
            return serializationManager.Read<EntFeature>(node, context, notNullableOverride: true);

        if (node.Has(TileFeature.TileDataFieldTag))
            return serializationManager.Read<TileFeature>(node, context, notNullableOverride: true);

        if (node.Has(DecalFeature.DecalDataFieldTag))
            return serializationManager.Read<DecalFeature>(node, context, notNullableOverride: true);

        return serializationManager.Read<Feature>(node, context, notNullableOverride: true);
    }
}
