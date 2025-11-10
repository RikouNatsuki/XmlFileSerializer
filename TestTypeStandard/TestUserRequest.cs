namespace WindowsFormsApp1.TestTypeStandard
{
    public class TestUserRequestInitParam
    {
        public Config.EquipmentType UserAccountType { get; set; }
        public int AuthalizationTryCount { get; set; }
        public int Password { get; set; }

        public TestUserRequestInitParam()
        {
            UserAccountType         = Config.EquipmentType.Development;
            AuthalizationTryCount   = 1;
            Password                = 0;
        }
    }

    public class TestUserRequest
    {
        public Config.EquipmentType UserAccountType { get; set; }
        public int AuthalizationTryCount { get; set; }
        public int Password { get; set; }

        public TestUserRequest(TestUserRequestInitParam initParam)
        {
            UserAccountType         = initParam.UserAccountType;
            AuthalizationTryCount   = initParam.AuthalizationTryCount;
            Password                = initParam.Password;
        }

        public TestUserRequest() { }
    }
}
