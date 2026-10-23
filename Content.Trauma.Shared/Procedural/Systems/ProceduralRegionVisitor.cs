using Content.Trauma.Shared.Procedural.Regions;
using Robust.Shared.Random;

namespace Content.Trauma.Shared.Procedural.Systems;

public sealed class ProceduralRegionVisitor : IChunkGeneratorVisitor<ProceduralRegionVisitor.Args>
{
    public readonly record struct Args(IRobustRandom Random, Vector2i GridIndices);

    private readonly List<Box2i> _bspCache = new();
    private readonly Stack<Box2i> _bspPendingBoxes = new();

    Region IChunkGeneratorVisitor<Args>.VisitBSPGenerator(BSPChunkGenerator generator, Args args)
    {
        _bspCache.Clear();
        _bspPendingBoxes.Clear();

        SplitBox(
            _bspCache,
            _bspPendingBoxes,
            (Box2i) Box2.CenteredAround(Vector2.Zero, generator.AreaSize).Rounded(0),
            new Vector2i(generator.MinRoomWidth.Get(args.Random), generator.MinRoomHeight.Get(args.Random)),
            generator.Variation.Get(args.Random),
            generator.CutProb.Get(args.Random),
            args.Random);

        var chunks = new Chunk[_bspCache.Count];
        for (int i = 0; i < _bspCache.Count; i++)
        {
            var box = _bspCache[i];
            chunks[i] = new Chunk();
        }

        return new Region(chunks, args.GridIndices);
    }

    /// <summary>
    /// Iteratively splits a <see cref="Box2i"/> into pieces separated by 1-tile thick walls until it comes close to the minSize dimensions.
    /// </summary>
    /// <param name="list">A resulting list that stores all successfully split boxes.</param>
    /// <param name="pendingBoxes">A stack cache for boxes.</param>
    /// <param name="startBox">The original box to split.</param>
    /// <param name="minSize">Minimal size of a room</param>
    /// <param name="variation">
    /// The percentage amount by which to compare the aspect ratio of a splitting box to force a direction of a split.
    /// Higher value forces the rooms to look more stretched.
    /// </param>
    /// <param name="cutProb">
    /// The probability to skip the last cut before making a box that is smaller than the minimal size.
    /// Higher values means more bigger rooms surrounded by smaller ones.
    /// </param>
    /// <param name="random">The random to use.</param>
    /// <returns>All boxes that were split from the original box.</returns>
    private static void SplitBox(
        List<Box2i> list,
        Stack<Box2i> pendingBoxes,
        Box2i startBox,
        Vector2i minSize,
        float variation,
        float cutProb,
        IRobustRandom random)
    {
        DebugTools.Assert(startBox.IsValid());

        pendingBoxes.Push(startBox);

        while (pendingBoxes.Count > 0)
        {
            var box = pendingBoxes.Pop();
            bool isHorizontalSplit;

            if ((float) box.Width / box.Height > 1f + variation)
                isHorizontalSplit = false;
            else if ((float) box.Height / box.Width > 1f + variation)
                isHorizontalSplit = true;
            else
                isHorizontalSplit = random.Prob(0.5f);

            Box2i box1, box2;

            if (isHorizontalSplit)
            {
                var forbiddenBoxHeight = box.Height / 2;

                // This height is excluded from both boxes and gets replaced by a wall
                var cutHeight = random.Next(box.Bottom + forbiddenBoxHeight, box.Top - forbiddenBoxHeight);

                box1 = new Box2i(box.BottomLeft, new Vector2i(box.Right, cutHeight - 1));
                box2 = new Box2i(new Vector2i(box.Left, cutHeight), box.TopRight);
            }
            else
            {
                var forbiddenBoxWidth = box.Width / 2;

                // This width is excluded from both boxes and gets replaced by a wall
                var cutWidth = random.Next(box.Left + forbiddenBoxWidth, box.Right - forbiddenBoxWidth);

                box1 = new Box2i(box.BottomLeft, new Vector2i(cutWidth - 1, box.Top));
                box2 = new Box2i(new Vector2i(cutWidth, box.Bottom), box.TopRight);
            }

            var isBox1Small = box1.Width < minSize.X || box1.Height < minSize.Y;
            var isBox2Small = box2.Width < minSize.X || box2.Height < minSize.Y;

            if ((isBox1Small || isBox2Small) && random.Prob(cutProb))
            {
                list.Add(box);
                continue;
            }

            if (isBox1Small)
                list.Add(box1);
            else
                pendingBoxes.Push(box1);

            if (isBox2Small)
                list.Add(box2);
            else
                pendingBoxes.Push(box2);
        }
    }
}
