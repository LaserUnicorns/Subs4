namespace Subs4.Common.Classes
{
    public class Address
    {
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public int House { get; set; }
        public int Building { get; set; }
        public int Flat { get; set; }
        public int? Room { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}, {2}, {3}/{4}/{5} {6}", ZipCode, City, Street, House, Building, Flat, Room.HasValue ? "к." + Room.Value : "").Trim();
        }
    }
}