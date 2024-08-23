using Microsoft.AspNetCore.Mvc;

namespace cinemate.Models.User
{
    public class SignupUserValidation
    {

        public String? UserNameErrorMessage { get; set; }

        public String? FirstNameErrorMessage { get; set; }

        public String? SurnameErrorMessage { get; set; }

        public String? EmailErrorMessage { get; set; }

        public String? PhoneNumberErrorMessage { get; set; }

        public String? PasswordErrorMessage { get; set; }
       
        public String? AvataErrorMessager { get; set; }
    }
}
