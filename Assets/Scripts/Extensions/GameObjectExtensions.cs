using UnityEngine;

public static class GameObjectExtensions
{
    public static void DestroyAllChildren(this GameObject gameObject)
    {
        var transform = gameObject.transform;
        for (var i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i);
            child.transform.SetParent(null);
            GameObject.Destroy(child.gameObject);
        }
    }
}
