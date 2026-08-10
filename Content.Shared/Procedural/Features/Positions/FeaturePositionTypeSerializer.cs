using System.Globalization;
using System.Numerics;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using Robust.Shared.Utility;

namespace Content.Shared.Procedural.Features.Positions;

[TypeSerializer]
public sealed class FeaturePositionTypeSerializer :
    ITypeReader<FeaturePosition, ValueDataNode>
{
    public ValidationNode Validate(ISerializationManager serializationManager,
        ValueDataNode node,
        IDependencyCollection dependencies,
        ISerializationContext? context = null)
    {
        // ConstantFeaturePosition validation
        if (VectorSerializerUtility.TryParseArgs(node.Value, 2, out _))
            return new ValidatedValueNode(node);

        return new ErrorNode(node, "Custom validation not supported! Please specify the type manually!");
    }

    public FeaturePosition Read(ISerializationManager serializationManager,
        ValueDataNode node,
        IDependencyCollection dependencies,
        SerializationHookContext hookCtx,
        ISerializationContext? context = null,
        ISerializationManager.InstantiationDelegate<FeaturePosition>? instanceProvider = null)
    {
        var type = typeof(FeaturePosition);

        if (VectorSerializerUtility.TryParseArgs(node.Value, 2, out var args))
        {
            var x = float.Parse(args[0], CultureInfo.InvariantCulture);
            var y = float.Parse(args[1], CultureInfo.InvariantCulture);
            return new ConstantFeaturePosition(new Vector2(x, y));
        }

        return (FeaturePosition) serializationManager.Read(type, node, context)!;
    }
}
