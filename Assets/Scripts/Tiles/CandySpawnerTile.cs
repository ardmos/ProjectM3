using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 디자인 레벨에서만 사용되는 타일입니다. 
/// 런타임에 GameBoardManager에게 이 타일이 캔디 스포너를 배치하는 장소라는걸 알립니다.
/// </summary>
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

        return base.StartUp(position, tilemap, go);
    }
}
