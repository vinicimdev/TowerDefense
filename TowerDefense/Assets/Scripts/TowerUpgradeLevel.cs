using UnityEngine;

[CreateAssetMenu(fileName = "New Tower Upgrade Level", menuName = "Tower/Tower Upgrade Level")]
public class TowerUpgradeLevel : ScriptableObject
{
    public int damageModifier;
    public float fireRateModifier;
    public float rangeModifier;
    public int goldCost = 0;

    // Outros modificadores que quiser, como:
    // public float splashRadius;
    // public float slowAmount;
}
