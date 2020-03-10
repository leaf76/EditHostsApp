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
    partial class Form1 : Form
    {
        DataTable table = new DataTable();
        List<hostsDataB> _hostsb = new List<hostsDataB>();
        List<NameIP> _NameIP = new List<NameIP>();
        private string DeleteName;
        private string DeleteIP;
        private int DeleteIndex;
        private int ReplaceIndex;
        public string[] values = null;
        //讀取hosts沒有被註解的內容

        public Form1()
        {
            InitializeComponent();
            hostsView();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            loadDatagridview();
        }
 
        public void Hostsuses()
        {
            variableUse.IP = textBoxIP.Text;
            variableUse.Name = textBoxName.Text;
            variableUse.Comment = textBoxComment.Text;
        }
        //Enter(確認)
        private void button1_Click(object sender, EventArgs e)
        {
            Entered();
        }
        //Delete(刪除)
        private void button2_Click(object sender, EventArgs e)
        {
            Deleted();
        }
        //Replace(變更)
        private void button3_Click(object sender, EventArgs e)
        {
            Replaced();
        }

        private void dataGridViewHosts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            variableUse.indexRowReplace = e.RowIndex;
            SelectCellRows();
        }


        public void hostsView()
        {
            //每次重新刷新
            _hostsb.Clear();
            //讀取內容行數
            var hostslines = File.ReadAllLines(path.hostspath);
            //先略過hosts 檔案內被comment的增加之後hosts內的檔案內容
            //類似從hosts到target同步讀取
            //做出類似comment功能 略過value[1]之後的值
            for (var i = 0; i < hostslines.Length; i++)
            {
                values = hostslines[i].Split('#');
                var splitStore = "";
                //target和datagridview中的資料顯示
                //本文
                splitStore = values[0];
                //註解
                var theComment = values.Skip(1);
                //join組合回去
                var result = string.Join("#", theComment);
                var contentConcat = string.Format("" + splitStore + "#" + result);
                //將原hosts檔案行數為i
                //DataBComment為join後的comment
                //OriginDataB 為分割後分為註解和非註解的values[0]
                _hostsb.Add(new hostsDataB() { DataIndex = i, DataBComment = result, OriginDataB = splitStore });
            }

            var forRTB = from file in File.ReadLines(path.hostspath, Encoding.UTF8)
                         select new
                         {
                             lines = file
                         };
            richTextBox1.Text = "";
            foreach (var item in forRTB)
            {
                richTextBox1.AppendText(item.lines + "\n");
            }

        }

        public void forloadData()
        {
            string[] values;
            //regex 的模式因為會有各種狀況故作此模式應變
            string pattern = @"\s+|\t|\t{2,}|#";
            foreach (var item in _hostsb)
            {
                if (!String.IsNullOrWhiteSpace(item.OriginDataB))
                {
                    values = Regex.Split(item.OriginDataB, pattern);
                    //因應狀況column數
                    var row = new string[3];
                    for (var j = 0; j < 3; j++)
                    {
                        //j大於2 DataBComment中的值
                        //原先在這裡開啟檔案時會產生錯誤直接將j>2時的comment顯示出來即可
                        if (j >= 2)
                        {
                            row[2] = item.DataBComment;
                        }
                        else
                        {
                            row[j] = values[j].Trim();
                        }
                    }
                    table.Rows.Add(row);
                }
            }
        }


        //重新寫入hosts檔案內容
        public void writeInTxt()
        {
            //新增兩list變數做為儲存
            var x1 = new List<string>();
            var x2 = new List<string>();
            //為儲存合併origin 和 comment 的變數
            var storeString = new List<string>();
            foreach (var item in _hostsb)
            {
                x1.Add(item.OriginDataB);
                x2.Add(item.DataBComment);
            }
            //結合兩個list使用LINQ的 ZIP
            var TwostringCat = x1.Zip(x2, (first, second) => new { Origin = first, Comment = second });

            foreach (var item in TwostringCat)
            {
                storeString.Add(item.Origin + "#" + item.Comment);
            }
            File.WriteAllLines(path.hostspath, storeString.Take(storeString.Count));
            hostsView();
            //foreach (var item in _hostsb)
            //{
            //   if( item.DataIndex >= 21)
            //    {
            //        Debug.WriteLine(item.OriginDataB);
            //    }
            //}
        }

        public bool CheckNameREGEX()
        {
            string pattern = @"^([\S\s]+)\.([\S\s]+)\.(com)$";
            Regex rx = new Regex(pattern);
            if (rx.IsMatch(variableUse.Name))
            {
                return true;
            }
            MessageBox.Show("不正常名稱，請更改", "Message");
            return false;
        }
        public bool CheckIPREGEX()
        {
            string pattern = @"^([1-2][0-9][0-9])\.([1-2][0-9][0-9]|[0-9][0-9])\.([1-2][0-9][0-9]|[0-9][0-9]|[0-9])\.([1-2][0-9][0-9]|[0-9][0-9]|[0-9])$|^([1-2][0-9][0-9])\.([1-2][0-9][0-9]|[0-9][0-9])\.([1-2][0-9][0-9]|[0-9][0-9]|[0-9])\.([1-2][0-9][0-9]|[0-9][0-9]|[0-9])(\:|\::)([0-9][0-9]|[0-9][0-9][0-9][0-9])";
            Regex rx = new Regex(pattern);
            if (rx.IsMatch(variableUse.IP))
            {
                return true;
            }
            MessageBox.Show("不正常IP，請修正", "Message");
            return false;
        }



        public void loadDatagridview()
        {
            //增加Clumn欄位
            table.Columns.Add("IP", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Comment", typeof(string));
            dataGridViewHosts.DataSource = table;
            forloadData();
        }

        //確認增加
        public void Addatagridview()
        {
            //新增一個欄位i+1
            var i = _hostsb.Count;
            //新增一個row
            table.Rows.Add(textBoxIP.Text, textBoxName.Text, textBoxComment.Text);
            dataGridViewHosts.DataSource = table;
            _hostsb.Add(new hostsDataB() { OriginDataB = textBoxIP.Text + " " + textBoxName.Text + " ", DataBComment = textBoxComment.Text, DataIndex = i + 1 });
            writeInTxt();
            MessageBox.Show("Success Enter!", "Message");
        }

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

        private void richTextBox1_TextChanged(object sender, EventArgs e) {

        }
    }
}
