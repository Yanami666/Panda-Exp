using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ChessPuzzleSlot : MonoBehaviour
{
    [Header("这个格子需要的棋子 ID / Required piece ID for this slot")]
    public int requiredPieceID = 0;

    [Header("当前是否已被正确棋子占用 / Is filled with correct piece")]
    public bool isOccupied = false;
}