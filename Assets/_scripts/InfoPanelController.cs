using UnityEngine;

public class InfoPanelController : MonoBehaviour
{
    public GameObject infoPanel;

    void Start()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }

    public void ShowInfo()
    {
        if (infoPanel != null)
            infoPanel.SetActive(true);
    }

    public void HideInfo()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }
}
