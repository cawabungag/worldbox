using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputView : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
	[SerializeField]
	private float _minCameraSize;
	public float MinCameraSize => _minCameraSize;

	[SerializeField]
	private float _maxCameraSize;
	public float MaxCameraSize => _maxCameraSize;

	[SerializeField]
	private float _cameraScalingSpeed;
	public float CameraScalingSpeed => _cameraScalingSpeed;

	[SerializeField]
	private AnimationCurve _zoomSensitivityCurve;
	public float GetZoomSensitivity(float size) => _zoomSensitivityCurve.Evaluate(size);

	[SerializeField]
	private AnimationCurve _scrollSensitivityCurve;
	public float GetScrollSensitivity(float size) => _scrollSensitivityCurve.Evaluate(size);

	public ReactiveCommand<PointerEventData> Down { get; } = new();
	public ReactiveCommand<PointerEventData> Drag { get; } = new();
	public ReactiveCommand<PointerEventData> Up { get; } = new();
	
	public void OnDrag(PointerEventData eventData) => Drag.Execute(eventData);
	public void OnPointerDown(PointerEventData eventData) => Down.Execute(eventData);
	public void OnPointerUp(PointerEventData eventData) => Up.Execute(eventData);
}