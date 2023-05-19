using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using Entity;

namespace Managers
{
    /*
     * LevelManager initializes the scene and manages changes in the game.
     * Implements the singleton pattern, but not thread safe.
     */
    public class LevelManager : MonoBehaviour
    {
        // Singleton
        public static LevelManager self;

        // X value of the bottom-left corner of the map.
        public int BLCornerX
        { get { return -1 * (mapWidth / 2); } }
        // y value of the bottom-left corner of the map.
        public int BLCornerY
        { get { return -1 * (mapLength / 2); } }
        // True if the game should be paused.
        public bool Pause { get; private set; }
        // Current food instantitated in scene.
        public GameObject foodInstance { get; private set; }

        [SerializeField]
        private GameObject snakePrefab;
        [SerializeField]
        private Vector3Int spawnPosition;
        [SerializeField]
        private RuleTile ruleTile;
        [SerializeField]
        private int mapWidth = 5, mapLength = 5;
        [SerializeField]
        private GameObject foodPrefab;
        [SerializeField]
        private UnityEngine.UI.Text score;
        [SerializeField]
        private KeyCode menuKey;

        [HideInInspector]
        public Snake player;

        private Grid grid;
        private Tilemap[] tilemaps;
        private List<Vector3Int> mapTiles;
        private Utility.RandomTilePicker tilePicker;

        private void Awake()
        {
            // Initializing Singleton (Not thread safe)
            if (self == null)
                self = this;
            else
                return;

            grid = FindObjectOfType<Grid>();
            tilemaps = grid.transform.GetComponentsInChildren<Tilemap>(false);
        }

        private void Start()
        {
            if (tilemaps.Length == 0)
                throw new System.InvalidOperationException("Grid does not have any layers.");

            createTilemap(tilemaps[0], ruleTile);

            mapTiles = new List<Vector3Int>();
            for (int a = BLCornerY + 1; a < mapLength + BLCornerY - 1; a++)
            {
                for (int b = BLCornerX + 1; b < mapWidth + BLCornerX - 1; b++)
                {
                    mapTiles.Add(new Vector3Int(b, a, 0));
                }
            }
            tilePicker = new Utility.RandomTilePicker(mapTiles);

            spawnPlayer();
            spawnFood();
        }

        private void Update()
        {
            if (Input.GetKeyDown(menuKey) && !MenuManager.self.Sliding)
            {
                if(MenuManager.self.IsOpen)
                {
                    float time = MenuManager.self.close();
                    Invoke("setPauseToFalse", time);
                }
                else
                {
                    MenuManager.self.open();
                    Pause = true;
                }
            }

            if (foodInstance == null && player.Head.CurrentDirection != Direction.None)
                spawnFood();

            // Updates score
            score.text = "" + (player.Length - 1);
        }

        // Registers a collision between a wall collider and a moving collider.
        // registerCollision must be called for every such collision in the scene.
        public void registerTrigger(Snake snake, GameObject trigger, CollisionType type)
        {
            if (type == CollisionType.Wall || type == CollisionType.Segment)
            {
                snake.die();
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else if (type == CollisionType.Food)
            {
                Destroy(trigger);
                snake.addSegment();
                snake.startProcessFood();

                foodInstance = null;
            }
        }

        // Spawns player at spawn location.
        private void spawnPlayer()
        {
            GameObject instance = Instantiate(snakePrefab);
            instance.transform.position = tilemaps[0].GetCellCenterWorld(spawnPosition);

            player = instance.GetComponent<Snake>();
            player.map = tilemaps[0];
        }

        // Creates a mapWidth x mapLength tilemap.
        // tile is the RuleTile to fill the map.
        private void createTilemap(Tilemap map, RuleTile tile)
        {
            for (int l = BLCornerY; l < BLCornerY + mapLength; l++)
            {
                for (int w = BLCornerX; w < BLCornerX + mapWidth; w++)
                {
                    Vector3Int position = new Vector3Int(w, l, 0);
                    map.SetTile(position, tile);
                }
            }
        }

        // Spawns food on a random tile that the player is not on.
        private void spawnFood()
        {
            Vector3Int spawnLocation = tilePicker.getRandomTile(player.getOccupiedTiles());
            foodInstance = Instantiate(foodPrefab);
            foodInstance.transform.position = tilemaps[0].GetCellCenterWorld(spawnLocation);
        }

        // Sets Pause to false.
        private void setPauseToFalse()
        {
            Pause = false;
        }
    }
}
