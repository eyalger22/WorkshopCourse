using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Market.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public RegisterModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }
        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        [BindProperty]
        public string Email { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            // Do nothing on page load
        }
        
        public IActionResult OnPost()
        {
            if (Username is null) Username = string.Empty;
            if (Password is null) Password = string.Empty;
            if (ConfirmPassword is null) ConfirmPassword = string.Empty;
            if (Email is null) Email = string.Empty;
            string username = Username.Trim();
            string password = Password.Trim();
            string confirmPassword = ConfirmPassword.Trim();
            string email = Email.Trim();

            if (password != confirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                return Page();
            }
            /*
            SqlConnection conn = new SqlConnection("Data Source=localhost;Initial Catalog=MyDatabase;Integrated Security=True");
            conn.Open();
            SqlCommand cmd = new SqlCommand("INSERT INTO Users (Username, Password, Email) VALUES (@username, @password, @email)", conn);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.Parameters.AddWithValue("@email", email);
            int result = cmd.ExecuteNonQuery();
            conn.Close();

            if (result == 1)
            {
                HttpContext.Session.SetString("username", username);
                return RedirectToPage("Welcome");
            }
            else
            {
                ErrorMessage = "Registration failed.";
                return Page();
            }*/
            ErrorMessage = "Registration failed";
            return Page();
        }
    }
}
