using Godot;
using System;

namespace CustomTerrain;

[Tool]
public partial class ArrayMeshTest:Node3D
{
    public override void _Ready()
    {
        MeshTest();
    }

    private void MeshTest()
    {
        Vector3 direction = new Vector3(1, 0, 1);
        var mesh=GenerateMeshUtil.GenerateArrayMeshRepaired(64,16,direction);
        //var mesh=GenerateMeshUtil.GenerateArrayMeshOrigin(128,16);
        var meshInstance=new MeshInstance3D();
        meshInstance.Mesh=mesh;
        AddChild(meshInstance);
        
    }
}