﻿using System.ComponentModel;
using System.Diagnostics;

namespace JFToolKits
{
	/// <summary>
	/// 指定属性或事件的说明。
	/// </summary>
	internal sealed class ResDescriptionAttribute : DescriptionAttribute
	{
		/// <summary>
		/// 说明是否已本地化。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool localized;
		/// <summary>
		/// 初始化 <see cref="JFToolKits.ResDescriptionAttribute"/> 类的新实例。
		/// </summary>
		public ResDescriptionAttribute() { }
		/// <summary>
		/// 初始化 <see cref="JFToolKits.ResDescriptionAttribute"/> 
		/// 类的新实例并带有说明。
		/// </summary>
		/// <param name="description">说明文本。</param>
		public ResDescriptionAttribute(string description) : base(description) { }
		/// <summary>
		/// 获取存储在此特性中的说明。
		/// </summary>
		/// <value>存储在此特性中的说明。</value>
		public override string Description
		{
			get
			{
				if (!localized)
				{
					base.DescriptionValue = Resources.ResourceManager.GetString(base.DescriptionValue, Resources.Culture);
					localized = true;
				}
				return base.DescriptionValue;
			}
		}
	}
}
