using Market.DomainLayer.Users;

namespace Market.DomainLayer.Market.Validation;

public class ValidationCheck
{
    public static void Validate(bool isError, string errorMessage)
    {
        if (isError)
        {
            throw new Exception(errorMessage);
        }
    }
}