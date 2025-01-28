using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueHandler : MonoBehaviour
{
    public List<TetrominoData> queuedPieces;
    public int queueLength = 4;

    public PreviewEntries previewEntries;
    public SpriteRenderer[] renderers;

    public void UpdateDisplay()
    {
        for (int i = 0; i < renderers.Length; ++i)
        {
            renderers[i].sprite = previewEntries.entries[queuedPieces[i].tetromino];
        }
    }
}
