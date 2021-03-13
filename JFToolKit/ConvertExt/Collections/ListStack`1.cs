﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace JFToolKits.Collections
{
	/// <summary>
	/// 表示同一任意类型的实例的大小可变的后进先出 (LIFO) 集合。
	/// 该集合还允许使用索引访问堆栈中的元素。
	/// </summary>
	/// <typeparam name="T">指定堆栈中的元素的类型。</typeparam>
	/// <seealso cref="Stack{T}"/>
	[Serializable]
	public class ListStack<T> : Stack<T>
	{
		/// <summary>
		/// 获取 <see cref="Stack{T}"/> 实例的 <c>_array</c> 字段。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly Func<Stack<T>, T[]> getArrayField =
			typeof(Stack<T>).CreateDelegate<Func<Stack<T>, T[]>>("_array");
		/// <summary>
		/// 初始化 <see cref="ListStack{T}"/> 类的新实例，该实例为空并且具有默认初始容量。
		/// </summary>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="ListStack{T}"/> 类的新实例。
		/// </summary>
		/// </overloads>
		public ListStack() { }
		/// <summary>
		/// 初始化 <see cref="ListStack{T}"/> 类的新实例，
		/// 该实例包含从指定集合复制的元素并且具有足够的容量来容纳所复制的元素。
		/// </summary>
		/// <param name="collection">从其中复制元素的集合。</param>
		public ListStack(IEnumerable<T> collection) : base(collection) { }
		/// <summary>
		/// 初始化 <see cref="ListStack{T}"/> 类的新实例，
		/// 该实例为空并且具有指定的初始容量或默认初始容量（这两个容量中的较大者）。
		/// </summary>
		/// <param name="capacity"><see cref="ListStack{T}"/> 可包含的初始元素数。</param>
		public ListStack(int capacity) : base(capacity) { }
		/// <summary>
		/// 获取指定索引处的元素。
		/// </summary>
		/// <param name="index">要获取的元素从零开始的索引。</param>
		/// <value>指定索引处的元素。</value>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 不是 
		/// <see cref="ListStack{T}"/> 中的有效索引。</exception>
		public T this[int index]
		{
			get
			{
				if (index < 0)
				{
					throw CommonExceptions.ArgumentNegative("index", index);
				}
				if (index >= this.Count)
				{
					throw CommonExceptions.ArgumentOutOfRange("index", index);
				}
				Contract.EndContractBlock();
				return getArrayField(this)[index];
			}
		}
	}
}
