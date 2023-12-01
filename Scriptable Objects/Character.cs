using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ReGlore/Create Character")]
public class Character : ScriptableObject
{
    [SerializeField] private float health;
    [SerializeField] private float damage;
    [SerializeField] private string targetTag;
    public float Health => health;
    public float Damage => damage;
    public string TargetTag => targetTag;
}
