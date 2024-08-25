using VisualArrays.VisualArrays;

namespace VisualArrays.Collections;

/// <summary>
/// Représente un tableau de chaînes affichant les en-têtes d'une grille.
/// </summary>
public class HeaderArray
{
    internal string[] va_elements = null;
    private BaseGrid va_owner;
    private bool va_column; // vrai si en-têtes de colonnes, faux si en-têtes de rangées
    /// <summary>
    /// Fournit la taille du tableau des en-têtes.
    /// </summary>
    public int Length => va_elements.Length;

    //========================================================================================
    /// <summary>
    /// Supporte une simple itération sur le tableau en-têtes.
    /// </summary>
    /// <returns>Un objet de type 'Sprite'.</returns>
    public IEnumerator<string> GetEnumerator()
    {
        foreach (string chaine in va_elements)
            yield return chaine;
    }
    //========================================================================================
    /// <summary>
    /// Initialise une collection d'objets 'Sprite'.
    /// </summary>
    /// <param name="pOwner">Propriétaire du tableau des en-têtes.</param>
    /// <param name="pSize">Taille du tableau.</param>
    /// <param name="pColumn">Une en-tête de colonne ou non.</param>
    public HeaderArray(BaseGrid pOwner, int pSize, bool pColumn)
    {
        va_owner = pOwner;
        va_elements = new string[pSize];
        va_column = pColumn;
    }
    //========================================================================================
    /// <summary>
    /// Obtient ou définit le texte à l'index spécifié dans le tableau des en-têtes.
    /// </summary>
    /// <param name="pIndex"></param>
    /// <returns></returns>
    public string this[int pIndex]
    {
        get
        {
            if (pIndex >= va_elements.Length || pIndex < 0)
            {
                throw new VisualArrayException("Débordement de la grille : pIndex = " + pIndex);
            }

            return va_elements[pIndex];
        }
        set
        {
            if (pIndex >= va_elements.Length || pIndex < 0)
            {
                throw new VisualArrayException("Débordement de la grille : pIndex = " + pIndex);
            }

            va_elements[pIndex] = value;

            if (va_column)
            {
                va_owner.DrawColumnHeader(pIndex, value);
            }
            else
            {
                va_owner.DrawRowHeader(pIndex, value);
            }
        }
    }
}