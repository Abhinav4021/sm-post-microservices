namespace Post.Cmd.Infrastructure.Config
{
    public class MongoDbConfig
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string Collection{ get; set; }
    }
}