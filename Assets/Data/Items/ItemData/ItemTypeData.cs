using UnityEngine;

[CreateAssetMenu(fileName = "New Item Type", menuName = "STALKER/Item Type")]
public class ItemTypeData : ScriptableObject
{
    public string typeName;
    public Color editorColor = Color.white;
    public float defaultHealthEffect = 0f;
    public float defaultRadiationEffect = 0f;
    public float defaultStaminaEffect = 0f;
    public int maxStackSize = 99;
    public bool canBeEquipped = false;
}