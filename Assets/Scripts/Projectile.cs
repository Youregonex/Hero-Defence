using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Debug Fields")]
    [SerializeField] private int _damage;

    public int Damage => _damage;
}
