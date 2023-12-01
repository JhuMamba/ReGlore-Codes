using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    [SerializeField] Collider swordCollider;
    [SerializeField] string targetTag;
    [SerializeField] float _damage;
    private float _currentDamage;
    private bool isAttackedOnce = false;

    public bool isEquipped = false;
    private void Awake()
    {
        swordCollider = GetComponent<Collider>();
    }
    private void Start()
    {
        swordCollider.enabled = false;
    }
    public void InitAttack(float damage)
    {
        _currentDamage = _damage + damage;
        isAttackedOnce = false;
        StartCoroutine(EnableSwordCollider());
    }
    IEnumerator EnableSwordCollider()
    {
        // Wait for a short duration before enabling the collider
        yield return new WaitForSeconds(0.3f);

        // Enable the sword collider to detect hits
        swordCollider.enabled = true;

        // Disable the sword collider after a short duration
        yield return new WaitForSeconds(0.7f);

        swordCollider.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if ( other.CompareTag(targetTag) && !isAttackedOnce)
        {
            IDamageable targetObject = other.GetComponent<IDamageable>();
            if ( targetObject != null )
            {
                targetObject.GetHit(_currentDamage);
                isAttackedOnce=true;
            }
        }
    }
}
