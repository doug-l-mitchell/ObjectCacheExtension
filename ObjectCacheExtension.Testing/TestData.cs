using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectCacheExtension.Testing
{
    public class TestData
    {
        public int IntValue { get; set; }
        public string StringValue { get; set; }
    }


    public static class TestDataFactory
    {
        public static TestData GetData()
        {
            return new TestData { IntValue = 7, StringValue = "Seven" };
        }
    }
}
