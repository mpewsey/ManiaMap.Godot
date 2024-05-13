using GdUnit4;
using Godot;
using MPewsey.ManiaMap;
using MPewsey.ManiaMap.Samples;

namespace MPewsey.ManiaMapGodot.Drawing.Tests
{
    [TestSuite]
    public class TestLayoutTileMapBook
    {
        private const string LayoutTileMapBookScene = "uid://npua7l2tbnn0";

        private static ISceneRunner LoadScene(string name)
        {
            var scene = ResourceLoader.Load<PackedScene>(name);
            Assertions.AssertThat(scene != null).IsTrue();
            return ISceneRunner.Load(scene.ResourcePath, true, true);
        }

        [TestCase]
        public void TestDrawMap()
        {
            var runner = LoadScene(LayoutTileMapBookScene);
            var map = runner.Scene() as LayoutTileMapBook;
            Assertions.AssertThat(map != null).IsTrue();

            var results = BigLayoutSample.Generate(12345);
            Assertions.AssertThat(results.Success).IsTrue();
            var layout = results.GetOutput<Layout>("Layout");
            var layoutState = new LayoutState(layout);

            ManiaMapManager.Initialize(layout, layoutState);
            map.DrawPages();
        }
    }
}
