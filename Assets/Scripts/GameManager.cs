using EliteChess.Entities;
using EliteChess.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EliteChess.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] UIManager _UIManager;
        [SerializeField] AudioManger _AudioManager;
        internal Settings settings = null;
        Piece[,] Pieces = new Piece[16, 16];
        Tuple<int, int> Selected = null;
        Tuple<int, int> boardersPosition = new Tuple<int, int>(0,0);
        Player NowPlaying = Player.None;

        int players = 4;
        int BoarderTime = 5;
        int Round = -1;
        bool IsGameOver = false;
        bool IsFirstLanch = true;

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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!IsFirstLanch)
                {
                    _UIManager.ToggleMenu();
                }
            }
        }

        internal void TileClicked(int pos_i, int pos_j)
        {
            if (Pieces[pos_i,pos_j].IsBlocked)
            {
                return;
            }
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
            RefreshScreen();
        }

        internal void ShowLog(string str)
        {
            _UIManager.ShowLog(str);
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
                        Move(fromX, fromY, toX, toY);
                    }
                    break;
                case PieceType.Bishop:
                    if (BishopCanMove(fromX, fromY, toX, toY))
                    {
                        Move(fromX, fromY, toX, toY);
                    }
                    break;
                case PieceType.Rook:
                    if (RookCanMove(fromX, fromY, toX, toY))
                    {
                        Move(fromX, fromY, toX, toY);
                    }
                    break;
                case PieceType.Knight:
                    if (KnightCanMove(fromX, fromY, toX, toY))
                    {
                        Move(fromX, fromY, toX, toY);
                    }
                    break;
                default:
                    LogManager.Log("error in piece", LogType.Error);
                    break;
            }
        }

        private bool KnightCanMove(int fromX, int fromY, int toX, int toY)
        {
            if (Mathf.Abs((fromX - toX) * (fromY - toY)) == 2)
            {
                return true;
            }
            else
            {
                LogManager.Log($"cannot move rook from ({fromX},{fromY}) to ({toX},{toY})");
            }
            return false;
        }

        private bool RookCanMove(int fromX, int fromY, int toX, int toY)
        {
            var dx = Mathf.Abs(toX - fromX);
            var dy = Mathf.Abs(toY - fromY);
            if ((dy == 0 && dx != 0) || (dx == 0 && dy != 0))
            {
                if (!IsBlocked(fromX, fromY, toX, toY))
                {
                    return true;
                }
            }
            else
            {
                LogManager.Log($"cannot move rook from ({fromX},{fromY}) to ({toX},{toY})");
            }
            return false;
        }

        private bool BishopCanMove(int fromX, int fromY, int toX, int toY)
        {
            var dx = Mathf.Abs(toX - fromX);
            var dy = Mathf.Abs(toY - fromY);
            if (dx == dy && dx != 0 && dy != 0)
            {
                if (!IsBlocked(fromX, fromY, toX, toY))
                {
                    return true;
                }
            }
            else
            {
                LogManager.Log($"cannot move bishop from ({fromX},{fromY}) to ({toX},{toY})");
            }
            return false;
        }

        private void Move(int fromX, int fromY, int toX, int toY)
        {
            if (Pieces[toX,toY]._type != PieceType.None)
            {
                _AudioManager.PlayEat();
                LogManager.Log($"{Pieces[fromX, fromY]._player} {Pieces[fromX, fromY]._type} eats {Pieces[toX, toY]._player} {Pieces[toX, toY]._type}");
            }
            else
            {
                _AudioManager.PlayMove();
            }
            Pieces[toX, toY]._player = Pieces[fromX, fromY]._player;
            Pieces[toX, toY]._type = Pieces[fromX, fromY]._type;
            Pieces[fromX, fromY]._player = Player.None;
            Pieces[fromX, fromY]._type = PieceType.None;
            NextTurn();
        }

        private void NextTurn()
        {
            Round++;
            CloseBoarders();
            NowPlaying = GetNextPlayer();
            if (IsGameOver)
            {
                GameOver();
                return;
            }
        }

        private Player GetNextPlayer()
        {
            var found = false;
            Player newPlayer = Player.None;
            Player p = Player.None;
            foreach (var piece in Pieces)
            {
                if (p == Player.None && piece._type != PieceType.None)
                {
                    p = piece._player;
                }
                else if(piece._type != PieceType.None && p != piece._player)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                LogManager.Log("Game Over");
                IsGameOver = true;
                return p;
            }    
            switch (NowPlaying)
            {
                case Player.None:
                    newPlayer = (Player)Random.Range(1, 5);
                    break;
                case Player.Red:
                    newPlayer = Player.Blue;
                    break;
                case Player.Blue:
                    newPlayer = Player.Green;
                    break;
                case Player.Green:
                    newPlayer = Player.Yellow;
                    break;
                case Player.Yellow:
                    newPlayer = Player.Red;
                    break;
                default:
                    LogManager.Log("cannot find next player...", LogType.Error);
                    newPlayer = (Player)Random.Range(1, 5);
                    break;
            }
            foreach (var piece in Pieces)
            {
                if (piece._player == newPlayer && piece._type != PieceType.None)
                {
                    return newPlayer;
                }
            }
            LogManager.Log($"{newPlayer} is out of the game");
            NowPlaying = newPlayer;
            return GetNextPlayer();
        }

        private void GameOver()
        {
            foreach (var piece in Pieces)
            {
                piece.IsBlocked = true;
            }
            _UIManager.ShowWinner(NowPlaying);
        }

        private void CloseBoarders()
        {
            int startX = boardersPosition.Item1;
            int startY = boardersPosition.Item2;
            _UIManager.ShowBoarderTime(BoarderTime - (Round % BoarderTime));
            if (Round!=0 && Round % BoarderTime == 0)
            {
                Pieces[startX, startY].IsBlocked = true;
                Pieces[15-startX, 15-startY].IsBlocked = true;
                for (int x = startX+1; x < 16-startX; x++)
                {
                    Pieces[x, startY].IsBlocked = true;
                    Pieces[x, 15-startY].IsBlocked = true;
                }
                for (int y = startY + 1; y < 16 - startY; y++)
                {
                    Pieces[startX, y].IsBlocked = true;
                    Pieces[15-startX, y].IsBlocked = true;
                }
                boardersPosition = new Tuple<int, int>(startX + 1, startY + 1);
            }
            foreach (var p in Pieces)
            {
                if (p.IsBlocked)
                {
                    p._player = Player.None;
                    p._type = PieceType.None;
                }
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
            if (Pieces[fromX, fromY]._player == Pieces[toX, toY]._player && Pieces[toX, toY]._type != PieceType.None)
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

        private void RefreshScreen()
        {
            _UIManager.RefreshBoard(Pieces, Selected);
            if (!IsGameOver)
            {
                _UIManager.RefreshUI(NowPlaying);
            }
        }

        private void Start()
        {
            _UIManager.ShohwMenu(BoarderTime);
        }

        internal void BootGame(int _players, int _BoarderTime)
        {
            IsFirstLanch = false;
            players = _players;
            BoarderTime = _BoarderTime * players;
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    Pieces[i,j] = new Piece(Player.None, PieceType.None);
                }
            }
            SetPlayers();
            NextTurn();
            RefreshScreen();
        }

        private void SetPlayers()
        {
            Pieces[0, 0]._player = Player.Red;
            Pieces[0, 2]._player = Player.Red;
            Pieces[2, 0]._player = Player.Red;
            Pieces[1, 2]._player = Player.Red;
            Pieces[2, 1]._player = Player.Red;
            Pieces[1, 1]._player = Player.Red;
            Pieces[2, 2]._player = Player.Red;
            Pieces[0, 0]._type = PieceType.Queen;
            Pieces[2, 0]._type = PieceType.Rook;
            Pieces[1, 2]._type = PieceType.Rook;
            Pieces[2, 1]._type = PieceType.Bishop;
            Pieces[0, 2]._type = PieceType.Bishop;
            Pieces[1, 1]._type = PieceType.Knight;
            Pieces[2, 2]._type = PieceType.Knight;

            Pieces[15, 15]._player = Player.Blue;
            Pieces[13, 15]._player = Player.Blue;
            Pieces[15, 13]._player = Player.Blue;
            Pieces[13, 14]._player = Player.Blue;
            Pieces[14, 13]._player = Player.Blue;
            Pieces[14, 14]._player = Player.Blue;
            Pieces[13, 13]._player = Player.Blue;
            Pieces[15, 15]._type = PieceType.Queen;
            Pieces[13, 15]._type = PieceType.Rook;
            Pieces[14, 13]._type = PieceType.Rook;
            Pieces[15, 13]._type = PieceType.Bishop;
            Pieces[13, 14]._type = PieceType.Bishop;
            Pieces[14, 14]._type = PieceType.Knight;
            Pieces[13, 13]._type = PieceType.Knight;

            if (players > 2)
            {
                Pieces[0, 15]._player = Player.Yellow;
                Pieces[2, 15]._player = Player.Yellow;
                Pieces[0, 13]._player = Player.Yellow;
                Pieces[2, 14]._player = Player.Yellow;
                Pieces[1, 13]._player = Player.Yellow;
                Pieces[1, 14]._player = Player.Yellow;
                Pieces[2, 13]._player = Player.Yellow;
                Pieces[0, 15]._type = PieceType.Queen;
                Pieces[2, 15]._type = PieceType.Rook;
                Pieces[1, 13]._type = PieceType.Rook;
                Pieces[0, 13]._type = PieceType.Bishop;
                Pieces[2, 14]._type = PieceType.Bishop;
                Pieces[1, 14]._type = PieceType.Knight;
                Pieces[2, 13]._type = PieceType.Knight;
            }

            if (players > 3)
            {
                Pieces[15, 0]._player = Player.Green;
                Pieces[13, 0]._player = Player.Green;
                Pieces[15, 2]._player = Player.Green;
                Pieces[13, 1]._player = Player.Green;
                Pieces[14, 2]._player = Player.Green;
                Pieces[14, 1]._player = Player.Green;
                Pieces[13, 2]._player = Player.Green;
                Pieces[15, 0]._type = PieceType.Queen;
                Pieces[13, 0]._type = PieceType.Rook;
                Pieces[14, 2]._type = PieceType.Rook;
                Pieces[13, 1]._type = PieceType.Bishop;
                Pieces[15, 2]._type = PieceType.Bishop;
                Pieces[14, 1]._type = PieceType.Knight;
                Pieces[13, 2]._type = PieceType.Knight;
            }
        }
    }
}