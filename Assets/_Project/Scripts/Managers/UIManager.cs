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
    [SerializeField] Transform playerDeckContainer;

    [Header("Attack Scene")]
    [SerializeField] GameObject drawCardsScene;
    [SerializeField] Button drawCardsButton;
    [Space]
    [SerializeField] GameObject selectCardsScene;
    [SerializeField] AttackCardUI attackCardPrefab;
    [SerializeField] Transform attackCardsContainer;
    [SerializeField] Button attackButton;
    [Space]
    [SerializeField] GameObject endTurnScene;
    [SerializeField] Button completeTurnButton;
    [Space]
    [SerializeField] GameObject enemyTurnScene;
    [SerializeField] Button enemyTurnButton;

    protected override void InitializeInstance()
    {
        base.InitializeInstance();

        //set buttons on click
        drawCardsButton.onClick.AddListener(FightManager.instance.PlayerDrawCards);
        attackButton.onClick.AddListener(FightManager.instance.PlayerAttack);
        completeTurnButton.onClick.AddListener(FightManager.instance.UpdatePlayerTurn);
        enemyTurnButton.onClick.AddListener(FightManager.instance.EnemyAttack);
    }

    [Button]
    public void UpdateAll()
    {
        UpdateEnemyHealth();
        UpdateEnemyCardsSelection();
        UpdatePlayersHealth();
        UpdateCurrentPlayer();
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
        UpdateElementsInContainer(playerHealthPrefab, playersHealthContainer, players.Length, (value, i) =>
        {
            PlayerTest player = players[i];
            value.Init(player.PlayerName, player.CurrentHealth);
        });
    }

    /// <summary>
    /// Set name and deck for current player
    /// </summary>
    public void UpdateCurrentPlayer()
    {
        PlayerTest currentPlayer = FightManager.instance.CurrentPlayer;
        currentPlayerText.text = currentPlayer.PlayerName;
        UpdateValuesInContainer(playerDeckContainer, currentPlayer.CurrentDeck);
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
        StartCoroutine(UpdateElementsInContainer(attackCardPrefab, attackCardsContainer, currentPlayer.NumberOfAttackCardsToDraw, delayBetweenInstantiate: 0.25f, (value, i) =>
        {
            //instantiate AttackCard, set icon, and set button OnClick
            Sprite icon = IconsManager.instance.GetIcon(currentPlayer.AttackCards[i]);
            value.Init(icon, () =>
            {
                bool isCardSelected = FightManager.instance.PlayerSelectAttackCard(i);
                value.ToggleSelectCard(isCardSelected);
            });
        }));
    }

    /// <summary>
    /// Show end turn scene
    /// </summary>
    public void ShowEndTurnScene()
    {
        ShowScene(endTurnScene);
    }

    /// <summary>
    /// Show enemy turn scene
    /// </summary>
    public void ShowEnemyTurnScene()
    {
        ShowScene(enemyTurnScene);
    }

    #region private API

    private void UpdateValuesInContainer(Transform container, List<EValuesTest> values)
    {
        //instantiate ValueUI and set icon
        UpdateElementsInContainer(valuePrefab, container, values.Count, (value, i) =>
        {
            Sprite icon = IconsManager.instance.GetIcon(values[i]);
            value.Init(icon);
        });
    }

    private void UpdateElementsInContainer<T>(T prefab, Transform container, int length, System.Action<T, int> onInstantiate) where T : Object
    {
        //remove previous
        for (int i = container.childCount - 1; i >= 0; i--)
            Destroy(container.GetChild(i).gameObject);

        //instantiate icons
        for (int i = 0; i < length; i++)
        {
            T value = Instantiate(prefab, container);
            onInstantiate?.Invoke(value, i);
        }
    }

    private IEnumerator UpdateElementsInContainer<T>(T prefab, Transform container, int length, float delayBetweenInstantiate, System.Action<T, int> onInstantiate) where T : Object
    {
        //remove previous
        for (int i = container.childCount - 1; i >= 0; i--)
            Destroy(container.GetChild(i).gameObject);

        //instantiate icons
        for (int i = 0; i < length; i++)
        {
            T value = Instantiate(prefab, container);
            onInstantiate?.Invoke(value, i);
            yield return new WaitForSeconds(delayBetweenInstantiate);
        }
    }

    private void ShowScene(GameObject sceneToShow)
    {
        //hide every scene
        drawCardsScene.SetActive(false);
        selectCardsScene.SetActive(false);
        endTurnScene.SetActive(false);
        enemyTurnScene.SetActive(false);

        //show only the correct one
        sceneToShow.SetActive(true);
    }

    #endregion
}
