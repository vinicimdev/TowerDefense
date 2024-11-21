using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TowerIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string towerDescription;
    public TextMeshProUGUI descriptionText;
    private bool isMouseOver = false;
    public Vector2 offset = new Vector2(1000, -20); // Offset para a posi��o do texto (X, Y)

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Ativa e atualiza o texto de descri��o
        descriptionText.text = towerDescription;
        Vector2 mousePosition = Input.mousePosition;
        descriptionText.transform.position = mousePosition + offset;
        descriptionText.gameObject.SetActive(true);
        isMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Desativa o texto de descri��o quando o mouse sai do �cone
        descriptionText.gameObject.SetActive(false);
        isMouseOver = false;
    }

    void Update()
    {
        // Atualiza a posi��o do texto para seguir o mouse apenas quando necess�rio
        if (isMouseOver)
        {
            Vector2 mousePosition = Input.mousePosition;
            descriptionText.transform.position = mousePosition + offset;
        }
    }
}
