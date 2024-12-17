using redd096;
using redd096.Attributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SimpleInstance<UIManager>
{
    [SerializeField] ValueUI valuePrefab;

    [Header("Enemy")]
    [SerializeField] TMP_Text enemyNameText;
    [SerializeField] Transform enemyHealthContainer;

    [Header("ROCK PAPER SCISSORS")]
    [SerializeField] Transform enemyCardsContainer;

    [Header("Players")]
    [SerializeField] PlayerHealthUI playerHealthPrefab;
    [SerializeField] Transform playersHealthContainer;

    [Header("Current Player")]
    [SerializeField] TMP_Text currentPlayerText;

    [Header("Attack Scene")]
    [SerializeField] GameObject drawCardsScene;
    [SerializeField] Button drawCardsButton;
    [Space]
    [SerializeField] GameObject selectCardsScene;
    [SerializeField] AttackCardUI attackCardPrefab;
    [SerializeField] Transform attackCardsContainer;
    [SerializeField] Button attackButton;
    [Space]
    [SerializeField] Button completeTurnButton;
    [Space]
    [SerializeField] GameObject enemyTurnScene;
    [SerializeField] Button enemyTurnButton;

    [Header("Deck Piles")]
    [SerializeField] Button drawPileButton;
    [SerializeField] Button discardPileButton;
    [SerializeField] GameObject deckPileCanvas;
    [SerializeField] GameObject drawPileText;
    [SerializeField] GameObject discardPileText;
    [SerializeField] Button closePileButton;
    [SerializeField] Transform deckPileContainer;

    //pooling
    Pooling<ValueUI> pool_valuePrefab = new Pooling<ValueUI>();
    Pooling<PlayerHealthUI> pool_playerHealthPrefab = new Pooling<PlayerHealthUI>();
    Pooling<AttackCardUI> pool_attackCardPrefab = new Pooling<AttackCardUI>();

    private AttackCardUI[] playerCardsInScene;


    protected override void InitializeInstance()
    {
        base.InitializeInstance();

        //set buttons on click
        drawCardsButton.onClick.AddListener(FightManager.instance.PlayerDrawCards);
        attackButton.onClick.AddListener(FightManager.instance.PlayerAttack);
        completeTurnButton.onClick.AddListener(FightManager.instance.UpdatePlayerTurn);
        enemyTurnButton.onClick.AddListener(FightManager.instance.EnemyAttack);
        drawPileButton.onClick.AddListener(() => TogglePileCanvas(true, true));
        discardPileButton.onClick.AddListener(() => TogglePileCanvas(true, false));
        closePileButton.onClick.AddListener(() => TogglePileCanvas(false));
    }

    [Button(ButtonAttribute.EEnableType.PlayMode)]
    private void UpdateUIInScene()
    {
        UpdateEnemyHealth();
        UpdateEnemyCardsSelection();
        UpdatePlayersHealth();
        UpdateCurrentPlayerName();
    }

    /// <summary>
    /// If there are placeholders in scene from the editor, remove them
    /// </summary>
    public void RemovePlaceholders()
    {
        //remove placeholders
        RemovePlaceholders(enemyHealthContainer);
        RemovePlaceholders(enemyCardsContainer);
        RemovePlaceholders(playersHealthContainer);
        RemovePlaceholders(attackCardsContainer);
        RemovePlaceholders(deckPileContainer);
    }

    /// <summary>
    /// Instantiate enemy health icons and set name
    /// </summary>
    public void UpdateEnemyHealth()
    {
        enemyNameText.text = FightManager.instance.Enemy.EnemyName;
        UpdateValuesInContainer(enemyHealthContainer, FightManager.instance.Enemy.CurrentLife);
    }

    /// <summary>
    /// Instantiate enemy cards icons
    /// </summary>
    public void UpdateEnemyCardsSelection()
    {
        //ROCK PAPER SCISSORS
        UpdateValuesInContainer(enemyCardsContainer, FightManager.instance.Enemy.SelectedCards);
    }

    /// <summary>
    /// Instantiate PlayerHealths and set name and health
    /// </summary>
    public void UpdatePlayersHealth()
    {
        PlayerTest[] players = FightManager.instance.Players;
        UpdateElementsInContainer(pool_playerHealthPrefab, playerHealthPrefab, playersHealthContainer, players.Length, (value, i) =>
        {
            PlayerTest player = players[i];
            value.Init(player.PlayerName, player.CurrentHealth);
        });
    }

    /// <summary>
    /// Set name for current player
    /// </summary>
    public void UpdateCurrentPlayerName()
    {
        PlayerTest currentPlayer = FightManager.instance.CurrentPlayer;
        currentPlayerText.text = currentPlayer.PlayerName;
    }

    /// <summary>
    /// Enable or disable attack button
    /// </summary>
    /// <param name="isEnabled"></param>
    public void ToggleAttackButton(bool isEnabled)
    {
        attackButton.interactable = isEnabled;
    }

    /// <summary>
    /// Show draw cards scene and set its button
    /// </summary>
    public void ShowDrawCardsScene()
    {
        ShowScene(drawCardsScene);
    }

    /// <summary>
    /// Show select cards scene and set cards' button
    /// </summary>
    public void ShowSelectCardsScene()
    {
        ShowScene(selectCardsScene);

        PlayerTest currentPlayer = FightManager.instance.CurrentPlayer;
        playerCardsInScene = new AttackCardUI[currentPlayer.NumberOfAttackCardsToDraw];

        StartCoroutine(UpdateElementsInContainer(pool_attackCardPrefab, attackCardPrefab, attackCardsContainer, currentPlayer.NumberOfAttackCardsToDraw, delayBetweenInstantiate: 0.25f, (value, i) =>
        {
            //save reference between index and card in UI
            playerCardsInScene[i] = value;

            //instantiate AttackCard, set icon, and set button OnClick
            Sprite icon = IconsManager.instance.GetIcon(currentPlayer.AttackCards[i]);
            value.Init(icon, () =>
            {
                ESelectCardTest isCardSelected = FightManager.instance.PlayerSelectAttackCard(i);

                //update ui
                value.ToggleSelectCard(isCardSelected == ESelectCardTest.Selected);
                UpdateSelectedCardsNumbers();

            });
        }));
    }

    private void UpdateSelectedCardsNumbers()
    {
        PlayerTest currentPlayer = FightManager.instance.CurrentPlayer;

        //update every card in scene
        for (int i = 0; i < currentPlayer.AttackCards.Count; i++)
        {
            //not selected
            if (currentPlayer.SelectedCardsToAttack.Contains(i) == false)
            {
                playerCardsInScene[i].SetNumberText(false);
                continue;
            }

            //find index in selected cards
            for (int number = 0; number < currentPlayer.SelectedCardsToAttack.Count; number++)
            {
                if (currentPlayer.SelectedCardsToAttack[number] == i)
                {
                    playerCardsInScene[i].SetNumberText(true, (number + 1).ToString());
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Enable or disable CompleteTurn button. When enable is true, disable OnClick event on the attack cards
    /// </summary>
    /// <param name="isEnabled"></param>
    public void ToggleCompleteTurnButton(bool isEnabled)
    {
        completeTurnButton.interactable = isEnabled;

        //on enable, remove onClick event from attack cards
        if (isEnabled)
        {
            foreach (var card in pool_attackCardPrefab.PooledObjects)
            {
                card.RemoveOnClickEvent();
            }
        }
    }

    /// <summary>
    /// Show enemy turn scene
    /// </summary>
    public void ShowEnemyTurnScene()
    {
        ShowScene(enemyTurnScene);
    }

    /// <summary>
    /// Show or hide Pile Canvas
    /// </summary>
    /// <param name="show">show or hide</param>
    /// <param name="deckPile">deck pile or discard pile</param>
    public void TogglePileCanvas(bool show, bool deckPile = true)
    {
        //when show, set text and instantiate player cards
        if (show)
        {
            drawPileText.SetActive(deckPile);
            discardPileText.SetActive(deckPile == false);

            PlayerTest currentPlayer = FightManager.instance.CurrentPlayer;
            UpdateValuesInContainer(deckPileContainer, deckPile ? currentPlayer.CurrentDeck : currentPlayer.UsedCards);
        }

        //show or hide canvas
        deckPileCanvas.SetActive(show);
    }

    #region private API

    private void RemovePlaceholders(Transform container)
    {
        //destroy gameObjects placed in editor
        for (int i = container.childCount - 1; i >= 0; i--)
            Destroy(container.GetChild(i).gameObject);
    }

    private void UpdateValuesInContainer(Transform container, List<EValuesTest> values)
    {
        //instantiate ValueUI and set icon
        UpdateElementsInContainer(pool_valuePrefab, valuePrefab, container, values.Count, (value, i) =>
        {
            Sprite icon = IconsManager.instance.GetIcon(values[i]);
            value.Init(icon);
        });
    }

    private void UpdateElementsInContainer<T>(Pooling<T> pool, T prefab, Transform container, int length, System.Action<T, int> onInstantiate) where T : Object
    {
        //remove previous
        for (int i = container.childCount - 1; i >= 0; i--)
            Pooling.Destroy(container.GetChild(i).gameObject);

        //instantiate icons
        for (int i = 0; i < length; i++)
        {
            T value = pool.Instantiate(prefab, container);
            onInstantiate?.Invoke(value, i);
        }
    }

    private IEnumerator UpdateElementsInContainer<T>(Pooling<T> pool, T prefab, Transform container, int length, float delayBetweenInstantiate, System.Action<T, int> onInstantiate) where T : Object
    {
        //remove previous
        for (int i = container.childCount - 1; i >= 0; i--)
            Pooling.Destroy(container.GetChild(i).gameObject);

        //instantiate icons
        for (int i = 0; i < length; i++)
        {
            T value = pool.Instantiate(prefab, container);
            onInstantiate?.Invoke(value, i);
            yield return new WaitForSeconds(delayBetweenInstantiate);
        }
    }

    private void ShowScene(GameObject sceneToShow)
    {
        //hide every scene
        drawCardsScene.SetActive(false);
        selectCardsScene.SetActive(false);
        enemyTurnScene.SetActive(false);

        //show only the correct one
        sceneToShow.SetActive(true);
    }

    #endregion
}
