using System.Xml.Serialization;

namespace WindowsFormsApp1.TestTypeCustom
{
    public class TestUserOptionsAddInitParam
    {
        public int UserAccountNo { get; set; }
        public string NorticeMessage { get; set; }
        public Config.ProgressStatus SendDeviceId { get; set; }

        public TestUserOptionsAddInitParam()
        {
            this.UserAccountNo  = 1234;
            this.NorticeMessage = "initialized.";
            this.SendDeviceId   = Config.ProgressStatus.NotStarted;
        }
    }

    public class TestUserOptionsAdd
    {
        [XmlElement("UserAccountNo")]
        public int UserAccountNo { get; private set; }
        [XmlElement("NorticeMessage")]
        private string NorticeMessage { get; set; }
        [XmlElement("SendDeviceId")]
        public Config.ProgressStatus SendDeviceId { get; private set; }

        public TestUserOptionsAdd(TestUserOptionsAddInitParam initParam)
        { 
            this.UserAccountNo  = initParam.UserAccountNo;
            this.NorticeMessage = initParam.NorticeMessage;
            this.SendDeviceId   = initParam.SendDeviceId;
        }
    }
}
