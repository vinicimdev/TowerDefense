using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TowerIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string towerDescription;
    public TextMeshProUGUI descriptionText;
    private bool isMouseOver = false;
    public Vector2 offset = new Vector2(1000, -20); // Offset para a posição do texto (X, Y)

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Ativa e atualiza o texto de descrição
        descriptionText.text = towerDescription;
        Vector2 mousePosition = Input.mousePosition;
        descriptionText.transform.position = mousePosition + offset;
        descriptionText.gameObject.SetActive(true);
        isMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Desativa o texto de descrição quando o mouse sai do ícone
        descriptionText.gameObject.SetActive(false);
        isMouseOver = false;
    }

    void Update()
    {
        // Atualiza a posição do texto para seguir o mouse apenas quando necessário
        if (isMouseOver)
        {
            Vector2 mousePosition = Input.mousePosition;
            descriptionText.transform.position = mousePosition + offset;
        }
    }
}
