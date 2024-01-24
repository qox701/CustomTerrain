using Godot;
using System;
using System.Collections.Generic;

namespace CustomTerrain;

public static class GenerateMeshUtil
{
    public static ArrayMesh GenerateArrayMeshOrigin(float Size,int PlaneCount)
    {
        var array = new Godot.Collections.Array();
        array.Resize((int)ArrayMesh.ArrayType.Max);
        
        
        //Generate a mesh from a list of vertices and indices
        List<Vector3> vertices = new();
        List<int> indices = new();
        
        //Genrate a 16*16 Plane
        {
            int width = PlaneCount+1;
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < width; z++)
                {
                    vertices.Add(new Vector3((float)x/(float)(PlaneCount)*Size, 0, (float)z/(float)(PlaneCount)*Size));
                }
            }
            
            for (int x = 0; x < width - 1; x++)
            {
                for (int z = 0; z < width - 1; z++)
                {
                    indices.Add(x + z * width);
                    indices.Add(x + (z + 1) * width);
                    indices.Add(x + 1 + z * width);

                    indices.Add(x + 1 + z * width);
                    indices.Add(x + (z + 1) * width);
                    indices.Add(x + 1 + (z + 1) * width);
                }
            }
        }
        
        array[(int) Mesh.ArrayType.Vertex] = vertices.ToArray();
        array[(int)Mesh.ArrayType.Index] = indices.ToArray();
        
        var arrayMesh = new ArrayMesh();
        arrayMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles,array);
        return arrayMesh;
    }

    public static ArrayMesh GenerateArrayMeshRepaired(float Size, int PlaneCount, Vector3 direction)
    {
        var array = new Godot.Collections.Array();
        array.Resize((int)ArrayMesh.ArrayType.Max);
        
        
        //Generate a mesh from a list of vertices and indices
        List<Vector3> vertices = new();
        List<int> indices = new();
        
        //Genrate a 16*16 Plane
        
            int width = PlaneCount+1;
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < width; z++)
                {
                    vertices.Add(new Vector3((float)x/(float)(PlaneCount)*Size, 0, (float)z/(float)(PlaneCount)*Size));
                }
            }
            
            for (int x = 0; x < width - 1; x++)
            {
                for (int z = 0; z < width - 1; z++)
                {
                    indices.Add(x + z * width);
                    indices.Add(x + (z + 1) * width);
                    indices.Add(x + 1 + z * width);

                    indices.Add(x + 1 + z * width);
                    indices.Add(x + (z + 1) * width);
                    indices.Add(x + 1 + (z + 1) * width);
                }
            }
        

        if (direction.X != 0)
        {
            if (direction.X > 0)
            {
                for (int i = 0; i < width-1; i+=2)
                {
                    //vertices[(i+1)*width-1] += new Vector3(0,Size / (float)PlaneCount,0);
                    vertices[i+width*(width-1)+1]+=new Vector3(0, 0, Size / (float)PlaneCount);

                }
            }
            else
            {
                for (int i = 0; i < width-1; i += 2)
                {
                    //vertices[i * width] += new Vector3(0, 0, Size / (float)PlaneCount);
                    vertices[i+1] += new Vector3( 0,0, -Size / (float)PlaneCount);
                }
            }
        }

        if (direction.Z != 0)
        {
            if (direction.Z > 0)
            {
                for (int i = 1; i < width; i+=2)
                {
                    //vertices[i+width*(width-1)] += new Vector3(Size / PlaneCount,0,0);
                    vertices[(i+1)*width-1] += new Vector3(Size / (float)PlaneCount,0,0);
                }
            }
            else
            {
                for (int i = 1; i < width; i += 2)
                {
                    vertices[i*width] += new Vector3(-Size / PlaneCount, 0, 0);
                }
            }
        }
        
        array[(int) Mesh.ArrayType.Vertex] = vertices.ToArray();
        array[(int)Mesh.ArrayType.Index] = indices.ToArray();
        
        var arrayMesh = new ArrayMesh();
        arrayMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles,array);
        return arrayMesh;
    }

    public static ArrayMesh GenerateArrayMeshWithSeamless(float Size,int PlaneCount,Vector3 direction)
    {
        if(direction==Vector3.Zero)//Dont need to repair
            return GenerateArrayMeshOrigin(Size, PlaneCount);
        else //need to repair
        {
            return GenerateArrayMeshRepaired(Size, PlaneCount, direction);
        }
    }

    public static Vector3 SeamlessDirection(Vector3 CenterPos,
        HashSet<Vector3> _meshPosList,float Size)
    {
        float firstOffset = Size *1.5f;
        float secondOffset = Size /2;
        
        var direction = Vector3.Zero;
        
        Vector3[] offsets =
        {
            new Vector3(firstOffset, 0, secondOffset),
            new Vector3(firstOffset, 0, -secondOffset),
            
            new Vector3(-firstOffset, 0, secondOffset),
            new Vector3(-firstOffset, 0, -secondOffset),
            
            new Vector3(secondOffset, 0, firstOffset),
            new Vector3(-secondOffset, 0, firstOffset),
            
            new Vector3(secondOffset, 0, -firstOffset),
            new Vector3(-secondOffset, 0, -firstOffset)
        };
        foreach (var offset in offsets)
        {

            if (_meshPosList.Contains(CenterPos + offset))
            {
                if(Mathf.Abs(offset.X)>Mathf.Abs(offset.Z))
                    direction+=new Vector3(offset.X>0?1:-1,0,0);
                else
                    direction+=new Vector3(0,0,offset.Z>0?1:-1);
            }
            
        }
        
        
        return direction;
    }
}