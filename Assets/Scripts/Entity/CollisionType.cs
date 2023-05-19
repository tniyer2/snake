
namespace Entity
{
    // CollisionType describes the types of objects a snake can collide with.
    [System.Serializable]
    public enum CollisionType
    {
        Default = 0, Wall = 1, Segment = 2, Food = 3
    }
}
