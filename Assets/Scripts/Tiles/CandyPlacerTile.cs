using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


/// <summary>
/// ������ ���������� ���Ǵ� Ÿ���Դϴ�. 
/// ��Ÿ�ӿ��� GameBoardManager���� �� Ÿ���� ĵ�� ��ġ�ϴ� ��Ҷ�°� �˸���, GameBoardManager�� �� ���ӿ�����Ʈ�� �ı��մϴ�.
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
