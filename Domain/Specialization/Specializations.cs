using BackOffice.Domain.Shared;
using System;

namespace BackOffice.Domain.Specialization
{
    public class Specializations : EntityId
    {

        public Specializations(string value) : base(value)
        { 
        }



        public override string AsString()
        {
            return ObjValue.ToString();
        }

        protected override object CreateFromString(string text)
        {
            return new Specializations(text);
        }


        public override bool Equals(object obj)
        {
            if (obj is Specializations other)
            {
                return ObjValue.Equals(other.ObjValue);
            }
            return false;
        }






    }
}
