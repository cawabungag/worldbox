using Core.WindowService;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
	public class UiToolHudView : BaseView
	{
		[SerializeField]
		private UiToolElementView _uiToolElementView;
		public UiToolElementView UiToolElementView => _uiToolElementView;

		[SerializeField]
		private RectTransform _toolsElementsTarget;
		public RectTransform ToolsElementsTarget => _toolsElementsTarget;

		[SerializeField]
		private ToggleGroup _toggleGroup;
		public ToggleGroup ToggleGroup => _toggleGroup;
	}
}