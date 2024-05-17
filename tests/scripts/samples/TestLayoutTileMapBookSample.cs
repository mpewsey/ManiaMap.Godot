using GdUnit4;
using Godot;
using MPewsey.ManiaMapGodot.Tests;
using System.Threading.Tasks;

namespace MPewsey.ManiaMapGodot.Samples.Tests
{
    [TestSuite]
    public class TestLayoutTileMapSample
    {
        private const string LayoutTileMapSampleScene = "uid://cvw8glksk76gx";

        [TestCase]
        public async Task TestSampleSceneRuns()
        {
            var runner = SceneRunner.RunScene(LayoutTileMapSampleScene);
            var buttons = runner.Scene().FindChildren("*", nameof(Button));
            Assertions.AssertThat(buttons.Count).IsEqual(1);
            var button = buttons[0] as Button;
            Assertions.AssertThat(button != null).IsTrue();
            button.EmitSignal(BaseButton.SignalName.Pressed);
            Assertions.AssertThat(button.Disabled).IsTrue();
            var timeout = 6000;
            var timer = 0;

            while (button.Disabled && timer < timeout)
            {
                await runner.AwaitMillis(10);
                timer += 10;
            }

            Assertions.AssertThat(timer).IsLess(timeout);
            Assertions.AssertThat(button.Disabled).IsFalse();
        }
    }
}