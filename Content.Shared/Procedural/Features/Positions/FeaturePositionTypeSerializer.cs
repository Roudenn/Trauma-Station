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

        // RangeFeaturePosition validation
        if (VectorSerializerUtility.TryParseArgs(node.Value, 4, out _))
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
        if (VectorSerializerUtility.TryParseArgs(node.Value, 2, out var args))
        {
            var x = float.Parse(args[0], CultureInfo.InvariantCulture);
            var y = float.Parse(args[1], CultureInfo.InvariantCulture);
            return new ConstantFeaturePosition(new Vector2(x, y));
        }

        if (VectorSerializerUtility.TryParseArgs(node.Value, 4, out var argsRange))
        {
            var x1 = float.Parse(argsRange[0], CultureInfo.InvariantCulture);
            var y1 = float.Parse(argsRange[1], CultureInfo.InvariantCulture);
            var x2 = float.Parse(argsRange[2], CultureInfo.InvariantCulture);
            var y2 = float.Parse(argsRange[3], CultureInfo.InvariantCulture);
            return new RangeFeaturePosition(new Vector2(x1, y1), new Vector2(x2, y2));
        }

        return serializationManager.Read<FeaturePosition>(node, context, notNullableOverride: true);
    }
}
