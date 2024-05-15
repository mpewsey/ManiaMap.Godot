using GdUnit4;
using Godot;
using MPewsey.ManiaMap;
using MPewsey.ManiaMap.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace MPewsey.ManiaMapGodot.Generators.Tests
{
    [TestSuite]
    public class TestGenerationPipeline
    {
        private const string SingleSquareCrossGeneratorScene = "uid://bdo1fd7ghmg2w";
        private const string ArtifactDirectory = "user://tests/generators";

        [Before]
        public void Before()
        {
            var path = ProjectSettings.GlobalizePath(ArtifactDirectory);

            if (Directory.Exists(path))
                Directory.Delete(path, true);

            Directory.CreateDirectory(path);
        }

        private void SaveImages(string filename, Layout layout)
        {
            var directory = ProjectSettings.GlobalizePath(ArtifactDirectory);
            var path = Path.Combine(directory, filename);
            var map = new LayoutMap();
            map.SaveImages(path, layout);
        }

        private static ISceneRunner LoadScene(string name)
        {
            var scene = ResourceLoader.Load<PackedScene>(name);
            Assertions.AssertThat(scene != null).IsTrue();
            return ISceneRunner.Load(scene.ResourcePath, true, true);
        }

        [TestCase]
        public void TestRun()
        {
            var runner = LoadScene(SingleSquareCrossGeneratorScene);
            var pipeline = runner.Scene() as GenerationPipeline;
            Assertions.AssertThat(pipeline != null).IsTrue();
            var results = pipeline.Run();
            Assertions.AssertThat(results.Success).IsTrue();
            var layout = results.GetOutput<Layout>("Layout");
            Assertions.AssertThat(layout != null).IsTrue();
            SaveImages("test_run.png", layout);
        }

        [TestCase]
        public async Task TestRunAsync()
        {
            var runner = LoadScene(SingleSquareCrossGeneratorScene);
            var pipeline = runner.Scene() as GenerationPipeline;
            Assertions.AssertThat(pipeline != null).IsTrue();
            var results = await pipeline.RunAsync();
            Assertions.AssertThat(results.Success).IsTrue();
            var layout = results.GetOutput<Layout>("Layout");
            Assertions.AssertThat(layout != null).IsTrue();
            SaveImages("test_run_async.png", layout);
        }

        [TestCase]
        public async Task TestRunAttemptsAsync()
        {
            var runner = LoadScene(SingleSquareCrossGeneratorScene);
            var pipeline = runner.Scene() as GenerationPipeline;
            Assertions.AssertThat(pipeline != null).IsTrue();
            var randomSeedInput = pipeline.FindChild(nameof(RandomSeedInput));
            Assertions.AssertThat(randomSeedInput != null).IsTrue();
            randomSeedInput.Free();
            var results = await pipeline.RunAttemptsAsync(12345);
            Assertions.AssertThat(results.Success).IsTrue();
            var layout = results.GetOutput<Layout>("Layout");
            Assertions.AssertThat(layout != null).IsTrue();
            SaveImages("test_run_attempts_async.png", layout);
        }
    }
}