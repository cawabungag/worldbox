using TMPro;
using UnityEngine;

namespace UI
{
	public class VoxelRenderCounter : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI _text;

		public void SetText(string text)
		{
			_text.text = text;
		}
	}
}