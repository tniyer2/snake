using UnityEngine;

namespace Entity
{
    // A collection of extension methods for the Direction enum.
    public static class DirectionExtensions
    {
        // Converts a Direction enum into a Vector3Int.
        public static Vector3Int toVector3Int(this Direction d)
        {
            switch (d)
            {
                case Direction.Right:
                    return new Vector3Int(1, 0, 0);
                case Direction.Left:
                    return new Vector3Int(-1, 0, 0);
                case Direction.Up:
                    return new Vector3Int(0, 1, 0);
                case Direction.Down:
                    return new Vector3Int(0, -1, 0);
                default:
                    return new Vector3Int(0, 0, 0);
            }
        }
    }
}
