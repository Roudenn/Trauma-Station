// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Content.Shared.EntityShapes.Shapes;

public sealed partial class BoxEntityShape : EntityShape
{
    [DataField]
    public bool Hollow;

    public override TResult Accept<TArgs, TResult>(IEntityShapeVisitor<TArgs, TResult> visitor, TArgs args)
        => visitor.VisitBoxShape(this, args);
}
