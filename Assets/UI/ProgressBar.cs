using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Farming;
using UnityEngine.Tilemaps;
using Character;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI fillText;
    [SerializeField] private TileSelector tileSelector;

    public float Fill {set { fillImage.fillAmount = value; }}

    public void SetText (string text)
    {
        fillText.text = text;
        fillImage.fillAmount = 1.0f;

    }

    // void Update()
    // {
    //     FarmTile tile = tileSelector.GetSelectedTile();
    //     if(tile == null) {return;}

    //     if(tile.GetCondition == FarmTile.Condition.Tilled) {
    //         fillImage.fillAmount -= 0.25f * Time.deltaTime;

    //     }
    // }
}
