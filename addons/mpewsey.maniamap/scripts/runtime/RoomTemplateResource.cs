using Godot;
using MPewsey.Common.Serialization;
using MPewsey.ManiaMap;
using MPewsey.ManiaMapGodot.Exceptions;
using System.Threading.Tasks;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// Provides a reference to a room scene and information for the room required by the procedural generator.
    /// </summary>
    [Tool]
    [GlobalClass]
    public partial class RoomTemplateResource : Resource
    {
        /// <summary>
        /// The room template's unique ID.
        /// </summary>
        [Export] public int Id { get; set; } = Rand.GetRandomId();

        /// <summary>
        /// The template name.
        /// </summary>
        [Export] public string TemplateName { get; set; } = "<None>";

        /// <summary>
        /// The associated scene's path.
        /// </summary>
        [Export] public string ScenePath { get; set; }

        /// <summary>
        /// The associated scene's `uid://` path.
        /// </summary>
        [Export] public string SceneUidPath { get; set; }

        /// <summary>
        /// The serialized text for the ManiaMap room template.
        /// </summary>
        [ExportGroup("Serialized Text")]
        [Export(PropertyHint.MultilineText)] public string SerializedText { get; set; }

        public override void _ValidateProperty(Godot.Collections.Dictionary property)
        {
            base._ValidateProperty(property);
            var name = property["name"].AsStringName();

            if (name == PropertyName.ScenePath || name == PropertyName.SceneUidPath || name == PropertyName.SerializedText)
                property["usage"] = (int)(property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ReadOnly);
        }

        /// <summary>
        /// Sets the scene paths and serialized text to the room template based on the specified room.
        /// </summary>
        /// <param name="room">The room to associate with the template.</param>
        public void Initialize(IRoomNode room)
        {
            var node = (Node)room;
            var scenePath = node.SceneFilePath;
            var sceneUidPath = ResourceUid.IdToText(ResourceLoader.GetResourceUid(scenePath));
            var template = room.GetMMRoomTemplate(Id, TemplateName);

            SerializedText = JsonSerialization.GetJsonString(template, new JsonWriterSettings());
            ScenePath = scenePath;
            SceneUidPath = sceneUidPath;
        }

        /// <summary>
        /// Returns the ManiaMap room template used by the procedural generator.
        /// </summary>
        /// <exception cref="RoomTemplateNotInitializedException">Thrown if the serialized text is null or whitespace, indicating the template has not been initialized.</exception>
        public RoomTemplate GetMMRoomTemplate()
        {
            if (string.IsNullOrWhiteSpace(SerializedText))
                throw new RoomTemplateNotInitializedException($"Serialized text has not been assigned: {ResourcePath}");

            return JsonSerialization.LoadJsonString<RoomTemplate>(SerializedText);
        }

        /// <summary>
        /// Returns the controlling associated scene path. The `uid://` path is prioritized first, then the scene path.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="RoomTemplateNotInitializedException">Thrown if a scene path is null or whitespace, indicating that the template has not been initialized.</exception>
        /// <exception cref="PathDoesNotExistException">Thrown if both the `uid://` path and scene path do not exist per the resource loader.</exception>
        public string GetScenePath()
        {
            if (string.IsNullOrWhiteSpace(SceneUidPath) || string.IsNullOrWhiteSpace(ScenePath))
                throw new RoomTemplateNotInitializedException($"Scene path has not been assigned: {ResourcePath}");

            if (ResourceLoader.Exists(SceneUidPath))
                return SceneUidPath;

            if (ResourceLoader.Exists(ScenePath))
                return ScenePath;

            throw new PathDoesNotExistException($"Scene paths do not exist: (SceneUidPath = {SceneUidPath}, ScenePath = {ScenePath})");
        }

        /// <summary>
        /// Loads and returns the referenced scene.
        /// </summary>
        public PackedScene LoadScene()
        {
            return ResourceLoader.Load<PackedScene>(GetScenePath());
        }

        /// <summary>
        /// Asynchronously loads and returns the referenced scene.
        /// </summary>
        public Task<PackedScene> LoadSceneAsync(bool useSubThreads = false)
        {
            return AsyncResourceLoader.LoadAsync<PackedScene>(GetScenePath(), string.Empty, useSubThreads);
        }
    }
}