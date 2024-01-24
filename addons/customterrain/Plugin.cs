#if TOOLS
using Godot;
using System;

namespace CustomTerrain;

[Tool]
public partial class Plugin : EditorPlugin
{
	public override void _EnterTree()
	{
		//Add the CustomTerrain Node to the editor
		var script=GD.Load<Script>("res://addons/customterrain/CustomTerrain.cs");
		var icon=GD.Load<Texture2D>("res://addons/customterrain/icon.svg");
		AddCustomType("CustomTerrain","Node3D",script,icon);
		
		//Add the ArrayMeshTest Node to the editor
		var script2=GD.Load<Script>("res://addons/customterrain/ArrayMeshTest.cs");
		AddCustomType("ArrayMeshTest","Node3D",script2,icon);
	}

	public override void _ExitTree()
	{
		// Clean-up of the plugin goes here.
	}
}
#endif
