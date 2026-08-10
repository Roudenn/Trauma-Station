// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Content.Shared.EntityShapes.Shapes;

/// <summary>
/// Returns a singe tile at the specified position.
/// </summary>
public sealed partial class SingleEntityShape : EntityShape
{
    public override TResult Accept<TArgs, TResult>(IEntityShapeVisitor<TArgs, TResult> visitor, TArgs args)
        => visitor.VisitSingleShape(this, args);
}
