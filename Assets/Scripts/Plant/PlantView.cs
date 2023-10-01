using UnityEngine;
using Zenject;

namespace Plant
{
	public class PlantView : MonoBehaviour
	{
		public class Pool : MonoMemoryPool<PlantView>
		{
		}
	}
}