using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JFToolKits
{
    /// <summary>
    /// 字节数据池,用于Tcp/Com 通讯的辅助类
    /// </summary>
    public class JFBytesBuff
    {
        public JFBytesBuff()
        {
            Prefix = null;
            Suffix = null;
        }


        List<Byte> _buff = new List<byte>();


        public List<Byte> Buff { get { return _buff; } }
      

        


        public void Lock()
        {
            Monitor.Enter(_buff);
        }


        public void Unlock()
        {
            Monitor.Exit(_buff);
        }

        
        public byte[] Prefix 
        {
            get;set;
        }

        //byte[] _suffix = null;
        public byte[] Suffix
        {
            get;set;
        }

        /// <summary>
        /// 缓冲区中的字节数
        /// </summary>
        public int Bytes { get { return _buff.Count; } }

        public void Clear()
        {
            lock (_buff)
            {
                _buff.Clear();
            }
        }


        /// <summary>
        /// 向缓冲区添加字节数据
        /// </summary>
        /// <param name="bytes"></param>
        public void AddBytes(byte[] bytes)
        {
            if (null == bytes || 0 == bytes.Length)
                return;
            lock (_buff)
            {
                _buff.AddRange(bytes);
            }
        }



        /// <summary>
        /// 由指定前缀和后缀定义的数据行数
        /// </summary>
        /// <returns></returns>
        public int Lines()
        {
            if (_buff.Count == 0)
                return 0;

            if(null == Prefix || 0 == Prefix.Length) //未定义前缀
            {
                if (null == Suffix || Suffix.Length == 0)//未定义后缀
                    return 1;

                //定义了后缀 , 查找指定的后缀,每个后缀前的字节都作为一个完整的Line
                if (_buff.Count < Suffix.Length)
                    return 0;
                int findCount = 0; //已经找到的Suffix数量
                for (int i = 0; i < _buff.Count - Suffix.Length + 1; i++)
                {
                    bool isMatched = true;
                    for (int j = 0; j < Suffix.Length; j++)
                    {
                        if (_buff[i + j] != Suffix[j])
                        {
                            isMatched = false;
                            break;
                        }
                    }
                    if (isMatched)
                    {
                        findCount++;
                        i += Suffix.Length;
                    }

                }
                return findCount;



            }
            else//定义了前缀
            {
                int findCount = 0;
                if(null == Suffix || Suffix.Length == 0)//未定义后缀
                {
                    if (_buff.Count <= Prefix.Length)
                        return 0;
                    
                    for(int i = 0;i < _buff.Count-Prefix.Length;i++)
                    {
                        bool isMatched = true;
                        for (int j = 0; j < Suffix.Length; j++)
                        {
                            if (_buff[i + j] != Suffix[j])
                            {
                                isMatched = false;
                                break;
                            }
                        }
                        if (isMatched)
                        {
                            findCount++;
                            i += Suffix.Length;
                        }
                    }

                
                }
                else//既定义了前缀,也定义了后缀
                {

                }

                return findCount;
            }


            

        }


        /// <summary>
        /// 获取某一行的开始位置,在Buff中的字节顺序(不包含已定义的前缀)
        /// </summary>
        /// <param name="lineIndex"></param>
        /// <returns></returns>
        public int LineStart(int lineIndex)
        {
            return -1;
        }


        /// <summary>
        /// 获取指定的行字节内容(不包含前后缀)
        /// </summary>
        /// <param name="lineIndex"></param>
        /// <returns></returns>
        public byte[] GetLine(int lineIndex)
        {
            return null;
        }

        /// <summary>
        /// 移除指定的行
        /// </summary>
        /// <param name="lineIndex"></param>
        public void RemoveLine(int lineIndex)
        {

        }





    }
}
