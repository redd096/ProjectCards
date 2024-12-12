using UnityEngine;
using redd096;

public class IconsManager : SimpleInstance<IconsManager>
{
    [SerializeField] FIconValue[] icons;

    public Sprite GetIcon(EValuesTest value)
    {
        foreach (var v in icons)
        {
            if (v.Value == value)
                return v.Icon;
        }

        return null;
    }

    [System.Serializable]
    public struct FIconValue
    {
        public EValuesTest Value;
        public Sprite Icon;
    }
}