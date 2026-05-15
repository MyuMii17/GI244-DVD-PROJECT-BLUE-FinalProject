using UnityEngine;
public enum ArrowType{ Normal, Ricochet, Charge }

[CreateAssetMenu(fileName = "ArrowData", menuName = "Scriptable Objects/BaseData/ArrowData")]

public class ArrowData : BaseData
{
    [Header("Arrow Settings")]
    [SerializeField] private ArrowType arrowType;
    [SerializeField] private int damage;
    [SerializeField] private float cooldownTime;

    [Header("Ricochet Settings")]
    [SerializeField] private int maxChain;
    [SerializeField] private int chainRange;
    [SerializeField] private bool isCanRicochet;
    
    [Header("Charge Settings")]
    [SerializeField] private float chargeTime;
    [SerializeField] private float maxChargeDamage;
    [SerializeField] private float minChargeDamage;
    [SerializeField] private float chargeAcceleration;
    [SerializeField] private bool isCanCharge;
}
