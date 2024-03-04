using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using JetBrains.Annotations;

public class Piece : MonoBehaviour
{
    public int x;
    public int y;
    public Board board;

    public enum type
    {
        elephant,
        giraffe,
        hippo,
        monkey,
        panda,
        parrot,
        penguin,
        pig,
        rabbit,
        snake
    };

    public type pieceType;

    public void Setup(int x_, int y_, Board board_)
    {
        this.x = x_;
        this.y = y_;
        this.board = board_;
    }

    public void Move(int desx, int desy)
    {
        transform.DOMove(new Vector3(desx, desy, -5f), 0.25f).SetEase(Ease.InOutCubic).onComplete = () => { x = desx; y = desy; };
    }

    [ContextMenu("Test Move")]
    public void MoveTest()
    {
        Move(0, 0);
    }
}
