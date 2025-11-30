using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections; // IMPORTANTE para corrutinas

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

    [Header("Sonido")]
    public AudioSource buttonAudioSource;
    public AudioClip clickSound;
    public AudioClip failSound;
    public AudioClip winSound;

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

        // CONECTAR SONIDO A LOS BOTONES
        if (leftButton != null)
            leftButton.onClick.AddListener(() => { Debug.Log("LEFT CLICK"); PlayClickSound(); });

        if (centerButton != null)
            centerButton.onClick.AddListener(() => { Debug.Log("CENTER CLICK"); PlayClickSound(); });

        if (rightButton != null)
            rightButton.onClick.AddListener(() => { Debug.Log("RIGHT CLICK"); PlayClickSound(); });
    }

    void SetupNewRound()
    {
        badIndex = Random.Range(0, 3);
        if (resultText != null)
            resultText.text = "";
    }

    void PlayClickSound()
    {
        Debug.Log("PlayClickSound");

        if (buttonAudioSource != null && clickSound != null)
        {
            buttonAudioSource.PlayOneShot(clickSound);
        }
        else
        {
            Debug.LogWarning("Falta buttonAudioSource o clickSound en el Inspector");
        }
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

            // SONIDO FAIL
            if (buttonAudioSource != null && failSound != null)
            {
                buttonAudioSource.PlayOneShot(failSound);
            }

            currentMultiplier = 1;
            UpdateMultiplierText();

            if (GameManager.Instance != null)
                GameManager.Instance.endMinigame(false);
        }
        else
        {
            // GOL
            if (resultText != null)
                resultText.text = "YOU WON";

            currentMultiplier++;
            UpdateMultiplierText();

            // ¿ha llegado al objetivo?
            if (currentMultiplier >= objetivoMultiplicador)
            {
                if (!juegoTerminado)
                {
                    juegoTerminado = true;

                    if (buttonAudioSource != null && winSound != null)
                    {
                        buttonAudioSource.PlayOneShot(winSound);
                    }

                    StartCoroutine(WaitAndWin(5f));  // ESPERA 3s Y LUEGO GANA
                }
            }
            else
            {
                SetupNewRound();
            }
        }
    }

    private IEnumerator WaitAndWin(float delay)
    {
        yield return new WaitForSeconds(delay); // aquí sí se puede usar yield [web:91][web:96]

        if (GameManager.Instance != null)
            GameManager.Instance.endMinigame(true);
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
