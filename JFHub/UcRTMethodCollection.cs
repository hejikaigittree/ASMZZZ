using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JFInterfaceDef;

namespace JFHub
{
    public partial class UcRTMethodCollection : JFRealtimeUI
    {
        public UcRTMethodCollection()
        {
            InitializeComponent();
        }

        private void UcMethodCollection_Load(object sender, EventArgs e)
        {
            isLoaded = true;
            UpdateDataPoolRow();
            UpdateSrc2UI();
        }
        bool isLoaded = false;
        JFMethodFlow _methodFlow = null;

        public void UpdateDataPoolRow()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(UpdateDataPoolRow));
                return;
            }
            if (null == _methodFlow)
                return;
            int mtdCount = _methodFlow.Count;
            List<string> ExistOutputIDs = new List<string>();

            dgvDataPool.Rows.Clear();
            for (int i = 0; i < mtdCount; i++)
            {
                JFMethodFlow.MethodItem mi = _methodFlow.GetItem(i);
                ///获取各Method对象的实时界面
                IJFMethod mtd = mi.Value;
                ///更新数据池列表(内部)
                string[] outputNames = mi.Value.MethodOutputNames;
                if (null != outputNames)
                    foreach (string outname in outputNames)
                    {
                        DataGridViewRow row = new DataGridViewRow();
                        DataGridViewTextBoxCell cellName = new DataGridViewTextBoxCell();
                        string outputID = mi.OutputID(outname);
                        cellName.Value = outputID;
                        ExistOutputIDs.Add(outputID);
                        row.Cells.Add(cellName);
                        DataGridViewTextBoxCell cellValue = new DataGridViewTextBoxCell();
                        row.Cells.Add(cellValue);
                        dgvDataPool.Rows.Add(row);
                    }


            }
            dgvOutterDataPool.Rows.Clear();
            ///跟新外部数据池
            string[] OutterAvailedDataIDs = _methodFlow.OutterAvailedIDs;
            if (null != OutterAvailedDataIDs)
                foreach (string dataID in OutterAvailedDataIDs)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    DataGridViewTextBoxCell cellName = new DataGridViewTextBoxCell();
                    cellName.Value = dataID;
                    ExistOutputIDs.Add(dataID);
                    row.Cells.Add(cellName);
                    DataGridViewTextBoxCell cellValue = new DataGridViewTextBoxCell();
                    row.Cells.Add(cellValue);
                    dgvOutterDataPool.Rows.Add(row);
                }
        }

        public override void UpdateSrc2UI() 
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateSrc2UI));
                return;
            }
            if (null == _methodFlow)
                return;



            ///更新内部数据池
            Dictionary<string, object> dataPool = _methodFlow.DataPool;
            Dictionary<string, Type> dataTypes = _methodFlow.TypePool;
            foreach (DataGridViewRow row in dgvDataPool.Rows)
            {
                string paramID = row.Cells[0].Value as string;
                if (dataPool.ContainsKey(paramID))
                {
                    object paramValue = dataPool[paramID];
                    if (null == paramValue)
                        row.Cells[1].Value = "null";
                    else
                    {
                        Type paramType = dataTypes[paramID];
                        if (paramType.IsValueType && paramType.IsPrimitive || paramType.IsEnum || paramType == typeof(string))
                            row.Cells[1].Value = paramValue.ToString();
                        else
                        {
                            Type valuesType = paramValue.GetType();
                            if (valuesType.IsValueType && valuesType.IsPrimitive || valuesType.IsEnum || valuesType == typeof(string))
                                row.Cells[1].Value = paramValue.ToString();
                            else
                                row.Cells[1].Value = "类型:" + paramType.Name + " 无法显示";
                        }
                    }
                }
                else
                {
                    row.Cells[1].Value = "无效ID";
                }
            }
            ///更新外部数据池
            Dictionary<string, object> outterDataPool = _methodFlow.OutterDataPool;
            Dictionary<string, Type> outterDataTypes = _methodFlow.OutterTypePool;
            foreach (DataGridViewRow row in dgvOutterDataPool.Rows)
            {
                string paramID = row.Cells[0].Value as string;
                if (outterDataPool.ContainsKey(paramID))
                {
                    object paramValue = outterDataPool[paramID];
                    if (null == paramValue)
                        row.Cells[1].Value = "null";
                    else
                    {
                        Type paramType = outterDataTypes[paramID];
                        if (paramType.IsValueType && paramType.IsPrimitive || paramType.IsEnum || paramType == typeof(string))
                            row.Cells[1].Value = paramValue.ToString();
                        Type valueType = paramValue.GetType();
                        if (valueType.IsValueType && valueType.IsPrimitive || valueType.IsEnum || valueType == typeof(string))
                            row.Cells[1].Value = paramValue.ToString();
                        else
                            row.Cells[1].Value = "类型:" + valueType.Name + " 无法显示";
                    }
                }
                else
                {
                    row.Cells[1].Value = "无效ID";
                }
            }
        }

        public void SetInnerFlow(JFMethodFlow innerFlow)
        {
            _methodFlow = innerFlow;
            if (isLoaded)
            {
                UpdateDataPoolRow();
                UpdateSrc2UI();
            }
        }


    }
}
