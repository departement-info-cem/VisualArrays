using VisualArrays.Sprites;

namespace VisualArrays.Others;

//-------------------------------------------------------------------------------------
/// <summary>
/// Représente un objet événement qui se produit avant le changement du contenu d'une cellule d'une VisualArrays
/// </summary>
public class BeforeValueChangedArgs<ValueType> : EventArgs
{
    /// <summary>
    /// Indique si la nouvelle valeur doit être acceptée
    /// </summary>
    public bool AcceptValueChanged;
    /// <summary>
    /// Nouvelle valeur proposée
    /// </summary>
    public ValueType NewValue;
    /// <param name="pAcceptValueChanged">Indique si le changement de valeur est accepté</param>
    /// <param name="pNewValue">Nouvelle valeur proposée</param>
    public BeforeValueChangedArgs(bool pAcceptValueChanged, ValueType pNewValue)
    {
        AcceptValueChanged = pAcceptValueChanged;
        NewValue = pNewValue;
    }
}
/// <summary>
/// Représente un objet événement qui se produit avant le changement du contenu d'une cellule d'une VisualBoolArray
/// </summary>
public class BeforeValueChangedArgsVBA : BeforeValueChangedArgs<bool>
{
    /// <summary>
    /// Initialise un BeforeValueChangedVBA
    /// </summary>
    /// <param name="pAcceptValueChanged">Indique si le changement de valeur est accepté</param>
    /// <param name="pNewValue">Nouvelle valeur proposée</param>
    public BeforeValueChangedArgsVBA(bool pAcceptValueChanged, bool pNewValue)
        : base(pAcceptValueChanged, pNewValue)
    {
    }
}
/// <summary>
/// Représente un objet événement qui se produit avant le changement du contenu d'une cellule d'une VisualCharArray
/// </summary>
public class BeforeValueChangedArgsVCA : BeforeValueChangedArgs<char>
{
    /// <summary>
    /// Initialise un BeforeValueChangedVCA
    /// </summary>
    /// <param name="pAcceptValueChanged">Indique si le changement de valeur est accepté</param>
    /// <param name="pNewValue">Nouvelle valeur proposée</param>
    public BeforeValueChangedArgsVCA(bool pAcceptValueChanged, char pNewValue)
        : base(pAcceptValueChanged, pNewValue)
    {
    }
}
/// <summary>
/// Représente un objet événement qui se produit avant le changement du contenu d'une cellule d'une VisualStringArray
/// </summary>
public class BeforeValueChangedArgsVSA : BeforeValueChangedArgs<string>
{
    /// <summary>
    /// Initialise un BeforeValueChangedVSA
    /// </summary>
    /// <param name="pAcceptValueChanged">Indique si le changement de valeur est accepté</param>
    /// <param name="pNewValue">Nouvelle valeur proposée</param>
    public BeforeValueChangedArgsVSA(bool pAcceptValueChanged, string pNewValue)
        : base(pAcceptValueChanged, pNewValue)
    {
    }
}
/// <summary>
/// Représente un objet événement qui se produit avant le changement du contenu d'une cellule d'une VisualDecimalArray
/// </summary>
public class BeforeValueChangedArgsVDA : BeforeValueChangedArgs<decimal>
{
    /// <summary>
    /// Initialise un BeforeValueChangedVDA
    /// </summary>
    /// <param name="pAcceptValueChanged">Indique si le changement de valeur est accepté</param>
    /// <param name="pNewValue">Nouvelle valeur proposée</param>
    public BeforeValueChangedArgsVDA(bool pAcceptValueChanged, decimal pNewValue)
        : base(pAcceptValueChanged, pNewValue)
    {
    }
}
/// <summary>
/// Représente un objet événement qui se produit avant le changement du contenu d'une cellule d'une VisualIntArray
/// </summary>
public class BeforeValueChangedArgsVIA : BeforeValueChangedArgs<int>
{
    /// <summary>
    /// Initialise un BeforeValueChangedVIA
    /// </summary>
    /// <param name="pAcceptValueChanged">Indique si le changement de valeur est accepté</param>
    /// <param name="pNewValue">Nouvelle valeur proposée</param>
    public BeforeValueChangedArgsVIA(bool pAcceptValueChanged, int pNewValue)
        : base(pAcceptValueChanged, pNewValue)
    {
    }
}
//-------------------------------------------------------------------------------------
/// <summary>
/// Fournit les données des événements pour les en-têtes de rangées.
/// </summary>
public class RowHeaderEventArgs : EventArgs
{
    /// <summary>
    /// Index de la rangée où se produit l'événement 
    /// </summary>
    public int Row;
    /// <param name="pRow">rangée ou se produit l'événement</param>
    public RowHeaderEventArgs(int pRow)
    {
        Row = pRow;
    }
}

