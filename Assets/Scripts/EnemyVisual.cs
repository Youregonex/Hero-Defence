using UnityEngine;
using DG.Tweening;

public class EnemyVisual : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] float _fadeInTime = .3f;
    [SerializeField] Color _fadeFromColor;

    [Header("Debug Fields")]
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        _spriteRenderer.DOColor(_fadeFromColor, _fadeInTime).From();
    }
}