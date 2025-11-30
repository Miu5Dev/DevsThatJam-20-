using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlackjackManager : MonoBehaviour
{
    [Header("UI")]
    public Transform playerCardsPanel;
    public Transform dealerCardsPanel;

    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI dealerScoreText;
    public TextMeshProUGUI resultText;

    public Button dealButton;
    public Button hitButton;
    public Button standButton;

    [Header("Cartas")]
    public GameObject cardPrefab;      // prefab con Image
    public Sprite[] cardSprites;       // 10 sprites, index 0 = carta 1, index 1 = carta 2, etc.
    public int[] cardValues = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

    private List<int> deck = new List<int>();
    private List<int> playerHand = new List<int>();
    private List<int> dealerHand = new List<int>();

    private bool roundActive = false;

    void Start()
    {
        if (resultText != null) resultText.text = "";

        dealButton.onClick.AddListener(OnDealClicked);
        hitButton.onClick.AddListener(OnHitClicked);
        standButton.onClick.AddListener(OnStandClicked);

        SetButtonsState(start: true);
    }

    void OnDealClicked()
    {
        StartNewRound();
    }

    void OnHitClicked()
    {
        if (!roundActive) return;
        DealCardToPlayer();

        int playerScore = CalculateHandValue(playerHand);
        UpdateScores();

        if (playerScore > 21)
        {
            EndRound("You lose.");
            GameManager.Instance.endMinigame(false);
        }
    }

    void OnStandClicked()
    {
        if (!roundActive) return;
        StartCoroutine(DealerTurn());
    }

    void StartNewRound()
    {
        roundActive = true;
        resultText.text = "";

        ClearCards();
        playerHand.Clear();
        dealerHand.Clear();

        BuildAndShuffleDeck();

        // Repartir 2 a cada uno
        DealCardToPlayer();
        DealCardToDealer();
        DealCardToPlayer();
        DealCardToDealer();

        UpdateScores();
        SetButtonsState(start: false);
    }

    void BuildAndShuffleDeck()
    {
        deck.Clear();
        // A�adir 4 copias de cada carta (como 4 palos) o las que quieras
        for (int i = 0; i < cardValues.Length; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                deck.Add(i); // guardamos el �ndice de la carta
            }
        }

        // Shuffle
        for (int i = 0; i < deck.Count; i++)
        {
            int rand = Random.Range(i, deck.Count);
            int temp = deck[i];
            deck[i] = deck[rand];
            deck[rand] = temp;
        }
    }

    int DrawCardIndex()
    {
        if (deck.Count == 0)
        {
            BuildAndShuffleDeck();
        }

        int idx = deck[0];
        deck.RemoveAt(0);
        return idx;
    }

    void DealCardToPlayer()
    {
        int cardIndex = DrawCardIndex();
        playerHand.Add(cardIndex);
        SpawnCardUI(playerCardsPanel, cardIndex);
    }

    void DealCardToDealer()
    {
        int cardIndex = DrawCardIndex();
        dealerHand.Add(cardIndex);
        SpawnCardUI(dealerCardsPanel, cardIndex);
    }

    void SpawnCardUI(Transform parent, int cardIndex)
    {
        GameObject cardGO = Instantiate(cardPrefab, parent);
        Image img = cardGO.GetComponent<Image>();
        if (img != null && cardSprites != null && cardIndex >= 0 && cardIndex < cardSprites.Length)
        {
            img.sprite = cardSprites[cardIndex];
        }
    }

    int CalculateHandValue(List<int> hand)
    {
        int total = 0;
        foreach (int idx in hand)
        {
            int value = cardValues[idx];
            total += value;
        }
        return total;
    }

    void UpdateScores()
    {
        int playerScore = CalculateHandValue(playerHand);
        int dealerScore = CalculateHandValue(dealerHand);

        if (playerScoreText != null)
            playerScoreText.text = "Player: " + playerScore;
        if (dealerScoreText != null)
            dealerScoreText.text = "Dealer: " + dealerScore;
    }

    System.Collections.IEnumerator DealerTurn()
    {
        roundActive = false;

        // Dealer roba hasta llegar a 17 o m�s
        while (CalculateHandValue(dealerHand) < 17)
        {
            DealCardToDealer();
            UpdateScores();
            yield return new WaitForSeconds(0.5f);
        }

        int playerScore = CalculateHandValue(playerHand);
        int dealerScore = CalculateHandValue(dealerHand);

        if (dealerScore > 21)
        {
            EndRound("The dealer passed. �You won!");
            GameManager.Instance.endMinigame(true);
        }
        else if (playerScore > dealerScore)
        {
            EndRound("�You won!");
            GameManager.Instance.endMinigame(true);
        }
        else if (playerScore < dealerScore)
        {
            EndRound("You lose.");
            GameManager.Instance.endMinigame(false);
        }
        else
        {
            EndRound("Tie.");
        }
    }

    void EndRound(string message)
    {
        if (resultText != null)
            resultText.text = message;

        roundActive = false;
        SetButtonsState(start: true);
    }

    void SetButtonsState(bool start)
    {
        dealButton.interactable = start;
        hitButton.interactable = !start;
        standButton.interactable = !start;
    }

    void ClearCards()
    {
        foreach (Transform t in playerCardsPanel)
        {
            Destroy(t.gameObject);
        }
        foreach (Transform t in dealerCardsPanel)
        {
            Destroy(t.gameObject);
        }
    }
}
