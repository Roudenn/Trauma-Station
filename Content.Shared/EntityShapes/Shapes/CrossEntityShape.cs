// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Content.Shared.EntityShapes.Shapes;

/// <summary>
/// Represents a simple shape out of one horizontal and one vertical line combined.
/// </summary>
public sealed partial class CrossEntityShape : EntityShape
{
    public override TResult Accept<TArgs, TResult>(IEntityShapeVisitor<TArgs, TResult> visitor, TArgs args)
        => visitor.VisitCrossShape(this, args);
}
