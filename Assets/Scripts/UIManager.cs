using EliteChess.Entities;
using EliteChess.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EliteChess.Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] Color NeutralHalo = Color.white;
        
        [SerializeField] List<GameObject> Center = null;
        [SerializeField] List<GameObject> AllTiles = null;

        [SerializeField] Text ScoreRed = null;
        [SerializeField] Text ScoreBlue = null;
        [SerializeField] Text ScoreYellow = null;
        [SerializeField] Text ScoreGreen = null;
        [SerializeField] Text UIText = null;

        [SerializeField] Sprite None = null;
        [SerializeField] Sprite Bishop = null;
        [SerializeField] Sprite Knight = null;
        [SerializeField] Sprite Rook = null;
        [SerializeField] Sprite Queen = null;

        Color WhiteTile = new Color(1f, 0.80784313725f, 0.61960784313f);
        Color BlackTile = new Color(0.81960784313f, 0.54509803921f, 0.27843137254f);
        bool colorFlip = false;

        private void Awake()
        {
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    AllTiles[16 *i +j].name = $"{i},{j}";
                }
            }
        }

        internal void RefreshBoard(Piece[,] pieces, Tuple<int, int> selected)
        {
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    AllTiles[16 * i + j].GetComponentInChildren<Image>().sprite = None;
                    AllTiles[16 * i + j].GetComponent<Image>().color = ChangeColor();

                    if (pieces[i,j]._player != Player.None)
                    {
                        HandlePlayer(j,i,pieces[i, j]);
                    }

                    if (pieces[i, j].IsBlocked)
                    {
                        AllTiles[16 * i + j].GetComponent<Image>().color = Color.grey;
                    }

                    if (pieces[i, j].IsCenter)
                    {
                        AllTiles[16 * i + j].GetComponent<Image>().color = GetPlayerColor(pieces[i, j]._player);
                    }
                }
                ChangeColor();
            }
            if (selected != null)
            {
                AllTiles[16 * selected.Item1 + selected.Item2].GetComponent<Image>().color = Color.magenta;
            }
        }

        private Color ChangeColor()
        {
            if (colorFlip)
            {
                colorFlip = !colorFlip;
                return BlackTile;
            }
            colorFlip = !colorFlip;
            return WhiteTile;
        }

        private void HandlePlayer(int j, int i, Piece piece)
        {
            var img = AllTiles[16 * i + j].GetComponentInChildren<Image>();
            img.sprite = GetSprite(piece._type);
            img.color = GetPlayerColor(piece._player);
        }

        private Sprite GetSprite(PieceType _type)
        {
            switch (_type)
            {
                case PieceType.Queen:
                    return Queen;
                case PieceType.Bishop:
                    return Bishop;
                case PieceType.Rook:
                    return Rook;
                case PieceType.Knight:
                    return Knight;
                default:
                    return None;
            }
        }

        private Color GetPlayerColor(Player player)
        {
            switch (player)
            {
                case Player.Red:
                    return Color.red;
                case Player.Blue:
                    return Color.blue;
                case Player.Green:
                    return Color.green;
                case Player.Yellow:
                    return Color.yellow;
                default:
                    return Color.white;
            }
        }

        internal void RefreshUI(Player nowPlaying)
        {
            UIText.text = $"Turn: {nowPlaying}";
        }

        internal void RefreshScore(int _scoreRed, int _scoreBlue, int _scoreYellow, int _scoreGreen)
        {
            ScoreRed.text = _scoreRed.ToString();
            ScoreBlue.text = _scoreBlue.ToString();
            ScoreYellow.text = _scoreYellow.ToString();
            ScoreGreen.text = _scoreGreen.ToString();
        }

        internal void ShowWinner(Player p)
        {
            if (p == Player.None)
            {
                UIText.text = "IT'S A TIE !";
            }
            else
            {
                UIText.text = $"{p.ToString().ToUpper()} IS THE WINNER !";
            }
        }
    }
}