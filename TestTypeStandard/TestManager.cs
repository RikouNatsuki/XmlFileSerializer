using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace WindowsFormsApp1.TestTypeStandard
{
    [XmlRoot("UserManagerABC")]
    public class TestManager
    {
        public static string USE_TYPE = "TestTypeStandard.TestManager";

        public List<TestUser> UserList { get; set; }

        [XmlIgnore]
        public DateTime UpdatedDate { get; set; }

        public void SetUpdatedAt(DateTime date) { UpdatedDate = date; }

        public TestManager()
        {
            UserList = new List<TestUser>();
            UpdatedDate = DateTime.Now;
        }

        public void Add(TestUser alpha)
        {
            UserList.Add(alpha);
            UpdatedDate = DateTime.Now;
        }
    }
}
