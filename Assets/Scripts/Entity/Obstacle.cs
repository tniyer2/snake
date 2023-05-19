using UnityEngine;

namespace Entity
{
    // Obstacle registers trigger collisions with a Snake.
    // Attach to the GameObject you want the obstacle to be.
    // GameObject must have a trigger collider.
    public class Obstacle : MonoBehaviour
    {
        [SerializeField]
        private CollisionType collisionType;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            Segment segment = collider.GetComponent<Segment>();

            if (segment == null)
                return;
            if (segment.transform.parent == null)
                throw new System.InvalidOperationException("segment does not have a parent GameObject: " + segment.name);

            Snake snake = segment.transform.parent.GetComponent<Snake>();
            if (snake == null)
                throw new System.InvalidOperationException("segment's parent is missing a Snake component: " + segment.name);

            Managers.LevelManager.self.registerTrigger(snake, gameObject, collisionType);
        }
    }
}
