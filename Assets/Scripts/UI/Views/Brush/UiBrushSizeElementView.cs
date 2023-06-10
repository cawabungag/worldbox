using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
	public class UiBrushSizeElementView : MonoBehaviour
	{
		[SerializeField]
		private Toggle _toggle;

		[SerializeField]
		private Image _image;
		
		public Toggle Togle => _toggle;
		public Image Image => _image;
	}
}