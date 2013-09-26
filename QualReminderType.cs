using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceEmailReminders
{
    class QualReminderType
    {
        public string LName
        {
            get;
            set;
        }
        public string FName
        {
            get;
            set;
        }
        public DateTime? ExpireDt
        {
            get;
            set;
        }
        public string qualCompany
        {
            get;
            set;
        }
        public string qualID
        {
            get;
            set;
        }
        public string qualDescr
        {
            get;
            set;
        }
        public string numDays
        {
            get;
            set;
        }

    }
}
