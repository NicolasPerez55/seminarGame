using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageHandler : MonoBehaviour
{
    public TetrominoData piece;

    public SpriteRenderer render;
    public PreviewEntries previewEntries;

    public bool used;

    public void UpdateDisplay()
    {
        if (piece.tetromino == Tetromino.Empty)
        {
            render.sprite = null;

            return;
        }

        render.sprite = previewEntries.entries[piece.tetromino];
    }

    public void AddToStorage(TetrominoData data)
    {
        piece = data;
    }
}
