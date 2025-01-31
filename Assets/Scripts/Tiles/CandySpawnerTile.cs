using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// ������ ���������� ���Ǵ� Ÿ���Դϴ�. 
/// ��Ÿ�ӿ� GameBoardManager���� �� Ÿ���� ĵ�� �����ʸ� ��ġ�ϴ� ��Ҷ�°� �˸��ϴ�.
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