//-------------------------------------------------------------------------------------
/// <summary>
/// Fournit les données des événements pour les en-têtes de colonnes.
/// </summary>
public class ColumnHeaderEventArgs : EventArgs
{
    /// <summary>
    /// Index de la colonne où se produit l'événement 
    /// </summary>
    public int Column;
    /// <param name="pColumn">colonne ou se produit l'événement</param>
    public ColumnHeaderEventArgs(int pColumn)
    {
        Column = pColumn;
    }
}

//-------------------------------------------------------------------------------------
/// <summary>
/// Fournit les données des événements sur la gestion de la souris sur un Sprite
/// </summary>
public class SpriteMouseEventArgs : MouseEventArgs
{
    private Sprite m_sprite;
    /// <summary>
    /// Sprite sur lequel l'événement s'est produit.
    /// </summary>
    public Sprite Sprite
    {
        get { return m_sprite; }
    }
    /// <summary>
    ///  Initialise une nouvelle instance de la classe SpriteMouseEventArgs.
    /// <param name="pButton">Une des valeurs System.Windows.Forms.MouseButtons indiquant le bouton de la souris sur lequel l'utilisateur a appuyé.</param>
    /// <param name="pClicks">Le nombre de fois où l'utilisateur a cliqué sur un bouton de la souris.</param>
    /// <param name="pX">Coordonnée x d'un clic de la souris en pixels.</param>
    /// <param name="pY">Coordonnée y d'un clic de la souris en pixels.</param>
    /// <param name="pDelta">Décompte signé du nombre de détentes de rotation de la roulette de la souris.</param>
    /// <param name="pSprite">Sprite sur lequel l'événement s'est produit</param>
    /// </summary>
    public SpriteMouseEventArgs(MouseButtons pButton, int pClicks, int pX, int pY, int pDelta,Sprite pSprite)
        : base(pButton, pClicks, pX, pY, pDelta)
    {
        m_sprite = pSprite;
    }
}
//-------------------------------------------------------------------------------------
/// <summary>
/// Fournit les données des événements sur une cellule.
/// </summary>
public class CellEventArgs : EventArgs
{
    /// <summary>
    /// Index de la cellule où se produit l'événement 
    /// </summary>
    public int Index;
    /// <summary>
    /// Rangée de la cellule où se produit l'événement 
    /// </summary>
    public int Row;
    /// <summary>
    /// Colonne de la cellule où se produit l'événement 
    /// </summary>
    public int Column;
    /// <summary>
    ///  Initialise une nouvelle instance de la classe CellEventArgs.
    /// </summary>
    /// <param name="pIndex">Index de la cellule où se produit l'événement</param>
    /// <param name="pRow">Rangée de la cellule où se produit l'événement</param>
    /// <param name="pColumn">Colonne de la cellule où se produit l'événement</param>
    public CellEventArgs(int pIndex, int pRow, int pColumn)
    {
        Index = pIndex;
        Row = pRow;
        Column = pColumn;
    }
}

//-------------------------------------------------------------------------------------
/// <summary>
/// Fournit les données des événements sur un Sprite.
/// </summary>
public class SpriteEventArgs : EventArgs
{
    /// <summary>
    /// Sprite sur lequel l'événement s'est produit.
    /// </summary>
    public Sprite Sprite;
    /// <summary>
    ///  Initialise une nouvelle instance de la classe SpriteEventArgs.
    /// </summary>
    /// <param name="pSprite">Sprite où se produit l'événement</param>
    public SpriteEventArgs(Sprite pSprite)
    {
        Sprite = pSprite;
    }
}

