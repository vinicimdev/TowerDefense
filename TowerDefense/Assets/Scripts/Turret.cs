using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public enum TowerUpgradePath
{
    None,
    Damage,
    Control,
    Efficiency
}
public enum AttackPriority
{
    First,
    Last,
    MostTank
}

public class Turret : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform turretRotationPoint;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private GameObject towerRange;
    [SerializeField] private TowerUpgradeData upgradeData;
    [SerializeField] private SpriteRenderer baseRenderer; // Referência no Inspector

    [Header("Base Attributes")]
    [SerializeField] private float baseRange = 3;
    [SerializeField] private float baseFireRate = 1;
    [SerializeField] private int baseDamage = 1;
    [SerializeField] private float rotationSpeed = 200;
    [SerializeField] private float rangeScaleFactor = 1f;

    [Header("Targeting Settings")]
    [SerializeField] private AttackPriority attackPriority = AttackPriority.First;

    private List<Transform> enemiesInRange = new List<Transform>();

    private float targetingRange;
    private float bps;
    private int damage;

    private Transform target;
    private float timeUntilFire;
    private bool isTowerSelected = false;

    private Dictionary<TowerUpgradePath, int> upgradeLevels = new();


    private void Start()
    {
        ApplyUpgradeStats();

        if (towerRange != null)
        {
            float initialScale = towerRange.transform.localScale.x;
            rangeScaleFactor = initialScale / (baseRange * 2f); // scale.x / (raio * 2)
        }

        Debug.Log("Damage path count: " + upgradeData.damagePathLevels.Count);
        Debug.Log("Control path count: " + upgradeData.controlPathLevels.Count);
        Debug.Log("Efficiency path count: " + upgradeData.efficiencyPathLevels.Count);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hitCollider = Physics2D.OverlapPoint(mousePos);

            if (hitCollider == null || hitCollider.transform != gameObject.transform)
            {
                isTowerSelected = false;
                towerRange.SetActive(false);
            }
        }

        // Checa se o alvo ainda é válido
        if (target == null || !IsTargetInRange(target))
        {
            target = FindTarget();
        }

        if (target == null)
        {
            return;
        }

        RotateTowardsTarget();
        timeUntilFire += Time.deltaTime;

        if (timeUntilFire >= 1f / bps)
        {
            Shoot();
            timeUntilFire = 0f;
        }
    }

    private void Shoot()
    {
        GameObject bulletObj = Instantiate(bulletPrefab, firingPoint.position, Quaternion.identity);
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        bulletScript.SetTarget(target);
        bulletScript.SetDamage(damage);

        // Verifica se o caminho de dano chegou ao nível 2 ou mais -> Ativa AOE
        if (upgradeLevels.TryGetValue(TowerUpgradePath.Damage, out int damageLevel) && damageLevel >= 2)
        {
            Debug.Log("Aplicando dano em área com nível 2 de dano.");
            bulletScript.EnableAreaDamage(1.5f); // Ajustável
        }

        // Verifica se o caminho de controle chegou ao nível 2 ou mais -> Ativa congelamento
        if (upgradeLevels.TryGetValue(TowerUpgradePath.Control, out int controlLevel) && controlLevel >= 2)
        {
            bulletScript.SetFreeze(1f); // Tempo de congelamento ajustável
        }

        // Se quiser futuramente adicionar algo no caminho Efficiency, é só fazer:
        // if (upgradeLevels.TryGetValue(TowerUpgradePath.Efficiency, out int effLevel) && effLevel >= X)
    }

    private Transform FindTarget()
    {
        enemiesInRange.Clear();

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, targetingRange, enemyMask);
        foreach (Collider2D hit in hits)
        {
            enemiesInRange.Add(hit.transform);
        }

        if (enemiesInRange.Count == 0)
        {
            return null;
        }

        switch (attackPriority)
        {
            case AttackPriority.First:
                enemiesInRange.Sort((a, b) => a.GetInstanceID().CompareTo(b.GetInstanceID()));
                return enemiesInRange[0];

            case AttackPriority.Last:
                return enemiesInRange[enemiesInRange.Count - 1];

            case AttackPriority.MostTank:
                Transform mostTank = null;
                float highestHealth = -1f;
                foreach (var enemy in enemiesInRange)
                {
                    Health enemyScript = enemy.GetComponent<Health>();
                    if (enemyScript != null && enemyScript.hitPoints > highestHealth)
                    {
                        highestHealth = enemyScript.hitPoints;
                        mostTank = enemy;
                    }
                }
                return mostTank;

            default:
                return enemiesInRange[0];
        }
    }
    private bool IsTargetInRange(Transform potentialTarget)
    {
        if (potentialTarget == null)
            return false;

        return Vector2.Distance(transform.position, potentialTarget.position) <= targetingRange;
    }


    private bool CheckTargetIsInRange()
    {
        return Vector2.Distance(target.position, transform.position) <= targetingRange;
    }

    private void RotateTowardsTarget()
    {
        float angle = Mathf.Atan2(target.position.y - transform.position.y, target.position.x - transform.position.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        turretRotationPoint.rotation = Quaternion.RotateTowards(turretRotationPoint.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void OnMouseDown()
    {
        isTowerSelected = true;

        if (towerRange != null)
        {
            towerRange.SetActive(true);
        }
        else
        {
            Debug.LogWarning("towerRange não atribuído na torre: " + name);
        }

        // Exibir UI de upgrade
        TowerUpgradeUI ui = FindObjectOfType<TowerUpgradeUI>();
        if (ui != null)
        {
            ui.Open(this);
        }
        else
        {
            Debug.LogWarning("TowerUpgradeUI não encontrado na cena.");
        }
    }

    public void UpgradePath(TowerUpgradePath chosenPath)
    {
        int currentLevel = GetLevelForPath(chosenPath);
        TowerUpgradeLevel nextLevelData = GetUpgradeLevelData(chosenPath, currentLevel);
        if (nextLevelData == null)
        {
            Debug.Log("Não há mais upgrades disponíveis para esse caminho.");
            return;
        }

        if (!upgradeLevels.ContainsKey(chosenPath))
            upgradeLevels[chosenPath] = 0;

        // Conta quantos caminhos já foram usados e o total de upgrades
        int totalUpgrades = upgradeLevels.Values.Sum();
        int pathsUsed = upgradeLevels.Count(kvp => kvp.Value > 0);

        // Impede mais de 4 upgrades no total
        if (totalUpgrades >= 4)
        {
            Debug.Log("Limite total de upgrades atingido (4).");
            return;
        }

        // Impede mais de 3 upgrades em qualquer caminho
        if (upgradeLevels[chosenPath] >= 3)
        {
            Debug.Log("Máximo de 3 upgrades por caminho.");
            return;
        }

        // Se for um caminho novo (ainda não usado)
        bool isNewPath = upgradeLevels[chosenPath] == 0;

        // Se já tem dois caminhos com upgrades e está tentando iniciar um terceiro
        if (isNewPath && pathsUsed >= 2)
        {
            Debug.Log("Você só pode usar dois caminhos diferentes.");
            return;
        }

        // Se este for o caminho secundário (nível 1) e já está tentando passar de 1
        if (!isNewPath && pathsUsed == 2 && upgradeLevels[chosenPath] >= 1)
        {
            var primaryPath = upgradeLevels.FirstOrDefault(kvp => kvp.Key != chosenPath && kvp.Value >= 2).Key;
            if (primaryPath != TowerUpgradePath.None && primaryPath != chosenPath)
            {
                Debug.Log("O caminho secundário só pode ter 1 upgrade.");
                return;
            }
        }

        //  Só agora tenta gastar o ouro
        if (!LevelManager.main.SpendGold(nextLevelData.goldCost))
        {
            Debug.Log("Ouro insuficiente para upgrade.");
            return;
        }

        // Aplica o upgrade
        upgradeLevels[chosenPath]++;
        ApplyUpgradeStats();
    }





    private TowerUpgradeLevel GetUpgradeLevelData(TowerUpgradePath path, int index)
    {
        switch (path)
        {
            case TowerUpgradePath.Damage:
                return upgradeData.damagePathLevels[index];
            case TowerUpgradePath.Control:
                return upgradeData.controlPathLevels[index];
            case TowerUpgradePath.Efficiency:
                return upgradeData.efficiencyPathLevels[index];
            default:
                return new TowerUpgradeLevel();
        }
    }

    private int GetMaxLevelForPath(TowerUpgradePath path)
    {
        switch (path)
        {
            case TowerUpgradePath.Damage:
                return upgradeData.damagePathLevels.Count;
            case TowerUpgradePath.Control:
                return upgradeData.controlPathLevels.Count;
            case TowerUpgradePath.Efficiency:
                return upgradeData.efficiencyPathLevels.Count;
            default:
                return 0;
        }
    }

    private void ApplyUpgradeStats()
    {
        damage = baseDamage;
        bps = baseFireRate;
        targetingRange = baseRange;

        foreach (var kvp in upgradeLevels)
        {
            for (int i = 0; i < kvp.Value; i++)
            {
                TowerUpgradeLevel data = GetUpgradeLevelData(kvp.Key, i);
                damage += data.damageModifier;
                bps += data.fireRateModifier;
                targetingRange += data.rangeModifier;
            }
        }

        if (towerRange != null)
        {
            float diameter = targetingRange * 2f * rangeScaleFactor;
            towerRange.transform.localScale = new Vector3(diameter, diameter, 1f);
        }

        if (baseRenderer != null)
        {
            baseRenderer.color = upgradeLevels.Count switch
            {
                1 => Color.yellow,
                2 => Color.red,
                _ => Color.white
            };
        }
    }
    private TowerUpgradeLevel GetCurrentUpgradeLevel()
    {
        // Opcional: retorna o maior nível do caminho com maior upgrade
        var mainPath = TowerUpgradePath.None;
        int highestLevel = 0;

        foreach (var kvp in upgradeLevels)
        {
            if (kvp.Value > highestLevel)
            {
                mainPath = kvp.Key;
                highestLevel = kvp.Value;
            }
        }

        return highestLevel > 0 ? GetUpgradeLevelData(mainPath, highestLevel - 1) : new TowerUpgradeLevel();
    }
    private bool IsPrimaryPath(TowerUpgradePath path)
    {
        return upgradeLevels.TryGetValue(path, out int lvl) && lvl == 3;
    }

    public TowerUpgradeLevel GetNextUpgradeLevel(TowerUpgradePath path)
    {
        if (path == TowerUpgradePath.None)
            return null;

        int level = upgradeLevels.ContainsKey(path) ? upgradeLevels[path] : 0;

        List<TowerUpgradeLevel> levels = path switch
        {
            TowerUpgradePath.Damage => upgradeData.damagePathLevels,
            TowerUpgradePath.Control => upgradeData.controlPathLevels,
            TowerUpgradePath.Efficiency => upgradeData.efficiencyPathLevels,
            _ => null
        };

        if (levels == null || level >= levels.Count)
            return null;

        return levels[level];
    }

    public int GetLevelForPath(TowerUpgradePath path)
    {
        return upgradeLevels.TryGetValue(path, out var level) ? level : 0;
    }
    public void SetAttackPriority(AttackPriority newPriority)
    {
        attackPriority = newPriority;
    }

    public AttackPriority GetAttackPriority()
    {
        return attackPriority;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, targetingRange);
    }
#endif
}
