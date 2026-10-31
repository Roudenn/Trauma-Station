using Content.Trauma.Shared.Procedural.Components;
using Content.Trauma.Shared.Procedural.Effects;
using Content.Trauma.Shared.Procedural.Prototypes;
using Content.Trauma.Shared.Procedural.Systems;

namespace Content.IntegrationTests.Tests._Trauma;

[TestFixture]
public sealed class ProceduralTest : GameTest
{
    [SidedDependency(Side.Server)] private ProceduralSystem _procedural = default!;

    private const string Loader = "ProceduralLoader";

    private const string TestRegionConfig = "TestRegionConfig";

    [TestPrototypes]
    private const string Prototypes = @"
- type: entity
  id: ProceduralLoader
  components:
  - type: ProceduralLoader

- type: entityEffect
  id: TestProceduralPlatingFill
  effects:
  - !type:RegionTileFill
    tile: Plating

- type: entityEffect
  id: TestProceduralCenterWalls
  effects:
  - !type:ChunkFeature
    feature:
      ent: WallSolid
      offset: !type:ChunkSideFeaturePosition
        side: Center

- type: entityEffect
  id: TestProceduralBoundaryWalls0
  effects:
  - !type:ChunkBoundaryWalls
    table:
      id: WallSolid
    enlargement: 0

- type: entityEffect
  id: TestProceduralBoundaryWalls1
  effects:
  - !type:ChunkBoundaryWalls
    table:
      id: WallSolid
    enlargement: 1

- type: proceduralConfig
  id: TestRegionConfig
  chunkGenerator: !type:BoxChunkGenerator
    regionSize: 16,16
    chunkDimensions: 8,8
  regionGenerator: !type:GridRegionGenerator
    regionInterval: 128,128
  regionEffects: TestProceduralPlatingFill
  seed: 65

- type: proceduralConfig
  id: TestChunkConfigCenterWalls
  chunkGenerator: !type:BoxChunkGenerator
    regionSize: 16,16
    chunkDimensions: 8,8
  regionGenerator: !type:GridRegionGenerator
    regionInterval: 128,128
  regionEffects: TestProceduralPlatingFill
  chunkEffects: TestProceduralCenterWalls
  seed: 65

- type: proceduralConfig
  id: TestChunkConfigBoundaryWalls0
  chunkGenerator: !type:BoxChunkGenerator
    regionSize: 16,16
    chunkDimensions: 8,8
  regionGenerator: !type:GridRegionGenerator
    regionInterval: 128,128
  regionEffects: TestProceduralPlatingFill
  chunkEffects: TestProceduralBoundaryWalls0
  seed: 65

- type: proceduralConfig
  id: TestChunkConfigBoundaryWalls1
  chunkGenerator: !type:BoxChunkGenerator
    regionSize: 16,16
    chunkDimensions: 8,8
  regionGenerator: !type:GridRegionGenerator
    regionInterval: 128,128
  regionEffects: TestProceduralPlatingFill
  chunkEffects: TestProceduralBoundaryWalls1
  seed: 65
";

    [Test, TestOf(typeof(RegionTileFill))]
    public async Task RegionTileFill_Test()
    {
        var testMap = await Pair.CreateTestMap();
        await Server.WaitPost(() =>
        {
            _procedural.EnsureProcedural(testMap.Grid, TestRegionConfig);
            SEntMan.SpawnAtPosition(Loader, testMap.GridCoords);
        });

        await Pair.RunTicksSync(10);

        await Server.WaitAssertion(() =>
        {
            Assert.That(SEntMan.TryGetComponent<ProceduralGridComponent>(testMap.Grid.Owner, out var proceduralComp));
            Assert.That(proceduralComp!.Config, Is.EqualTo(SProtoMan.Index<ProceduralConfigPrototype>(TestRegionConfig).Config));
            Assert.That(proceduralComp.Random, Is.Not.Null);
            Assert.That(proceduralComp.Regions, Has.Count.EqualTo(4));
        });
    }
}
