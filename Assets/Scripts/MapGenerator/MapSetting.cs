using System;
using UnityEngine;

[Serializable]
public class MapSetting
{
	[SerializeField] private int width;
	[SerializeField] private int height;

	public int Width
	{
		get => width;
		set => width = value;
	}

	public int Height
	{
		get => height;
		set => height = value;
	}
}