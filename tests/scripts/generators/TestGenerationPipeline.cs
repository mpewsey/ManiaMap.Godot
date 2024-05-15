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

        private static void SaveImages(string filename, Layout layout)
        {
            var directory = ProjectSettings.GlobalizePath(ArtifactDirectory);
            var path = Path.Combine(directory, filename);
            var map = new LayoutMap();
            map.SaveImages(path, layout);
        }

        private static void SetRandomSeed(GenerationPipeline pipeline, int seed)
        {
            var randomSeed = pipeline.FindChild(nameof(RandomSeedInput)) as RandomSeedInput;
            Assertions.AssertThat(randomSeed != null).IsTrue();
            randomSeed.RandomSeed = seed;
        }

        private static ISceneRunner LoadScene(string name)
        {
            var scene = ResourceLoader.Load<PackedScene>(name);
            Assertions.AssertThat(scene != null).IsTrue();
            return ISceneRunner.Load(scene.ResourcePath, true, true);
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
            var runner = LoadScene(SingleSquareCrossGeneratorScene);
            var pipeline = runner.Scene() as GenerationPipeline;
            Assertions.AssertThat(pipeline != null).IsTrue();
            SetRandomSeed(pipeline, 12345);
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
            var runner = LoadScene(SingleSquareCrossGeneratorScene);
            var pipeline = runner.Scene() as GenerationPipeline;
            Assertions.AssertThat(pipeline != null).IsTrue();
            SetRandomSeed(pipeline, 12345);
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
            var runner = LoadScene(SingleSquareCrossGeneratorScene);
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