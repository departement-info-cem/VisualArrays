using System.Collections;
using VisualArrays.Sprites;

namespace VisualArrays.Collections;

/// <summary>
/// Représente une collection d'objets 'Sprite' qui s'affichent sur une grille.
/// </summary>
public class SpriteCollection : CollectionBase
{
    private BaseGrid va_owner;
    //public SpriteCollection()
    //{
    //    va_owner = null;
    //}
    /// <summary>
    /// Initialise une SpriteCollection
    /// </summary>
    /// <param name="pOwner">Grille propriétaire des Sprites</param>
    public SpriteCollection(BaseGrid pOwner)
    {
        va_owner = pOwner;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pSprite"></param>
    /// <returns></returns>
    public Sprite Add(Sprite pSprite)
    {
        pSprite.Owner = va_owner;
        this.InnerList.Add(pSprite);
        pSprite.RecalcBoundsAndRedraw();
        //va_owner.UpdateSprites(pSprite.Bounds);
        return pSprite;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pSprite"></param>
    public void Remove(Sprite pSprite)
    {
        this.InnerList.Remove(pSprite);
        va_owner.UpdateSprites(pSprite.Bounds);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pSprite"></param>
    /// <returns></returns>
    public bool Contains(Sprite pSprite)
    {
        return this.InnerList.Contains(pSprite);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pIndex"></param>
    /// <returns></returns>
    public Sprite this[int pIndex]
    {
        get { return (Sprite)this.InnerList[pIndex]; }
        set
        {
            Sprite newSprite = value;
            newSprite.Owner = va_owner;
            this.InnerList[pIndex] = newSprite;
            va_owner.UpdateSprites(newSprite.Bounds);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pSprites"></param>
    public void AddRange(Sprite[] pSprites)
    {
        foreach (Sprite objSprite in pSprites)
            objSprite.Owner = va_owner;
        this.InnerList.AddRange(pSprites);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Sprite[] GetValues()
    {
        Sprite[] sprites = new Sprite[this.InnerList.Count];
        this.InnerList.CopyTo(0, sprites, 0, this.InnerList.Count);
        return sprites;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    protected override void OnInsertComplete(int index, object value)
    {
        base.OnInsertComplete(index, value);
        ((Sprite)value).RecalcBoundsAndRedraw(); // 28-06-2011
        //va_owner.UpdateSprites(((Sprite)value).Bounds);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    protected override void OnRemoveComplete(int index, object value)
    {
        base.OnRemoveComplete(index, value);
        va_owner.UpdateSprites(((Sprite)value).Bounds);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    protected override void OnSetComplete(int index, object oldValue, object newValue)
    {
        base.OnSetComplete(index, oldValue, newValue);
        va_owner.UpdateSprites(((Sprite)oldValue).Bounds);
        va_owner.UpdateSprites(((Sprite)newValue).Bounds);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Place le 'Sprite' à l'avant plan devant tous les autres Sprites
    /// </summary>
    /// <param name="pSprite">Sprite à déplacer à l'avant plan</param>
    protected internal void BringToFront(Sprite pSprite)
    {
        this.InnerList.Remove(pSprite);
        this.InnerList.Add(pSprite);
        va_owner.UpdateSprites(((Sprite)pSprite).Bounds);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Place le 'Sprite' au dernier plan derrière tous les autres Sprites
    /// </summary>
    /// <param name="pSprite">Sprite à déplacer à l'arrière plan</param>
    protected internal void SendToBack(Sprite pSprite)
    {
        this.InnerList.Remove(pSprite);
        this.InnerList.Insert(0, pSprite);
        va_owner.UpdateSprites(pSprite.Bounds);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    ///     Recherche le Sprite spécifié et retourne l'index de base zéro de la première
    ///     occurrence dans la collection.
    /// </summary>
    /// <param name="pSprite">Sprite à localiser dans la collection</param>
    /// <returns>
    ///     Index de base zéro de la première occurrence de item dans la collection, s'il existe , sinon -1.
    ///</returns>
    public int IndexOf(Sprite pSprite)
    {
        for (int index = 0; index < this.Count; index++)
            if (this[index] == pSprite) return index;
        return -1;
    }
    /// <summary>
    ///     Exécute des processus personnalisés supplémentaires après l'effacement du
    ///     contenu de l'instance de System.Collections.CollectionBase.
    /// </summary>
    protected override void OnClearComplete()
    {
        base.OnClearComplete();
        va_owner.Refresh();
    }
}