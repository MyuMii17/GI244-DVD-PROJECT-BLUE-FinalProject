using UnityEngine;

[CreateAssetMenu(fileName = "BaseData", menuName = "Scriptable Objects/BaseData")]
public class BaseData : ScriptableObject
{
    [Header("Main Settings")]
    [SerializeField] private Sprite Icon;
    [SerializeField] private GameObject Prefeb;
    [SerializeField] private string ObjectName;
    [SerializeField, TextArea(3, 10)] private string Description;
    [SerializeField] private float Mass;
    [SerializeField] private float Acceleration;
    [SerializeField] private float LinearDamp;
    [SerializeField] private bool IsCanDestroy;
}
