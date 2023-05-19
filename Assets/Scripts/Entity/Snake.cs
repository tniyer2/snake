using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Entity
{
    /*
     * Snake is the entity that the player will control.
     * It is made up of multiple segments.
     */
    public class Snake : MonoBehaviour, IDamageable
    {
        public Segment Head
        {
            get
            {
                if (segments == null || segments.Count == 0)
                    return null;
                return segments[0];
            }
        }
        public Segment Tail
        {
            get
            {
                if (segments == null || segments.Count < 2)
                    return null;
                return segments[segments.Count - 1];
            }
        }
        public int Length
        {
            get
            {
                if (segments == null)
                    return 0;
                else
                    return segments.Count;
            }
        }

        public Tilemap map { get; set; }
        // Implements IDamageable
        public float Health { get; private set; }

        // All prefabs must have a Segment component
        [SerializeField]
        private GameObject headPrefab;
        [SerializeField]
        private GameObject bodyPrefab;
        [SerializeField]
        private GameObject tailPrefab;
        [SerializeField]
        private float moveSpeed = 10f;
        [SerializeField]
        private int animationDistance;  // Distance Snake has to be from food to animate.
        [SerializeField]
        private int digestLimit;        // Segment to stop digesting at.
        [SerializeField]
        private AudioClip eatingClip;

        public Dictionary<Vector3Int, Direction> turnPoints;
        [HideInInspector]
        public List<Segment> segments;

        private AudioSource audioSource;
        private float moveTimeCounter;
        private bool firstDirection = true;

        private void Awake()
        {
            GameObject head = Instantiate(headPrefab, transform);

            segments = new List<Segment>();
            segments.Add(head.GetComponent<Segment>());
            segments[0].snake = this;

            turnPoints = new Dictionary<Vector3Int, Direction>();
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            Vector3Int gridPosition = map.WorldToCell(transform.position);
            segments[0].gridPosition = gridPosition;
        }

        private void Update()
        {
            if (Managers.LevelManager.self.Pause)
                return;

            if (firstDirection)
                setStartDirection();
            else
                tryAddTurnPoint();

            moveTimeCounter += Time.deltaTime;
            if (moveTimeCounter >= (1 / moveSpeed))
            {
                moveTimeCounter = 0;
                moveSegments();
            }

            checkProximity();
        }

        // Pre-Condition: segments is initialized and has at least one element(Head).
        // Inserts a segment after the tail (pushes tail back one)
        public void addSegment()
        {
            bool addingTail = segments.Count == 1;

            GameObject prefab = addingTail ? tailPrefab : bodyPrefab;

            GameObject instance = Instantiate(prefab, transform);
            Segment segment = instance.GetComponent<Segment>();
            segment.snake = this;

            if (addingTail)
            {
                segments.Add(segment);
                segment.gridPosition = Head.gridPosition - Head.CurrentDirection.toVector3Int();
                segment.CurrentDirection = Head.CurrentDirection;
            }
            else
            {
                // Inserts before the last element(tail)
                segments.Insert(segments.Count - 1, segment);
                segment.gridPosition = Tail.gridPosition;
                segment.CurrentDirection = Tail.CurrentDirection;

                Tail.gridPosition -= Tail.CurrentDirection.toVector3Int();
                Tail.transform.position = toWorld(Tail.gridPosition);
            }

            segment.transform.position = toWorld(segment.gridPosition);
        }

        // Pre-Condition: firstDirection is false.
        // Sets all segments to inputted direction.
        // Post-Condition: firstDirection is true.
        private void setStartDirection()
        {
            if (Head != null)
            {
                Direction newDirection = getDirection();

                if (newDirection != Direction.None)
                {
                    firstDirection = false;

                    foreach (Segment seg in segments)
                        seg.CurrentDirection = newDirection;
                }
            }
        }

        // Tries to add a turn point where the Head is.
        // Fails if getDirection() returns Direction.None.
        private void tryAddTurnPoint()
        {
            Direction newDirection = getDirection();

            if (newDirection == Direction.None ||
                newDirection == Head.CurrentDirection ||
                newDirection == (Direction)(-1 * (int)Head.CurrentDirection))
                return;

            turnPoints[Head.gridPosition] = newDirection;
        }

        // Calls move() on all segments
        private void moveSegments()
        {
            for (int a = 0; a < segments.Count; a++)
            {
                Segment segment = segments[a];
                bool last = a == segments.Count - 1;

                if (turnPoints.ContainsKey(segment.gridPosition))
                {
                    segment.CurrentDirection = turnPoints[segment.gridPosition];
                    if (last)
                    {
                        turnPoints.Remove(segment.gridPosition);
                    }
                }

                segment.move(map.cellSize);
            }
        }

        // Returns the direction from the input
        private Direction getDirection()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                return Direction.Right;
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                return Direction.Left;
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                return Direction.Up;
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                return Direction.Down;

            return Direction.None;
        }

        // Returns a list of tiles that the segments of this snake are on.
        public List<Vector3Int> getOccupiedTiles()
        {
            if (segments == null)
                throw new System.InvalidOperationException();

            List<Vector3Int> tiles = new List<Vector3Int>();
            foreach (Segment seg in segments)
                tiles.Add(seg.gridPosition);

            return tiles;
        }

        // Converts cell coordinate to world position
        public Vector3 toWorld(Vector3Int cell)
        {
            return map.GetCellCenterWorld(cell);
        }

        // index is the index of the segment calling processFood();
        public void processFood(int index)
        {
            if (index == digestLimit || index >= segments.Count - 1)
                return;

            segments[index + 1].myAnimator.SetTrigger("Digest");
        }

        // Animates segments processing food
        public void startProcessFood()
        {
            processFood(0);
            audioSource.clip = eatingClip;
            audioSource.Play();
        }

        // If snake is close enough to food, plays animation.
        public void checkProximity()
        {
            Vector3Int foodPosition = map.WorldToCell(Managers.LevelManager.self.foodInstance.transform.position);
            int distance = (int)(Head.gridPosition - foodPosition).magnitude;

            bool close = distance <= animationDistance;

            Head.myAnimator.SetBool("Open", close);
        }

        // Implements IDamageable
        // Destroys this GameObject 
        public void die()
        {
            Destroy(gameObject);
        }

        // Implements IDamageable
        // Reduces health by damage and dies if health <= 0
        public void takeDamage(float damage)
        {
            Health -= damage;

            if (Health <= 0)
                die();
        }
    }
}