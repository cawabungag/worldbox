using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
	public class UiToolElementView : MonoBehaviour
	{
		[SerializeField]
		private Image _iconImage;
		[SerializeField]
		private Toggle _toggle;
		
		public Image IconImage => _iconImage;
		public Toggle Toggle => _toggle;
	}
}