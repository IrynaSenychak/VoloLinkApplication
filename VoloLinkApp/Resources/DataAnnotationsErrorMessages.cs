using System.ComponentModel.DataAnnotations;

namespace VoloLinkApp.Resources
{
    public class DataAnnotationsErrorMessages
    {
        public const string Required = "Поле {0} є обов'язковим.";
        public const string StringLength = "Поле {0} повинно бути довжиною від {2} до {1} символів.";
        public const string EmailAddress = "Поле {0} не є правильною email адресою.";
        public const string Compare = "Поля {0} і {1} не співпадають.";
        public const string Password = "Пароль повинен містити мінімум 6 символів, хоча б одну велику літеру, цифру та спеціальний символ.";
    }
}