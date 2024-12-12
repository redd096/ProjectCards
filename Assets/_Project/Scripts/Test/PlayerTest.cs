using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    [SerializeField] string playerName = "Bob";
    [SerializeField] int startHealth = 12;
    [SerializeField] int numberOfAttackCardsToDraw = 3;
    [SerializeField] int numberOfAttackCardsForDamage = 1;
    [SerializeField] EValuesTest[] startDeck;

    private int currentHealth;
    private List<EValuesTest> currentDeck = new List<EValuesTest>();
    private List<EValuesTest> usedCards = new List<EValuesTest>();
    private List<EValuesTest> attackCards = new List<EValuesTest>();
    private List<int> selectedCardsToAttack = new List<int>();

    public string PlayerName => playerName;
    public int NumberOfAttackCardsToDraw => numberOfAttackCardsToDraw;
    public int NumberOfAttackCardsForDamage => numberOfAttackCardsForDamage;
    public int CurrentHealth => currentHealth;
    public List<EValuesTest> CurrentDeck => currentDeck;
    public List<EValuesTest> UsedCards => usedCards;
    public List<EValuesTest> AttackCards => attackCards;
    public List<int> SelectedCardsToAttack => selectedCardsToAttack;

    #region private API

    private void PutCardsRandomInDeck(EValuesTest[] cardsToRandomize, List<EValuesTest> deck)
    {
        //add random cards from cardsToRandomize
        EValuesTest[] cards = RandomUtility.CantReturnTheSame(cardsToRandomize.Length, cardsToRandomize);
        deck.AddRange(cards);
    }

    private void ShuffleUsedInDeck()
    {
        //put used cards in deck
        PutCardsRandomInDeck(usedCards.ToArray(), currentDeck);
        usedCards.Clear();
    }

    #endregion

    public void ResetCurrentLife()
    {
        currentHealth = startHealth;
    }

    public void ResetDecks()
    {
        //reset decks
        currentDeck.Clear();
        usedCards.Clear();
        attackCards.Clear();

        //put start cards in deck
        PutCardsRandomInDeck(startDeck, currentDeck);
    }

    /// <summary>
    /// Draw cards from deck
    /// </summary>
    public void DrawAttackCards()
    {
        for (int i = 0; i < numberOfAttackCardsToDraw; i++)
        {
            //if there aren't cards in deck, reset it
            if (currentDeck.Count <= 0)
                ShuffleUsedInDeck();

            //draw card from deck
            attackCards.Add(currentDeck[0]);
            currentDeck.RemoveAt(0);
        }
    }

    public void Attack()
    {
        //get selected attack cards
        EValuesTest[] damage = new EValuesTest[selectedCardsToAttack.Count];
        for (int i = 0; i < selectedCardsToAttack.Count; i++)
        {
            int selectedCard = selectedCardsToAttack[i];
            damage[i] = attackCards[selectedCard];
        }
        selectedCardsToAttack.Clear();

        //put attack cards in used cards
        usedCards.AddRange(attackCards);
        attackCards.Clear();

        //damage enemy
        var enemy = FightManager.instance.Enemy;
        enemy.GetDamage(damage, this);
    }

    public void GetDamage(int damage)
    {
        currentHealth -= damage;
    }

    public void GetDamage(EValuesTest damage)
    {
        if (usedCards.Contains(damage))
            usedCards.Remove(damage);
    }
}
