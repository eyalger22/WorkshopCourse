namespace Market.Pages.Modles
{
    public class UserModel
    {
        public int id {  get; set; }
        public int mode { get; set; } // 1 for Guest. 2 for member. 3 for system maneger.

        public int shop { get; set; }
        public int product { get; set; } 
    }
}
