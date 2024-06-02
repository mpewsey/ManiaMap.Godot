#if TOOLS
using Godot;

namespace MPewsey.ManiaMapGodot.Editor
{
    [Tool]
    public partial class BoxGizmo : MeshInstance3D
    {
        private static StringName AlbedoParameterName { get; } = "albedo";

        public static BoxGizmo CreateInstance(DoorThreshold3D threshold)
        {
            var material = ManiaMapResources.Materials.AlbedoMaterial;
            var gizmo = new BoxGizmo() { Mesh = new BoxMesh(), MaterialOverride = material };
            var edges = new MeshInstance3D() { Mesh = CreateCubeEdgeMesh(), MaterialOverride = material };
            threshold.OnSizeChanged += gizmo.OnDoorThreshold3DSizeChanged;

            gizmo.AddChild(edges);
            threshold.AddChild(gizmo);
            gizmo.OnDoorThreshold3DSizeChanged(threshold.Size);

            return gizmo;
        }

        private void OnDoorThreshold3DSizeChanged(Vector3 size)
        {
            Scale = size * GetParent<Node3D>().Scale;
            var lineColor = ManiaMapProjectSettings.GetDoorThreshold3DLineColor();
            var fillColor = ManiaMapProjectSettings.GetDoorThreshold3DFillColor();
            SetInstanceShaderParameter(AlbedoParameterName, fillColor);
            var edges = GetChild<MeshInstance3D>(0);
            edges.SetInstanceShaderParameter(AlbedoParameterName, lineColor);
        }

        public static ArrayMesh CreateCubeEdgeMesh(float size = 1)
        {
            var mesh = new ArrayMesh();
            var tool = new SurfaceTool();
            tool.Begin(Mesh.PrimitiveType.Lines);
            var scale = 0.5f * size;

            var a = new Vector3(-1, -1, -1) * scale;
            var b = new Vector3(1, -1, -1) * scale;
            var c = new Vector3(1, 1, -1) * scale;
            var d = new Vector3(-1, 1, -1) * scale;

            var e = new Vector3(-1, -1, 1) * scale;
            var f = new Vector3(1, -1, 1) * scale;
            var g = new Vector3(1, 1, 1) * scale;
            var h = new Vector3(-1, 1, 1) * scale;

            // Back side
            tool.AddVertex(a);
            tool.AddVertex(b);

            tool.AddVertex(b);
            tool.AddVertex(c);

            tool.AddVertex(c);
            tool.AddVertex(d);

            tool.AddVertex(d);
            tool.AddVertex(a);

            // Front side
            tool.AddVertex(e);
            tool.AddVertex(f);

            tool.AddVertex(f);
            tool.AddVertex(g);

            tool.AddVertex(g);
            tool.AddVertex(h);

            tool.AddVertex(h);
            tool.AddVertex(e);

            // Left side
            tool.AddVertex(a);
            tool.AddVertex(e);

            tool.AddVertex(d);
            tool.AddVertex(h);

            // Right side
            tool.AddVertex(b);
            tool.AddVertex(f);

            tool.AddVertex(c);
            tool.AddVertex(g);

            tool.Commit(mesh);
            return mesh;
        }
    }
}
#endif