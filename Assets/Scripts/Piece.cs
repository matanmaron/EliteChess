using EliteChess.Enums;

namespace EliteChess.Entities
{
    public class Piece
    {
        public Player _player;
        public PieceType _type;
        public bool IsCenter = false;

        public Piece(Player player, PieceType type)
        {
            _player = player;
            _type = type;
        }
    }

}
