using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Sprites;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace InfiniteCanvas.Utilities.Editor
{
	public static class ShadowCaster2DMenuCommands
	{
		private const string _OUTLINE_SETTINGS_PATH = "Assets/Settings/OutlineSettings.asset";

		[MenuItem("CONTEXT/ShadowCaster2D/Copy Collider Shape")]
		private static void CopyColliderShape(MenuCommand menuCommand)
		{
			// Get the context as ShadowCaster2D
			var shadowCaster = menuCommand.context as ShadowCaster2D;
			if (shadowCaster == null)
				return;

			var polygonCollider = shadowCaster.GetComponent<PolygonCollider2D>();
			if (polygonCollider == null)
			{
				Debug.LogWarning("No PolygonCollider2D found on the same GameObject.");
			}
			else
			{
				Undo.RecordObject(shadowCaster, "Copied Collider Shape");
				// Create a SerializedObject representing the ShadowCaster2D
				var serializedObject = new SerializedObject(shadowCaster);
				serializedObject.Update();

				// Get the SerializedProperty of the fields you want to modify
				var pointsProperty = serializedObject.FindProperty("m_ShapePath");
				var shapePathHashProperty = serializedObject.FindProperty("m_ShapePathHash");

				// Modify the properties
				pointsProperty.arraySize = polygonCollider.points.Length;
				for (var i = 0; i < polygonCollider.points.Length; ++i)
					pointsProperty.GetArrayElementAtIndex(i).vector3Value = polygonCollider.points[i];

				var hash = GetShapePathHash(shadowCaster.shapePath);
				shapePathHashProperty.intValue = hash;

				// Apply the changes back to the ShadowCaster2D
				serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty(shadowCaster);
			}
		}

		[MenuItem("CONTEXT/ShadowCaster2D/Copy Collider Shape", true)]
		private static bool ValidateCopyColliderShape(MenuCommand menuCommand)
		{
			// Simply disable the menu item if the known condition is not met.
			var shadowCaster = menuCommand.context as ShadowCaster2D;
			var polygonCollider = shadowCaster.GetComponent<PolygonCollider2D>();

			return polygonCollider != null;
		}

		[MenuItem("CONTEXT/ShadowCaster2D/Copy Sprite Shape &#q")]
		private static void CopySpriteShape(MenuCommand menuCommand)
		{
			var settings = LoadOrCreateSettings();
			if (settings == null) return;

			// Get the ShadowCaster2D component
			var shadowCaster = menuCommand.context as ShadowCaster2D;
			if (shadowCaster == null) return;

			// Get the SpriteRenderer and Sprite from the same GameObject
			var spriteRenderer = shadowCaster.GetComponent<SpriteRenderer>();
			if (spriteRenderer == null) return;
			var sprite = spriteRenderer.sprite;
			if (sprite == null) return;

			// Generate the outline from the sprite using the SpriteOutlineGenerator
			Debug.Log($"Generating outline with {settings}");
			var outlines = GenerateOutlineFromSprite(sprite,
			                                         settings.Tolerance,
			                                         settings.AlphaTolerance,
			                                         settings.HoleDetection);


			if (outlines == null || outlines.Length == 0 || outlines[0].Length == 0) return;

			// Record the changes to ShadowCaster2D for Undo system
			Undo.RecordObject(shadowCaster, "Copy Sprite Shape");

			// Create a SerializedObject representing the ShadowCaster2D
			var serializedObject = new SerializedObject(shadowCaster);
			serializedObject.Update();
			var pointsProperty = serializedObject.FindProperty("m_ShapePath");
			var shapePathHashProperty = serializedObject.FindProperty("m_ShapePathHash");
			var shadowCastingSource = serializedObject.FindProperty("m_ShadowCastingSource");
			shadowCastingSource.enumValueIndex = 1;

			// Modify the SerializedProperties with the computed outline
			pointsProperty.arraySize = outlines[0].Length;
			for (var i = 0; i < outlines[0].Length; ++i)
				pointsProperty.GetArrayElementAtIndex(i).vector3Value = outlines[0][i];
			var hash = GetShapePathHash(Array.ConvertAll(outlines[0],
			                                             v => new Vector3(v.x,
			                                                              v.y,
			                                                              0))); // We use outline vertices here
			shapePathHashProperty.intValue = hash;

			// Apply the changes back to the ShadowCaster2D
			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(shadowCaster);
		}

		private static SpriteOutlineSettings LoadOrCreateSettings()
		{
			var guids = AssetDatabase.FindAssets("t:SpriteOutlineSettings");
			SpriteOutlineSettings settings = null;

			if (guids.Length > 0)
			{
				var path = AssetDatabase.GUIDToAssetPath(guids[0]);
				Debug.Log($"Using settings at path [{path}]");
				settings = AssetDatabase.LoadAssetAtPath<SpriteOutlineSettings>(path);
			}
			else
			{
				settings = ScriptableObject.CreateInstance<SpriteOutlineSettings>();
				// If asset doesn't exist, create a new one at the specified path
				AssetDatabase.CreateAsset(settings, _OUTLINE_SETTINGS_PATH);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}

			return settings;
		}

		private static int GetShapePathHash(Vector3[] path)
		{
			unchecked
			{
				var hashCode = (int)2166136261;

				if (path != null)
					foreach (var point in path)
						hashCode = (hashCode * 16777619) ^ point.GetHashCode();
				else
					hashCode = 0;

				return hashCode;
			}
		}

		public static Vector2[][] GenerateOutlineFromSprite(Sprite sprite,
		                                                    float  tolerance,
		                                                    byte   alphaTolerance,
		                                                    bool   holeDetection)
		{
			// Get the internal method GenerateOutlineFromSprite via Reflection
			var generateOutlineFromSprite = typeof(SpriteUtility).GetMethod("GenerateOutlineFromSprite",
			                                                                BindingFlags.NonPublic
			                                                              | BindingFlags.Static);

			if (generateOutlineFromSprite == null)
			{
				Debug.LogError("Could not find GenerateOutlineFromSprite method.");
				return null;
			}

			// Invoke the method, passing the correct parameters
			Vector2[][] paths = null;
			object[] parameters = { sprite, tolerance, alphaTolerance, holeDetection, paths };
			generateOutlineFromSprite.Invoke(null, parameters);

			return (Vector2[][])parameters[4]; // paths are returned via the last parameter
		}
	}
}