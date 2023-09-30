using System.Collections.Generic;
using Core.WindowService;
using Db.Brush;
using DefaultNamespace.Components.Input;
using DefaultNamespace.Utils;
using Leopotam.EcsLite;
using UI.Views;
using UniRx;
using UnityEngine;
using Zenject;

namespace UI.Presenters
{
	public class UiBrushHudPresenter : BasePresenter<UiBrushHudView>, IInitializable
	{
		private ReactiveProperty<BrushType> _brushTypeReactiveProperty = new();
		private IntReactiveProperty _brushSizeReactiveProperty = new();
		private BoolReactiveProperty _showBrushPanelProperty = new();

		private readonly Dictionary<BrushType, UiBrushElementView> _brushesElementsBuffer = new();
		private readonly BrushesData _brushesData;
		private readonly EcsWorld _input;

		public UiBrushHudPresenter(UiBrushHudView view,
			BrushesData brushesData,
			[Inject(Id = WorldUtils.INPUT_WORLD_NAME)]
			EcsWorld input) : base(view)
		{
			_brushesData = brushesData;
			_input = input;
		}

		public override bool IsPopUp => true;

		public void Initialize()
		{
			_brushTypeReactiveProperty.Subscribe(OnChangeBrushType);
			_brushSizeReactiveProperty.Subscribe(OnChangeBrushSize);
			_showBrushPanelProperty.Subscribe(OnChangeShowPanel);

			View.ShowBrushPanel.onClick.AddListener(OnClickShowBrushPanel);
			View.HideBrushPanel.onClick.AddListener(OnClickHideBrushPanel);

			var brushes = _brushesData.GetBrushes();
			foreach (var brush in brushes)
			{
				if (!_brushesElementsBuffer.TryGetValue(brush.BrushType, out var brushElement))
				{
					brushElement = Object.Instantiate(View.BrushElementView, View.BrushesTarget);
					_brushesElementsBuffer.Add(brush.BrushType, brushElement);
				}

				var brushSizeElement = Object.Instantiate(View.BrushSizeElementView, brushElement.BrushSizeTarget);
				brushSizeElement.Image.sprite = brush.BrushSprite;
				brushSizeElement.Togle.group = View.ToggleGroup;
				brushSizeElement.Togle.onValueChanged.AddListener(isOn =>
					OnChangeBrush(isOn, brush.BrushType, brush.Size));
			}
		}

		private void OnClickShowBrushPanel()
			=> _showBrushPanelProperty.Value = true;

		private void OnClickHideBrushPanel()
			=> _showBrushPanelProperty.Value = false;

		private void OnChangeShowPanel(bool isShow)
		{
			View.ShowBrushPanel.gameObject.SetActive(!isShow);
			View.HideBrushPanel.gameObject.SetActive(isShow);
			View.BrushesTarget.gameObject.SetActive(isShow);
		}

		private void OnChangeBrush(bool isOn, BrushType brushType, int brushSize)
		{
			if (isOn)
			{
				_brushTypeReactiveProperty.Value = brushType;
				_brushSizeReactiveProperty.Value = brushSize;
				return;
			}

			_brushTypeReactiveProperty.Value = InputUtils.DEFAULT_BRUSH_TYPE;
		}

		private void OnChangeBrushType(BrushType brushType) =>
			_input.GetUniqueRef<InputBrushTypeComponent>().Value = brushType;

		private void OnChangeBrushSize(int brushSize) =>
			_input.GetUniqueRef<InputBrushSizeComponent>().Value = brushSize;
	}
}