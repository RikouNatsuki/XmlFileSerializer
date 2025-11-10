using System;

namespace WindowsFormsApp1.TestTypeCustom
{

    public class TestUserHistoryInitParam
    {
        public Config.EquipmentType EquipStatus { get; set; }
        public Config.EquipmentType EquipStatusBefore { get; set; }

        public DateTime LoginTime { get; set; }
        public DateTime LogoutTime { get; set; }
        public bool IsLogin { get; set; }

        public TestUserHistoryInitParam()
        {
            this.EquipStatus        = Config.EquipmentType.Operator;
            this.EquipStatusBefore  = Config.EquipmentType.PowerUser;
            this.LoginTime          = DateTime.MinValue;
            this.LogoutTime         = DateTime.MinValue;
            this.IsLogin            = false;
        }
    }

    public class TestUserHistory
    {
        public Config.EquipmentType EquipStatus { get; private set; }
        public Config.EquipmentType EquipStatusBefore { get; private set; }

        public DateTime LoginTime { get; private set; }
        public DateTime LogoutTime { get; private set; }
        public bool IsLogin { get; private set; }

        public TestUserHistory(TestUserHistoryInitParam initParam)
        {
            this.EquipStatus        = initParam.EquipStatus;
            this.EquipStatusBefore  = initParam.EquipStatusBefore;
            this.LoginTime          = initParam.LoginTime;
            this.LogoutTime         = initParam.LogoutTime;
            this.IsLogin            = initParam.IsLogin;
        }

    }
}
