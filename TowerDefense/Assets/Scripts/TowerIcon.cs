using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TowerIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string towerDescription;
    public TextMeshProUGUI descriptionText;
    public RectTransform canvasRectTransform; // Referência ao RectTransform do Canvas
    public GameObject descriptionFather; // Objeto pai do texto
    private bool isMouseOver = false;
    public Vector2 offset = new Vector2(20, -20); // Offset ajustado para uma posição mais próxima

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Ativa e atualiza o texto de descrição
        descriptionText.text = towerDescription;
        UpdateDescriptionPosition(Input.mousePosition);
        descriptionFather.SetActive(true);
        isMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Desativa o objeto pai do texto quando o mouse sai do ícone
        descriptionFather.SetActive(false);
        isMouseOver = false;
    }

    void Update()
    {
        // Atualiza a posição do texto para seguir o mouse apenas quando necessário
        if (isMouseOver)
        {
            UpdateDescriptionPosition(Input.mousePosition);
        }
    }

    private void UpdateDescriptionPosition(Vector2 mousePosition)
    {
        // Converte a posição do mouse para o espaço local do Canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            mousePosition,
            null,
            out Vector2 localPoint
        );

        // Aplica o offset
        Vector2 newPosition = localPoint + offset;

        // Ajusta para não ultrapassar os limites do Canvas
        RectTransform rectTransform = descriptionText.GetComponent<RectTransform>();
        Vector2 clampedPosition = new Vector2(
            Mathf.Clamp(newPosition.x, 0, canvasRectTransform.rect.width - rectTransform.rect.width),
            Mathf.Clamp(newPosition.y, -canvasRectTransform.rect.height + rectTransform.rect.height, 0)
        );

        // Atualiza a posição do texto
        rectTransform.anchoredPosition = clampedPosition;
    }
}
