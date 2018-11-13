using UnityEngine;

public class Bullet : MonoBehaviour
{
    public static void InitializeBullet(Bullet obj)
    {
        obj.gameObject.SetActive(true);
    }

    public static void DisposeBullet(Bullet obj)
    {
        obj.gameObject.SetActive(false);
    }
}
