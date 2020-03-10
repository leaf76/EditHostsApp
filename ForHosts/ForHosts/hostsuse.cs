using System.Diagnostics;
using System.IO;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Data;
using System.Collections.Generic;


namespace ForHosts
{
    public partial class Form1 {



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
                var contentConcat = string.Format("" +splitStore + "#" + result);
                //將原hosts檔案行數為i
                //DataBComment為join後的comment
                //OriginDataB 為分割後分為註解和非註解的values[0]
                _hostsb.Add(new hostsDataB() { DataIndex = i, DataBComment = result , OriginDataB = splitStore });
            }

            var forRTB = from file in File.ReadLines(path.hostspath, Encoding.UTF8)
                         select new
                         {
                             lines = file
                         };
            richTextBox1.Text = "";
            foreach (var item in forRTB)
            {
                richTextBox1.AppendText(item.lines+"\n");
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
                        if (j >= 2 )
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
            table.Rows.Add(textBoxIP.Text, textBoxName.Text,textBoxComment.Text);
            dataGridViewHosts.DataSource = table;
            _hostsb.Add(new hostsDataB() { OriginDataB = textBoxIP.Text + " " + textBoxName.Text+" ", DataBComment = textBoxComment.Text, DataIndex = i + 1 });
            writeInTxt();
            MessageBox.Show("Success Enter!", "Message");
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
            string pattern = @"^([1-2][0-9][0-9])\.([1-2][0-9][0-9]|[0-9][0-9])\.([1-2][0-9][0-9]|[0-9][0-9]|[0-9])\.([1-2][0-9][0-9]|[0-9][0-9]|[0-9])$";
            Regex rx = new Regex(pattern);
            if (rx.IsMatch(variableUse.IP))
            {
                return true;
            }
            MessageBox.Show("不正常IP，請修正", "Message");
            return false;
        }
    }
}
