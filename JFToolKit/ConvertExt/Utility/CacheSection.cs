﻿using System.Configuration;

namespace JFToolKits.Utility
{
	/// <summary>
	/// 表示 JFToolKits.cache 缓冲池配置节。
	/// </summary>
	/// <remarks>JFToolKits.cache 配置的使用请参见 <see cref="CacheFactory"/>。</remarks>
	/// <seealso cref="CacheFactory"/>
	public sealed class CacheSection : ConfigurationSection
	{
		/// <summary>
		/// 初始化 <see cref="CacheSection"/> 类的新实例。
		/// </summary>
		public CacheSection() { }
		/// <summary>
		/// 获取缓存配置。
		/// </summary>
		/// <value>缓存配置。</value>
		[ConfigurationProperty("", IsDefaultCollection = true)]
		[ConfigurationCollection(typeof(CacheElementCollection), AddItemName = "cache")]
		public CacheElementCollection Caches
		{
			get { return ((CacheElementCollection)this[""]); }
		}
	}
}
