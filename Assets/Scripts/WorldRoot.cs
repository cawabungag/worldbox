using UnityEngine;

namespace DefaultNamespace
{
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class WorldRoot : MonoBehaviour
	{
		[SerializeField]
		private MeshFilter _meshFilter;
		public MeshFilter MeshFilter => _meshFilter;
	}
}