using UnityEngine;

namespace Entity
{
    /*
     * A chain of segments make up a snake.
     * Segment models one of the segments of a snake.
     */
    public class Segment : MonoBehaviour
    {
        public Snake snake { get; set; }
        public Vector3Int gridPosition;
        public Direction CurrentDirection { get; set; }

        [HideInInspector]
        public Animator myAnimator;

        private void Awake()
        {
            myAnimator = GetComponent<Animator>();
        }

        public void AnimMessage1()
        {
            snake.processFood(snake.segments.IndexOf(this));
        }

        // Moves the segment wherever it should move next.
        // Rotates segment to face its CurrenDirection.
        // cellSize is the size of the cells on the tilemap the snake is moving on.
        public void move(Vector3 cellSize)
        {
            float angle;

            switch (CurrentDirection)
            {
                case Direction.Right:
                    gridPosition.x += 1;
                    angle = 180;
                    break;
                case Direction.Left:
                    gridPosition.x -= 1;
                    angle = 0;
                    break;
                case Direction.Up:
                    gridPosition.y += 1;
                    angle = 90;
                    break;
                case Direction.Down:
                    gridPosition.y -= 1;
                    angle = -90;
                    break;
                default:
                    angle = 0;
                    break;
            }

            transform.position = snake.toWorld(gridPosition);

            Quaternion q = Quaternion.AngleAxis(angle, -1 * Vector3.forward);
            transform.rotation = q;
        }
    }
}