//-------------------------------------------------------------------------------------
/// <summary>
/// Fournit les données des événements sur la gestion de la souris sur une cellule
/// </summary>
public class CellMouseEventArgs1D : MouseEventArgs
{
    /// <summary>
    /// Index de la cellule où se produit l'événement 
    /// </summary>
    public int Index;
    /// <summary>
    ///  Initialise une nouvelle instance de la classe CellMouseEventArgs.
    /// <param name="pButton">Une des valeurs System.Windows.Forms.MouseButtons indiquant le bouton de la souris sur lequel l'utilisateur a appuyé.</param>
    /// <param name="pClicks">Le nombre de fois où l'utilisateur a cliqué sur un bouton de la souris.</param>
    /// <param name="pX">Coordonnée x d'un clic de la souris en pixels.</param>
    /// <param name="pY">Coordonnée y d'un clic de la souris en pixels.</param>
    /// <param name="pDelta">Décompte signé du nombre de détentes de rotation de la roulette de la souris.</param>
    /// <param name="pIndex">Index de la cellule où se produit l'événement</param>
    /// </summary>
    public CellMouseEventArgs1D(MouseButtons pButton, int pClicks, int pX, int pY, int pDelta, int pIndex)
        : base(pButton, pClicks, pX, pY, pDelta)
    {
        Index = pIndex;
    }
}
//-------------------------------------------------------------------------------------
/// <summary>
/// Fournit les données des événements sur la gestion de la souris sur une cellule
/// </summary>
public class CellMouseEventArgs : CellMouseEventArgs1D
{
    /// <summary>
    /// Adresse de la cellule où se produit l'événement 
    /// </summary>
    public Address Address;
    /// <summary>
    /// Rangée de la cellule où se produit l'événement 
    /// </summary>
    public int Row;
    /// <summary>
    /// Colonne de la cellule où se produit l'événement 
    /// </summary>
    public int Column;
    /// <summary>
    ///  Initialise une nouvelle instance de la classe CellMouseEventArgs.
    /// <param name="pButton">Une des valeurs System.Windows.Forms.MouseButtons indiquant le bouton de la souris sur lequel l'utilisateur a appuyé.</param>
    /// <param name="pClicks">Le nombre de fois où l'utilisateur a cliqué sur un bouton de la souris.</param>
    /// <param name="pX">Coordonnée x d'un clic de la souris en pixels.</param>
    /// <param name="pY">Coordonnée y d'un clic de la souris en pixels.</param>
    /// <param name="pDelta">Décompte signé du nombre de détentes de rotation de la roulette de la souris.</param>
    /// <param name="pIndex">Index de la cellule où se produit l'événement</param>
    /// <param name="pRow">Rangée de la cellule où se produit l'événement</param>
    /// <param name="pColumn">Colonne de la cellule où se produit l'événement</param>
    /// </summary>
    public CellMouseEventArgs(MouseButtons pButton, int pClicks, int pX, int pY, int pDelta,  int pIndex, int pRow, int pColumn) 
        : base(pButton,pClicks,pX,pY,pDelta,pIndex)
    {
        Row = pRow;
        Column = pColumn;
        Address = new Address(pRow, pColumn);
    }
}

