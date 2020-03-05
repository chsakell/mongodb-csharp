using System;
using Bogus;
using Bogus.DataSets;
using MongoDb.Csharp.Samples.Models;

namespace MongoDb.Csharp.Samples.Core
{
    public class RandomData
    {
        public static User GeneratePerson(string locale = "en")
        {
            var person = new Faker<User>(locale)
                .RuleFor(u => u.Gender, f => f.PickRandom<Gender>())
                .RuleFor(u => u.FirstName, (f, u) => 
                    f.Name.FirstName(u.Gender.MapToLibGender()))
                .RuleFor(u => u.LastName, (f, u) => f.Name.LastName(u.Gender.MapToLibGender()))
                .RuleFor(u => u.UserName, (f, u) => f.Internet.UserName(u.FirstName, u.LastName))
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .RuleFor(u => u.Avatar, f => f.Internet.Avatar())
                .RuleFor(u => u.DateOfBirth, (f, u) => f.Date.Past(50, new DateTime?(Date.SystemClock().AddYears(-20))))
                .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Website, f => f.Internet.DomainName())
                .RuleFor(u => u.Address, f => GenerateCardAddress(locale))
                .RuleFor(u => u.Company, (f, u) => new CompanyCard()
                {
                    Name = f.Company.CompanyName(new int?()),
                    CatchPhrase = f.Company.CatchPhrase(),
                    Bs = f.Company.Bs()
                })
                .RuleFor(u => u.Salary, (f,u) => f.Finance.Amount(1000, 5000));

            return person.Generate();
        }

        public static AddressCard GenerateCardAddress(string locale = "en")
        {
            var cardAddress = new Faker<AddressCard>(locale)
                .RuleFor(a => a.Street, (f, u) => f.Address.StreetAddress())
                .RuleFor(a => a.Suite, (f, u) => f.Address.SecondaryAddress())
                .RuleFor(a => a.City, (f, u) => f.Address.City())
                .RuleFor(a => a.State, (f, u) => f.Address.State())
                .RuleFor(a => a.ZipCode, (f, u) => f.Address.ZipCode())
                .RuleFor(a => a.Geo, (f, u) => new AddressCard.AppCardGeo()
                {
                    Lat = f.Address.Latitude(-90.0, 90.0),
                    Lng = f.Address.Longitude(-180.0, 180.0)
                });

            return cardAddress.Generate();
        }
    }
}
