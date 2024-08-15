using Godot;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// Contains resource references for the project.
    /// </summary>
    public static class ManiaMapResources
    {
        /// <summary>
        /// Contains enums strings.
        /// </summary>
        public static class Enums
        {
            /// <summary>
            /// The door code enum string.
            /// </summary>
            public const string DoorCodeFlags = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,All:67108863";
        }

        /// <summary>
        /// Contains scene paths.
        /// </summary>
        public static class Scenes
        {
            public static PathRef LayoutGraphEditorScene { get; } = new PathRef("uid://ckyjrhwvcs6fi", "res://addons/mpewsey.maniamap/scenes/layout_graph_editor/layout_graph_editor.tscn");
            public static PathRef RoomNode2DToolbarScene { get; } = new PathRef("uid://ceij50wkmgvyi", "res://addons/mpewsey.maniamap/scenes/room_node_2d_toolbar/room_node_2d_toolbar.tscn");
            public static PathRef RoomNode3DToolbarScene { get; } = new PathRef("uid://b25t77npx7a3l", "res://addons/mpewsey.maniamap/scenes/room_node_3d_toolbar/room_node_3d_toolbar.tscn");
        }

        /// <summary>
        /// Contains materials.
        /// </summary>
        public static class Materials
        {
            public static PathRef AlbedoMaterialPath { get; } = new PathRef("uid://ppa2shs6thgv", "res://addons/mpewsey.maniamap/materials/albedo_material.tres");
            public static Material AlbedoMaterial { get; } = AlbedoMaterialPath.Load<Material>();
        }

        /// <summary>
        /// Contains icon paths.
        /// </summary>
        public static class Icons
        {
            public const string GenerationPipelineIcon = "uid://duyd6brii17k6";
            public const string GenerationInputIcon = "uid://cwamj1v5tj7kb";
            public const string GenerationStepIcon = "uid://d3tergu5u6l2u";

            public const string DoorNode2DIcon = "uid://c3qo4t1dq7ww0";
            public const string CollectableSpot2Dicon = "uid://b0mqt5vbeuxm3";
            public const string RoomFlag2DIcon = "uid://da1uw5osiatcm";
            public const string RoomNode2DIcon = "uid://bv1ymp1iusjv7";
            public const string Feature2DIcon = "uid://cvq4emefwjfpa";

            public const string DoorNode3DIcon = "uid://cse3mdyr4sypk";
            public const string CollectableSpot3Dicon = "uid://8p2ik53aw6co";
            public const string RoomFlag3DIcon = "uid://bodqwckopvjes";
            public const string RoomNode3DIcon = "uid://dk37e24jj1ukl";
            public const string Feature3DIcon = "uid://dldn43al5erf";
        }
    }
}