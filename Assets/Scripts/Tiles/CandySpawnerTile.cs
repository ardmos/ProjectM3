using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "CandySpawnerPlacerTile", menuName = "2D Match/Tile/Candy Spawner Placer")]
public class CandySpawnerTile : TileBase
{
    public Sprite EditorPreviewSprite;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = !Application.isPlaying ? EditorPreviewSprite : null;
    }

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            return false;
#endif

        // Register the spawner position
        GameBoardManager.RegisterSpawner(position);

        // Remove the tile from the Tilemap after registering it
        /*        Tilemap map = tilemap.GetComponent<Tilemap>();
                if (map != null)
                {
                    map.SetTile(position, null); // Remove the tile at this position
                }*/

        Debug.Log($"tilemap:{tilemap}, go:{go}");


        return base.StartUp(position, tilemap, go);
    }
}
