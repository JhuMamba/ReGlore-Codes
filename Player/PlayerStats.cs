using JhutenFPP.PlayerControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour, IDamageable
{
    public Image HealthBar;

    private float maxHealth;
    private float health;
    private float _damage;
    
    [SerializeField]
    private Character playerSo;

    private PlayerAudio _playerAudio;
    public float Health => health;
    public float Damage => _damage;
    public float Armor { get; set; }

    private void Awake()
    {
        maxHealth = playerSo.Health;
        _damage = playerSo.Damage;

        health = maxHealth;
    }
    private void Start()
    {
        _playerAudio = GetComponent<PlayerAudio>();
    }
    public void GetHit(float damage)
    {
        _playerAudio.PlayHitSFX();
        health -= damage - (Armor / damage);
        UpdateHealthBar();
        if (health <= 0) PlayerController.Instance.IsDead = true;
    }
    public void UseConsumable(int amount)
    {
        health = (health + amount >= maxHealth) ? maxHealth : health + amount;
        UpdateHealthBar();
    }
    private void UpdateHealthBar()
    {
        HealthBar.fillAmount = health / maxHealth;
    }
}
