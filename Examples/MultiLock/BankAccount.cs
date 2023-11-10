namespace Example.MultiLock
{
    public class BankAccount
    {
        public BankAccount(string ownerName) => _ownerName = ownerName;

        private readonly string _ownerName;
        public long Rubles;

        public override string ToString() => $"{_ownerName} has {Rubles} RUB";
    }
}