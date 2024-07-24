using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Baddie : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 3f;
    [SerializeField] private float _damageThreshold = 0.2f;
    [SerializeField] private GameObject _baddieDeathParticles;
    [SerializeField] private AudioClip _baddieDeathSound;
    private float _currentHealth;

    //awake
    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void DamageBaddie(float damageAmount)
    {
        _currentHealth -= damageAmount;
        if (_currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.Instance.BaddieKilled();
        Instantiate(_baddieDeathParticles, transform.position, Quaternion.identity);

        AudioSource.PlayClipAtPoint(_baddieDeathSound, transform.position);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float impactVelocity = collision.relativeVelocity.magnitude;
        if (impactVelocity > _damageThreshold)
        {
            DamageBaddie(impactVelocity);
        }
    }
}
