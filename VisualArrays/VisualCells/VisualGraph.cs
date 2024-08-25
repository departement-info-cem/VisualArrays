using System.ComponentModel;
using VisualArrays.Appearance;

namespace VisualArrays.VisualCells
{
    /// <summary>
    /// Classe abstraite qui représente une cellule qui peut afficher une barre de graphique. 
    /// </summary>
    public abstract partial class VisualGraph<BaseType> : VisualValue<BaseType>
    {
        #region Code pour la gestion d'un click dans une cellule en mode graphique

        //============================================================================================
        /// <summary>
        /// Indique si le contrôle permet la saisie d'une valeur directement en cliquant dans la zone
        /// </summary>
        protected bool va_allowGraphClick = true;
        /// <summary>
        /// Indique si le contrôle permet la saisie d'une valeur directement en cliquant dans la zone
        /// </summary>
        [DefaultValue(true), Category("VisualArrays"), Browsable(true), Description("Indique si le contrôle permet la saisie d'une valeur directement en cliquant dans la zone")]
        public bool AllowGraphClick
        {
            get { return va_allowGraphClick; }
            set { va_allowGraphClick = value; }
        }
        #endregion

        #region GraphAppearance

        //============================================================================================
        private GraphAppearance m_graphAppearance = null;
        /// <summary>
        /// Détermine différents aspects de l'apparence en mode graphique
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category("VisualArrays")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GraphAppearance GraphAppearance
        {
            get { return m_graphAppearance; }
            set
            {
                if (value != m_graphAppearance)
                {
                    m_graphAppearance = value;
                    Refresh();
                }
            }
        }
        #endregion

        #region Propriétés
        //=========================================================================================================
        private bool m_waitForEnter = true;
        /// <summary>
        /// Détermine si le contrôle attend la touche Enter avant de lancer un événement ValueChanged.
        /// </summary>
        [Category("VisualBool"), DefaultValue(true), Browsable(true), Description("Détermine si le contrôle attend la touche Enter avant de lancer un événement ValueChanged")]
        public bool WaitForEnter
        {
            get { return m_waitForEnter; }
            set { m_waitForEnter = value; }
        }
        #endregion

        #region Constructeur
        //============================================================================================
        /// <summary>
        /// Initialise les champs concernant la partie graphique.
        /// </summary>
        public VisualGraph()
        {
            m_graphAppearance = new GraphAppearance(this);
            InitializeComponent();
        }

        #endregion
    }
}
