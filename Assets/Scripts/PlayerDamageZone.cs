using UnityEngine;
using System;

public class PlayerDamageZone : MonoBehaviour
{
    public event Action<Enemy> OnEnemyEntered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Enemy enemy))
        {
            OnEnemyEntered?.Invoke(enemy);
        }
    }
}
