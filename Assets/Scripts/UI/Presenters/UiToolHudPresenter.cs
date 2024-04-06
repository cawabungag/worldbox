using System;
using Core.WindowService;
using Db.Tools;
using DefaultNamespace.Components.Input;
using DefaultNamespace.Utils;
using Leopotam.EcsLite;
using Tools;
using UI.Views;
using Zenject;
using Object = UnityEngine.Object;

namespace UI.Presenters
{
	public class UiToolHudPresenter : BasePresenter<UiToolHudView>, IInitializable
	{
		private readonly ToolsData _toolsData;
		private readonly UiSaveHudPresenter _saveHudPresenter;
		private readonly IWindowService _windowService;
		private readonly EcsPool<InputToolComponent> _poolInputTool;
		private readonly EcsPool<InputIsBrushToolComponent> _poolIsBrushToolComponent;
		private readonly EcsFilter _filterTool;
		private readonly EcsFilter _ecsFilter;
		public override bool IsPopUp => false;

		public UiToolHudPresenter(UiToolHudView view,
			ToolsData toolsData,
			[Inject(Id = WorldUtils.INPUT_WORLD_NAME)]
			EcsWorld inputWorld, UiSaveHudPresenter saveHudPresenter) : base(view)
		{
			_toolsData = toolsData;
			_saveHudPresenter = saveHudPresenter;
			_poolInputTool = inputWorld.GetPool<InputToolComponent>();
			_poolIsBrushToolComponent = inputWorld.GetPool<InputIsBrushToolComponent>();
			_filterTool = inputWorld.Filter<InputToolComponent>().End();
			_ecsFilter = inputWorld.Filter<InputIsBrushToolComponent>().End();
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
			var toolData = _toolsData.GetTool(toggleToolType);
			ref var isBrushTool = ref _poolIsBrushToolComponent.Get(_ecsFilter.GetRawEntities()[0]).Value;
			isBrushTool = _toolsData.GetTool(toggleToolType).IsBrushTool;

			if (toggleToolType == toolType)
			{
				if (isOn) 
					return;
				
				if (toolData.IsSystemTool)
				{
					switch (toolData.WindowType)
					{
						case ToolWindowType.None:
							break;
						case ToolWindowType.Save:
							_saveHudPresenter.Close();
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
				
				toolType = ToolType.None;
				return;
			}

			toolType = toggleToolType;
			if (toolData.IsSystemTool)
			{
				switch (toolData.WindowType)
				{
					case ToolWindowType.None:
						break;
					case ToolWindowType.Save:
						_saveHudPresenter.Open();
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
	}
}