//-------------------------------------------------------------------------------------
/// <summary>
/// Objet contenant les informations nécessaires sur l'utilisation d'une opération glisser-déposer
/// </summary>
public class VisualArraysDragEventArgs : EventArgs
{
    /// <summary>
    /// Nom de la grille source de l'opération glisser-déposer
    /// </summary>
    public string SourceGridName;
    /// <summary>
    /// Index source de l'opération glisser-déposer
    /// </summary>
    public int SourceIndex;
    /// <summary>
    /// Adresse source de l'opération glisser-déposer
    /// </summary>
    public Address SourceAddress;
    /// <summary>
    /// Fournit des informations concernant l'opération glisser-déposer
    /// </summary>
    /// <param name="pSourceGridName">Nom de la grille source de l'opération glisser-déposer</param>
    /// <param name="pSourceIndex">Index source de l'opération glisser-déposer</param>
    /// <param name="pSourceAddress">Adresse source de l'opération glisser-déposer</param>
    public VisualArraysDragEventArgs(string pSourceGridName, int pSourceIndex, Address pSourceAddress)
    {
        SourceGridName = pSourceGridName;
        SourceIndex = pSourceIndex;
        SourceAddress = pSourceAddress;
    }
}
//-------------------------------------------------------------------------------------
/// <summary>
/// Objet contenant les informations nécessaires sur l'utilisation d'une opération glisser-déposer
/// </summary>
public class CellDragEventArgs : VisualArraysDragEventArgs
{
    /// <summary>
    ///  true pour annuler l'opération sinon, false.
    /// </summary>
    public bool Cancel;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pSourceGridName">Nom de la grille source de l'opération glisser-déposer</param>
    /// <param name="pSourceIndex">Index source de l'opération glisser-déposer</param>
    /// <param name="pSourceAddress">Adresse source de l'opération glisser-déposer</param>
    /// <param name="pCancel">true pour annuler l'opération, false autrement</param>
    public CellDragEventArgs(string pSourceGridName, int pSourceIndex, Address pSourceAddress, bool pCancel)
        : base(pSourceGridName, pSourceIndex, pSourceAddress)
    {
        Cancel = pCancel;
    }
}
//-------------------------------------------------------------------------------------
/// <summary>
/// Objet contenant les informations nécessaires sur l'utilisation d'une opération glisser-déposer
/// </summary>
public class SpriteDragEventArgs : EventArgs
{
    /// <summary>
    /// Nom de la grille source de l'opération glisser-déposer
    /// </summary>
    public string SourceGridName;
    /// <summary>
    /// Index du Sprite dans la collection des Sprites de la grille source
    /// </summary>
    public Sprite Sprite;
    /// <summary>
    ///  true pour annuler l'opération sinon, false.
    /// </summary>
    public bool Cancel;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pSourceGridName">Nom de la grille source de l'opération glisser-déposer</param>
    /// <param name="pSprite">Sprite impliqué dans l'opération glisser-déposer</param>
    /// <param name="pCancel">true pour annuler l'opération, false autrement</param>
    public SpriteDragEventArgs(string pSourceGridName, Sprite pSprite, bool pCancel)
    {
        SourceGridName = pSourceGridName;
        Sprite = pSprite;
        Cancel = pCancel;
    }
}

//-------------------------------------------------------------------------------------
/// <summary>
/// Objet contenant les informations nécessaires sur l'utilisation d'une opération glisser-déposer d'un Sprite
/// </summary>
public class SpriteDragDropEventArgs : VisualArraysDragEventArgs
{
    /// <summary>
    /// Sprite source de l'opération glisser-déposer
    /// </summary>
    public Sprite Sprite;
    /// <summary>
    /// Index destinataire de l'opération glisser-déposer
    /// </summary>
    public int DestinationIndex;
    /// <summary>
    /// Adresse destinataire de l'opération glisser-déposer
    /// </summary>
    public Address DestinationAddress;
    /// <summary>
    /// Emplacement en pixels du Sprite à l'intérieur de la cellule
    /// </summary>
    public Point DestinationPixels;
    /// <summary>
    /// Fournit des informations concernant l'opération glisser-déposer pour un Sprite
    /// </summary>
    /// <param name="pSourceGridName">Nom de la grille source de l'opération glisser-déposer</param>
    /// <param name="pSprite">Sprite source de l'opération glisser-déposer</param>
    /// <param name="pSourceIndex">Index source de l'opération glisser-déposer</param>
    /// <param name="pSourceAddress">Adresse source de l'opération glisser-déposer</param>
    /// <param name="pDestinationIndex">Index destinataire de l'opération glisser-déposer</param>
    /// <param name="pDestinationAddress">Adresse destinataire de l'opération glisser-déposer</param>
    /// <param name="pDestinationPixels">Emplacement en pixels dans la cellule destinataire de l'opération glisser-déposer</param>
    public SpriteDragDropEventArgs(string pSourceGridName,Sprite pSprite, int pSourceIndex, Address pSourceAddress, int pDestinationIndex, Address pDestinationAddress,Point pDestinationPixels)
        : base(pSourceGridName, pSourceIndex, pSourceAddress)
    {
        SourceGridName = pSourceGridName;
        Sprite = pSprite;
        SourceIndex = pSourceIndex;
        SourceAddress = pSourceAddress;
        DestinationIndex = pDestinationIndex;
        DestinationAddress = pDestinationAddress;
        DestinationPixels = pDestinationPixels;
    }
}

