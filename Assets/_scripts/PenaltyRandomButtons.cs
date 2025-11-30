using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PenaltyRandomButtons : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI multiplierText;
    public GameObject exitButtonObject;
    
    [Header("Apuesta")]
    public int apuesta = 10;       
    public int objetivoMultiplicador = 5;

    [Header("Botones")]
    public Button leftButton;
    public Button centerButton;
    public Button rightButton;

    private int badIndex;
    private int currentMultiplier = 1;
    private bool canShoot = false;
    private bool juegoTerminado = false;

    void Start()
    {
        
        if (exitButtonObject != null)
            exitButtonObject.SetActive(false);

        currentMultiplier = 1;
        UpdateMultiplierText();
        SetupNewRound();
        canShoot = true;
    }

    void SetupNewRound()
    {
        badIndex = Random.Range(0, 3);
        if (resultText != null)
            resultText.text = "";
    }

    public void OnLeftButton() { TryShoot(0); }
    public void OnCenterButton() { TryShoot(1); }
    public void OnRightButton() { TryShoot(2); }

    void TryShoot(int chosenIndex)
    {
        if (!canShoot || juegoTerminado) return;

        bool isBad = (chosenIndex == badIndex);

        if (isBad)
        {
            // FALLO -> pierdes apuesta
            if (resultText != null)
                resultText.text = "YOU FAIL";
            
            currentMultiplier = 1;
            UpdateMultiplierText();

            if (GameManager.Instance != null)
                GameManager.Instance.endMinigame(false);
        }
        else
        {
            // GOL
            if (resultText != null)
                resultText.text = "GOAL";

            currentMultiplier++;
            UpdateMultiplierText();

            // �ha llegado al objetivo?
            if (currentMultiplier >= objetivoMultiplicador)
            {
                if (GameManager.Instance != null)
                    GameManager.Instance.endMinigame(true);
            }
            else
            {
                SetupNewRound();
            }
        }
    }

    void UpdateMultiplierText()
    {
        if (multiplierText != null)
            multiplierText.text = "Multiplicator: x" + currentMultiplier;
    }

    void DesactivarBotones()
    {
        if (leftButton != null) leftButton.interactable = false;
        if (centerButton != null) centerButton.interactable = false;
        if (rightButton != null) rightButton.interactable = false;
        canShoot = false;
    }

    void MostrarBotonSalir()
    {
        if (exitButtonObject != null)
            exitButtonObject.SetActive(true);
    }
}
