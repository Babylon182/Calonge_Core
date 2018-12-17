using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
	public GameEvent gameEvent;
	public UnityEvent response;

    private void OnEnable()
    {
        gameEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        gameEvent.RegisterListener(this);
    }

    public void OnEventRaised()
    {
        response.Invoke();
    }

    public void OnDestroy()
    {
        gameEvent.ClearListeners();
    }
}
