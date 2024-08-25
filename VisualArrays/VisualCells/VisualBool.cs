using System.ComponentModel;
using VisualArrays.Appearance;

namespace VisualArrays.VisualCells
{
    /// <summary>
    /// Représente une cellule booléenne. 
    /// </summary>
    public partial class VisualBool : VisualValue<bool>
    {
        #region Événements
        /// <summary>
        /// Se produit lorsque la valeur du contrôle change
        /// </summary>
        [Description("Se produit lorsque la valeur du contrôle change")]
        public override event EventHandler ValueChanged;
        #endregion

        #region Constructeur
        //=========================================================================================================
        /// <summary>
        /// Initialise un contrôle VisualBool
        /// </summary>
        public VisualBool()
        {
            va_valueAppearance = new VisualBoolAppearance(this);
            InitializeComponent();
            m_backgroundVE = GetNewBkgVisualElement(va_valueAppearance.va_falseAppearance);
            m_foreGroundVE = GetNewBkgVisualElement(va_valueAppearance.va_trueAppearance);
        }
        #endregion

        #region Propriétés
        //============================================================================================
        /// <summary>
        /// VisualElement utilisé pour dessiner le fond de la cellule
        /// </summary>
        internal CellVisualElement.CellVisualElement m_foreGroundVE = null;

        /// <summary>
        /// Détermine différents aspects de l'apparence selon la valeur de la propriété Value
        /// </summary>
        protected VisualBoolAppearance va_valueAppearance;
        /// <summary>
        /// Détermine différents aspects de l'apparence selon la valeur de la propriété Value
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public VisualBoolAppearance ValueAppearance
        {
            get { return va_valueAppearance; }
            set
            {
                va_valueAppearance = value;
                m_backgroundVE = GetNewBkgVisualElement(va_valueAppearance.va_falseAppearance);
                m_foreGroundVE = GetNewBkgVisualElement(va_valueAppearance.va_trueAppearance);
                Refresh();
            }
        }
        //=========================================================================================================
        private bool m_toogle = true;
        /// <summary>
        /// Détermine si un click sur le contrôle provoque une inversion de son état.
        /// </summary>
        [Category("VisualBool"), DefaultValue(true), Browsable(true), Description("Détermine si un click sur le contrôle provoque une inversion de son état")]
        public bool Toggle
        {
            get { return m_toogle; }
            set { m_toogle = value; }
        }
        //=========================================================================================================
        private bool m_value = false;
        /// <summary>
        /// La valeur actuelle du contrôle VisualBool.
        /// </summary>
        [Category("VisualBool"),DefaultValue(false), Browsable(true), Description("La valeur actuelle du contrôle VisualBool")]
        public override bool Value
        {
            get { return m_value; }
            set
            {
                if (m_value != value)
                {
                    m_value = value;
                    using (Graphics gr = CreateGraphics())
                        DrawContent(gr);
                    if (ValueChanged != null)
                        ValueChanged(this, EventArgs.Empty);
                }
            }
        }
        #endregion

        #region Méthodes privées
        //=========================================================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pGraphics"></param>
        protected override void DrawContent(Graphics pGraphics)
        {
            if (pGraphics == null)
                pGraphics = CreateGraphics();

            Rectangle cellBounds = new Rectangle(Padding.Left, Padding.Top, Width - (Padding.Left + Padding.Right), Height - (Padding.Top + Padding.Bottom));

            if (m_value)
            {
                if (m_foreGroundVE != null)
                    m_foreGroundVE.Draw(pGraphics, cellBounds);
                //using (Brush pinceau = new SolidBrush(va_valueAppearance.True.BackgroundColor))
                  //  pGraphics.FillRectangle(pinceau, cellBounds);
            }
            else
            {
                if (m_backgroundVE != null)
                    m_backgroundVE.Draw(pGraphics, cellBounds);
                //using (Brush pinceau = new SolidBrush(va_valueAppearance.False.BackgroundColor))
                  //  pGraphics.FillRectangle(pinceau, cellBounds);
            }
            //------------------------------------------------------------------------------------------------
            if (ShowIndex && DesignMode) // on va afficher l'index du contrôle dans son conteneur
            {
                VisualArraysTools.DrawText(pGraphics, cellBounds, "[" + Index + "]", ForeColor, Font, m_valueAlign);
            }
            base.DrawContent(pGraphics);
        }
        #endregion

        #region Méthodes redéfinies
        //=========================================================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            Focus();
            if (!ReadOnly)
            {
                if (m_toogle)
                    Value = !Value;
            }
        }
        #endregion

        #region RandomizeValue
        /// <summary>
        /// Assigne une valeur aléatoire à la propriété Value dans l'intervalle : Minimum à Maximum
        /// </summary>
        public void RandomizeValue()
        {
            Value = VisualArraysTools.RandomBool();
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        internal override void UpdateVisualElement()
        {
            m_backgroundVE = GetNewBkgVisualElement(va_valueAppearance.va_falseAppearance);
            m_foreGroundVE = GetNewBkgVisualElement(va_valueAppearance.va_trueAppearance);
            Refresh();
        }
    }
}