//-------------------------------------------------------------------------------------
/// <summary>
/// Objet contenant les informations nécessaires sur l'utilisation d'une opération glisser-déposer d'un Sprite
/// </summary>
public class SpriteOutsideDropEventArgs : VisualArraysDragEventArgs
{
    /// <summary>
    /// Sprite source de l'opération glisser-déposer
    /// </summary>
    public Sprite Sprite;
    /// <summary>
    /// Emplacement en pixels ou le Sprite à été déposé
    /// </summary>
    public Point Destination;
    /// <summary>
    /// Fournit des informations concernant l'opération glisser-déposer pour un Sprite déposé à l'extérieur des cellules
    /// </summary>
    /// <param name="pSourceGridName">Nom de la grille source de l'opération glisser-déposer</param>
    /// <param name="pSprite">Sprite source de l'opération glisser-déposer</param>
    /// <param name="pSourceIndex">Index source de l'opération glisser-déposer</param>
    /// <param name="pSourceAddress">Adresse source de l'opération glisser-déposer</param>
    /// <param name="pDestination">Emplacement en pixels ou le Sprite à été déposé</param>
    public SpriteOutsideDropEventArgs(string pSourceGridName, Sprite pSprite, int pSourceIndex, Address pSourceAddress, Point pDestination)
        : base(pSourceGridName, pSourceIndex, pSourceAddress)
    {
        SourceGridName = pSourceGridName;
        Sprite = pSprite;
        SourceIndex = pSourceIndex;
        SourceAddress = pSourceAddress;
        Destination = pDestination;
    }
}
//-------------------------------------------------------------------------------------
/// <summary>
/// Objet contenant les informations nécessaires sur l'utilisation d'une opération glisser-déposer pour un Sprite
/// </summary>
public class SpriteDragOverEventArgs : SpriteDragDropEventArgs
{
    /// <summary>
    /// Indique 
    /// </summary>
    public bool Accepted;
    /// <summary>
    /// Fournit des informations concernant l'opération glisser-déposer d'un Sprite
    /// </summary>
    /// <param name="pSourceGridName">Nom de la grille source de l'opération glisser-déposer</param>
    /// <param name="pSprite">Sprite source de l'opération glisser-déposer</param>
    /// <param name="pSourceIndex">Index source de l'opération glisser-déposer</param>
    /// <param name="pSourceAddress">Adresse source de l'opération glisser-déposer</param>
    /// <param name="pDestinationIndex">Index destinataire de l'opération glisser-déposer</param>
    /// <param name="pDestinationAddress">Adresse destinataire de l'opération glisser-déposer</param>
    /// <param name="pDestinationPixels">Emplacement en pixels du Sprite dans la cellule destinataire de l'opération glisser-déposer</param>
    public SpriteDragOverEventArgs(string pSourceGridName, Sprite pSprite, int pSourceIndex, Address pSourceAddress, int pDestinationIndex, Address pDestinationAddress, Point pDestinationPixels)
        : base(pSourceGridName,pSprite, pSourceIndex, pSourceAddress, pDestinationIndex, pDestinationAddress,pDestinationPixels)
    {
        Accepted = true;
    }
}
//-------------------------------------------------------------------------------------
/// <summary>
/// Objet contenant les informations nécessaires sur l'utilisation d'une opération glisser-déposer
/// </summary>
public class CellDragDropEventArgs : VisualArraysDragEventArgs
{
    /// <summary>
    /// Index destinataire de l'opération glisser-déposer
    /// </summary>
    public int DestinationIndex;
    /// <summary>
    /// Adresse destinataire de l'opération glisser-déposer
    /// </summary>
    public Address DestinationAddress;
    /// <summary>
    /// Fournit des informations concernant l'opération glisser-déposer
    /// </summary>
    /// <param name="pSourceGridName">Nom de la grille source de l'opération glisser-déposer</param>
    /// <param name="pSourceIndex">Index source de l'opération glisser-déposer</param>
    /// <param name="pSourceAddress">Adresse source de l'opération glisser-déposer</param>
    /// <param name="pDestinationIndex">Index destinataire de l'opération glisser-déposer</param>
    /// <param name="pDestinationAddress">Adresse destinataire de l'opération glisser-déposer</param>
    public CellDragDropEventArgs(string pSourceGridName,int pSourceIndex, Address pSourceAddress, int pDestinationIndex, Address pDestinationAddress): base(pSourceGridName,pSourceIndex,pSourceAddress)
    {
        DestinationIndex = pDestinationIndex;
        DestinationAddress = pDestinationAddress;
    }
}

