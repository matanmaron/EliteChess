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
        
        [SerializeField] List<GameObject> AllTiles = null;
        [SerializeField] Text UIText = null;
        [SerializeField] Text BoarderText = null;
        [SerializeField] Text LogText = null;

        [SerializeField] Sprite None = null;
        [SerializeField] Sprite Bishop = null;
        [SerializeField] Sprite Knight = null;
        [SerializeField] Sprite Rook = null;
        [SerializeField] Sprite Queen = null;

        [SerializeField] GameObject Menu = null;
        [SerializeField] InputField BoarderTime = null;

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
                }
                ChangeColor();
            }
            if (selected != null)
            {
                AllTiles[16 * selected.Item1 + selected.Item2].GetComponent<Image>().color = Color.magenta;
            }
        }

        internal void ShowLog(string str)
        {
            LogText.text = str;
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

        internal void ShowBoarderTime(int time)
        {
            BoarderText.text = $"Boarders Closing in: {time}";
        }

        internal void ToggleMenu()
        {
            Menu.SetActive(!Menu.activeSelf);
        }

        public void OnQuitButton()
        {
            Application.Quit();
        }

        public void OnTwoPlayersButton()
        {
            GameManager.Instance.BootGame(2, int.Parse(BoarderTime.text));
            Menu.SetActive(false);
        }
        public void OnThreePlayersButton()
        {
            GameManager.Instance.BootGame(3, int.Parse(BoarderTime.text));
            Menu.SetActive(false);
        }
        public void OnFourPlayersButton()
        {
            GameManager.Instance.BootGame(4, int.Parse(BoarderTime.text));
            Menu.SetActive(false);
        }

        internal void ShohwMenu(int boarderTime)
        {
            BoarderTime.text = boarderTime.ToString();
            Menu.SetActive(true);
        }
    }
}