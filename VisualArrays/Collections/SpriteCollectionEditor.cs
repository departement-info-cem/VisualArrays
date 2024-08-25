using VisualArrays.Sprites;

namespace VisualArrays.Collections
{
    /// <summary>
    /// Représente un éditeur de collection de 'Sprite'
    /// </summary>
    public class SpriteCollectionEditor : System.ComponentModel.Design.CollectionEditor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public SpriteCollectionEditor(Type type)
            : base(type)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool CanSelectMultipleInstances()
        {
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override bool CanRemoveInstance(object value)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override Type CreateCollectionItemType()
        {
            return typeof(Sprite);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override Type[] CreateNewItemTypes()
        {
            return new Type[] { typeof(FillShapeSprite), typeof(ImageSprite), typeof(ShapeSprite), typeof(TextSprite), typeof(SegmentSprite) };
        }
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="itemType"></param>
        ///// <returns></returns>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        protected override object CreateInstance(Type itemType)
        {
            Sprite objSprite = (Sprite)base.CreateInstance(itemType);
            objSprite.Owner = ((BaseGrid)(this.Context.Instance));
            this.CreateCollectionForm();
            return objSprite;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override CollectionForm CreateCollectionForm()
        {
            return base.CreateCollectionForm();
        }
        //public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        //{
        //    //((BaseVisualArray)(context.Instance)).BackColor = System.Drawing.Color.Red;
        //    ((BaseVisualArray)(context.Instance)).Refresh();
        //    //((Sprite)value).Index = 1;
        //    return base.EditValue(context, provider, value);
        //}
    }
}
