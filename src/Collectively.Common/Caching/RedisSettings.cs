namespace Collectively.Common.Caching
{
    public class RedisSettings
    {
        public string ConnectionString { get; set; }
        public int Database { get; set; }
        public bool Enabled { get; set; }        
    }
}