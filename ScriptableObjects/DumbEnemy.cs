using Events;
using UnityEngine;

public class DumbEnemy : MonoBehaviour {

    public FloatReference maxHp;
    public FloatReference moveSpeed;

    private void Awake()
    {
        EventsManager.SubscribeToEvent<EventHeroDamaged>(HeroDamage);
        EventsManager.SubscribeToEvent<EventHeroDead>(HeroDead);
    }

    private void HeroDead(EventHeroDead dataEvent)
    {
        Debug.Log(dataEvent.score);
        Debug.Log(dataEvent.continuesLeft);   
    }

    private void HeroDamage(EventHeroDamaged dataEvent)
    {
        Debug.Log("HeroDamage: " +dataEvent.damage);
        Debug.Log("HeroDamage: " +dataEvent.knockBack);   
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            EventHeroDamaged data = new EventHeroDamaged(10, new Vector3(45,45,45));
            EventsManager.DispatchEvent(data);
        }
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            EventHeroDead data = new EventHeroDead(10000, 2);
            EventsManager.DispatchEvent(data);
        }
        
        if (Input.GetKeyDown(KeyCode.H))
        {
            EventsManager.UnsubscribeToEvent<EventHeroDamaged>(HeroDamage);
        }
        
        if (Input.GetKeyDown(KeyCode.J))
        {
            EventsManager.UnsubscribeToEvent<EventHeroDead>(HeroDead);
        }
    }

    public void CheckState()
    {
        Debug.Log("My life is " + maxHp.Value);
        Debug.Log("My speed is " + moveSpeed.Value);
    }
}
