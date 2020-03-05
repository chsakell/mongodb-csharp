using System.Runtime.InteropServices.ComTypes;

namespace MongoDb.Csharp.Samples.Crud.Insert
{
    public class Sport
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int TotalEvents { get; set; }

        public Sport(int id, string title, int totalEvents)
        {
            Id = id;
            Title = title;
            TotalEvents = totalEvents;
        }
    }
}
