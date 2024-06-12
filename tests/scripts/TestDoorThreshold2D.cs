using GdUnit4;
using Godot;

namespace MPewsey.ManiaMapGodot.Tests
{
    [TestSuite]
    public class TestDoorThreshold2D
    {
        private const string EmptyScene = "uid://m8fcccufss1";

        private Vector2[] Points { get; } = new Vector2[]
        {
            new Vector2(200, 300),
            new Vector2(150, 300),
            new Vector2(200, 275),
            new Vector2(250, 300),
            new Vector2(200, 325),
            new Vector2(175, 300),
            new Vector2(200, 287.5f),
        };

        private Vector2[] Parameters { get; } = new Vector2[]
        {
            new Vector2(0.5f, 0.5f),
            new Vector2(0, 0.5f),
            new Vector2(0.5f, 0),
            new Vector2(1, 0.5f),
            new Vector2(0.5f, 1),
            new Vector2(0.25f, 0.5f),
            new Vector2(0.5f, 0.25f),
        };

        [TestCase]
        public void TestOutOfBoundsParameterizePosition()
        {
            var runner = SceneRunner.RunScene(EmptyScene);
            var threshold = new DoorThreshold2D() { Size = new Vector2(100, 50), Position = new Vector2(200, 300) };
            runner.Scene().AddChild(threshold);

            var parameter1 = new Vector2(-100, 300);
            var point1 = threshold.ParameterizePosition(parameter1);
            Assertions.AssertThat(point1).IsEqual(new Vector2(0, 0.5f));

            var parameter2 = new Vector2(200, -100);
            var point2 = threshold.ParameterizePosition(parameter2);
            Assertions.AssertThat(point2).IsEqual(new Vector2(0.5f, 0));

            var parameter3 = new Vector2(1000, 300);
            var point3 = threshold.ParameterizePosition(parameter3);
            Assertions.AssertThat(point3).IsEqual(new Vector2(1, 0.5f));

            var parameter4 = new Vector2(200, 1000);
            var point4 = threshold.ParameterizePosition(parameter4);
            Assertions.AssertThat(point4).IsEqual(new Vector2(0.5f, 1));
        }

        [TestCase]
        public void TestParameterizePosition()
        {
            var runner = SceneRunner.RunScene(EmptyScene);
            var threshold = new DoorThreshold2D() { Size = new Vector2(100, 50), Position = new Vector2(200, 300) };
            runner.Scene().AddChild(threshold);

            var points = Points;
            var expected = Parameters;

            for (int i = 0; i < points.Length; i++)
            {
                var point = points[i];
                var parameters = threshold.ParameterizePosition(point);
                var checkPoint = threshold.InterpolatePosition(parameters);
                Assertions.AssertThat(parameters).IsEqual(expected[i]);
                Assertions.AssertThat(checkPoint).IsEqual(point);
            }
        }

        [TestCase]
        public void TestOutOfBoundsInterpolatePosition()
        {
            var runner = SceneRunner.RunScene(EmptyScene);
            var threshold = new DoorThreshold2D() { Size = new Vector2(100, 50), Position = new Vector2(200, 300) };
            runner.Scene().AddChild(threshold);

            var point1 = new Vector2(-100, 0.5f);
            var parameter1 = threshold.InterpolatePosition(point1);
            Assertions.AssertThat(parameter1).IsEqual(new Vector2(150, 300));

            var point2 = new Vector2(0.5f, -100);
            var parameter2 = threshold.InterpolatePosition(point2);
            Assertions.AssertThat(parameter2).IsEqual(new Vector2(200, 275));

            var point3 = new Vector2(100, 0.5f);
            var parameter3 = threshold.InterpolatePosition(point3);
            Assertions.AssertThat(parameter3).IsEqual(new Vector2(250, 300));

            var point4 = new Vector2(0.5f, 100);
            var parameter4 = threshold.InterpolatePosition(point4);
            Assertions.AssertThat(parameter4).IsEqual(new Vector2(200, 325));
        }

        [TestCase]
        public void TestInterpolatePosition()
        {
            var runner = SceneRunner.RunScene(EmptyScene);
            var threshold = new DoorThreshold2D() { Size = new Vector2(100, 50), Position = new Vector2(200, 300) };
            runner.Scene().AddChild(threshold);

            var parameters = Parameters;
            var expected = Points;

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var point = threshold.InterpolatePosition(parameter);
                var checkParameter = threshold.ParameterizePosition(point);
                Assertions.AssertThat(point).IsEqual(expected[i]);
                Assertions.AssertThat(checkParameter).IsEqual(parameter);
            }
        }
    }
}