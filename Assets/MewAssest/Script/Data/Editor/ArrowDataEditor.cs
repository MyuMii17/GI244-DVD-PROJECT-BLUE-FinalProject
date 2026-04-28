using UnityEditor;
    
[CustomEditor(typeof(ArrowData))]
public class ArrowDataEditor : Editor
{
    SerializedProperty arrowType;
    SerializedProperty damage;
    SerializedProperty cooldownTime;
    SerializedProperty maxChain;
    SerializedProperty chainRange;
    SerializedProperty isCanRicochet;
    SerializedProperty chargeTime;
    SerializedProperty maxChargeDamage;
    SerializedProperty minChargeDamage;
    SerializedProperty chargeAcceleration;
    SerializedProperty isCanCharge;

    void OnEnable()
    {
        arrowType = serializedObject.FindProperty("arrowType");
        damage = serializedObject.FindProperty("damage");
        cooldownTime = serializedObject.FindProperty("cooldownTime");
        maxChain = serializedObject.FindProperty("maxChain");
        chainRange = serializedObject.FindProperty("chainRange");
        isCanRicochet = serializedObject.FindProperty("isCanRicochet");
        chargeTime = serializedObject.FindProperty("chargeTime");
        maxChargeDamage = serializedObject.FindProperty("maxChargeDamage");
        minChargeDamage = serializedObject.FindProperty("minChargeDamage");
        chargeAcceleration = serializedObject.FindProperty("chargeAcceleration");
        isCanCharge = serializedObject.FindProperty("isCanCharge");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        DrawPropertiesExcluding(serializedObject, 
            "arrowType", "damage", "cooldownTime",
            "maxChain", "chainRange", "isCanRicochet",
            "chargeTime", "maxChargeDamage", "minChargeDamage",
            "chargeAcceleration", "isCanCharge");

        EditorGUILayout.PropertyField(arrowType);
        EditorGUILayout.PropertyField(damage);
        EditorGUILayout.PropertyField(cooldownTime);

        var type = (ArrowType)arrowType.enumValueIndex;
        switch (type)
        {
            case ArrowType.Normal:
                break;
            case ArrowType.Ricochet:
                EditorGUILayout.PropertyField(maxChain);
                EditorGUILayout.PropertyField(chainRange);
                EditorGUILayout.PropertyField(isCanRicochet);
                break;
            case ArrowType.Charge:
                EditorGUILayout.PropertyField(chargeTime);
                EditorGUILayout.PropertyField(maxChargeDamage);
                EditorGUILayout.PropertyField(minChargeDamage);
                EditorGUILayout.PropertyField(chargeAcceleration);
                EditorGUILayout.PropertyField(isCanCharge);
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }        
}
