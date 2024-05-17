using GdUnit4;
using Godot;
using System.Threading.Tasks;

namespace MPewsey.ManiaMapGodot.Tests
{
    public static class SceneRunner
    {
        public static ISceneRunner RunScene(string path)
        {
            var scene = ResourceLoader.Load<PackedScene>(path);
            Assertions.AssertThat(scene != null).IsTrue();
            return ISceneRunner.Load(scene.ResourcePath, true, true);
        }

        public static async Task AwaitPhysicsFrames(this ISceneRunner runner, int count = 2)
        {
            for (int i = 0; i < count; i++)
            {
                await runner.Scene().ToSignal(runner.Scene().GetTree(), SceneTree.SignalName.PhysicsFrame);
            }
        }
    }
}