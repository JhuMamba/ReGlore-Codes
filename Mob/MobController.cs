using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobController : MonoBehaviour, IDamageable
{ 
    public float detectRange = 10f;
    public float attackRange = 2f;
    public float movementSpeed = 3f;
    public float rotationSpeed = 5f;
    public float attackCooldown = 2f;
    public Character characterStats;
    public SwordAttack swordAttack;

    private Animator animator;
    private Transform player;
    private MobAudio m_Audio;
    private bool isAttacking;

    private List<string> animations;

    public float health;
    public float damage;
    public bool IsDead;

    void Start()
    {
        animator = GetComponent<Animator>();
        m_Audio = GetComponent<MobAudio>();

        animations = new List<string>()
        {
            "Hit1",
            "Fall1",
            "Attack1h1",
        };
        health = characterStats.Health;
        damage = characterStats.Damage;
        IsDead = false;
    }

    void Update()
    {
        if (IsDead) return;
        // Find the player
        if (player == null)
        {
            player = FindPlayer();
        }
        else
        {
            // Check if the player is within detect range
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectRange)
            {
                // Rotate towards the player
                RotateTowards(player.position);

                // If the player is within attack range, attack
                if (distanceToPlayer <= attackRange && !isAttacking)
                {
                    animator.SetFloat("speedv", 0.0f);
                    StartCoroutine(Attack());
                }
                else if (isAttacking) return;
                else
                {
                    // Move towards the player
                    MoveTowards(player.position);
                }
            }
            else
            {
                animator.SetFloat("speedv", 0.0f);
            }
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;

        PlayAnimation("Attack1h1");
        swordAttack.InitAttack(damage);
        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
    }

    void MoveTowards(Vector3 targetPosition)
    {
        // Calculate the direction to the target
        Vector3 direction = (targetPosition - transform.position).normalized;
        // Use a ray to check for obstacles in the movement path
        Ray ray = new Ray(transform.position + new Vector3(0f,1f,0f), direction);
        if (Physics.Raycast(ray, out RaycastHit hit, 1.5f))
        {
            animator.SetFloat("speedv", 0f);
            return;
        }
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
        animator.SetFloat("speedv", 1.0f);
    }

    void RotateTowards(Vector3 targetPosition)
    {
        // Rotate towards the target position
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion toRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
    }

    Transform FindPlayer()
    {
        // You may need to customize this based on how your player is identified in the scene
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            return playerObject.transform;
        }

        return null;
    }

    void PlayAnimation(string animationName)
    {
        // Check if the requested animation is in the list
        if (animations.Contains(animationName))
        {
            // Play the animation
            animator.Play(animationName);
        }
        else
        {
            Debug.LogWarning("Animation " + animationName + " not found in the list.");
        }
    }

    public void GetHit(float damageAmount)
    {
        m_Audio.PlayHitSFX();
        health -= damageAmount;
        if (health <= 0)
        {
            IsDead = true;
            GetComponent<CapsuleCollider>().enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;
            PlayAnimation("Fall1");
        }
    }
}
