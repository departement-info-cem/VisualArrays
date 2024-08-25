using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

namespace VisualArrays.Others
{
    #region Structure Address
    /// <summary>
    /// Représente l'adresse d'une cellule dans une grille sous la forme rangée et colonne.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(AddressConverter))]
    [ComVisible(true)]
    public struct Address
    {
        /// <summary>
        ///     Représente une adresse où la valeur int.MinValue est affectée aux propriétés
        ///     Address.Row et Address.Column. 
        /// </summary>
        public static readonly Address Empty = new Address(int.MinValue, int.MinValue);
        //// <summary>
        //// Obtient une valeur indiquant si cette adresse est vide.
        //// </summary>
        ////[Browsable(false)]
        ////public bool IsEmpty { get { return this == Address.Empty; } }
        /// <summary>
        /// Compare deux Adresses. Le résultat spécifie si les valeurs des propriétés 
        /// Address.Row ou Address.Column des deux adresses sont différentes.
        /// </summary>
        /// <param name="left">Adresse à comparer</param>
        /// <param name="right">Adresse à comparer</param>
        /// <returns>Si les deux adresses sont différentes</returns>
        public static bool operator !=(Address left, Address right)
        {
            return left.Row != right.Row || left.m_column != right.m_column;
        }
        /// <summary>
        /// Compare deux Adresses. Le résultat spécifie si les valeurs des propriétés 
        /// Address.Row et Address.Column des deux adresses sont égales.
        /// </summary>
        /// <param name="left">Adresse à comparer</param>
        /// <param name="right">Adresse à comparer</param>
        /// <returns>Si les deux adresses sont identiques</returns>
        public static bool operator ==(Address left, Address right)
        {
            return left.Row == right.Row && left.m_column == right.m_column;
        }
        //----------------------------------------------------------------
        private int m_row;
        /// <summary>
        /// Obtient ou définit la valeur pour la rangée.
        /// </summary>
        [DefaultValue(typeof(int), "0"), Description("Rangée")]
        public int Row
        {
            get { return m_row; }
            set { m_row = value; }
        }        
        //----------------------------------------------------------------
        private int m_column;
        /// <summary>
        /// Obtient ou définit la valeur pour la colonne.
        /// </summary>
        [DefaultValue(typeof(int), "0"), Description("Colonne")]
        public int Column
        {
            get { return m_column; }
            set { m_column = value; }
        }
        //----------------------------------------------------------------
        private void ResetAddress()
        {
            m_row = 0;
            m_column = 0;
        }
        //----------------------------------------------------------------
        private bool ShouldSerializeAddress()
        {
            return (m_row > 0 || m_column > 0);
        }
        //----------------------------------------------------------------
        /// <summary>
        /// Initialise l'adresse.
        /// </summary>
        /// <param name="pRow">La rangée.</param>
        /// <param name="pColumn">La colonne.</param>
        public Address(int pRow, int pColumn)
        {
            m_row = pRow;
            m_column = pColumn;
        }
        /// <summary>
        /// Applique un déplacement à l'adresse selon les valeurs spécifiées
        /// </summary>
        /// <param name="pRow">Valeur de l'offset de la rangée</param>
        /// <param name="pColumn">Valeur de l'offset de la colonne</param>
        public void Offset(int pRow, int pColumn)
        {
            Row += pRow;
            m_column += pColumn;
        }
        /// <summary>
        /// Retourne un String qui représente cette adresse
        /// </summary>
        /// <returns>Rangée et colonne sous la forme (Row,Column)</returns>
        public override string ToString()
        {
            return m_row + "," + m_column;
        }
        /// <summary>
        ///  Spécifie si cette Address contient la même rangée et colonne que le System.Object spécifié.
        /// </summary>
        /// <param name="obj">System.Object à tester.</param>
        /// <returns>true si obj possède la même rangée et colonne que cette Address</returns>
        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            Address pAddress = (Address)obj;
            return (m_row == pAddress.Row) && (m_column == pAddress.m_column);
        }
        /// <summary>
        /// Retourne un code de hachage pour cette Address
        /// </summary>
        /// <returns>Valeur entière qui spécifie une valeur de hachage pour cette Address</returns>
        public override int GetHashCode()
        {
            return m_row ^ m_column;
        }
    }
    #endregion

    #region AddrressConverter
    /// <summary>
    /// 
    /// </summary>
    public class AddressConverter : TypeConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context,
           Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }
        // Overrides the ConvertFrom method of TypeConverter.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext context,
           CultureInfo culture, object value)
        {
            if (value is string)
            {
                string[] v = ((string)value).Split(new char[] { ',' });
                return new Address(int.Parse(v[0]), int.Parse(v[1]));
            }
            return base.ConvertFrom(context, culture, value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }
            // Always call the base to see if it can perform the conversion.
            return base.CanConvertTo(context, destinationType);
        }
        // Overrides the ConvertTo method of TypeConverter.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context,
           CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                ConstructorInfo ci = typeof(Address).GetConstructor(new Type[]{typeof(int),typeof(int)});
                Address t = (Address)value;
                return new InstanceDescriptor(ci, new object[] { t.Row, t.Column});
            }

            // Always call base, even if you can't convert.
            return base.ConvertTo(context, culture, value, destinationType);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(value);
        }
    }


    #endregion

}
