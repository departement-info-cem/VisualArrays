namespace VisualArrays.Others
{
    /// <summary>
    /// 
    /// </summary>
    interface ISelectable
    {
        //============================================================================================
        /// <summary>
        /// 
        /// </summary>
        int SelectedIndex
        {
            get;
            set;
        }
        //============================================================================================
        /// <summary>
        /// 
        /// </summary>
        SelectionMode SelectionMode
        {
            get;
            set;
        }
        //============================================================================================
        /// <summary>
        /// 
        /// </summary>
        int SelectionSize
        {
            get;
            set;
        }
        //============================================================================================
        /// <summary>
        /// 
        /// </summary>
        int SelectionOffset
        {
            get;
            set;
        }
        //============================================================================================
        /// <summary>
        /// 
        /// </summary>
        Color SelectionColor
        {
            get;
            set;
        }
        //============================================================================================
        /// <summary>
        /// 
        /// </summary>
        event EventHandler SelectedIndexChanged;
    }
}
