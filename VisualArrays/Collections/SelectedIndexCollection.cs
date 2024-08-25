namespace VisualArrays.Collections;

/// <summary>
/// Représente une collection d'index d'éléments sélectionnés dans une grille.
/// </summary>
public class SelectedIndexCollection
{
    private SortedList<int, int> va_colIndices;
    //private ListBox.SelectedIndexCollection va_colTest;
    private BaseGrid va_owner;
    //========================================================================================
    /// <summary>
    /// Obtient le nombre d'éléments de la collection.
    /// </summary>
    public int Count => va_colIndices.Count;

    //========================================================================================
    /// <summary>
    /// Initialise une collection d'index.
    /// </summary>
    /// <param name="pOwner">Propriétaire de la collection.</param>
    public SelectedIndexCollection(BaseGrid pOwner)
    {
        va_owner = pOwner;
        va_colIndices = new SortedList<int, int>();
    }
    ////========================================================================================
    //public int this[pIndex]
    //{
    //    //get { return va_colIndices[

    //}
    //========================================================================================
    /// <summary>
    /// Supprimer l'index spécifié de la collection.
    /// </summary>
    /// <param name="pIndex">Index de la collection.</param>
    public void Remove(int pIndex)
    {
        //va_colIndices.Remove
    }
    //========================================================================================
    /// <summary>
    /// Supporte une simple itération sur la collection d'indices.
    /// </summary>
    /// <returns>Un index.</returns>
    public IEnumerator<int> GetEnumerator()
    {
        foreach (KeyValuePair<int,int> keyValue in va_colIndices)
            yield return keyValue.Value;
    }
    //========================================================================================
    /// <summary>
    /// Supprime tous les indexd e la collection.
    /// </summary>
    public void Clear()
    {
        va_colIndices.Clear();
    }
    //========================================================================================
    /// <summary>
    /// Détermine si l'index réside dans la collection.
    /// </summary>
    /// <param name="pIndex">Index à localiser.</param>
    /// <returns>True si l'index est localisé, false sinon.</returns>
    public bool Contains(int pIndex)
    {
        return va_colIndices.ContainsValue(pIndex);
    }
}