//-------------------------------------------------------------------------------------
/// <summary>
/// Objet contenant les informations nécessaires sur l'utilisation d'une opération glisser-déposer
/// </summary>
public class CellDragOverEventArgs : CellDragDropEventArgs
{
    /// <summary>
    /// Indique 
    /// </summary>
    public bool Accepted;
    /// <summary>
    /// Fournit des informations concernant l'opération glisser-déposer
    /// </summary>
    /// <param name="pSourceGridName">Nom de la grille source de l'opération glisser-déposer</param>
    /// <param name="pSourceIndex">Index source de l'opération glisser-déposer</param>
    /// <param name="pSourceAddress">Adresse source de l'opération glisser-déposer</param>
    /// <param name="pDestinationIndex">Index destinataire de l'opération glisser-déposer</param>
    /// <param name="pDestinationAddress">Adresse destinataire de l'opération glisser-déposer</param>
    public CellDragOverEventArgs(string pSourceGridName, int pSourceIndex, Address pSourceAddress, int pDestinationIndex, Address pDestinationAddress)
        : base(pSourceGridName, pSourceIndex, pSourceAddress,pDestinationIndex,pDestinationAddress)
    {
        Accepted = true;
    }
}
/// <summary>
/// Fournit les données des événements sur une cellule.
/// </summary>
public class CellVCEventArgs : EventArgs
{
    //------------------------------------------------------------------------------------
    private int m_index;
    /// <summary>
    /// Index du contrôle dont la valeur a changée
    /// </summary>
    public int Index
    {
        get { return m_index; }
    }
    //------------------------------------------------------------------------------------
    /// <summary>
    /// Fournit les données des événements sur une cellule.
    /// </summary>
    /// <param name="pIndex">Index du contrôle</param>
    public CellVCEventArgs(int pIndex)
    {
        m_index = pIndex;
    }
}
//-------------------------------------------------------------------------------------
/// <summary>
/// Fournit les données d'un événement ValueChanged d'une cellule d'une grille.
/// Lorsque la valeur de Index est -1 cela indique le changement de plusieurs valeurs.
/// </summary>
public class ValueChangedEventArgs : EventArgs
{
    /// <summary>
    /// Adresse de la cellule où se produit l'événement 
    /// </summary>
    public Address Address;
    /// <summary>
    /// Index de la cellule où se produit l'événement 
    /// </summary>
    public int Index;
    /// <summary>
    /// Rangée de la cellule où se produit l'événement 
    /// </summary>
    public int Row;
    /// <summary>
    /// Colonne de la cellule où se produit l'événement 
    /// </summary>
    public int Column;
    /// <summary>
    ///  Initialise une nouvelle instance de la classe CellEventArgs.
    /// </summary>
    /// <param name="pIndex">Index de la cellule où se produit l'événement</param>
    /// <param name="pRow">Rangée de la cellule où se produit l'événement</param>
    /// <param name="pColumn">Colonne de la cellule où se produit l'événement</param>
    /// <param name="pAddress">Adresse de la cellule où se produit l'événement</param>
    public ValueChangedEventArgs(int pIndex, int pRow, int pColumn, Address pAddress)
    {
        Address = pAddress;
        Index = pIndex;
        Row = pRow;
        Column = pColumn;
    }
}