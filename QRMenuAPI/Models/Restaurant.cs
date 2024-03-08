using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace QRMenuAPI.Models
{
	public class Restaurant
	{

        public int Id { get; set; }

        [StringLength(200, MinimumLength = 2)]
        [Column(TypeName = "nvarchar(200)")]
        public string Name { get; set; } = "";

        [Phone]
        [Column(TypeName = "varchar(30)")]
        public string Phone { get; set; } = "";

        [Column(TypeName = "char(5)")]
        [StringLength(5, MinimumLength = 5)]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; } = "";

        [StringLength(200, MinimumLength = 5)]
        [Column(TypeName = "nvarchar(200)")]
        public string AddressDetail { get; set; } = "";

        [DataType(DataType.DateTime)]
        [Column(TypeName = "smalldatetime")]
        public DateTime RegisterDate { get; set; }

        public int CompanyId { get; set; }
        public byte StateId { get; set; }

        [ForeignKey("StateId")]
        public State? State { get; set; }
        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }
    }
}

