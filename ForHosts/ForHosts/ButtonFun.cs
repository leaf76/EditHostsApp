using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace ForHosts
{
    public partial class Form1
    {
        //hostsDataB _hostsDataB = new hostsDataB();
        List<hostsDataB> _hostsb = new List<hostsDataB>();
        List<NameIP> _NameIP = new List<NameIP>();
        private string DeleteName;
        private string DeleteIP;
        private int DeleteIndex;
        private int ReplaceIndex;
        //For load datagridview
        //新增
        public void Entered()
        {
            //輸入的值傳入variableUse.cs的prop
            Hostsuses();
            //確認輸入的名稱和IP是否正確
            if (!CheckNameREGEX() || !CheckIPREGEX())
            {
                return;
            }
            //_hoshtsb為list class確認是否包含textboxName中的內容
            foreach (var item in _hostsb)
            {
                if (item.OriginDataB.Contains(textBoxName.Text))
                {
                    MessageBox.Show("欄位已有相同名稱", "Message");
                    return;
                }
            }
            //進行加入gridview的內容
            Addatagridview();
            //確認是否有新增
            foreach (var itema in _hostsb)
            {
                Debug.WriteLine(itema.DataIndex + " " + itema.OriginDataB + "#" + itema.DataBComment);
            }
        }

        //刪除
        public void Deleted()
        {
            //get在cell select中選擇到的欄位Name和IP以之後做使用
            foreach (var itemNI in _NameIP)
            {
                DeleteName = itemNI.Name;
                DeleteIP = itemNI.IP;
            }
            //確認欄位是否是空的，若為空的則不可刪除。
            if ((String.IsNullOrEmpty(DeleteName) && String.IsNullOrEmpty(DeleteIP)) || String.IsNullOrEmpty(textBoxName.Text) || String.IsNullOrEmpty(textBoxIP.Text))
            {
                MessageBox.Show("空的請勿刪除，請選擇一個欄位", "Message");
                return;
            }
            //_hostsb的OriginDataB 包含DeleteName和DeleteIP 則可以刪除該行欄位的值
            foreach (var item in _hostsb)
            {
                if (item.OriginDataB.Contains(DeleteName) && item.OriginDataB.Contains(DeleteIP))
                {
                    //確認刪除為第幾行
                    Debug.WriteLine(item.DataIndex + " " + item.OriginDataB);
                    DeleteIndex = item.DataIndex;
                }
            }
            //刪除該index位置的值
            _hostsb.RemoveAt(DeleteIndex);
            //刪除gridview中的值
            variableUse.indexRowDelete = dataGridViewHosts.CurrentCell.RowIndex;
            dataGridViewHosts.Rows.RemoveAt(variableUse.indexRowDelete);
            //清空所有textbox值
            textBoxName.Text = "";
            textBoxIP.Text = "";
            textBoxComment.Text = "";
            //重新寫入在hosts中的function
            writeInTxt();
            //確認是否有刪除
            foreach (var itema in _hostsb)
            {
                Debug.WriteLine(itema.DataIndex + " " + itema.OriginDataB + "#" + itema.DataBComment);
            }
        }
        //更改
        public void Replaced()
        {
            //輸入的值傳入variableUse.cs的prop
            Hostsuses();
            //確認輸入的名稱和IP是否正確
            if (!CheckNameREGEX() || !CheckIPREGEX())
            {
                return;
            }
            //作出在replace中選擇的欄位index位置
            DataGridViewRow newDataRow = dataGridViewHosts.Rows[variableUse.indexRowReplace];
            //get在cell select中選擇到的欄位Name和IP以之後做使用
            foreach (var itemNI in _NameIP)
            {
                DeleteName = itemNI.Name;
                DeleteIP = itemNI.IP;
            }
            //確認欄位是否是空的，若為空的則不可更改。
            if ((String.IsNullOrEmpty(DeleteName) && String.IsNullOrEmpty(DeleteIP)) || String.IsNullOrEmpty(textBoxName.Text) || String.IsNullOrEmpty(textBoxIP.Text))
            {
                MessageBox.Show("空的請勿變更，請選擇一個欄位", "Message");
                return;
            }
            //做兩種DeleteIndex 和 ReplaceIndex 作為更改的判斷依據
            //先做刪除再做插入的方式作為修改模式
            foreach (var item in _hostsb)
            {
                if (item.OriginDataB.Contains(DeleteName) && item.OriginDataB.Contains(DeleteIP))
                {
                    //確認刪除為第幾行
                    Debug.WriteLine(item.DataIndex + " " + item.OriginDataB);
                    DeleteIndex = item.DataIndex;
                    ReplaceIndex = item.DataIndex;
                }
            }
            //選擇的欄位顯示在textbox上
            newDataRow.Cells[0].Value = textBoxIP.Text;
            newDataRow.Cells[1].Value = textBoxName.Text;
            newDataRow.Cells[2].Value = textBoxComment.Text;
            _hostsb.RemoveAt(DeleteIndex);
            _hostsb.Insert(ReplaceIndex, new hostsDataB() { OriginDataB = textBoxIP.Text + " " + textBoxName.Text + " ", DataBComment = textBoxComment.Text });
            textBoxName.Text = "";
            textBoxIP.Text = "";
            textBoxComment.Text = "";
            writeInTxt();
            //確認是否更改
            foreach (var itema in _hostsb)
            {
                Debug.WriteLine(itema.DataIndex + " " + itema.OriginDataB + "#" + itema.DataBComment);
            }
        }

        public void SelectCellRows()
        {
            //if(e.RowIndex > -1)
            //{
            //    var val = this.dataGridViewHosts[,e.RowIndex].Value.ToString();
            //    Debug.WriteLine(val);
            //}
            if (variableUse.indexRowReplace == -1)
            {
                return;
            }
            DataGridViewRow row = dataGridViewHosts.Rows[variableUse.indexRowReplace];

            textBoxIP.Text = row.Cells[0].Value.ToString();
            textBoxName.Text = row.Cells[1].Value.ToString();
            textBoxComment.Text = row.Cells[2].Value.ToString();
            _NameIP.Add(new NameIP() { Name = row.Cells[0].Value.ToString(), IP = row.Cells[1].Value.ToString() });
        }
    }
}
