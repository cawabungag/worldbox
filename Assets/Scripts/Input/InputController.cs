using System;
using System.Collections.Generic;
using System.Linq;
using Core.Camera;
using DefaultNamespace.Components.Input;
using DefaultNamespace.Utils;
using Leopotam.EcsLite;
using Services.Map;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class InputController : IInitializable, IDisposable
{
	private readonly EcsWorld _input;
	private readonly ISceneCamera _camera;
	private readonly IMapService _mapService;
	private readonly InputView _inputView;
	private readonly CompositeDisposable _disposables = new();
	private readonly Dictionary<int, TouchEvent> _touchEvents = new();
	private readonly List<int> _eventsToRemove = new();
	private float _startDistanceCamera;

	private Vector2 _previousTouchDistance;
	private Vector2 _previousWorldTouchCenter;

	private EcsPool<InputDrawPositionComponent> _poolDrawPosiiton;
	private EcsPool<InputToolComponent> _poolInputTool;

	public InputController([Inject(Id = WorldUtils.INPUT_WORLD_NAME)] EcsWorld input,
		ISceneCamera camera,
		IMapService mapService,
		InputView inputView)
	{
		_input = input;
		_camera = camera;
		_mapService = mapService;
		_inputView = inputView;
	}

	public void Initialize()
	{
		_poolDrawPosiiton = _input.GetPool<InputDrawPositionComponent>();
		_poolInputTool = _input.GetPool<InputToolComponent>();

		_inputView.Down.Subscribe(OnDown).AddTo(_disposables);
		_inputView.Drag.Subscribe(OnDrag).AddTo(_disposables);
		_inputView.Up.Subscribe(OnUp).AddTo(_disposables);
		Observable.EveryLateUpdate().Subscribe(OnLateUpdate).AddTo(_disposables);
	}

	public void Dispose() => _disposables.Dispose();

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

	private void OnLateUpdate(long obj)
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

	private void ProcessOneTouch()
	{
		var touchEvent = _touchEvents.Values.First();

		// if (_poolInputTool.get)
		// _input.ReplaceDrawPosition(touchEvent.Data.position);
		// else
		// {
		var touch = _touchEvents.Values.First();
		switch (touch.TouchState)
		{
			case ToushState.Begin:
				_previousWorldTouchCenter = GetWorldPosition(touch.Data.position);
				Debug.LogError($"Set _previousWorldTouchCenter: {_previousWorldTouchCenter}");
				break;
			case ToushState.Drag:
				DragCamera(touch.Data.position);
				break;
			case ToushState.End:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
		// }
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
			_previousWorldTouchCenter = GetWorldPosition(touchCenter);
			return;
		}

		if (firstTouch.TouchState != ToushState.Drag || secondTouch.TouchState != ToushState.Drag)
			return;

		DragCamera(touchCenter);

		var sizeModify = _inputView.GetZoomSensitivity(_camera.Size);
		var cameraScalingSpeed = touchDistance.sqrMagnitude > _previousTouchDistance.sqrMagnitude
			? -_inputView.CameraScalingSpeed * sizeModify
			: _inputView.CameraScalingSpeed * sizeModify;
		_camera.Size = Mathf.Clamp(_camera.Size + cameraScalingSpeed, _inputView.MinCameraSize,
			_inputView.MaxCameraSize);
		_previousTouchDistance = touchDistance;
	}

	private void DragCamera(Vector2 touchCenter)
	{
		Debug.LogError($"{touchCenter}");
		var worldTouchPosition = GetWorldPosition(touchCenter);

		var positionDifference = worldTouchPosition - _previousWorldTouchCenter;
		Debug.LogError($"Set worldTouchPosition: {_previousWorldTouchCenter} delta: {positionDifference}");

		_camera.Translate(positionDifference);
		var position = _camera.Position;
		var mapRect = _mapService.GetMapRect();
		position.x = Mathf.Clamp(position.x, mapRect.xMin, mapRect.xMax);
		position.y = Mathf.Clamp(position.y, mapRect.yMin, mapRect.yMax);
		_camera.Position = position;
	}

	private Vector2 GetWorldPosition(Vector3 pos)
	{
		Debug.LogError($"Screen point: {pos}");
		return _camera.ScreenToWordPoint(pos);
	}

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

	private enum TouchType
	{
		Single,
		Double
	}
}