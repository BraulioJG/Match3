using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tileObject;

    public float cameraSizeOffset;
    public float cameraVerticalOffset;

    public GameObject[] availablePieces;

    Tile[,] tiles;
    Piece[,] pieces;

    Tile startTile;
    Tile endTile;

    // Start is called before the first frame update
    void Start()
    {
        tiles = new Tile[width, height];
        pieces = new Piece[width, height];
        SetupBoard();
        PositionCamera();
        SetupPieces();
    }

    private void SetupPieces()
    {
        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                var selectedPiece = availablePieces[UnityEngine.Random.Range(0, availablePieces.Length)];
                var o = Instantiate(selectedPiece, new Vector3(x, y, -5), Quaternion.identity);
                o.transform.parent = transform;
                pieces[x, y] = o.GetComponent<Piece>();
                pieces[x, y]?.Setup(x, y, this);
            }
        }
        if (CheckMatchNonDestructive())
        {
            DeleteAllPieces();
            SetupPieces();
        }
    }

    private void SetupBoard()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                var o = Instantiate(tileObject, new Vector3(x, y, -5), Quaternion.identity);
                o.transform.parent = transform;
                tiles[x,y] = o.GetComponent<Tile>();
                tiles[x,y]?.Setup(x, y, this);
            }
        }
    }

    private void PositionCamera()
    {
        float newPosX = (float)width / 2f;
        float newPosY = (float)height / 2f;
        Camera.main.transform.position = new Vector3(newPosX - 0.5f, newPosY - 0.5f + cameraVerticalOffset, -10f);

        float horizontal = width + 1;
        float vertical = (height / 2) + 1;
        Camera.main.orthographicSize = horizontal > vertical ? horizontal + cameraSizeOffset : vertical;
    }

    public void TileDown(Tile tile_)
    {
        startTile = tile_;
    }

    public void TileOver(Tile tile_)
    {
        endTile = tile_;
    }

    public void TileUp(Tile tile_)
    {
        if (startTile != null && endTile != null && IsCloseTo(startTile, endTile))
        {
            SwapTiles();
            CheckMatchDestructive();
        }
    }

    private void SwapTiles()
    {
        var StartPiece = pieces[startTile.x, startTile.y];
        var EndPiece = pieces[endTile.x, endTile.y];

        StartPiece.Move(endTile.x, endTile.y);
        EndPiece.Move(startTile.x, startTile.y);

        pieces[startTile.x,startTile.y] = EndPiece;
        pieces[endTile.x, endTile.y] = StartPiece;
    }

    private void SwapTiles(Tile startTile, Tile endTile)
    {
        var StartPiece = pieces[startTile.x, startTile.y];
        var EndPiece = pieces[endTile.x, endTile.y];

        StartPiece.Move(endTile.x, endTile.y);
        EndPiece.Move(startTile.x, startTile.y);

        pieces[startTile.x, startTile.y] = EndPiece;
        pieces[endTile.x, endTile.y] = StartPiece;
    }

    public bool IsCloseTo(Tile start, Tile end)
    {
        if(Math.Abs(start.x - end.x) == 1 && start.y == end.y)
        {
            return true;
        }
        if (Math.Abs(start.y - end.y) == 1 && start.x == end.x)
        {
            return true;
        }
        return false;
    }

    public bool CheckMatchDestructive()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (x + 1 < width && x - 1 >= 0)
                {
                    if (pieces[x, y].pieceType == pieces[x + 1, y].pieceType && pieces[x, y].pieceType == pieces[x - 1, y].pieceType)
                    {
                        if (pieces[x + 1, y].gameObject != null)
                        {
                            Destroy(pieces[x + 1, y].gameObject);
                        }
                        if (pieces[x - 1, y].gameObject != null)
                        {
                            Destroy(pieces[x - 1, y].gameObject);
                        }
                        if (pieces[x, y].gameObject != null)
                        {
                            Destroy(pieces[x, y].gameObject);
                        }
                        DownRowXMatch(x, y);
                        return true;
                    }
                }
                if (y + 1 < height && y - 1 >= 0)
                {
                    if (pieces[x, y].pieceType == pieces[x, y + 1].pieceType && pieces[x, y].pieceType == pieces[x, y - 1].pieceType)
                    {
                        if (pieces[x, y + 1].gameObject != null)
                        {
                            Destroy(pieces[x, y + 1].gameObject);
                        }
                        if (pieces[x, y - 1].gameObject != null)
                        {
                            Destroy(pieces[x, y - 1].gameObject);
                        }
                        if (pieces[x, y].gameObject != null)
                        {
                            Destroy(pieces[x, y].gameObject);
                        }
                        DownRowYMatch(x, y);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool CheckMatchNonDestructive()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (x + 1 < width && x - 1 >= 0)
                {
                    if (pieces[x, y].pieceType == pieces[x + 1, y].pieceType && pieces[x, y].pieceType == pieces[x - 1, y].pieceType)
                    {
                        return true;
                    }
                }
                if (y + 1 < height && y - 1 >= 0)
                {
                    if (pieces[x, y].pieceType == pieces[x, y + 1].pieceType && pieces[x, y].pieceType == pieces[x, y - 1].pieceType)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool DeleteAllPieces()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Destroy(pieces[x, y].gameObject);
            }
        }
        return false;
    }

    public void DownRowYMatch(int x, int yLimit)
    {
        for (yLimit = yLimit + 2; yLimit < height; yLimit++)
        {
            pieces[x, yLimit].Move(x, yLimit - 3);
            SwapTiles(tiles[x, yLimit], tiles[x, yLimit - 3]);
        }
        var selectedPiece1 = availablePieces[UnityEngine.Random.Range(0, availablePieces.Length)];
        var o1 = Instantiate(selectedPiece1, new Vector3(x, height - 3, -5), Quaternion.identity);
        o1.transform.parent = transform;
        pieces[x, height - 3] = o1.GetComponent<Piece>();
        pieces[x, height - 3]?.Setup(x, height - 3, this);
        var selectedPiece2 = availablePieces[UnityEngine.Random.Range(0, availablePieces.Length)];
        var o2 = Instantiate(selectedPiece2, new Vector3(x, height - 2, -5), Quaternion.identity);
        o2.transform.parent = transform;
        pieces[x, height - 2] = o2.GetComponent<Piece>();
        pieces[x, height - 2]?.Setup(x, height - 2, this);
        var selectedPiece3 = availablePieces[UnityEngine.Random.Range(0, availablePieces.Length)];
        var o3 = Instantiate(selectedPiece3, new Vector3(x, height - 1, -5), Quaternion.identity);
        o3.transform.parent = transform;
        pieces[x, height - 1] = o3.GetComponent<Piece>();
        pieces[x, height - 1]?.Setup(x, height - 1, this);
        CheckMatchDestructive();
    }

    public void DownRowXMatch(int x, int yLimit)
    {
        for (yLimit = yLimit + 1; yLimit < height; yLimit++)
        {
            pieces[x, yLimit].Move(x, yLimit - 1);
            SwapTiles(tiles[x, yLimit], tiles[x, yLimit - 1]);
            pieces[x - 1, yLimit].Move(x - 1, yLimit - 1);
            SwapTiles(tiles[x - 1, yLimit], tiles[x - 1, yLimit - 1]);
            pieces[x + 1, yLimit].Move(x + 1, yLimit - 1);
            SwapTiles(tiles[x + 1, yLimit], tiles[x + 1, yLimit - 1]);
        }
        var selectedPiece1 = availablePieces[UnityEngine.Random.Range(0, availablePieces.Length)];
        var o1 = Instantiate(selectedPiece1, new Vector3(x, height - 1, -5), Quaternion.identity);
        o1.transform.parent = transform;
        pieces[x, height - 1] = o1.GetComponent<Piece>();
        pieces[x, height - 1]?.Setup(x, height - 1, this);
        var selectedPiece2 = availablePieces[UnityEngine.Random.Range(0, availablePieces.Length)];
        var o2 = Instantiate(selectedPiece2, new Vector3(x - 1, height - 1, -5), Quaternion.identity);
        o2.transform.parent = transform;
        pieces[x - 1, height - 1] = o2.GetComponent<Piece>();
        pieces[x - 1, height - 1]?.Setup(x - 1, height - 1, this);
        var selectedPiece3 = availablePieces[UnityEngine.Random.Range(0, availablePieces.Length)];
        var o3 = Instantiate(selectedPiece3, new Vector3(x + 1, height - 1, -5), Quaternion.identity);
        o3.transform.parent = transform;
        pieces[x + 1, height - 1] = o3.GetComponent<Piece>();
        pieces[x + 1, height - 1]?.Setup(x + 1, height - 1, this);
        CheckMatchDestructive();
    }

}