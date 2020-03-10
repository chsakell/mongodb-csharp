using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using Bogus.DataSets;
using Bogus.Extensions;
using MongoDb.Csharp.Samples.Models;

namespace MongoDb.Csharp.Samples.Core
{
    public class RandomData
    {
        public static List<string> AvailableSports = new List<string>()
        {
            "Soccer","Basketball", "Tennis","Volleyball", "Beach Volleyball", "American Football", 
            "Baseball", "Ice Hockey", "Formula 1", "Moto GP","Motor Sport", "Handball", "Water Polo",
            "Table Tennis", "Darts","Snooker", "MMA", "Boxing","Cricket", "Cycling", "Golf"
        };

        public static List<string> AvailableProfessions = new List<string>()
        {
            "Dentist","Photographer","Pharmacist","Teacher","Flight Attendant","Founder / Entrepreneur",
            "Personal Trainer","Waitress / Bartender","Physical Therapist","Lawyer","Marketing Manager","Pilot",
            "Producer","Visual Designer","Model","Engineer", "Firefighter","Doctor","Financial Adviser", "Police Officer",
            "Social-Media Manager","Nurse","Real-Estate Agent"
        };
        public static List<User> GenerateUsers(int count, string locale = "en")
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
                .RuleFor(u => u.Salary, (f, u) => Math.Round(f.Finance.Amount(1000, 5000)))
                .RuleFor(u => u.FavoriteSports,
                    (f, u) => f.PickRandom(AvailableSports, f.Random.Number(1, 19)).ToList())
                .RuleFor(u => u.Profession, (f, u) => f.PickRandom(AvailableProfessions, 1).FirstOrDefault());

            return person.Generate(count);
        }

        public static List<Order> GenerateOrders(int count, string locale = "en")
        {
            var orderIds = 0;
            var order = new Faker<Order>(locale)
                //Ensure all properties have rules. By default, StrictMode is false
                //Set a global policy by using Faker.DefaultStrictMode
                .StrictMode(true)
                //OrderId is deterministic
                .RuleFor(o => o.OrderId, f => orderIds++)
                //Pick some fruit from a basket
                .RuleFor(o => o.Item, f => f.PickRandom(f.Lorem.Sentence()))
                //A random quantity from 1 to 10
                .RuleFor(o => o.Quantity, f => f.Random.Number(1, 10))
                //A nullable int? with 80% probability of being null.
                //The .OrNull extension is in the Bogus.Extensions namespace.
                .RuleFor(o => o.LotNumber, f => f.Random.Int(0, 100).OrNull(f, .8f));

            return order.Generate(count);

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
