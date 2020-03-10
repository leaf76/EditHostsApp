using System;
using System.Collections.Generic;
using System.IO;
using System.Data;


namespace ForHosts
{
    public static class variableUse
    {
        public static string IP { get; set; }
        public static string Name { get; set; }
        public static string Comment { get; set; }
        public static string ClearMessage { get; set; }
        public static string[] DataRow { get; set; }
        public static int indexRowReplace { get; set; }
        public static int indexRowDelete { get; set; }

 
    }
    public static class path
    {
        public static string hostspath = @"C:\Windows\System32\drivers\etc\hosts";
        public static string targetpath = Environment.CurrentDirectory + @"\target.txt";
    }
    public class NameIP
    {
        public string Name { get; set; }
        public string IP { get; set; }
    }

    public class hostsDataB {
        public int DataIndex { get; set; }
        public bool DataFlag { get; set; }
        public string OriginDataB { get; set; }
        public string DataB { get; set; }
        public string DataBComment { get; set; }
    }
}
