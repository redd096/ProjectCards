using redd096;
using redd096.Attributes;
using UnityEngine;

public class FightManager : SimpleInstance<FightManager>
{
    //ROCK PAPER SCISSORS
    [Tooltip("Instead of remove value from enemy health, play rock paper scissor and if win, enemy lose the card it selected")] public bool USE_ROCK_PAPER_SCISSORS = true;
    [Tooltip("Enemy turn doesn't have turn to damage, but player receive damage if lose RockPaperScissors")] public bool NO_ENEMY_TURN = true;
    [Tooltip("Player doesn't have a counter, but lose cards like enemy when lose RockPaperScissors")] public bool PLAYER_LIFE_ARE_CARDS = true;

    [SerializeField] bool findEnemiesAndPlayersOnAwake;
    public EnemyTest Enemy;
    public PlayerTest[] Players;

    private int currentPlayerIndex;
    public PlayerTest CurrentPlayer => Players[currentPlayerIndex];

    protected override void InitializeInstance()
    {
        base.InitializeInstance();

        //find enemy and players in scene
        if (findEnemiesAndPlayersOnAwake)
        {
            FindEnemiesAndPlayersInScene();
        }

        //start fight
        StartFight();
    }

    [Button]
    private void FindEnemiesAndPlayersInScene()
    {
        //find enemy and players in scene
        Enemy = FindAnyObjectByType<EnemyTest>(FindObjectsInactive.Exclude);
        Players = FindObjectsByType<PlayerTest>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
    }

    private void StartFight()
    {
        UIManager.instance.RemovePlaceholders();

        //set enemy default values
        Enemy.ResetCurrentLife();
        UIManager.instance.UpdateEnemyHealth();
        UIManager.instance.UpdateEnemyCardsSelection();

        //set players default values
        foreach (var v in Players)
        {
            v.ResetCurrentLife();
            v.ResetDecks();
        }
        UIManager.instance.UpdatePlayersHealth();

        //show first player
        UIManager.instance.UpdateCurrentPlayerName();
        UIManager.instance.ShowDrawCardsScene();
    }

    /// <summary>
    /// Draw attack cards and move to select
    /// </summary>
    public void PlayerDrawCards()
    {
        //draw cards
        CurrentPlayer.DrawAttackCards();

        //be sure AttackButton is disabled
        UIManager.instance.ToggleAttackButton(false);
        UIManager.instance.ToggleCompleteTurnButton(false);

        //move to select cards
        UIManager.instance.ShowSelectCardsScene();
    }

    /// <summary>
    /// Select attack card. If this is the last one, move to attack
    /// </summary>
    /// <param name="indexSelectedCard"></param>
    /// <returns>Return if this card is selected or not</returns>
    public ESelectCardTest PlayerSelectAttackCard(int indexSelectedCard)
    {
        PlayerTest currentPlayer = CurrentPlayer;
        ESelectCardTest result = ESelectCardTest.NotSelected;

        //select card
        if (currentPlayer.SelectedCardsToAttack.Contains(indexSelectedCard) == false)
        {
            bool playerCanSelectOtherCards = currentPlayer.SelectedCardsToAttack.Count < currentPlayer.NumberOfAttackCardsForDamage;
            if (playerCanSelectOtherCards)
            {
                currentPlayer.SelectedCardsToAttack.Add(indexSelectedCard);
                result = ESelectCardTest.Selected;
            }
        }
        //or deselect card
        else
        {
            currentPlayer.SelectedCardsToAttack.Remove(indexSelectedCard);
            result = ESelectCardTest.Removed;
        }

        //if this is the last one, enable attack button
        bool isLastCardToSelect = currentPlayer.SelectedCardsToAttack.Count >= currentPlayer.NumberOfAttackCardsForDamage;
        UIManager.instance.ToggleAttackButton(isLastCardToSelect);

        return result;
    }

    /// <summary>
    /// Player do damage to enemy
    /// </summary>
    public void PlayerAttack()
    {
        //attack
        CurrentPlayer.Attack();
        UIManager.instance.UpdateEnemyHealth();
        UIManager.instance.UpdateEnemyCardsSelection();
        UIManager.instance.UpdatePlayersHealth();

        //and enable CompleteTurn button
        UIManager.instance.ToggleAttackButton(false);
        UIManager.instance.ToggleCompleteTurnButton(true);
    }

    /// <summary>
    /// Move to next player. If there aren't other players, move to enemy turn
    /// </summary>
    public void UpdatePlayerTurn()
    {
        currentPlayerIndex++;

        //if there aren't other players, start enemy turn
        if (currentPlayerIndex >= Players.Length)
        {
            currentPlayerIndex = 0;

            //ROCK PAPER SCISSORS
            if (NO_ENEMY_TURN == false)
            {
                UIManager.instance.ShowEnemyTurnScene();
                return;
            }
        }

        //else, start next player turn
        UIManager.instance.UpdateCurrentPlayerName();
        UIManager.instance.ShowDrawCardsScene();
    }

    /// <summary>
    /// Enemy do damage to players
    /// </summary>
    public void EnemyAttack()
    {
        //wnemy attack
        Enemy.Attack();
        UIManager.instance.UpdatePlayersHealth();

        //and move to player turn scene
        UIManager.instance.UpdateCurrentPlayerName();
        UIManager.instance.ShowDrawCardsScene();
    }
}