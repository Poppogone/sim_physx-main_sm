using UnityEngine;

public class HolePainter : MonoBehaviour
{
    public Terrain terrain;

    // 穴の中心位置（ワールド座標）を2つ指定
    public Vector3[] holeCenters = new Vector3[2];

    // 穴のサイズ（メートル単位）
    public float holeSizeMeters = 2.5f;

    void Start()
    {
        TerrainData terrainData = terrain.terrainData;
        int holesResolution = terrainData.holesResolution;
        Vector3 terrainSize = terrainData.size;

        // Terrainの 1セルあたりのサイズ（メートル）
        float cellWidth = terrainSize.x / (holesResolution - 1);
        float cellHeight = terrainSize.z / (holesResolution - 1);

        int holeSizeX = Mathf.RoundToInt(holeSizeMeters / cellWidth / 2);  // 半径
        int holeSizeZ = Mathf.RoundToInt(holeSizeMeters / cellHeight / 2); // 半径

        bool[,] holes = terrainData.GetHoles(0, 0, holesResolution, holesResolution);

        foreach (var worldHoleCenter in holeCenters)
        {
            Vector3 terrainLocalPos = worldHoleCenter - terrain.transform.position;
            float relX = terrainLocalPos.x / terrainSize.x;
            float relZ = terrainLocalPos.z / terrainSize.z;
            int centerX = Mathf.RoundToInt(relX * (holesResolution - 1));
            int centerZ = Mathf.RoundToInt(relZ * (holesResolution - 1));

            for (int i = -holeSizeX; i <= holeSizeX; i++)
            {
                for (int j = -holeSizeZ; j <= holeSizeZ; j++)
                {
                    int x = centerX + i;
                    int z = centerZ + j;
                    if (x >= 0 && x < holesResolution && z >= 0 && z < holesResolution)
                    {
                        holes[z, x] = false;
                    }
                }
            }
        }

        terrainData.SetHoles(0, 0, holes);
        Debug.Log("2つの穴を開けました！（ズレ補正済）");
    }
}
