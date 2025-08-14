using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerUpgradeUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panel;
    [SerializeField] private Animator anim;

    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI controlText;
    [SerializeField] private TextMeshProUGUI efficiencyText;
    [SerializeField] private TMP_Dropdown priorityDropdown;

    private Turret selectedTurret;

    public void Open(Turret turret)
    {
        if (turret == null)
        {
            Debug.LogWarning("Tentando abrir UI com torre nula.");
            return;
        }

        // Atualiza valor atual no dropdown
        if (priorityDropdown != null)
        {
            priorityDropdown.onValueChanged.RemoveAllListeners(); // evita múltiplos bindings
            priorityDropdown.value = (int)turret.GetAttackPriority();
            priorityDropdown.onValueChanged.AddListener(OnPriorityDropdownChanged);
        }

        selectedTurret = turret;

        UpdateUpgradeTexts();
        anim.SetBool("UpgradeOpen", true); // Abre o painel via animação
    }

    public void Close()
    {
        anim.SetBool("UpgradeOpen", false); // Fecha via botão
        selectedTurret = null;
    }

    private void UpdateUpgradeTexts()
    {
        UpdatePathText(TowerUpgradePath.Damage, damageText);
        UpdatePathText(TowerUpgradePath.Control, controlText);
        UpdatePathText(TowerUpgradePath.Efficiency, efficiencyText);
    }

    private void UpdatePathText(TowerUpgradePath path, TextMeshProUGUI textElement)
    {
        int currentLevel = selectedTurret.GetLevelForPath(path);
        TowerUpgradeLevel nextLevel = selectedTurret.GetNextUpgradeLevel(path);

        if (nextLevel != null)
        {
            textElement.text = $"Nível {currentLevel} (Custo: {nextLevel.goldCost}G)";
        }
        else
        {
            textElement.text = $"Nível {currentLevel} (Máximo)";
        }
    }

    public void UpgradeDamage() => TryUpgrade(TowerUpgradePath.Damage);
    public void UpgradeControl() => TryUpgrade(TowerUpgradePath.Control);
    public void UpgradeEfficiency() => TryUpgrade(TowerUpgradePath.Efficiency);

    private void TryUpgrade(TowerUpgradePath path)
    {
        if (selectedTurret != null)
        {
            selectedTurret.UpgradePath(path);
            UpdateUpgradeTexts(); // Atualiza a UI depois do upgrade
        }
        else
        {
            Debug.LogWarning($"Nenhuma torre selecionada para upgrade de {path}.");
        }
    }
    public void OnPriorityDropdownChanged(int value)
    {
        if (selectedTurret != null)
        {
            selectedTurret.SetAttackPriority((AttackPriority)value);
        }
    }
}
