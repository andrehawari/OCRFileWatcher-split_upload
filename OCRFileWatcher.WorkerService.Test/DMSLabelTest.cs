using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OCRFileWatcher.WorkerService.Test
{
    [TestClass]
    public class DMSLabelTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var path = @"file\test.txt";
            var file = ReadFile(path);

            RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace;



            //var transactionDMS = "Admission No";
            var mrNo = "MR No";

            var temp = file.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
            //var transactionNumber = Regex.Match(input, _patternConfig.Transaction).ToString();

            //var dmslabel = Regex.Match(input, _patternConfig.DMSKeyword);
            //var indexTrNumber = temp.ToList().FindIndex(x => Regex.IsMatch(x, transactionDMS));


            //var transactionNumber = temp[indexTrNumber].Split(':').ToArray()[1];
            var checkMRNo = Regex.Match(file, mrNo, options);
            var mrSuccess = checkMRNo.Success;
            Assert.IsFalse(mrSuccess);
            //Console.WriteLine(transactionNumber);


            //Console.WriteLine(file);
        }


        static string ReadFile(string path)
        {
            var openfiles = File.ReadAllText(path);

            return openfiles;
        }

    }
}
