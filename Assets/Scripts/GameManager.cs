using EliteChess.Entities;
using EliteChess.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EliteChess.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] UIManager _UIManager;
        internal Settings settings = null;
        Piece[,] Pieces = new Piece[16, 16];
        Tuple<int, int> Selected = null;

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
            if (Pieces[pos_i,pos_j]._player == Player.None && Selected == null)
            {
                Selected = new Tuple<int, int>(pos_i, pos_j);
            }
            else
            {
                Selected = null;
            }
            RefreshBoard();
        }

        private void RefreshBoard()
        {
            _UIManager.RefreshBoard(Pieces, Selected);
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
            Pieces[13, 15]._type = PieceType.Rook;
            Pieces[13, 15]._player = Player.Blue;
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
        }
    }
}