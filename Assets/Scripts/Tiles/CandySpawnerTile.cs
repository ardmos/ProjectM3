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

        //This tile is only used in editor to help design the level. At runtime, we notify the board that this tile is
        //a place for a gem. The Board will take care of creating a gem there.
        GameBoardManager.RegisterSpawner(position);

        return base.StartUp(position, tilemap, go);
    }
}
