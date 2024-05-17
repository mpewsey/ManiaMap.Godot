using GdUnit4;
using Godot;
using MPewsey.ManiaMap;
using MPewsey.ManiaMap.Drawing;
using MPewsey.ManiaMapGodot.Tests;
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

        private static void SaveImages(string filename, Layout layout)
        {
            var directory = ProjectSettings.GlobalizePath(ArtifactDirectory);
            var path = Path.Combine(directory, filename);
            var map = new LayoutMap();
            map.SaveImages(path, layout);
        }

        private static void PrintHeader(string methodName)
        {
            GD.Print("");
            GD.PrintRich($"[color=#00ffff]*** {nameof(TestGenerationPipeline)}.{methodName} ***[/color]");
        }

        [TestCase]
        public void TestRun()
        {
            PrintHeader(nameof(TestRun));
            var runner = SceneRunner.RunScene(SingleSquareCrossGeneratorScene);
            var pipeline = runner.Scene() as GenerationPipeline;
            Assertions.AssertThat(pipeline != null).IsTrue();
            pipeline.SetRandomSeed(12345);
            var results = pipeline.Run(logger: GD.Print);
            Assertions.AssertThat(results.Success).IsTrue();
            var layout = results.GetOutput<Layout>("Layout");
            Assertions.AssertThat(layout != null).IsTrue();
            SaveImages("test_run.png", layout);
        }

        [TestCase]
        public async Task TestRunAsync()
        {
            PrintHeader(nameof(TestRunAsync));
            var runner = SceneRunner.RunScene(SingleSquareCrossGeneratorScene);
            var pipeline = runner.Scene() as GenerationPipeline;
            Assertions.AssertThat(pipeline != null).IsTrue();
            pipeline.SetRandomSeed(12345);
            var results = await pipeline.RunAsync(logger: GD.Print);
            Assertions.AssertThat(results.Success).IsTrue();
            var layout = results.GetOutput<Layout>("Layout");
            Assertions.AssertThat(layout != null).IsTrue();
            SaveImages("test_run_async.png", layout);
        }

        [TestCase]
        public async Task TestRunAttemptsAsync()
        {
            PrintHeader(nameof(TestRunAttemptsAsync));
            var runner = SceneRunner.RunScene(SingleSquareCrossGeneratorScene);
            var pipeline = runner.Scene() as GenerationPipeline;
            Assertions.AssertThat(pipeline != null).IsTrue();
            var randomSeedInput = pipeline.FindChild(nameof(RandomSeedInput));
            Assertions.AssertThat(randomSeedInput != null).IsTrue();
            randomSeedInput.Free();
            var results = await pipeline.RunAttemptsAsync(12345, logger: GD.Print);
            Assertions.AssertThat(results.Success).IsTrue();
            var layout = results.GetOutput<Layout>("Layout");
            Assertions.AssertThat(layout != null).IsTrue();
            SaveImages("test_run_attempts_async.png", layout);
        }
    }
}