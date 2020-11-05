using EliteChess.Enums;
using System;

namespace EliteChess.Entities
{
    public class Piece
    {
        public Player _player;
        public PieceType _type;
        public bool IsCenter = false;
        public bool IsBlocked = false;

        public Piece(Player player, PieceType type)
        {
            _player = player;
            _type = type;
        }
    }

}
