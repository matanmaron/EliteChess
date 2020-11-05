﻿using EliteChess.Entities;
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
        [SerializeField] Color RedHalo = Color.red;
        [SerializeField] Color BlueHalo = Color.blue;
        
        [SerializeField] List<GameObject> Center = null;
        [SerializeField] List<GameObject> AllTiles = null;

        [SerializeField] Text ScoreRed = null;
        [SerializeField] Text ScoreBlue = null;
        [SerializeField] Text ScoreYellow = null;
        [SerializeField] Text ScoreGreen = null;
        [SerializeField] Text UIText = null;

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
                    AllTiles[16 * i + j].GetComponentInChildren<Text>().text = string.Empty;
                }
            }
            foreach (var c in Center)
            {
                c.GetComponent<Image>().color = NeutralHalo;
                c.GetComponentInChildren<Text>().color = Color.black;
            }
        }

        internal void RefreshBoard(Piece[,] pieces, Tuple<int, int> selected)
        {
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    AllTiles[16 * i + j].GetComponent<Image>().color = ChangeColor();
                    if (pieces[i, j].IsCenter)
                    {
                        AllTiles[16 * i + j].GetComponent<Image>().color = GetPlayerColor(pieces[i, j]._player);
                    }

                    if (pieces[i,j]._player != Player.None)
                    {
                        HandlePlayer(j,i,pieces[i, j]);
                    }
                    else
                    {
                        AllTiles[16 * i + j].GetComponentInChildren<Text>().text = string.Empty;
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
            var txt = AllTiles[16 * i + j].GetComponentInChildren<Text>();
            txt.text = piece._type.ToString();
            txt.color = GetPlayerColor(piece._player);
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
    }
}