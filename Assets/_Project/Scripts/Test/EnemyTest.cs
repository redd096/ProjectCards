using redd096.Attributes;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    [SerializeField] string enemyName = "Cattivone";
    [SerializeField] int enemyAttack = 1;
    [Space]
    [SerializeField] EValuesTest[] defaultLife;
    [SerializeField] int numberOfRandomLifes;
    [SerializeField] EValuesTest[] possibleRandomLifes;

    private List<EValuesTest> currentLife = new List<EValuesTest>();

    public string EnemyName => enemyName;
    public List<EValuesTest> CurrentLife => currentLife;

    //ROCK PAPER SCISSORS
    private List<EValuesTest> selectedCards = new List<EValuesTest>();
    public List<EValuesTest> SelectedCards => selectedCards;

    [Button]
    public void ResetCurrentLife()
    {
        //add default life
        currentLife = new List<EValuesTest>(defaultLife);

        //add random life
        EValuesTest[] values = RandomUtility.NotAlwaysSame(numberOfRandomLifes, possibleRandomLifes);
        currentLife.AddRange(values);
    }

    [Button]
    public void Attack()
    {
        //do damage to every player in scene
        foreach (var v in FightManager.instance.Players)
        {
            v.GetDamage(enemyAttack);
        }
    }

    public void GetDamage(EValuesTest[] playerAttack, PlayerTest player)
    {
        //ROCK PAPER SCISSORS
        if (FightManager.instance.USE_ROCK_PAPER_SCISSORS)
        {
            //from current life, select a card for every damage to try defend
            EValuesTest[] cards = RandomUtility.CantReturnTheSame(playerAttack.Length, currentLife.ToArray());
            selectedCards = new List<EValuesTest>(cards);

            //foreach card
            for (int i = 0; i < selectedCards.Count; i++)
            {
                EValuesTest attack = playerAttack[i];
                EValuesTest defense = selectedCards[i];

                //if player defeat enemy card, enemy lose used card
                if (attack == EValuesTest.Rock && defense == EValuesTest.Scissors
                    || attack == EValuesTest.Paper && defense == EValuesTest.Rock
                    || attack == EValuesTest.Scissors && defense == EValuesTest.Paper)
                {
                    currentLife.Remove(defense);
                }
                else if (FightManager.instance.NO_ENEMY_TURN)
                {
                    if (defense == EValuesTest.Rock && attack == EValuesTest.Scissors
                        || defense == EValuesTest.Paper && attack == EValuesTest.Rock
                        ||  defense == EValuesTest.Scissors && attack == EValuesTest.Paper)
                    {
                        if (FightManager.instance.PLAYER_LIFE_ARE_CARDS)
                            player.GetDamage(attack);
                        else
                            player.GetDamage(enemyAttack);
                    }
                }
            }

            return;
        }

        //remove health
        foreach (var v in playerAttack)
        {
            if (currentLife.Contains(v))
                currentLife.Remove(v);
        }
    }
}
