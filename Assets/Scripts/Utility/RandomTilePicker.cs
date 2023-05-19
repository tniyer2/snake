using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    /*
     * Picks a random tile in mapTiles.
     * Tiles can be blacklisted so RandomTilePicker won't select them.
     * Vector3Int will be used to store tile's location on its tilemap.
     */
    public class RandomTilePicker
    {
        private List<Vector3Int> mapTiles;

        public RandomTilePicker(List<Vector3Int> mapTiles)
        {
            this.mapTiles = mapTiles;
        }

        // Picks a random tile on the map that is not blacklisted.
        // Pre-Condition: blackList must only contain tiles that mapTiles contains. 
        public Vector3Int getRandomTile(List<Vector3Int> blackList)
        {
            int whiteListCount = mapTiles.Count - blackList.Count;

            if (whiteListCount == 0)
                throw new System.InvalidOperationException("There are no tiles left to pick.");
            if (whiteListCount < 0)
                throw new System.InvalidOperationException("There are more tiles blacklisted than the number of tiles on the map.");

            int rand = Random.Range(1, whiteListCount + 1);

            Vector3Int tilePicked = Vector3Int.zero;
            int counter = 0;
            for (int a = 0; a < mapTiles.Count; a++)
            {
                if (!blackList.Contains(mapTiles[a]))
                    counter++;
                if (counter == rand)
                {
                    tilePicked = mapTiles[a];
                    break;
                }
            }

            return tilePicked;
        }
    }
}