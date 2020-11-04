using EliteChess.Entities;
using EliteChess.Enums;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EliteChess.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] UIManager _UIManager;
        internal Settings settings = null;
        Piece[,] Pieces = new Piece[16, 16];
        Tuple<int, int> Selected = null;
        Player NowPlaying = Player.None;

        public static GameManager Instance { get; private set; } //singleton
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        internal void TileClicked(int pos_i, int pos_j)
        {
            LogManager.Log($"clicked ({pos_i},{pos_j})");
            if (Selected == null)
            {
                Selected = new Tuple<int, int>(pos_i, pos_j);
            }
            else
            {
                if (Pieces[Selected.Item1, Selected.Item2]._type != PieceType.None)
                {
                    LogManager.Log($"move {Selected.Item1},{Selected.Item2} to {pos_i},{pos_j}");
                    MovePiece(Pieces[Selected.Item1, Selected.Item2], Selected.Item1, Selected.Item2, pos_i, pos_j);
                }
                Selected = null;
            }
            RefreshBoard();
        }

        private void MovePiece(Piece piece, int fromX, int fromY, int toX, int toY)
        {
            if (piece._player != NowPlaying)
            {
                LogManager.Log("this is not yours !");
                return;
            }
            switch (piece._type)
            {
                case PieceType.Queen:
                    if (QueenCanMove(fromX, fromY, toX, toY))
                    {
                        MoveQueen(fromX, fromY, toX, toY);
                    }
                    break;
                case PieceType.Bishop:
                    break;
                case PieceType.Rook:
                    break;
                case PieceType.Night:
                    break;
                default:
                    LogManager.Log("error in piece...", LogType.Error);
                    break;
            }
        }

        private void MoveQueen(int fromX, int fromY, int toX, int toY)
        {
            if (Pieces[toX,toY]._type != PieceType.None)
            {
                LogManager.Log($"{Pieces[fromX, fromY]._player} {Pieces[fromX, fromY]._type} eats {Pieces[toX, toY]._player} {Pieces[toX, toY]._type}");
            }
            Pieces[toX, toY]._player = Pieces[fromX, fromY]._player;
            Pieces[toX, toY]._type = Pieces[fromX, fromY]._type;
            Pieces[fromX, fromY]._player = Player.None;
            Pieces[fromX, fromY]._type = PieceType.None;
            NowPlaying = NextTurnPlayer();
        }

        private Player NextTurnPlayer()
        {
            switch (NowPlaying)
            {
                case Player.None:
                    return (Player)Random.Range(1, 5);
                case Player.Red:
                    return Player.Blue;
                case Player.Blue:
                    return Player.Green;
                case Player.Green:
                    return Player.Yellow;
                case Player.Yellow:
                    return Player.Red;
                default:
                    LogManager.Log("cannot find next player...", LogType.Error);
                    return (Player)Random.Range(1, 5);
            }
        }

        private bool QueenCanMove(int fromX, int fromY, int toX, int toY)
        {
            var dx = Mathf.Abs(toX - fromX);
            var dy = Mathf.Abs(toY - fromY);
            if ((dy == 0 && dx != 0) || (dx == 0 && dy != 0) || (dx == dy && dx != 0 && dy != 0))
            {
                if (!IsBlocked(fromX, fromY, toX, toY))
                {
                    return true;
                }
            }
            else
            {
                LogManager.Log($"cannot move queen from ({fromX},{fromY}) to ({toX},{toY})");
            }
            return false;
        }

        private bool IsBlocked(int fromX, int fromY, int toX, int toY)
        {
            if (Pieces[fromX, fromY]._player == Pieces[toX, toY]._player)
            {
                LogManager.Log($"{Pieces[toX, toY]._type} ({toX},{toY}) in the way!");
                return true;
            }
            else if(fromX < toX && fromY == toY) //down
            {
                for (int x = fromX + 1; x < toX; x++)
                {
                    if (Pieces[x, fromY]._type != PieceType.None)
                    {
                        LogManager.Log($"{Pieces[x, fromY]._type} ({x},{fromY}) in the way!");
                        return true;
                    }
                }
            }
            else if (fromX < toX && fromY < toY) //right down
            {
                for (int x = fromX + 1, y = fromY + 1; x < toX && y < toY; x++, y++)
                {
                    if (Pieces[x, y]._type != PieceType.None)
                    {
                        LogManager.Log($"{Pieces[x, y]._type} ({x},{y}) in the way!");
                        return true;
                    }
                }
            }
            else if (fromX == toX && fromY < toY) //right
            {
                for (int y = fromY + 1; y < toY; y++)
                {
                    if (Pieces[fromX, y]._type != PieceType.None)
                    {
                        LogManager.Log($"{Pieces[fromX, y]._type} ({fromX},{y}) in the way!");
                        return true;
                    }
                }
            }
            else if (fromX > toX && fromY < toY) //right up
            {
                for (int x = fromX - 1, y = fromY + 1; x > toX && y < toY; x--, y++)
                {
                    if (Pieces[x, y]._type != PieceType.None)
                    {
                        LogManager.Log($"{Pieces[x, y]._type} ({x},{y}) in the way!");
                        return true;
                    }
                }
            }
            else if (fromX > toX && fromY == toY) //up
            {
                for (int x = fromX - 1; x > toX; x--)
                {
                    if (Pieces[x, fromY]._type != PieceType.None)
                    {
                        LogManager.Log($"{Pieces[x, fromY]._type} ({x},{fromY}) in the way!");
                        return true;
                    }
                }
            }
            else if (fromX > toX && fromY > toY) //left up
            {
                for (int x = fromX - 1, y = fromY - 1; x > toX && y > toY; x--, y--)
                {
                    if (Pieces[x, y]._type != PieceType.None)
                    {
                        LogManager.Log($"{Pieces[x, y]._type} ({x},{y}) in the way!");
                        return true;
                    }
                }
            }
            else if (fromX == toX && fromY > toY) //left
            {
                for (int y = fromY - 1; y > toY; y--)
                {
                    if (Pieces[fromX, y]._type != PieceType.None)
                    {
                        LogManager.Log($"{Pieces[fromX, y]._type} ({fromX},{y}) in the way!");
                        return true;
                    }
                }
            }
            else if (fromX < toX && fromY > toY) //left down
            {
                for (int x = fromX + 1, y = fromY - 1; x < toX && y > toY; x++, y--)
                {
                    if (Pieces[x, y]._type != PieceType.None)
                    {
                        LogManager.Log($"{Pieces[x, y]._type} ({x},{y}) in the way!");
                        return true;
                    }
                }
            }
            return false;
        }

        private void RefreshBoard()
        {
            _UIManager.RefreshBoard(Pieces, Selected);
            _UIManager.RefreshUI(NowPlaying);
        }

        private void Start()
        {
            BootGame();
        }

        private void BootGame()
        {
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    Pieces[i,j] = new Piece(Player.None, PieceType.None);
                }
            }
            SetPlayers();
            NowPlaying = NextTurnPlayer();
            RefreshBoard();
        }

        private void SetPlayers()
        {
            Pieces[0, 0]._player = Player.Red;
            Pieces[0, 0]._type = PieceType.Queen;
            Pieces[0, 2]._player = Player.Red;
            Pieces[0, 2]._type = PieceType.Rook;
            Pieces[2, 0]._player = Player.Red;
            Pieces[2, 0]._type = PieceType.Rook;
            Pieces[1, 2]._player = Player.Red;
            Pieces[1, 2]._type = PieceType.Bishop;
            Pieces[2, 1]._player = Player.Red;
            Pieces[2, 1]._type = PieceType.Bishop;
            Pieces[1, 1]._player = Player.Red;
            Pieces[1, 1]._type = PieceType.Night;
            Pieces[2, 2]._player = Player.Red;
            Pieces[2, 2]._type = PieceType.Night;

            Pieces[15, 15]._player = Player.Blue;
            Pieces[15, 15]._type = PieceType.Queen;
            Pieces[13, 15]._player = Player.Blue;
            Pieces[13, 15]._type = PieceType.Rook;
            Pieces[15, 13]._player = Player.Blue;
            Pieces[15, 13]._type = PieceType.Rook;
            Pieces[13, 14]._player = Player.Blue;
            Pieces[13, 14]._type = PieceType.Bishop;
            Pieces[14, 13]._player = Player.Blue;
            Pieces[14, 13]._type = PieceType.Bishop;
            Pieces[14, 14]._player = Player.Blue;
            Pieces[14, 14]._type = PieceType.Night;
            Pieces[13, 13]._player = Player.Blue;
            Pieces[13, 13]._type = PieceType.Night;

            Pieces[0, 15]._player = Player.Yellow;
            Pieces[0, 15]._type = PieceType.Queen;
            Pieces[2, 15]._player = Player.Yellow;
            Pieces[2, 15]._type = PieceType.Rook;
            Pieces[0, 13]._player = Player.Yellow;
            Pieces[0, 13]._type = PieceType.Rook;
            Pieces[2, 14]._player = Player.Yellow;
            Pieces[2, 14]._type = PieceType.Bishop;
            Pieces[1, 13]._player = Player.Yellow;
            Pieces[1, 13]._type = PieceType.Bishop;
            Pieces[1, 14]._player = Player.Yellow;
            Pieces[1, 14]._type = PieceType.Night;
            Pieces[2, 13]._player = Player.Yellow;
            Pieces[2, 13]._type = PieceType.Night;

            Pieces[15, 0]._player = Player.Green;
            Pieces[15, 0]._type = PieceType.Queen;
            Pieces[13, 0]._player = Player.Green;
            Pieces[13, 0]._type = PieceType.Rook;
            Pieces[15, 2]._player = Player.Green;
            Pieces[15, 2]._type = PieceType.Rook;
            Pieces[13, 1]._player = Player.Green;
            Pieces[13, 1]._type = PieceType.Bishop;
            Pieces[14, 2]._player = Player.Green;
            Pieces[14, 2]._type = PieceType.Bishop;
            Pieces[14, 1]._player = Player.Green;
            Pieces[14, 1]._type = PieceType.Night;
            Pieces[13, 2]._player = Player.Green;
            Pieces[13, 2]._type = PieceType.Night;
        }
    }
}