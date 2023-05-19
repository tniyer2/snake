using UnityEngine;

// Simple class for focusing Camera on the player.
// Attach script to the GameObject with the Camera.
public class LookAtPlayer : MonoBehaviour
{
    private void Update()
    {
        Entity.Snake snake = Managers.LevelManager.self.player;

        if (snake != null)
        {
            Transform target = snake.Head.transform;
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
        }
    }
}
