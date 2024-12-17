
/// <summary>
/// Used by FightManager to communicate what to do when player click a card
/// </summary>
public enum ESelectCardTest
{
    /// <summary>
    /// The card is added to SelectedCardsToAttack
    /// </summary>
    Selected,
    /// <summary>
    /// The card is removed from SelectedCardsToAttack
    /// </summary>
    Removed,
    /// <summary>
    /// The click did nothing (player tryed to select another card, but reached the limit of selectable cards)
    /// </summary>
    NotSelected,
}
