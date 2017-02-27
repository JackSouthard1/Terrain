using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biomes : MonoBehaviour {

	Landscape landscape;
	MapGenerator mapGen;

	public AnimationCurve moistureCurve;

	public int seed;
	public float noiseScale;
	public int octaves;
	public float persistance;
	public float lacunarity;

	// Use this for initialization
	void Awake () {
		landscape = FindObjectOfType<Landscape>();
		mapGen = FindObjectOfType<MapGenerator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public float[,] GenerateMoisture ()
	{
//		print("" + chunkSize * mapSize + seed + noiseScale + octaves + persistance + lacunarity + mapGen.offset + mapGen.normalizeMode);
		float [,] moistureMap = Noise.GenerateNoiseMap (MapGenerator.mapChunkSize * landscape.mapSize, MapGenerator.mapChunkSize * landscape.mapSize, seed, noiseScale, octaves, persistance, lacunarity, Vector2.zero + mapGen.offset, mapGen.normalizeMode);

		// apply curve
		int sizeX = moistureMap.GetLength(0);
		int sizeY = moistureMap.GetLength(1);

		for (int y = 0; y < sizeY; y++) {
			for (int x = 0; x < sizeX; x++) {
				moistureMap[x, y] = moistureMap[x, y] * moistureCurve.Evaluate(moistureMap[x, y]);
			}
		}

		return moistureMap;
	}
}
