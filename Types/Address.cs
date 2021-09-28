namespace Types
{
    public record Address(
       string Street,
       string City,
       string Country,
       string ZipCode,
       string HouseNumber)
    {
        public override string ToString()
        {
            return $"{Street} {HouseNumber}, {ZipCode} {City}, {Country}";
        }
    }

    public record CounterParty(
        string Name, string? MemberId = null);

    public record BankAccount(
        string Name,
        string Number);

}