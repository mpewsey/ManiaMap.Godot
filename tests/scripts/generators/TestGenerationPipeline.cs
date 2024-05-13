using GdUnit4;
using Godot;
using MPewsey.ManiaMap;
using System.Threading.Tasks;

namespace MPewsey.ManiaMapGodot.Generators.Tests
{
    [TestSuite]
    public class TestGenerationPipeline
    {
        private const string BigLayoutScene = "";

        private static ISceneRunner LoadScene(string name)
        {
            var scene = ResourceLoader.Load<PackedScene>(name);
            Assertions.AssertThat(scene != null).IsTrue();
            return ISceneRunner.Load(scene.ResourcePath, true, true);
        }

        [TestCase]
        public void TestRun()
        {
            var runner = LoadScene(BigLayoutScene);
            var pipeline = runner.Scene() as GenerationPipeline;
            Assertions.AssertThat(pipeline != null).IsTrue();
            var results = pipeline.Run();
            Assertions.AssertThat(results.Success).IsTrue();
            var layout = results.GetOutput<Layout>("Layout");
            Assertions.AssertThat(layout != null).IsTrue();
        }

        [TestCase]
        public async Task TestRunAsync()
        {
            var runner = LoadScene(BigLayoutScene);
            var pipeline = runner.Scene() as GenerationPipeline;
            Assertions.AssertThat(pipeline != null).IsTrue();
            var results = await pipeline.RunAsync();
            Assertions.AssertThat(results.Success).IsTrue();
            var layout = results.GetOutput<Layout>("Layout");
            Assertions.AssertThat(layout != null).IsTrue();
        }

        [TestCase]
        public async Task TestRunAttemptsAsync()
        {
            var runner = LoadScene(BigLayoutScene);
            var pipeline = runner.Scene() as GenerationPipeline;
            Assertions.AssertThat(pipeline != null).IsTrue();
            var randomSeedInput = pipeline.FindChild(nameof(RandomSeedInput));
            Assertions.AssertThat(randomSeedInput != null).IsTrue();
            randomSeedInput.Free();
            var results = await pipeline.RunAttemptsAsync(12345);
            Assertions.AssertThat(results.Success).IsTrue();
            var layout = results.GetOutput<Layout>("Layout");
            Assertions.AssertThat(layout != null).IsTrue();
        }
    }
}