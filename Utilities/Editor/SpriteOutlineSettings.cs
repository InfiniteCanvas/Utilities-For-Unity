using UnityEngine;

namespace InfiniteCanvas.Utilities.Editor
{
	[CreateAssetMenu(fileName = "SpriteOutlineSettings", menuName = "Infinite Canvas/Settings/Sprite Outline Settings")]
	public class SpriteOutlineSettings : ScriptableObject
	{
	#region Serialized Fields

		[Range(0f, 1f)]  public float Tolerance      = 0.5f;
		[Range(0,  255)] public byte  AlphaTolerance = 127;
		public                  bool  HoleDetection;

	#endregion

		public override string ToString()
			=> $"{base.ToString()}, {nameof(Tolerance)}: {Tolerance}, {nameof(AlphaTolerance)}: {AlphaTolerance}, {nameof(HoleDetection)}: {HoleDetection}";
	}
}