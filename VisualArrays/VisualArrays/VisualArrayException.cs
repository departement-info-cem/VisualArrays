namespace VisualArrays.VisualArrays;

/// <summary>
/// Définit une exception pour les débordements d'index de ligne ou de colonne.
/// </summary>
public class VisualArrayException:ApplicationException
{
    //--- Constructeur avec appel du constructeur de la classe de base -------
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pMessage"></param>
    public VisualArrayException(string pMessage): base(pMessage)
    {
    }
}