namespace VisualArrays
{
    partial class BaseGrid
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                if (va_gridOffScreenBitMap != null)
                    va_gridOffScreenBitMap.Dispose();
                if (va_gridOffScreenGraphic != null)
                    va_gridOffScreenGraphic.Dispose();
                if (va_spriteOffScreenBitMap != null)
                    va_spriteOffScreenBitMap.Dispose();
                if (va_spriteOffScreenGraphic != null)
                    va_spriteOffScreenGraphic.Dispose();
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // BaseGrid
            // 
            this.Name = "BaseGrid";
            this.ResumeLayout(false);

        }
        #endregion

    }
}