using System;
using BackOffice.Domain.Shared;

namespace BackOffice.Domain.Users
{
    public class LogId : EntityId
    {

        public LogId(String value):base(value)
        {

        }

        override
        protected Object CreateFromString(String text){
            return text;
        }

        override
        public String AsString(){
            return (String) base.Value;
        }
    }
}