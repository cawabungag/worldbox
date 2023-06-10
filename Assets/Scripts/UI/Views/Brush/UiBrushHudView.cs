using Core.WindowService;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
	public class UiBrushHudView : BaseView
	{
		[SerializeField]
		private RectTransform _brushesTarget;
		[SerializeField]
		private UiBrushElementView _brushElementView;
		[SerializeField]
		private UiBrushSizeElementView _brushSizeElementView;
		[SerializeField]
		private ToggleGroup _toggleGroup;
		[SerializeField]
		private Button _showBrushPanel;
		[SerializeField]
		private Button _hideBrushPanel;
		
		public UiBrushElementView BrushElementView => _brushElementView;
		public UiBrushSizeElementView BrushSizeElementView => _brushSizeElementView;
		public RectTransform BrushesTarget => _brushesTarget;
		public ToggleGroup ToggleGroup => _toggleGroup;
		public Button ShowBrushPanel => _showBrushPanel;
		public Button HideBrushPanel => _hideBrushPanel;
	}
}