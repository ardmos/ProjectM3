using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


/// <summary>
/// 디자인 레벨에서만 사용되는 타일입니다. 
/// 런타임에는 GameBoardManager에게 이 타일이 캔디를 배치하는 장소라는걸 알리고, GameBoardManager는 이 게임오브젝트를 파괴합니다.
/// </summary>
[CreateAssetMenu(fileName = "CandyPlacerTile", menuName = "2D Match/Tile/Candy Placer")]
public class CandyPlacerTile : TileBase
{

    public Sprite PreviewEditorSprite;
    [Tooltip("If null this will be a random gem")]
    public Candy PlacedCandy = null;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {       
        tileData.sprite = !Application.isPlaying ? PreviewEditorSprite : null;
    }

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            return false;
#endif

        GameBoardManager.RegisterCell(position, PlacedCandy);

        return base.StartUp(position, tilemap, go);
    }
}
