namespace TutoringAcademy.Settings
{
    // This class represents the settings required for connecting to a MongoDB database. It includes properties such as ConnectionString and DatabaseName, which are necessary for establishing a connection to the MongoDB server and accessing the specified database for storing and retrieving data in the tutoring academy application.
    public class MongoDBSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
    }
}