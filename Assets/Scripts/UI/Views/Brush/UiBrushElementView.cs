using UnityEngine;

namespace UI.Views
{
	public class UiBrushElementView : MonoBehaviour
	{
		[SerializeField]
		private RectTransform _brushSizeTarget;
		public RectTransform BrushSizeTarget => _brushSizeTarget;
	}
}