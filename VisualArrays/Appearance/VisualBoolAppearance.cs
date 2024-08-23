using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Drawing;
using System.Windows.Forms;
using VisualArrays.VisualCells;

namespace VisualArrays
{
    #region VisualBoolAppearance

    /// <summary>
    /// Fournit les informations concernant l'apparence des cellules dont l'état Enabled est true
    /// </summary>
    [TypeConverter(typeof(AppearanceConverter)), Description("Détermine différents aspects de l'apparence selon la valeur de la propriété Value")]
    public class VisualBoolAppearance
    {
        /// <summary>
        /// Initialise un BackgroundAppearance object
        /// </summary>
        protected VisualValue<bool> m_owner;
        /// <summary>
        /// Initialise un VisualBoolAppearance object
        /// </summary>
        /// <param name="pOwner">VisualArray propriétaire du CellsAppearance</param>
        public VisualBoolAppearance(VisualValue<bool> pOwner)
        {
            m_owner = pOwner;
            va_trueAppearance = new VisualBoolValueAppearance(pOwner);
            va_falseAppearance = new VisualBoolValueAppearance(pOwner);
        }

        #region true
        /// <summary>
        /// Détermine différents aspects de l'apparence lorsque l'état est true
        /// </summary>
        protected internal VisualBoolValueAppearance va_trueAppearance;
        /// <summary>
        /// Détermine différents aspects de l'apparence lorsque l'état est true
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category("VisualArrays")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public VisualBoolValueAppearance True
        {
            get { return va_trueAppearance; }
            set
            {
                va_trueAppearance = value;
                m_owner.Refresh();
            }
        }
        #endregion

        #region false
        /// <summary>
        /// Détermine différents aspects de l'apparence lorsque l'état est false
        /// </summary>
        protected internal VisualBoolValueAppearance va_falseAppearance;
        /// <summary>
        /// Détermine différents aspects de l'apparence lorsque l'état est false
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category("VisualArrays")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public VisualBoolValueAppearance False
        {
            get { return va_falseAppearance; }
            set
            {
                va_falseAppearance = value;
                m_owner.Refresh();
            }
        }
        #endregion
    }
    #endregion
}
