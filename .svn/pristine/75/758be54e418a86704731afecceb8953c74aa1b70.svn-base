using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JFInterfaceDef
{
    public class JFMotionParamTypeConvert: TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }

            // Always call the base to see if it can perform the conversion.
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                ConstructorInfo ci = typeof(JFMotionParam).GetConstructor(new Type[] { }/*new Type[]{typeof(Point),
                                                    typeof(Point),typeof(Point)}*/);
                JFMotionParam t = (JFMotionParam)value;
                return new InstanceDescriptor(ci, new object[] { });
            }

            // Always call base, even if you can't convert.
            return base.ConvertTo(context, culture, value, destinationType);

        }

        //public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, Type sourceType)
        //{
        //    if (sourceType == typeof(string))
        //    {
        //        return true;
        //    }
        //    return base.CanConvertFrom(context, sourceType);
        //}

        //public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        //{

        //}
    }
}
