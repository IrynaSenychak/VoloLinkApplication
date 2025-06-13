using System.ComponentModel.DataAnnotations;

namespace VoloLinkApp.Resources
{
    public class DataAnnotationsErrorMessages
    {
        public const string Required = "���� {0} � ����'�������.";
        public const string StringLength = "���� {0} ������� ���� �������� �� {2} �� {1} �������.";
        public const string EmailAddress = "���� {0} �� � ���������� email �������.";
        public const string Compare = "���� {0} � {1} �� ����������.";
        public const string Password = "������ ������� ������ ����� 6 �������, ���� � ���� ������ �����, ����� �� ����������� ������.";
    }
}