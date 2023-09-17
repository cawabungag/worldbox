using Core.WindowService;
using Db.Tools;
using DefaultNamespace.Components.Input;
using DefaultNamespace.Utils;
using Leopotam.EcsLite;
using Tools;
using UI.Views;
using UnityEngine;
using Zenject;

namespace UI.Presenters
{
	public class UiToolHudPresenter : BasePresenter<UiToolHudView>, IInitializable
	{
		private readonly ToolsData _toolsData;
		private readonly UiBrushHudPresenter _brushHudPresenter;
		private readonly EcsPool<InputToolComponent> _poolInputTool;
		private readonly EcsFilter _filterTool;
		public override bool IsPopUp => false;

		public UiToolHudPresenter(UiToolHudView view,
			ToolsData toolsData,
			[Inject(Id = WorldUtils.INPUT_WORLD_NAME)]
			EcsWorld inputWorld,
			UiBrushHudPresenter brushHudPresenter) : base(view)
		{
			_toolsData = toolsData;
			_brushHudPresenter = brushHudPresenter;
			_poolInputTool = inputWorld.GetPool<InputToolComponent>();
			_filterTool = inputWorld.Filter<InputToolComponent>().End();
		}

		public void Initialize()
		{
			var tools = _toolsData.GetTools();
			foreach (var tool in tools)
			{
				var toolElement = Object.Instantiate(View.UiToolElementView, View.ToolsElementsTarget);
				toolElement.Toggle.isOn = false;
				toolElement.IconImage.sprite = tool.Sprite;
				toolElement.Toggle.group = View.ToggleGroup;
				toolElement.Toggle.onValueChanged.AddListener(x => OnToggleValueChanged(x, tool.ToolType));
			}
		}

		private void OnToggleValueChanged(bool isOn, ToolType toggleToolType)
		{
			ref var toolType = ref _poolInputTool.Get(_filterTool.GetRawEntities()[0]).Value;
			if (toggleToolType == toolType)
			{
				if (isOn) 
					return;
				
				toolType = ToolType.None;
				return;
			}

			toolType = toggleToolType;
		}
	}
}