using GdUnit4;
using MPewsey.ManiaMap;
using MPewsey.ManiaMap.Samples;
using MPewsey.ManiaMapGodot.Tests;

namespace MPewsey.ManiaMapGodot.Drawing.Tests
{
    [TestSuite]
    public class TestLayoutTileMap
    {
        private const string LayoutTileMapScene = "uid://bw26nmbfsor1o";

        [TestCase]
        public void TestDrawMap()
        {
            var runner = SceneRunner.RunScene(LayoutTileMapScene);
            var map = runner.Scene() as LayoutTileMap;
            Assertions.AssertThat(map != null).IsTrue();

            var results = BigLayoutSample.Generate(12345);
            Assertions.AssertThat(results.Success).IsTrue();
            var layout = results.GetOutput<Layout>("Layout");
            var layoutState = new LayoutState(layout);

            var layoutPack = new LayoutPack(layout, layoutState);
            map.DrawMap(layoutPack);
        }
    }
}
