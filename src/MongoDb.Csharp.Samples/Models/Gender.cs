using Bogus.DataSets;

namespace MongoDb.Csharp.Samples.Models
{
    public enum Gender
    {
        Male,
        Female,
    }

    public static class GenderExtensions
    {
        public static Name.Gender MapToLibGender(this Gender gender)
        {
            return gender == Gender.Male ? Name.Gender.Male : Name.Gender.Female;
        }
    }
}
