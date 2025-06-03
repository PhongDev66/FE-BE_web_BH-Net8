namespace Shopping_Tutorial.Models
{
    public class ThongKe
    {
        public int Id { get; set; }

        public int Quantity {  get; set; } //sl ban

        public int Sold { get; set; } // sl don hang

        public decimal Revenue {  get; set; } // doanh thu

        public decimal Profit { get; set; } // loi nhuan


        public DateTime DateCreated {  get; set; }
    }
}
