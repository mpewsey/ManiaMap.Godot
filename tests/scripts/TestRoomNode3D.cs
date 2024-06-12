using GdUnit4;
using Godot;
using MPewsey.ManiaMap;
using MPewsey.ManiaMap.Exceptions;
using MPewsey.ManiaMapGodot.Generators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MPewsey.ManiaMapGodot.Tests
{
    [TestSuite]
    public class TestRoomNode3D
    {
        private const string EmptyScene = "uid://m8fcccufss1";
        private const string TestCellAreasScene = "uid://duaufvulkyino";
        private const string TestRoomScene = "uid://chi7ur84wslyu";
        private const string RoomTemplateDatabase = "uid://l38x064m8j7q";
        private const string ArtifactDirectory = "user://tests/room3d/";

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
            var runner = SceneRunner.RunScene(EmptyScene);
            var root = runner.Scene();
            var room = new RoomNode3D() { Rows = 3, Columns = 2, CellSize = new Vector3(100, 100, 100) };
            var feature = new Feature3D() { Position = new Vector3(150, 0, 250) };
            room.AddChild(feature);
            root.AddChild(room);

            room.AutoAssign();
            Assertions.AssertThat(feature.Room == room).IsTrue();
            Assertions.AssertThat(feature.Row).IsEqual(2);
            Assertions.AssertThat(feature.Column).IsEqual(1);
        }

        [TestCase]
        public void TestUpdateRoomTemplateResourceReturnsFalseIfNotSavedToFile()
        {
            var room = new RoomNode3D() { Rows = 3, Columns = 2, CellSize = new Vector3(100, 100, 100) };
            var feature = new Feature3D() { Position = new Vector3(150, 0, 250) };
            room.AddChild(feature);
            Assertions.AssertThat(room.UpdateRoomTemplate()).IsFalse();
            room.QueueFree();
        }

        [TestCase]
        public void TestUpdateRoomTemplateResource()
        {
            // Create and save a scene.
            var path = ArtifactDirectory + "test_room.tscn";
            var templatePath = ArtifactDirectory + "test_room.room_template.tres";
            var packedScene = new PackedScene();
            var room = new RoomNode3D() { Rows = 3, Columns = 2, CellSize = new Vector3(100, 100, 100) };
            var feature = new Feature3D() { Position = new Vector3(150, 0, 250) };
            var door = new DoorNode3D();

            room.AddChild(feature);
            room.AddChild(door);
            feature.Owner = room;
            door.Owner = room;

            packedScene.Pack(room);
            ResourceSaver.Save(packedScene, path);
            room.QueueFree();

            // Open and check the saved scenes.
            var runner = SceneRunner.RunScene(EmptyScene);
            var root = runner.Scene();
            var scene = ResourceLoader.Load<PackedScene>(path);
            var savedRoom = scene.Instantiate<RoomNode3D>();
            root.AddChild(savedRoom);

            Assertions.AssertThat(savedRoom.UpdateRoomTemplate()).IsTrue();
            var template = ResourceLoader.Load<RoomTemplateResource>(templatePath);
            Assertions.AssertThat(savedRoom.RoomTemplate == template).IsTrue();
            Assertions.AssertThat(ResourceLoader.Exists(templatePath)).IsTrue();

            var savedFeature = savedRoom.GetChild(0) as Feature3D;
            Assertions.AssertThat(savedFeature != null).IsTrue();
            Assertions.AssertThat(savedFeature.Room == savedRoom).IsTrue();
            Assertions.AssertThat(savedFeature.Row).IsEqual(2);
            Assertions.AssertThat(savedFeature.Column).IsEqual(1);
            savedRoom.QueueFree();
        }

        [TestCase]
        public void TestValidateRoomFlagsThrowsException()
        {
            var room = new RoomNode3D();
            var flag1 = new RoomFlag3D() { Id = 1 };
            var flag2 = new RoomFlag3D() { Id = 1 };
            room.AddChild(flag1);
            room.AddChild(flag2);
            Assertions.AssertThrown(() => room.ValidateRoomFlags()).IsInstanceOf<DuplicateIdException>();
            room.QueueFree();
        }

        [TestCase]
        public void TestValidateRoomFlags()
        {
            var room = new RoomNode3D();
            var flag1 = new RoomFlag3D() { Id = 1 };
            var flag2 = new RoomFlag3D() { Id = 2 };
            room.AddChild(flag1);
            room.AddChild(flag2);
            Assertions.AssertThat(room.FindChildren("*", nameof(RoomFlag3D), true, false).Count).IsEqual(2);
            room.ValidateRoomFlags();
            room.QueueFree();
        }

        [TestCase]
        public void TestCellIndexExists()
        {
            var room = new RoomNode3D() { Rows = 2, Columns = 3 };
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
        public void TestRoomFlag()
        {
            var database = ResourceLoader.Load<RoomTemplateDatabase>(RoomTemplateDatabase);
            var runner = SceneRunner.RunScene(TestRoomScene);
            var root = runner.Scene();
            var pipeline = root.FindChild(nameof(GenerationPipeline)) as GenerationPipeline;
            Assertions.AssertThat(pipeline != null).IsTrue();
            var results = pipeline.Run(logger: GD.Print);
            Assertions.AssertThat(results.Success).IsTrue();
            var layout = results.GetOutput<Layout>("Layout");
            var state = new LayoutState(layout);
            var roomId = layout.Rooms.Keys.First();
            var roomState = state.RoomStates[roomId];
            var layoutPack = new LayoutPack(layout, state, new ManiaMapSettings());

            var room = database.CreateRoom3DInstance(roomId, layoutPack, root);
            var roomFlags = room.FindChildren("*", nameof(RoomFlag3D), true, false);
            Assertions.AssertThat(roomFlags.Count).IsGreater(0);
            var roomFlag = roomFlags[0] as RoomFlag3D;
            Assertions.AssertThat(roomFlag != null).IsTrue();

            Assertions.AssertThat(roomFlag.FlagIsSet()).IsFalse();
            Assertions.AssertThat(roomState.Flags.Contains(roomFlag.Id)).IsFalse();

            Assertions.AssertThat(roomFlag.SetFlag()).IsTrue();
            Assertions.AssertThat(roomState.Flags.Contains(roomFlag.Id)).IsTrue();
            Assertions.AssertThat(roomFlag.FlagIsSet()).IsTrue();

            Assertions.AssertThat(roomFlag.RemoveFlag()).IsTrue();
            Assertions.AssertThat(roomState.Flags.Contains(roomFlag.Id)).IsFalse();
            Assertions.AssertThat(roomFlag.FlagIsSet()).IsFalse();
            Assertions.AssertThat(roomFlag.RemoveFlag()).IsFalse();

            Assertions.AssertThat(roomFlag.ToggleFlag()).IsTrue();
            Assertions.AssertThat(roomState.Flags.Contains(roomFlag.Id)).IsTrue();
            Assertions.AssertThat(roomFlag.FlagIsSet()).IsTrue();

            Assertions.AssertThat(roomFlag.ToggleFlag()).IsFalse();
            Assertions.AssertThat(roomState.Flags.Contains(roomFlag.Id)).IsFalse();
            Assertions.AssertThat(roomFlag.FlagIsSet()).IsFalse();
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
            var layoutPack = new LayoutPack(layout, state, settings);

            // Create room and hook up signals to detect the cells the test area is touching.
            var database = ResourceLoader.Load<RoomTemplateDatabase>(RoomTemplateDatabase);
            var room = database.CreateRoom3DInstance(roomId, layoutPack, root);
            var cellSize = room.CellSize;
            room.OnCellAreaEntered += (area, collision) => indexes.Add(new Vector2(area.Row, area.Column));
            room.OnCellAreaExited += (area, collision) => indexes.Remove(new Vector2(area.Row, area.Column));

            // Move the area around and test the detected areas.
            var area = root.FindChild(nameof(Area3D)) as Area3D;
            Assertions.AssertThat(area != null).IsTrue();
            Assertions.AssertThat(area.CollisionLayer).IsEqual(cellCollisionMask);

            // Area should start out with no collisions
            await runner.AwaitPhysicsFrames();
            Assertions.AssertThat(indexes.Count).IsEqual(0);
            Assertions.AssertThat(roomState.CellIsVisible(0, 0)).IsFalse();

            // Move area inside (0, 0) cell.
            area.GlobalPosition = new Vector3(3, 0, 3);
            await runner.AwaitPhysicsFrames();
            Assertions.AssertThat(indexes.Count).IsEqual(1);
            Assertions.AssertThat(indexes.Contains(new Vector2(0, 0))).IsTrue();
            Assertions.AssertThat(roomState.CellIsVisible(0, 0)).IsTrue();

            // Move area to border of (0, 0) and (0, 1) cell.
            area.GlobalPosition = new Vector3(cellSize.X, 0, 0);
            await runner.AwaitPhysicsFrames();
            Assertions.AssertThat(indexes.Count).IsEqual(2);
            Assertions.AssertThat(indexes.Contains(new Vector2(0, 0))).IsTrue();
            Assertions.AssertThat(indexes.Contains(new Vector2(0, 1))).IsTrue();
            Assertions.AssertThat(roomState.CellIsVisible(0, 0)).IsTrue();
            Assertions.AssertThat(roomState.CellIsVisible(0, 1)).IsTrue();

            // Move area to corners of center cells
            for (int i = 1; i < room.Rows; i++)
            {
                for (int j = 1; j < room.Columns; j++)
                {
                    area.GlobalPosition = cellSize * new Vector3(j, 0, i);
                    await runner.AwaitPhysicsFrames();
                    Assertions.AssertThat(indexes.Count).IsEqual(4);
                    Assertions.AssertThat(indexes.Contains(new Vector2(i, j))).IsTrue();
                    Assertions.AssertThat(indexes.Contains(new Vector2(i - 1, j))).IsTrue();
                    Assertions.AssertThat(indexes.Contains(new Vector2(i, j - 1))).IsTrue();
                    Assertions.AssertThat(indexes.Contains(new Vector2(i - 1, j - 1))).IsTrue();
                }
            }

            // Move area outside of all cells
            area.GlobalPosition = new Vector3(-100, -100, -100);
            await runner.AwaitPhysicsFrames();
            Assertions.AssertThat(indexes.Count).IsEqual(0);
        }

        [TestCase]
        public void TestGetCellActivityThrowsOutOfRangeException()
        {
            var room = new RoomNode3D() { Rows = 3, Columns = 3 };
            Assertions.AssertThrown(() => room.GetCellActivity(-1, 1)).IsInstanceOf<IndexOutOfRangeException>();
            Assertions.AssertThrown(() => room.GetCellActivity(1, -1)).IsInstanceOf<IndexOutOfRangeException>();
            room.QueueFree();
        }

        [TestCase]
        public void TestSetCellActivityByBoolean()
        {
            var room = new RoomNode3D() { Rows = 3, Columns = 3 };
            Assertions.AssertThat(room.GetCellActivity(0, 0)).IsTrue();
            room.SetCellActivity(0, 0, false);
            Assertions.AssertThat(room.GetCellActivity(0, 0)).IsFalse();
            room.QueueFree();
        }

        [TestCase]
        public void TestSetCellActivity()
        {
            var room = new RoomNode3D() { Rows = 3, Columns = 3 };
            Assertions.AssertThat(room.GetCellActivity(0, 0)).IsTrue();
            room.SetCellActivity(0, 0, CellActivity.Deactivate);
            Assertions.AssertThat(room.GetCellActivity(0, 0)).IsFalse();
            room.SetCellActivity(0, 0, CellActivity.Activate);
            Assertions.AssertThat(room.GetCellActivity(0, 0)).IsTrue();
            room.SetCellActivity(0, 0, CellActivity.Toggle);
            Assertions.AssertThat(room.GetCellActivity(0, 0)).IsFalse();
            room.SetCellActivity(0, 0, CellActivity.Toggle);
            Assertions.AssertThat(room.GetCellActivity(0, 0)).IsTrue();
            room.SetCellActivity(0, 0, CellActivity.None);
            Assertions.AssertThat(room.GetCellActivity(0, 0)).IsTrue();
            room.QueueFree();
        }

        [TestCase]
        public void TestSetCellActivityThrowsOutOfRangeException()
        {
            var room = new RoomNode3D() { Rows = 3, Columns = 3 };
            Assertions.AssertThrown(() => room.SetCellActivity(-1, -1, true)).IsInstanceOf<IndexOutOfRangeException>();
            Assertions.AssertThrown(() => room.SetCellActivity(-1, -1, CellActivity.Deactivate)).IsInstanceOf<IndexOutOfRangeException>();
            room.QueueFree();
        }

        [TestCase]
        public void TestSetCellActivities()
        {
            var room = new RoomNode3D() { Rows = 3, Columns = 3 };
            room.SetCellActivities(new Vector2I(1, 1), new Vector2I(2, 2), CellActivity.Deactivate);
            Assertions.AssertThat(room.GetCellActivity(1, 1)).IsFalse();
            Assertions.AssertThat(room.GetCellActivity(2, 1)).IsFalse();
            Assertions.AssertThat(room.GetCellActivity(1, 2)).IsFalse();
            Assertions.AssertThat(room.GetCellActivity(2, 2)).IsFalse();
            room.SetCellActivities(new Vector2I(2, 2), new Vector2I(1, 1), CellActivity.Activate);
            Assertions.AssertThat(room.GetCellActivity(1, 1)).IsTrue();
            Assertions.AssertThat(room.GetCellActivity(2, 1)).IsTrue();
            Assertions.AssertThat(room.GetCellActivity(1, 2)).IsTrue();
            Assertions.AssertThat(room.GetCellActivity(2, 2)).IsTrue();
            room.QueueFree();
        }

        [TestCase]
        public void TestSetCellActivitiesOutOfRangeDoesNothing()
        {
            var room = new RoomNode3D() { Rows = 3, Columns = 3 };
            room.SetCellActivities(new Vector2I(-1, -1), new Vector2I(2, 2), CellActivity.Deactivate);
            Assertions.AssertThat(room.GetCellActivity(1, 1)).IsTrue();
            Assertions.AssertThat(room.GetCellActivity(2, 1)).IsTrue();
            Assertions.AssertThat(room.GetCellActivity(1, 2)).IsTrue();
            Assertions.AssertThat(room.GetCellActivity(2, 2)).IsTrue();
            room.SetCellActivities(new Vector2I(2, 2), new Vector2I(-1, -1), CellActivity.Deactivate);
            Assertions.AssertThat(room.GetCellActivity(1, 1)).IsTrue();
            Assertions.AssertThat(room.GetCellActivity(2, 1)).IsTrue();
            Assertions.AssertThat(room.GetCellActivity(1, 2)).IsTrue();
            Assertions.AssertThat(room.GetCellActivity(2, 2)).IsTrue();
            room.QueueFree();
        }

        [TestCase]
        public void TestFindClosestActiveCellIndex()
        {
            var runner = SceneRunner.RunScene(EmptyScene);
            var root = runner.Scene();
            var room = new RoomNode3D() { Rows = 3, Columns = 3, CellSize = new Vector3(100, 100, 100) };
            root.AddChild(room);

            room.SetCellActivities(new Vector2I(1, 1), new Vector2I(2, 2), CellActivity.Deactivate);
            Assertions.AssertThat(room.FindClosestActiveCellIndex(Vector3.Zero)).IsEqual(Vector2I.Zero);
            Assertions.AssertThat(room.FindClosestActiveCellIndex(new Vector3(125, 1000, 50))).IsEqual(new Vector2I(0, 1));
            Assertions.AssertThat(room.FindClosestActiveCellIndex(new Vector3(125, -1000, 150))).IsEqual(new Vector2I(1, 0));
            Assertions.AssertThat(room.FindClosestActiveCellIndex(new Vector3(150, 0, 125))).IsEqual(new Vector2I(0, 1));
            room.QueueFree();
        }

        [TestCase]
        public void TestFindClosestDoorDirection()
        {
            var runner = SceneRunner.RunScene(EmptyScene);
            var root = runner.Scene();
            var room = new RoomNode3D() { Rows = 3, Columns = 3, CellSize = new Vector3(100, 100, 100) };
            root.AddChild(room);

            Assertions.AssertThat(room.FindClosestDoorDirection(0, 0, new Vector3(50, 50, 0))).IsEqual(DoorDirection.North);
            Assertions.AssertThat(room.FindClosestDoorDirection(0, 0, new Vector3(0, 50, 50))).IsEqual(DoorDirection.West);
            Assertions.AssertThat(room.FindClosestDoorDirection(0, 0, new Vector3(50, 50, 100))).IsEqual(DoorDirection.South);
            Assertions.AssertThat(room.FindClosestDoorDirection(0, 0, new Vector3(100, 50, 50))).IsEqual(DoorDirection.East);
            Assertions.AssertThat(room.FindClosestDoorDirection(0, 0, new Vector3(50, 100, 50))).IsEqual(DoorDirection.Top);
            Assertions.AssertThat(room.FindClosestDoorDirection(0, 0, new Vector3(50, 0, 50))).IsEqual(DoorDirection.Bottom);
            room.QueueFree();
        }

        [TestCase]
        public void TestGlobalPositionToCellIndex()
        {
            var runner = SceneRunner.RunScene(EmptyScene);
            var root = runner.Scene();
            var offset = new Vector3(543, 9084, 234);
            var room = new RoomNode3D() { Rows = 3, Columns = 3, CellSize = new Vector3(100, 100, 100), Position = offset };
            root.AddChild(room);

            for (int i = 0; i < room.Rows; i++)
            {
                for (int j = 0; j < room.Columns; j++)
                {
                    var position = room.CellSize * new Vector3(j, 0, i) + offset + 0.5f * room.CellSize;
                    Assertions.AssertThat(room.GlobalPositionToCellIndex(position)).IsEqual(new Vector2I(i, j));
                }
            }

            Assertions.AssertThat(room.GlobalPositionToCellIndex(Vector3.Zero)).IsEqual(new Vector2I(-1, -1));
            room.QueueFree();
        }

        [TestCase]
        public void TestLocalPositionToCellIndex()
        {
            var offset = new Vector3(543, 9084, 234);
            var room = new RoomNode3D() { Rows = 3, Columns = 3, CellSize = new Vector3(100, 100, 100), Position = offset };

            for (int i = 0; i < room.Rows; i++)
            {
                for (int j = 0; j < room.Columns; j++)
                {
                    var position = room.CellSize * new Vector3(j, 0, i) + 0.5f * room.CellSize;
                    Assertions.AssertThat(room.LocalPositionToCellIndex(position)).IsEqual(new Vector2I(i, j));
                }
            }

            Assertions.AssertThat(room.LocalPositionToCellIndex(new Vector3(-100, -100, -100))).IsEqual(new Vector2I(-1, -1));
            room.QueueFree();
        }

        [TestCase]
        public void TestCellCenterLocalPosition()
        {
            var offset = new Vector3(543, 9084, 234);
            var room = new RoomNode3D() { Rows = 3, Columns = 3, CellSize = new Vector3(100, 100, 100), Position = offset };
            Assertions.AssertThat(room.CellCenterLocalPosition(0, 0)).IsEqual(new Vector3(50, 50, 50));
            Assertions.AssertThat(room.CellCenterLocalPosition(1, 0)).IsEqual(new Vector3(50, 50, 150));
            Assertions.AssertThat(room.CellCenterLocalPosition(2, 0)).IsEqual(new Vector3(50, 50, 250));
            Assertions.AssertThat(room.CellCenterLocalPosition(0, 1)).IsEqual(new Vector3(150, 50, 50));
            Assertions.AssertThat(room.CellCenterLocalPosition(0, 2)).IsEqual(new Vector3(250, 50, 50));
            Assertions.AssertThat(room.CellCenterLocalPosition(1, 1)).IsEqual(new Vector3(150, 50, 150));
            Assertions.AssertThat(room.CellCenterLocalPosition(2, 2)).IsEqual(new Vector3(250, 50, 250));
            room.QueueFree();
        }

        [TestCase]
        public void TestCellCenterGlobalPosition()
        {
            var runner = SceneRunner.RunScene(EmptyScene);
            var root = runner.Scene();
            var offset = new Vector3(543, 9084, 234);
            var room = new RoomNode3D() { Rows = 3, Columns = 3, CellSize = new Vector3(100, 100, 100), Position = offset };
            root.AddChild(room);

            Assertions.AssertThat(room.CellCenterGlobalPosition(0, 0)).IsEqual(new Vector3(50, 50, 50) + offset);
            Assertions.AssertThat(room.CellCenterGlobalPosition(1, 0)).IsEqual(new Vector3(50, 50, 150) + offset);
            Assertions.AssertThat(room.CellCenterGlobalPosition(2, 0)).IsEqual(new Vector3(50, 50, 250) + offset);
            Assertions.AssertThat(room.CellCenterGlobalPosition(0, 1)).IsEqual(new Vector3(150, 50, 50) + offset);
            Assertions.AssertThat(room.CellCenterGlobalPosition(0, 2)).IsEqual(new Vector3(250, 50, 50) + offset);
            Assertions.AssertThat(room.CellCenterGlobalPosition(1, 1)).IsEqual(new Vector3(150, 50, 150) + offset);
            Assertions.AssertThat(room.CellCenterGlobalPosition(2, 2)).IsEqual(new Vector3(250, 50, 250) + offset);
            room.QueueFree();
        }

        [TestCase]
        public void TestGlobalPositionToCellIndexFromCellCenterGlobalPosition()
        {
            var runner = SceneRunner.RunScene(EmptyScene);
            var root = runner.Scene();
            var offset = new Vector3(543, 9084, 234);
            var room = new RoomNode3D() { Rows = 3, Columns = 3, CellSize = new Vector3(100, 100, 100), Position = offset };
            root.AddChild(room);

            for (int i = 0; i < room.Rows; i++)
            {
                for (int j = 0; j < room.Columns; j++)
                {
                    var center = room.CellCenterGlobalPosition(i, j);
                    var index = room.GlobalPositionToCellIndex(center);
                    Assertions.AssertThat(index).IsEqual(new Vector2I(i, j));
                }
            }

            room.QueueFree();
        }

        [TestCase]
        public void TestLocalPositionToCellIndexFromCellCenterLocalPosition()
        {
            var offset = new Vector3(543, 9084, 234);
            var room = new RoomNode3D() { Rows = 3, Columns = 3, CellSize = new Vector3(100, 100, 100), Position = offset };

            for (int i = 0; i < room.Rows; i++)
            {
                for (int j = 0; j < room.Columns; j++)
                {
                    var center = room.CellCenterLocalPosition(i, j);
                    var index = room.LocalPositionToCellIndex(center);
                    Assertions.AssertThat(index).IsEqual(new Vector2I(i, j));
                }
            }

            room.QueueFree();
        }
    }
}