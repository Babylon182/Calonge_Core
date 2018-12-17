using UnityEngine;

public class Manager : MonoBehaviour {

    public GameEvent gameEvent;

	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameEvent.Raise();
        }
	}
}
