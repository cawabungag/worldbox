using UnityEngine;
using Zenject;

namespace DefaultNamespace.Chunk
{
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class ChunkView : MonoBehaviour
	{
		[SerializeField]
		private MeshFilter _meshFilter;
		public MeshFilter MeshFilter => _meshFilter;
		
		public class Pool : MonoMemoryPool<ChunkView>
		{
		}
	}
}