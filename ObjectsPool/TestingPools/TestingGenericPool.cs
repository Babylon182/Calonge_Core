using ObjectsPool;
using UnityEditor;
using UnityEngine;

public class TestingGenericPool : MonoBehaviour {

	public Bullet bulletPrefab;
	private GenericPool<Bullet> bulletPool;    

	private static TestingGenericPool instance;
	public static TestingGenericPool Instance { get { return instance; } }
         
	void Awake()
	{
		instance = this;
		bulletPool = new GenericPool<Bullet>(8, BulletFactory, Bullet.InitializeBullet, Bullet.DisposeBullet);
	}

	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			bulletPool.GetObjectFromPool();
		}
		
		if (Input.GetKeyDown(KeyCode.J))
		{
			ReturnBulletToPool(Selection.activeGameObject.GetComponent<Bullet>());
		}
	}

	private Bullet BulletFactory()
	{
		return Instantiate<Bullet>(bulletPrefab);
	}

	public void ReturnBulletToPool(Bullet bullet)
	{
		bulletPool.ReturnObjectToPool(bullet);
	}
}
