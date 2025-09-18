using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple procedural map generator using Perlin noise.
/// - Attach to an empty GameObject in the scene.
/// - Assign one or more tile prefabs to `tilePrefabs` and optional thresholds.
/// - Use the inspector buttons or call GenerateMap() from other scripts.
/// This implementation works for both 2D (use sprites/prefabs) and 3D (use meshes) projects.
/// </summary>
public class map : MonoBehaviour
{
	[Header("Map Size")]
	public int width = 50;
	public int height = 50;

	[Header("Noise")]
	public float scale = 20f;
	public int seed = 0;
	public Vector2 offset = Vector2.zero;

	[Header("Tiles")]
	[Tooltip("Prefabs to choose from. The length should match thresholds, or thresholds will be generated automatically.")]
	public GameObject[] tilePrefabs;

	[Tooltip("Noise thresholds between 0 and 1. Example: [0.3,0.6] -> prefab0 for v<0.3, prefab1 for 0.3<=v<0.6, prefab2 for v>=0.6")] 
	public float[] thresholds;

	[Tooltip("Parent transform to keep hierarchy clean (optional)")]
	public Transform parentTransform;

	[Header("Runtime")]
	public bool autoGenerate = true;

	private System.Random rnd;

	// Generate button in the inspector
	[ContextMenu("Generate Map")]
	public void GenerateMap()
	{
		if (width <= 0 || height <= 0)
		{
			Debug.LogWarning("Width and Height must be > 0");
			return;
		}

		// Ensure tilePrefabs is populated
		if (tilePrefabs == null || tilePrefabs.Length == 0)
		{
			Debug.LogWarning("No tilePrefabs assigned. Please assign at least one prefab.");
			return;
		}

		// Prepare thresholds: if not provided or mismatched, generate evenly spaced thresholds
		if (thresholds == null || thresholds.Length != tilePrefabs.Length - 1)
		{
			thresholds = new float[Math.Max(0, tilePrefabs.Length - 1)];
			for (int i = 0; i < thresholds.Length; i++)
				thresholds[i] = (i + 1f) / tilePrefabs.Length;
		}

		// Initialize RNG
		rnd = new System.Random(seed);
		float seedOffsetX = (float)rnd.Next(-100000, 100000);
		float seedOffsetY = (float)rnd.Next(-100000, 100000);

		ClearMap();

		// Create parent if not assigned
		if (parentTransform == null)
		{
			GameObject p = new GameObject("GeneratedMap");
			parentTransform = p.transform;
			if (Application.isEditor)
				parentTransform.hideFlags = HideFlags.DontSaveInBuild;
		}

		// Generate tiles
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				float sampleX = (x + seedOffsetX + offset.x) / scale;
				float sampleY = (y + seedOffsetY + offset.y) / scale;
				float noise = Mathf.PerlinNoise(sampleX, sampleY);

				int index = IndexFromNoise(noise);
				GameObject prefab = tilePrefabs[Mathf.Clamp(index, 0, tilePrefabs.Length - 1)];

				Vector3 position = new Vector3(x - width / 2f, 0f, y - height / 2f);
				GameObject inst = Instantiate(prefab, position, Quaternion.identity, parentTransform);
				inst.name = $"tile_{x}_{y}_{prefab.name}";
			}
		}
	}

	int IndexFromNoise(float noise)
	{
		for (int i = 0; i < thresholds.Length; i++)
		{
			if (noise < thresholds[i])
				return i;
		}
		return thresholds.Length; // last index
	}

	[ContextMenu("Clear Map")]
	public void ClearMap()
	{
		if (parentTransform == null) return;

		// Destroy children safely in editor/build
		// Use immediate destroy in editor when not playing to update hierarchy
#if UNITY_EDITOR
		if (!Application.isPlaying)
		{
			for (int i = parentTransform.childCount - 1; i >= 0; i--)
			{
				UnityEditor.Undo.DestroyObjectImmediate(parentTransform.GetChild(i).gameObject);
			}
			UnityEditor.Undo.DestroyObjectImmediate(parentTransform.gameObject);
			parentTransform = null;
			return;
		}
#endif

		for (int i = parentTransform.childCount - 1; i >= 0; i--)
		{
			Destroy(parentTransform.GetChild(i).gameObject);
		}
		Destroy(parentTransform.gameObject);
		parentTransform = null;
	}

	void OnValidate()
	{
		width = Mathf.Max(1, width);
		height = Mathf.Max(1, height);
		scale = Mathf.Max(0.0001f, scale);

		if (autoGenerate && Application.isEditor && !Application.isPlaying)
		{
			// Delay call so Unity doesn't try to edit during serialization
#if UNITY_EDITOR
			UnityEditor.EditorApplication.delayCall += () =>
			{
				if (this == null) return;
				try { GenerateMap(); } catch { }
			};
#endif
		}
	}
}

