using GdUnit4;
using Godot;
using MPewsey.ManiaMap;
using MPewsey.ManiaMapGodot.Generators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MPewsey.ManiaMapGodot.Tests
{
    [TestSuite]
    public class TestRoomNode2D
    {
        private const string TestCellAreasScene = "uid://i1ywx2t50wxg";
        private const string RoomTemplateDatabase = "uid://cpbx1jxf4xwvd";
        private const string ArtifactDirectory = "user://tests/room2d/";

        [Before]
        public void Before()
        {
            var path = ProjectSettings.GlobalizePath(ArtifactDirectory);

            if (Directory.Exists(path))
                Directory.Delete(path, true);

            Directory.CreateDirectory(path);
        }

        [TestCase]
        public void TestAutoAssign()
        {
            var room = new RoomNode2D() { Rows = 3, Columns = 2, CellSize = new Vector2(100, 100) };
            var feature = new Feature2D() { Position = new Vector2(150, 250) };
            room.AddChild(feature);
            Assertions.AssertThat(room.AutoAssign()).IsTrue();
            Assertions.AssertThat(feature.Room == room).IsTrue();
            Assertions.AssertThat(feature.Row).IsEqual(2);
            Assertions.AssertThat(feature.Column).IsEqual(1);
            room.QueueFree();
        }

        [TestCase]
        public void TestUpdateRoomTemplateResourceReturnsFalseIfNotSavedToFile()
        {
            var room = new RoomNode2D() { Rows = 3, Columns = 2, CellSize = new Vector2(100, 100) };
            var feature = new Feature2D() { Position = new Vector2(150, 250) };
            room.AddChild(feature);
            Assertions.AssertThat(room.UpdateRoomTemplateResource()).IsFalse();
            room.QueueFree();
        }

        [TestCase]
        public void TestUpdateRoomTemplateResource()
        {
            var path = ArtifactDirectory + "test_room.tscn";
            var templatePath = ArtifactDirectory + "test_room.room_template.tres";
            var packedScene = new PackedScene();
            var room = new RoomNode2D() { Rows = 3, Columns = 2, CellSize = new Vector2(100, 100) };
            var feature = new Feature2D() { Position = new Vector2(150, 250) };
            var door = new DoorNode2D();

            room.AddChild(feature);
            room.AddChild(door);
            feature.Owner = room;
            door.Owner = room;

            packedScene.Pack(room);
            ResourceSaver.Save(packedScene, path);
            room.QueueFree();

            var scene = ResourceLoader.Load<PackedScene>(path);
            var savedRoom = scene.Instantiate<RoomNode2D>();
            Assertions.AssertThat(savedRoom.UpdateRoomTemplateResource()).IsTrue();
            var template = ResourceLoader.Load<RoomTemplateResource>(templatePath);
            Assertions.AssertThat(savedRoom.RoomTemplate == template).IsTrue();
            Assertions.AssertThat(ResourceLoader.Exists(templatePath)).IsTrue();

            var savedFeature = savedRoom.GetChild(0) as Feature2D;
            Assertions.AssertThat(savedFeature != null).IsTrue();
            Assertions.AssertThat(savedFeature.Room == savedRoom).IsTrue();
            Assertions.AssertThat(savedFeature.Row).IsEqual(2);
            Assertions.AssertThat(savedFeature.Column).IsEqual(1);
            savedRoom.QueueFree();
        }

        [TestCase]
        public void TestValidateRoomFlagsThrowsException()
        {
            var room = new RoomNode2D();
            var flag1 = new RoomFlag2D() { Id = 1 };
            var flag2 = new RoomFlag2D() { Id = 1 };
            room.AddChild(flag1);
            room.AddChild(flag2);
            Assertions.AssertThrown(() => room.ValidateRoomFlags()).IsInstanceOf<Exception>();
            room.QueueFree();
        }

        [TestCase]
        public void TestValidateRoomFlags()
        {
            var room = new RoomNode2D();
            var flag1 = new RoomFlag2D() { Id = 1 };
            var flag2 = new RoomFlag2D() { Id = 2 };
            room.AddChild(flag1);
            room.AddChild(flag2);
            Assertions.AssertThat(room.FindChildren("*", nameof(RoomFlag2D), true, false).Count).IsEqual(2);
            room.ValidateRoomFlags();
            room.QueueFree();
        }

        [TestCase]
        public void TestCellIndexExists()
        {
            var room = new RoomNode2D() { Rows = 2, Columns = 3 };
            Assertions.AssertThat(room.CellIndexExists(0, 0)).IsTrue();
            Assertions.AssertThat(room.CellIndexExists(1, 0)).IsTrue();
            Assertions.AssertThat(room.CellIndexExists(0, 2)).IsTrue();
            Assertions.AssertThat(room.CellIndexExists(-1, 0)).IsFalse();
            Assertions.AssertThat(room.CellIndexExists(0, -1)).IsFalse();
            Assertions.AssertThat(room.CellIndexExists(2, 0)).IsFalse();
            Assertions.AssertThat(room.CellIndexExists(0, 3)).IsFalse();
            room.QueueFree();
        }

        [TestCase]
        public async Task TestOnAreaEnteredCellArea()
        {
            uint cellCollisionMask = 1;
            var indexes = new HashSet<Vector2>();

            // Create a layout and initialize the ManiaMap manager.
            var runner = SceneRunner.RunScene(TestCellAreasScene);
            var root = runner.Scene();
            var settings = new ManiaMapSettings() { CellCollisionMask = cellCollisionMask };
            var pipeline = root.FindChild(nameof(GenerationPipeline)) as GenerationPipeline;
            Assertions.AssertThat(pipeline != null).IsTrue();
            var results = pipeline.Run(logger: GD.Print);
            Assertions.AssertThat(results.Success).IsTrue();
            var layout = results.GetOutput<Layout>("Layout");
            var state = new LayoutState(layout);
            var roomId = layout.Rooms.Keys.First();
            var roomState = state.RoomStates[roomId];
            ManiaMapManager.Initialize(layout, state, settings);

            // Create room and hook up signals to detect the cells the test area is touching.
            var database = ResourceLoader.Load<RoomTemplateDatabase>(RoomTemplateDatabase);
            var room = database.CreateRoom2DInstance(roomId, root);
            var cellSize = room.CellSize;
            room.OnCellAreaEntered += (area, collision) => indexes.Add(new Vector2(area.Row, area.Column));
            room.OnCellAreaExited += (area, collision) => indexes.Remove(new Vector2(area.Row, area.Column));

            // Move the area around and test the detected areas.
            var area = root.FindChild(nameof(Area2D)) as Area2D;
            Assertions.AssertThat(area != null).IsTrue();
            Assertions.AssertThat(area.CollisionLayer).IsEqual(cellCollisionMask);
            area.MoveToFront();

            // Area should start out with no collisions
            await runner.AwaitPhysicsFrames();
            Assertions.AssertThat(indexes.Count).IsEqual(0);
            Assertions.AssertThat(roomState.CellIsVisible(0, 0)).IsFalse();

            // Move area inside (0, 0) cell.
            area.GlobalPosition = new Vector2(10, 10);
            await runner.AwaitPhysicsFrames();
            Assertions.AssertThat(indexes.Count).IsEqual(1);
            Assertions.AssertThat(indexes.Contains(new Vector2(0, 0))).IsTrue();
            Assertions.AssertThat(roomState.CellIsVisible(0, 0)).IsTrue();

            // Move area to border of (0, 0) and (0, 1) cell.
            area.GlobalPosition = new Vector2(cellSize.X, 0);
            await runner.AwaitPhysicsFrames();
            Assertions.AssertThat(indexes.Count).IsEqual(2);
            Assertions.AssertThat(indexes.Contains(new Vector2(0, 0))).IsTrue();
            Assertions.AssertThat(indexes.Contains(new Vector2(0, 1))).IsTrue();
            Assertions.AssertThat(roomState.CellIsVisible(0, 0)).IsTrue();
            Assertions.AssertThat(roomState.CellIsVisible(0, 1)).IsTrue();

            // Move area outside of all cells
            area.GlobalPosition = new Vector2(-100, -100);
            await runner.AwaitPhysicsFrames();
            Assertions.AssertThat(indexes.Count).IsEqual(0);
        }
    }
}