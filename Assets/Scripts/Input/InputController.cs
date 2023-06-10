using System;
using System.Collections.Generic;
using System.Linq;
using Core.Camera;
using DefaultNamespace.Components.Input;
using DefaultNamespace.Utils;
using Leopotam.EcsLite;
using Tools;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class InputController : IInitializable, IDisposable, ILateTickable
{
	private readonly EcsWorld _input;
	private readonly ISceneCamera _camera;
	private readonly InputView _inputView;
	private readonly CompositeDisposable _disposables = new();
	private readonly Dictionary<int, TouchEvent> _touchEvents = new();
	private readonly List<int> _eventsToRemove = new();
	private float _startDistanceCamera;

	private Vector2 _previousTouchDistance;
	private Vector2 _previousWorldTouchCenter;

	private EcsPool<InputDrawPositionComponent> _poolDrawPosiiton;
	private EcsPool<InputToolComponent> _poolInputTool;
	private EcsFilter _filterTool;
	private Dictionary<ToolType, IUseToolStrategy> _useToolStrategies;

	public InputController([Inject(Id = WorldUtils.INPUT_WORLD_NAME)] EcsWorld input,
		ISceneCamera camera,
		InputView inputView,
		IUseToolStrategy[] toolStrategies)
	{
		_useToolStrategies = toolStrategies.ToDictionary(x => x.ToolType);
		_input = input;
		_camera = camera;
		_inputView = inputView;
	}

	public void Initialize()
	{
		_poolDrawPosiiton = _input.GetPool<InputDrawPositionComponent>();
		_poolInputTool = _input.GetPool<InputToolComponent>();
		_filterTool = _input.Filter<InputToolComponent>().End();

		_inputView.Down.Subscribe(OnDown).AddTo(_disposables);
		_inputView.Drag.Subscribe(OnDrag).AddTo(_disposables);
		_inputView.Up.Subscribe(OnUp).AddTo(_disposables);
	}

	private void OnDown(PointerEventData eventData)
		=> _touchEvents[eventData.pointerId] = new TouchEvent(ToushState.Begin, eventData);

	private void OnDrag(PointerEventData eventData)
	{
		if (!_touchEvents.ContainsKey(eventData.pointerId))
			return;

		_touchEvents[eventData.pointerId] = new TouchEvent(ToushState.Drag, eventData);
	}

	private void OnUp(PointerEventData eventData)
	{
		if (!_touchEvents.ContainsKey(eventData.pointerId))
			return;

		_touchEvents[eventData.pointerId] = new TouchEvent(ToushState.End, eventData);
	}

	private void ProcessOneTouch()
	{
		var touchEvent = _touchEvents.Values.First();

		if (_filterTool.GetEntitiesCount() > 0 && IsActiveTool(out var entity, out var toolType))
		{
			var touchPoint = touchEvent.Data.position;
			if (!_poolDrawPosiiton.Has(entity))
				_poolDrawPosiiton.Add(entity).Value = touchPoint;
			else
				_poolDrawPosiiton.Get(entity).Value = touchPoint;

			var worldTouchPoint = GetWorldPosition();
			var brushType = _input.GetUnique<InputBrushTypeComponent>().Value;
			var brushSize = _input.GetUnique<InputBrushSizeComponent>().Value;
			var isBrushTool = _input.GetUnique<InputIsBrushToolComponent>().Value;
			_useToolStrategies[toolType].Use(worldTouchPoint, brushType, brushSize, isBrushTool);
		}
		else
		{
			var touch = _touchEvents.Values.First();
			switch (touch.TouchState)
			{
				case ToushState.Begin:
					_previousWorldTouchCenter = GetWorldPosition();
					break;
				case ToushState.Drag:
					DragCamera(touch);
					break;
				case ToushState.End:
					break;
			}
		}
	}

	private bool IsActiveTool(out int entity, out ToolType toolType)
	{
		entity = _filterTool.GetRawEntities()[0];
		toolType = _poolInputTool.Get(entity).Value;
		return toolType != ToolType.None;
	}

	private void ProcessTwoTouches()
	{
		var firstTouch = _touchEvents.Values.First();
		var secondTouch = _touchEvents.Values.Last();

		if (firstTouch.TouchState == ToushState.End || secondTouch.TouchState == ToushState.End)
		{
			_touchEvents.Clear();
			return;
		}

		var touchDistance = secondTouch.Data.position - firstTouch.Data.position;
		var touchCenter = firstTouch.Data.position + touchDistance * 0.5f;

		if (firstTouch.TouchState == ToushState.Begin || secondTouch.TouchState == ToushState.Begin)
		{
			_previousTouchDistance = touchDistance;
			_previousWorldTouchCenter = GetWorldPosition();
			return;
		}

		if (firstTouch.TouchState != ToushState.Drag || secondTouch.TouchState != ToushState.Drag)
			return;

		DragCamera(firstTouch);

		var sizeModify = _inputView.GetZoomSensitivity(_camera.Size);
		var cameraScalingSpeed = touchDistance.sqrMagnitude > _previousTouchDistance.sqrMagnitude
			? -_inputView.CameraScalingSpeed * sizeModify
			: _inputView.CameraScalingSpeed * sizeModify;
		_camera.Size = Mathf.Clamp(_camera.Size + cameraScalingSpeed, _inputView.MinCameraSize,
			_inputView.MaxCameraSize);
		_previousTouchDistance = touchDistance;
	}

	private void DragCamera(TouchEvent touchEvent)
	{
		if (touchEvent.Data.delta.sqrMagnitude < Mathf.Epsilon)
			return;

		_camera.Translate(touchEvent.Data.delta);
	}

	//TODO: Need refactoring
	private Vector2 GetWorldPosition()
	{
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out var hit, 100))
		{
			var m = Matrix4x4.TRS(new Vector3(WorldUtils.WORLD_SIZE / 2, 0, WorldUtils.WORLD_SIZE / 2),
				Quaternion.identity, Vector3.one);
			var multiplyPoint3X4 = m.MultiplyPoint3x4(hit.point);
			var asd = new Vector3(multiplyPoint3X4.z, 0, multiplyPoint3X4.x);
			var xz = asd.ToXZ();
			return xz;
		}

		return Vector2.zero;
	}

	public void Dispose() => _disposables.Dispose();

	private enum ToushState
	{
		Begin,
		Drag,
		End
	}

	private class TouchEvent
	{
		public readonly ToushState TouchState;
		public readonly PointerEventData Data;

		public bool IsProcessed { get; private set; }

		public TouchEvent(ToushState touchState, PointerEventData data)
		{
			TouchState = touchState;
			Data = data;
		}

		public void Process() => IsProcessed = true;
	}

	public void LateTick()
	{
		if (_touchEvents.Values.All(t => t.IsProcessed))
			return;

		if (_touchEvents.Count == 2)
			ProcessTwoTouches();
		else if (_touchEvents.Count == 1)
			ProcessOneTouch();

		foreach (var entry in _touchEvents)
		{
			var touchEvent = entry.Value;
			touchEvent.Process();
			if (touchEvent.TouchState != ToushState.End)
				continue;
			_eventsToRemove.Add(entry.Key);
		}

		foreach (var eventId in _eventsToRemove)
		{
			_touchEvents.Remove(eventId);
		}

		_eventsToRemove.Clear();
	}
}