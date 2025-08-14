using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Tower Upgrade Data", menuName = "Tower/Tower Upgrade Data")]
public class TowerUpgradeData : ScriptableObject
{
    public List<TowerUpgradeLevel> damagePathLevels;
    public List<TowerUpgradeLevel> controlPathLevels;
    public List<TowerUpgradeLevel> efficiencyPathLevels;
}
