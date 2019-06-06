using System;

namespace ExactOnline.Models
{
    public class ExactOnlineUser
    {
            public __Metadata __metadata { get; set; }
            public int CurrentDivision { get; set; }
            public string DivisionCustomer { get; set; }
            public string DivisionCustomerCode { get; set; }
            public string DivisionCustomerName { get; set; }
            public object DivisionCustomerVatNumber { get; set; }
            public object DivisionCustomerSiretNumber { get; set; }
            public string FullName { get; set; }
            public string PictureUrl { get; set; }
            public string ThumbnailPicture { get; set; }
            public string ThumbnailPictureFormat { get; set; }
            public string UserID { get; set; }
            public string UserName { get; set; }
            public string LanguageCode { get; set; }
            public string Legislation { get; set; }
            public string Email { get; set; }
            public string Title { get; set; }
            public object Initials { get; set; }
            public string FirstName { get; set; }
            public object MiddleName { get; set; }
            public string LastName { get; set; }
            public string Gender { get; set; }
            public object Nationality { get; set; }
            public string Language { get; set; }
            public string Phone { get; set; }
            public object PhoneExtension { get; set; }
            public object Mobile { get; set; }
            public DateTime ServerTime { get; set; }
            public int ServerUtcOffset { get; set; }
            public string EmployeeID { get; set; }
        }

        public class __Metadata
        {
            public string uri { get; set; }
            public string type { get; set; }
        }
}
