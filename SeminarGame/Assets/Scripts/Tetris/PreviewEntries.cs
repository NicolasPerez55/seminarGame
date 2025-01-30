using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewEntries : MonoBehaviour
{
    public TetrominoDic[] tetrominoEntries;
    public Dictionary<Tetromino, Sprite> entries;

    void Awake()
    {
        entries = new Dictionary<Tetromino, Sprite>();

        for (int i = 0; i < tetrominoEntries.Length; ++i)
        {
            entries.Add(tetrominoEntries[i].type, tetrominoEntries[i].sprite);
        }
    }
}

[System.Serializable]
public struct TetrominoDic
{
    public Tetromino type;
    public Sprite sprite;
}
