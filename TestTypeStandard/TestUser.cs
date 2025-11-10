using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WindowsFormsApp1.TestTypeStandard
{
    public class TestUser
    {
        public int ConstractPattern;
        public int ConstractId { get; set; }

        public Config.DataType ConstractType { get; set; }

        public TestUserRequest UserRequest { get; set; }
        public TestUserHistory UserHistory { get; set; } 
        public TestUserOptions UserOptions { get; set; }

        public TestUser()
        {
            this.ConstractPattern   = 1;
            this.ConstractId        = -1;
            this.ConstractType      = Config.DataType.TypeA;

            UserRequest = new TestUserRequest(new TestUserRequestInitParam());
            UserHistory = new TestUserHistory(new TestUserHistoryInitParam());
            UserOptions = new TestUserOptions(new TestUserOptionsInitParam());
        }

        public TestUser(int id, Config.DataType type)
        {
            this.ConstractPattern   = 2;
            this.ConstractId        = id;
            this.ConstractType      = type;

            UserRequest = new TestUserRequest(new TestUserRequestInitParam());
            UserHistory = new TestUserHistory(new TestUserHistoryInitParam());
            UserOptions = new TestUserOptions(new TestUserOptionsInitParam());
        }

        public void SetTypeID(int id, Config.DataType type)
        {
            this.ConstractPattern   = 3;
            ConstractId             = id;
            ConstractType           = type;
        }

    }
}
