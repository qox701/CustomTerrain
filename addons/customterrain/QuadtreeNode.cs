using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomTerrain;
    
    public class QuadtreeNode
    {
        public Aabb Bounds { get;private set;  }
        public List<QuadtreeNode> Children{ get;private set;  }
        public int Depth{ get;private set;  }
        public int MaxChunkLevel{ get;private set;  }
        
        public Vector3 CenterPos => Bounds.GetCenter();
        public Vector3 InitPos => Bounds.Position;

        public QuadtreeNode(Aabb bounds, int depth, int maxChunkLevel)
        {
            Bounds = bounds;
            Depth = depth;
            MaxChunkLevel = maxChunkLevel;
            Children = new List<QuadtreeNode>();
        }
        
        public void SubDivide(Vector3 focusPoint,ref HashSet<Vector3> _nodeCenterPos)
        {
            float halfSize = Bounds.Size.X * 0.5f;
            float quaterSize = Bounds.Size.X * 0.25f;
            Vector3 halfExtents = new Vector3(halfSize, Bounds.Size.Y, halfSize);
            Vector3 halfExtents2 = new Vector3(halfSize/2, Bounds.Size.Y, halfSize/2);
            Vector3[] childrenPosition = 
            {
                new Vector3(0, 0, 0),
                new Vector3(quaterSize, 0, 0),
                new Vector3(0, 0, quaterSize),
                new Vector3(quaterSize, 0, quaterSize)
            };
            foreach (var offset in childrenPosition)
            {
                //Calculate the center for each child
                Vector3 childCenter = Bounds.Position + offset;//Where the mesh & AABB Init
                Vector3 childDisCenter = Bounds.Position + offset + new Vector3(quaterSize/2, 0, quaterSize/2);//The real center
                //Check the distance to the Lod center for each child
                float childDistence = childDisCenter.DistanceTo(focusPoint);
                float boundSize = Bounds.Size.X;
                if (Depth < MaxChunkLevel && childDistence < boundSize * 0.65f)
                {
                    //If center is within the minimum detail distance, subdivide
                    Aabb childBounds = new Aabb(childCenter, halfExtents);
                    QuadtreeNode childNode = new QuadtreeNode(childBounds, Depth + 1, MaxChunkLevel);
                    Children.Add(childNode);
                    //_nodeCenterPos.Add(childNode.CenterPos);
                    
                    childNode.SubDivide(focusPoint,ref _nodeCenterPos);
                }
                else
                {
                    //Center is outside the minimum detail distance, add child at this depth
                    //var childBounds = new Aabb(childCenter - Vector3.One*quaterSize, halfExtents);
                    var childBounds = new Aabb(childCenter , halfExtents2);
                    var childChunk = new QuadtreeNode(childBounds, Depth + 1, MaxChunkLevel);
                    Children.Add(childChunk);
                    _nodeCenterPos.Add(childChunk.CenterPos);
                }
            }
        }
    }
