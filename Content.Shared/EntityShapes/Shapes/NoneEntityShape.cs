// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Content.Shared.EntityShapes.Shapes;

/// <summary>
/// Shape that references a ProtoId containing some other shape.
/// </summary>
public sealed partial class NoneEntityShape : EntityShape
{
    public override TResult Accept<TArgs, TResult>(IEntityShapeVisitor<TArgs, TResult> visitor, TArgs args)
        => visitor.VisitNoneShape(this, args);
}
