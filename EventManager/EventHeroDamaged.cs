using Events;
using UnityEngine;

public class EventHeroDamaged : IGameEvent
{
    public float damage;
    public Vector3 knockBack;

    public EventHeroDamaged(float damage, Vector3 knockBack)
    {
        this.damage = damage;
        this.knockBack = knockBack;
    }
}

public class EventHeroDead : IGameEvent
{
    public float score;
    public int continuesLeft;

    public EventHeroDead(float score, int continuesLeft)
    {
        this.score = score;
        this.continuesLeft = continuesLeft;
    }
}