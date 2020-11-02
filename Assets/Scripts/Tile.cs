using EliteChess.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Tile : MonoBehaviour
{
    public int pos_i = -1;
    public int pos_j = -1;

    private void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(SendTile);
    }
    public void SendTile()
    {
        pos_i = 0;
        var nums = gameObject.name.Split(',');
        if (!int.TryParse(nums[0], out pos_i) || !int.TryParse(nums[1], out pos_j))
        {
            LogManager.Log($"Error converting {gameObject.name}");
        }
        GameManager.Instance.TileClicked(pos_i,pos_j);
    }

}
