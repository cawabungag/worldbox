using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
	public class TestVoxel : MonoBehaviour
	{
		public Button _Button;
		public MeshFilter _MeshRenderer;

		[Space]
		public Vector2 _texturea;
		public Vector2 _textureb;
		public Vector2 _texturec;
		public Vector2 _textured;
		
		private readonly List<Vector2> _textures = new();
		private readonly List<Vector3> _vertices = new();
		private readonly List<int> _triangles = new();
		private int _faceCount = 0;

		private void Start()
		{
			_Button.onClick.AddListener(GenerateMesh);
		}

		private void Update()
		{
			GenerateMesh();
		}

		[MenuItem("GenerateMesh")]
		public void GenerateMesh()
		{
			var _mesh = new Mesh();
			_mesh.Clear();

			_textures.Clear();
			_vertices.Clear();
			_triangles.Clear();
			_faceCount = 0;
			CreateVoxelGeometry(Vector3Int.zero, 1);

			_mesh.SetVertices(_vertices);
			_mesh.SetTriangles(_triangles, 0);
			_mesh.SetUVs(0, _textures);
			_MeshRenderer.mesh = _mesh;
		}

		private void CreateVoxelGeometry(Vector3Int position,
			int voxelSize)
		{
			var x = position.x;
			var y = position.y;
			var z = position.z;

			// Front
			_vertices.Add(new Vector3(x, y, z + 1) * voxelSize);
			_vertices.Add(new Vector3(x + 1, y, z + 1) * voxelSize);
			_vertices.Add(new Vector3(x + 1, y + 1, z + 1) * voxelSize);
			_vertices.Add(new Vector3(x, y + 1, z + 1) * voxelSize);

			_triangles.Add(_faceCount * 4 + 0);
			_triangles.Add(_faceCount * 4 + 1);
			_triangles.Add(_faceCount * 4 + 2);
			_triangles.Add(_faceCount * 4 + 0);
			_triangles.Add(_faceCount * 4 + 2);
			_triangles.Add(_faceCount * 4 + 3);

			AddTexture();

			_faceCount += 1;

			// Back
			_vertices.Add(new Vector3(x + 1, y, z) * voxelSize);
			_vertices.Add(new Vector3(x, y, z) * voxelSize);
			_vertices.Add(new Vector3(x, y + 1, z) * voxelSize);
			_vertices.Add(new Vector3(x + 1, y + 1, z) * voxelSize);

			_triangles.Add(_faceCount * 4 + 0);
			_triangles.Add(_faceCount * 4 + 1);
			_triangles.Add(_faceCount * 4 + 2);
			_triangles.Add(_faceCount * 4 + 0);
			_triangles.Add(_faceCount * 4 + 2);
			_triangles.Add(_faceCount * 4 + 3);

			AddTexture();


			_faceCount += 1;

			// Top
			_vertices.Add(new Vector3(x, y + 1, z) * voxelSize);
			_vertices.Add(new Vector3(x + 1, y + 1, z) * voxelSize);
			_vertices.Add(new Vector3(x + 1, y + 1, z + 1) * voxelSize);
			_vertices.Add(new Vector3(x, y + 1, z + 1) * voxelSize);

			_triangles.Add(_faceCount * 4 + 0);
			_triangles.Add(_faceCount * 4 + 1);
			_triangles.Add(_faceCount * 4 + 2);
			_triangles.Add(_faceCount * 4 + 0);
			_triangles.Add(_faceCount * 4 + 2);
			_triangles.Add(_faceCount * 4 + 3);

			AddTexture();

			_faceCount += 1;

			// Bottom
			_vertices.Add(new Vector3(x + 1, y, z) * voxelSize);
			_vertices.Add(new Vector3(x, y, z) * voxelSize);
			_vertices.Add(new Vector3(x, y, z + 1) * voxelSize);
			_vertices.Add(new Vector3(x + 1, y, z + 1) * voxelSize);

			_triangles.Add(_faceCount * 4 + 0);
			_triangles.Add(_faceCount * 4 + 1);
			_triangles.Add(_faceCount * 4 + 2);
			_triangles.Add(_faceCount * 4 + 0);
			_triangles.Add(_faceCount * 4 + 2);
			_triangles.Add(_faceCount * 4 + 3);

			AddTexture();

			_faceCount += 1;

			// Left
			_vertices.Add(new Vector3(x, y, z) * voxelSize);
			_vertices.Add(new Vector3(x, y, z + 1) * voxelSize);
			_vertices.Add(new Vector3(x, y + 1, z + 1) * voxelSize);
			_vertices.Add(new Vector3(x, y + 1, z) * voxelSize);

			_triangles.Add(_faceCount * 4 + 0);
			_triangles.Add(_faceCount * 4 + 1);
			_triangles.Add(_faceCount * 4 + 2);
			_triangles.Add(_faceCount * 4 + 0);
			_triangles.Add(_faceCount * 4 + 2);
			_triangles.Add(_faceCount * 4 + 3);

			AddTexture();

			_faceCount += 1;

			// Right
			_vertices.Add(new Vector3(x + 1, y, z + 1) * voxelSize);
			_vertices.Add(new Vector3(x + 1, y, z) * voxelSize);
			_vertices.Add(new Vector3(x + 1, y + 1, z) * voxelSize);
			_vertices.Add(new Vector3(x + 1, y + 1, z + 1) * voxelSize);

			_triangles.Add(_faceCount * 4 + 0);
			_triangles.Add(_faceCount * 4 + 1);
			_triangles.Add(_faceCount * 4 + 2);
			_triangles.Add(_faceCount * 4 + 0);
			_triangles.Add(_faceCount * 4 + 2);
			_triangles.Add(_faceCount * 4 + 3);

			AddTexture();

			_faceCount += 1;
		}

		private void AddTexture()
		{
			_textures.Add(_texturea);
			_textures.Add(_textureb);
			_textures.Add(_texturec);
			_textures.Add(_textured);
		}
	}
}