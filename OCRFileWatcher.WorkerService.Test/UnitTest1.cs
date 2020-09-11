using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace OCRFileWatcher.WorkerService.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {

            var stringarr = new List<Test>() 
            {
                new Test(){ Admis = "612512412", Nomor = "Satu"},
                new Test(){ Admis = "7235623542", Nomor = "Dua"},
                new Test(){ Admis = null, Nomor = "Tiga"},
                new Test(){ Admis = "83g43623", Nomor = "Empat"},
                new Test(){ Admis = null, Nomor = "Lima"},
            };

            var arr = stringarr.Where(x => x.Admis != null ).Select(x => x.Admis).ToArray();
            var res = string.Join('_', arr);

            var expected = "612512412_7235623542_83g43623";
            Assert.AreEqual(expected, res);

        }
    }

    public class Test
    {
        public string Nomor { get; set; }
        public string Admis { get; set; }
    }
}
