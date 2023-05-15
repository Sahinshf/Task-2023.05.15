namespace Task.Models.Common
{
    public abstract class BaseEntity // Instance almayacağımız üçün asbstract verə bilərik
    {
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
