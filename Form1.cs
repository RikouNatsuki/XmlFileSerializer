using Common.Reflection;
using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

#if false
// 標準XmlSerializer 検証用 データクラス群
using WindowsFormsApp1.TestTypeStandard;

#else
// カスタムXmlSerializer 検証用 データクラス群
// ※標準XmlSerializer の「public限定」「ルート要素名自動付与」などの制約を回避。
using WindowsFormsApp1.TestTypeCustom;

#endif

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.LblXmlTarget.Text = ".xml 化対象: " + TestManager.USE_TYPE;
        }

        private TestManager TestMgrNow { get; set; }

        private TestManager TestMgrNew { get; set; }

        /// <summary>
        /// 標準XMLシリアライザー／利用サンプル
        /// </summary>
        private void XmlSerializer_Standard_TEST()
        {
            try
            {
                XmlSerializerInit_DataSet();

                var filePath = "XmlSerializer_Standard_" + DateTime.Now.ToString("yyyyMMdd") + ".xml";

                // XML 書き込み
                XmlSerializer serializer = new XmlSerializer(typeof(TestManager));
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    serializer.Serialize(writer, TestMgrNow);
                }

                // XML 読み込み
                using (StreamReader reader = new StreamReader(filePath))
                {
                    TestManager _TestMgrNew = (TestManager)serializer.Deserialize(reader);
                    TestMgrNew = _TestMgrNew;
                }

                XmlSerializerDone_DateChk();
            }
            catch (Exception ex)
            {
                PutLog(ex.ToString());
            }
        }

        /// <summary>
        /// カスタムXMLシリアライザー／利用サンプル
        /// </summary>
        private void XmlSerializer_Custom_TEST()
        {
            try
            {
                XmlSerializerInit_DataSet();

                var filePath = "XmlSerializer_Custom_" + DateTime.Now.ToString("yyyyMMdd") + ".xml";

                // XML 書き込み
                FastMethodAccess.DebugFallbackEnabled = false;
                var errMsg = string.Empty;
                if (!Common.XML.XmlFileSerializer<TestManager>.TrySaveXML(TestMgrNow, filePath, out errMsg)) PutLog("SaveXML: " + errMsg);

                // XML 読み込み
                var chk2 = errMsg;
                TestManager _TestMgrNew;
                if (!Common.XML.XmlFileSerializer<TestManager>.TryLoadXML(out _TestMgrNew, filePath, out errMsg)) PutLog("LoadXML: " + errMsg);
                TestMgrNew = _TestMgrNew;

                XmlSerializerDone_DateChk();
            }
            catch (Exception ex)
            {
                PutLog(ex.ToString());
            }
        }

        private void XmlSerializerInit_DataSet()
        {
            TestMgrNow = new TestManager();

            var userRequestInit = new TestUserRequestInitParam();
            userRequestInit.AuthalizationTryCount = 1;

            // 試験情報セット1
            {
                var userOptionsAdd = new TestUserOptionsAdd(new TestUserOptionsAddInitParam()
                {
                    UserAccountNo = 12345,
                    NorticeMessage = "Hello World One",
                    SendDeviceId = Config.ProgressStatus.InProgress,
                });

                TestUser testUser = new TestUser(1, Config.DataType.TypeA)
                {
                    UserRequest = new TestUserRequest(userRequestInit),
                    UserHistory = new TestUserHistory(new TestUserHistoryInitParam()),
                    UserOptions = new TestUserOptions(new TestUserOptionsInitParam() { OptionsAdd = userOptionsAdd }),
                };

                TestMgrNow.Add(testUser);
            }

            // 試験情報セット2
            {
                var userOptionsAdd = new TestUserOptionsAdd(new TestUserOptionsAddInitParam()
                {
                    UserAccountNo = 22345,
                    NorticeMessage = "Hello World Two",
                    SendDeviceId = Config.ProgressStatus.InProgress,
                });

                TestUser testUser = new TestUser(2, Config.DataType.TypeB)
                {
                    UserRequest = new TestUserRequest(userRequestInit),
                    UserHistory = new TestUserHistory(new TestUserHistoryInitParam()),
                    UserOptions = new TestUserOptions(new TestUserOptionsInitParam() { OptionsAdd = userOptionsAdd }),
                };

                TestMgrNow.Add(testUser);
            }
        }

        private void XmlSerializerDone_DateChk()
        {
            PutLog("更新日時: " + TestMgrNew.UpdatedDate.ToString("yyyy-MM-dd HH:mm:ss"));

            foreach (TestUser a in TestMgrNew.UserList)
            {
                PutLog(string.Format("Constract ID: {0}, Type: {1}", a.ConstractId, a.ConstractType));
            }

            int[] test = new int[] { 1, 2, 3, 4, 5, };
            var chk = TestMgrNow.UserList.ToArray();
        }

        private void PutLog(string message)
        {
            Console.WriteLine(message);
        }

        private void BtnStandard_Click(object sender, EventArgs e)
        {
            XmlSerializer_Standard_TEST();
        }

        private void BtnCustom_Click(object sender, EventArgs e)
        {
            XmlSerializer_Custom_TEST();
        }
    }
}
