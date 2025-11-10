using System.Collections.Generic;

namespace WindowsFormsApp1.TestTypeCustom
{

    public class TestUserOptionsInitParam
    {
        public bool OperatorCall { get; set; }
        public int OperationStatus { get; set; }
        public List<int> FriendAccountNoList { get; set; }
        public List<int> BlockAccountNoList { get; set; }
        public TestUserOptionsAdd OptionsAdd { get; set; }

        public TestUserOptionsInitParam()
        {
            this.OperatorCall           = false;
            this.OperationStatus        = 0;
            this.FriendAccountNoList    = new List<int>();
            this.BlockAccountNoList     = new List<int>();
            this.OptionsAdd             = new TestUserOptionsAdd(new TestUserOptionsAddInitParam());
        }
    }


    public class TestUserOptions
    {
        public bool OperatorCall { get; private set; }
        public int OperationStatus { get; private set; }
        public List<int> FriendAccountNoList { get; private set; }
        public List<int> BlockAccountNoList { get; private set; }
        public TestUserOptionsAdd OptionsAdd { get; private set; }

        public TestUserOptions(TestUserOptionsInitParam initParam)
        {
            this.OperatorCall           = initParam.OperatorCall;
            this.OperationStatus        = initParam.OperationStatus;
            this.FriendAccountNoList    = initParam.FriendAccountNoList;
            this.BlockAccountNoList     = initParam.BlockAccountNoList;
            this.OptionsAdd             = initParam.OptionsAdd;
        }
    }
}
