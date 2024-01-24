using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomTerrain;

	/// <summary>
	/// Struct for storing integer vectors
	/// Mainly used to decrease the frequency of updating chunks
	/// </summary>
	public struct IntVector3
	{
		public int x;
		public int y;
		public int z;
		
		public IntVector3(int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
		
		public void Is(Vector3 other)
		{
			this.x=(int)other.X;
			this.y=(int)other.Y;
			this.z=(int)other.Z;
		}
		
		public bool Equals(IntVector3 other)
		{
			return x == other.x && y == other.y && z == other.z;
		}
		
		public Vector3 ToVector3()
		{
			return new Vector3(x,y,z);
		}
	}

	[Tool]
	public partial class CustomTerrain : Node3D
	{
		[Export] public Node3D CameraPoint;
		[Export] public float QuadtreeSize = 512;
		[Export] public int MaxChunkDepth = 5;
		[Export]public Material _material;
		[Export] public float Height = 512;

		[Export] public Texture2D HeightMap;
		[Export] public ShaderMaterial shader;
		

		//The camera position and the position of the last frame
		private IntVector3 _focusPoint = new IntVector3(0, 0, 0);
		private IntVector3 _focusPointLastFrame=new IntVector3(0, 0, 0);
		
		//The chunk tree and Lists for storing chunks
		private QuadtreeNode quadtree;
		private Dictionary<Vector3,MeshInstance3D> _nodeMeshList = new ();
		//private Dictionary<Vector3, bool> _nodeListCurrent = new ();//Contains the centerPos of the Meshes
		private HashSet<Vector3> _nodePos = new();
		
		
		
		public override void _Ready()
		{
			_nodePos.Clear();
			//Remove all children
			foreach (var child in GetChildren())
			{
				RemoveChild(child);
				child.QueueFree();
			}
			
			UpdateChunks();

			if (shader != null && HeightMap != null)
			{
				shader.SetShaderParameter("HeightmapTexture", HeightMap);
				shader.SetShaderParameter("worldSize", new Vector3(QuadtreeSize,Height,QuadtreeSize));
			}

		}
		
		
		public override void _Process(double delta)
		{
			if (CameraPoint==null)
				return;
			//Only update chunks when the camera moves one unit
			_focusPoint.Is(CameraPoint.Position - this.Position);
			if (!_focusPoint.Equals( _focusPointLastFrame))
			{
				UpdateChunks();
				shader.SetShaderParameter("terrainPos",this.Position);
				_focusPointLastFrame = _focusPoint;
			}
		}
		
		//Subdivide and visualize the chunks
		public void UpdateChunks()
		{
			foreach (var child in GetChildren())
			{
				RemoveChild(child);
				child.QueueFree();
			}
			
			_nodeMeshList.Clear();
			_nodePos.Clear();
			//Init the first chunk
			Aabb bounds=new Aabb(this.Position,new Vector3(QuadtreeSize,Height,QuadtreeSize));
			quadtree = new QuadtreeNode(bounds, 0, MaxChunkDepth);
			//Subdivide the chunk
			quadtree.SubDivide(_focusPoint.ToVector3(),ref _nodePos);
			
			//_nodeListCurrent.Clear();
			
			
			//Visualize the chunk
			VisualizeQuadtree(quadtree);
			
			//Remove chunks that are not visualized
			/*List<Vector3> chunksToRemove=new List<Vector3>();
			foreach (var chunkId in _nodeMeshList.Keys)
			{
				if(!_nodeListCurrent.ContainsKey(chunkId))
					chunksToRemove.Add(chunkId);
			}

			foreach (var chunkId in chunksToRemove)
			{
				_nodeMeshList[chunkId].QueueFree();
				_nodeMeshList.Remove(chunkId);
			}*/
			
		}
		
		//Create mesh instances for all chunks
		public void VisualizeQuadtree(QuadtreeNode node)
		{
			
			if (node.Children.Count == 0)
			{
				
				//Mark the chunk as visualized
				//_nodeListCurrent[node.CenterPos] = true;
				//if the chunk is already visualized, return
				if (_nodeMeshList.ContainsKey(node.CenterPos))
					return;

				//Create a mesh instance for the chunk
				var meshInstance = new MeshInstance3D();
				
				var mesh = GenerateMeshUtil.GenerateArrayMeshRepaired(node.Bounds.Size.X, 8,
					GenerateMeshUtil.SeamlessDirection(node.CenterPos, _nodePos, node.Bounds.Size.X));
				
				mesh.CustomAabb = new Aabb(Vector3.Zero, node.Bounds.Size);
				meshInstance.Mesh = mesh;
				meshInstance.Position = new Vector3(node.Bounds.Position.X, 0, node.Bounds.Position.Z);
				meshInstance.MaterialOverride=shader;
				
				/*var meshInstance3=new MeshInstance3D();
				var mesh3 = new BoxMesh();
				meshInstance3.Mesh = mesh3;
				meshInstance3.Position=node.CenterPos+GenerateMeshUtil.SeamlessDirection(node.CenterPos, _nodePos, node.Bounds.Size.X);
				this.AddChild(meshInstance3);*/

				this.AddChild(meshInstance);
				//Add the meshInstance of the chunk to the list
				_nodeMeshList[node.CenterPos] = meshInstance;
			}
			foreach (var child in node.Children)
			{
				//if it has children, visualize them
				VisualizeQuadtree(child);
			}
		}
	}

