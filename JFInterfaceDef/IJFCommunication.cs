using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFInterfaceDef
{

    /// <summary>
    /// JF通讯接口定义（继承类可能包括：TCPClient COM）
    /// </summary>
    public interface IJFCommunication:IJFErrorCode2Txt
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeoutMilliseconds"></param>
        /// <returns></returns>
        int Connect(int timeoutMilliseconds = -1);

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        int DisConnect();

        /// <summary>
        /// 当前是否连接成功
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 使用文本通讯时的编码方式
        /// </summary>
        Encoding Encoding { get; set; }

        /// <summary>
        /// 接受缓冲区的大小
        /// </summary>
        int ReadBuffSize { get; }

        /// <summary>
        /// 设置接受缓冲区的大小
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        int SetReadBuffSize(int bytes);


        int WriteBuffSize { get; }

        int SetWriteBuffSize(int bytes);






        /// <summary>
        /// 当前读缓冲区中存储的字节数
        /// </summary>
        /// <returns></returns>
        int CurrReadBuffBytes();

        /// <summary>
        /// 清空接受缓冲区
        /// </summary>
        void ClearReadBuff();


        /// <summary>
        /// 接收数据前缀,此前缀在从设备（或端口）读入内部buff时会被去掉
        /// </summary>
        byte[] ReadPrefix { get; set; }
        /// <summary>
        /// 接受数据后缀，此后缀在从设备（或端口）读入内部buff时会被去除
        /// </summary>
        byte[] ReadSuffix { get; set; }

        /// <summary>
        /// 发送数据前缀，向设备发送数据时，内部代码会自动加上此数据
        /// </summary>
        byte[] WritePrefix { get; set; }
        /// <summary>
        /// 发送数据后缀，向设备发送数据时，内部代码会自动加上
        /// </summary>
        byte[] WriteSuffix { get; set; }

        /// <summary>
        /// 查询指定的字节序列在buff中首次出现的位置
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        int IndexOfBytes(byte[] bytes);


        /// <summary>
        /// 查询指定的字节序列在buff中出现的位置(从Start下标开始)
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        int IndexOfBytes(byte[] bytes,int start);

        /// <summary>
        /// 从接受缓冲区中读出指定长度的字节序列
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="wantedBytes"></param>
        /// <param name="removeReaded"></param>
        /// <param name="timeoutMilliseconds"></param>
        /// <returns></returns>
        int Read(out byte[] buff,int wantedBytes, bool removeReaded = true, int timeoutMilliseconds = -1);


        /// <summary>
        /// 从接受缓冲区中读出一行数据(以指定的前缀/后缀)
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="removeReaded"></param>
        /// <param name="timeoutMilliseconds"></param>
        /// <returns></returns>
        int ReadLine(out byte[] buff, bool removeReaded = true, int timeoutMilliseconds = -1);


        /// <summary>
        /// 向设备发送一条数据(函数内部会加上前缀/后缀)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int WriteLine(byte[] data, int timeoutMilliseconds = -1);



        /// <summary>
        /// 从接收缓冲区中移除指定的字节数
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        int RemoveReadBuff(int bytes);


        /// <summary>
        /// 查询指定的文本在buff中首次出现的位置
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        int IndexOfTxt(string txt);

        /// <summary>
        /// 查询指定的文本在buff中出现的位置(从Start下标开始)
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        int IndexOfTxt(string txt, int start);

        /// <summary>
        /// 从接收缓冲区中读出一条文本数据
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="wantedChars"></param>
        /// <param name="removeReaded"></param>
        /// <param name="timeoutMilliseconds"></param>
        /// <returns></returns>
        int Read(out string txt, int wantedChars, bool removeReaded = true, int timeoutMilliseconds = -1);

        /// <summary>
        /// 从接受缓冲区中读出一行数据(已指定的前缀/后缀)
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="removeReaded"></param>
        /// <param name="timeoutMilliseconds"></param>
        /// <returns></returns>
        int ReadLine(out string txt, bool removeReaded = true, int timeoutMilliseconds = -1);

        /// <summary>
        /// 向设备发送一条文本数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="timeoutMilliseconds"></param>
        /// <returns></returns>
        int WriteLine(string data, int timeoutMilliseconds = -1);

    }
}
