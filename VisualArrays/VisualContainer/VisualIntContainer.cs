using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VisualArrays.VisualCells;

namespace VisualArrays
{
    /// <summary>
    /// Représente un conteneur de VisualInt
    /// </summary>
    public partial class VisualIntContainer : BaseVisualContainer<int,VisualInt>
    {
        #region Constructeur
        //========================================================================================================================
        /// <summary>
        /// Initialise un VisualIntContainer
        /// </summary>
        public VisualIntContainer()
        {
            InitializeComponent();
        }
        #endregion
    }
}
