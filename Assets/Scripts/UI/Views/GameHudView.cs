using Core.WindowService;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
	public class GameHudView : BaseView
	{
		[SerializeField]
		private ToolElementView _toolElementView;
		public ToolElementView ToolElementView => _toolElementView;

		[SerializeField]
		private RectTransform _toolsElementsTarget;
		public RectTransform ToolsElementsTarget => _toolsElementsTarget;

		[SerializeField]
		private ToggleGroup _toggleGroup;
		public ToggleGroup ToggleGroup => _toggleGroup;
	}
}