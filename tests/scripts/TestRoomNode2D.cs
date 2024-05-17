using GdUnit4;
using Godot;
using System.IO;

namespace MPewsey.ManiaMapGodot.Tests
{
    [TestSuite]
    public class TestRoomNode2D
    {
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
    }
}