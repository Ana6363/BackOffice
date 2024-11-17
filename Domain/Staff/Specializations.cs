using System;

namespace BackOffice.Domain.Staff
{
    public class Specializations : IEquatable<Specializations>
    {
        public SpecializationType Value { get; private set; }
        public enum SpecializationType
        {
            Cardiology,
            Orthopedics,
            Dermatology,
            Neurology,
            Pediatrics,
            Radiology,
            GeneralSurgery,
            Anaesthetist,
            Oncology,
            Ophthalmology,
            Psychiatry,
            Urology,
            Endocrinology,
            Gastroenterology,
            Rheumatology,
            Pulmonology,
            Nephrology,
            ObstetricsGynecology,
            EmergencyMedicine,
            FamilyMedicine,
            InstrumentingNurse,
            CirculatingNurse,
            NurseAnaesthetist,
            MedicalActionAssistant,


        }

        private Specializations(SpecializationType value)
        {
            Value = value;
        }

        public static Specializations FromString(string specialization)
        {
            if (Enum.TryParse(specialization, out SpecializationType parsedValue))
            {
                return new Specializations(parsedValue);
            }
            throw new ArgumentException($"Invalid specialization: {specialization}");
        }

        public static Specializations FromEnum(SpecializationType specialization)
        {
            return new Specializations(specialization);
        }

        public bool Equals(Specializations other)
        {
            if (other == null) return false;
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Specializations);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
