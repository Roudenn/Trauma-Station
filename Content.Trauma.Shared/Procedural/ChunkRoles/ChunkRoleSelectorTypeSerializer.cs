using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Content.Trauma.Shared.Procedural.ChunkRoles;

[TypeSerializer]
public sealed class ChunkRoleSelectorTypeSerializer :
    ITypeReader<ChunkRoleSelector, MappingDataNode>
{
    public ValidationNode Validate(ISerializationManager serializationManager,
        MappingDataNode node,
        IDependencyCollection dependencies,
        ISerializationContext? context = null)
    {
        if (node.Has(NestedChunkRoleSelector.IdFieldName))
            return serializationManager.ValidateNode<NestedChunkRoleSelector>(node, context);

        if (node.Has(SingleChunkRoleSelector.RoleFieldName))
            return serializationManager.ValidateNode<SingleChunkRoleSelector>(node, context);

        return new ErrorNode(node, "Custom validation not supported! Please specify the type manually!");
    }

    public ChunkRoleSelector Read(ISerializationManager serializationManager,
        MappingDataNode node,
        IDependencyCollection dependencies,
        SerializationHookContext hookCtx,
        ISerializationContext? context = null,
        ISerializationManager.InstantiationDelegate<ChunkRoleSelector>? instanceProvider = null)
    {
        if (node.Has(NestedChunkRoleSelector.IdFieldName))
            return serializationManager.Read<NestedChunkRoleSelector>(node, context, notNullableOverride: true);

        if (node.Has(SingleChunkRoleSelector.RoleFieldName))
            return serializationManager.Read<SingleChunkRoleSelector>(node, context, notNullableOverride: true);

        return serializationManager.Read<ChunkRoleSelector>(node, context, notNullableOverride: true);
    }
}
