using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using System.Runtime.Serialization;

namespace ProductManagement.Models
{
    [Table("Brand")]
    public class Brand
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int BrandId { get; set; }
        [Required]
        public string BrandName { get; set; }
        [Required]
        [StringLength(4)]
        public string SalesOrg { get; set; }
        [Required]
        [StringLength(2)]
        public string DistChan { get; set; }
    }

    [Table("Pattern")]
    public class Pattern
    {
        public Pattern()
        {
            Styles = new List<Style>();
        }
        
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int PatternId { get; set; }
        public int BrandId { get; set; }
        [Required]
        public string PatternName { get; set; }
        [Required]
        public DateTime LastUpdated { get; set; }

        public ICollection<Style> Styles { get; set; }
    }
    
    [Table("Style")]
    [DataContract]
    [Serializable]
    public class Style
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [DataMember]
        public int StyleId { get; set; }
        public int PatternId { get; set; }
        [Required]
        [DataMember(IsRequired = true)]
        public string StockNumber { get; set; }
        [MaxLength(4000)]
        [DataMember]
        public string MarketingDescription { get; set; }
        [MaxLength(4000)]
        [DataMember]
        public string TechBullets { get; set; }
        [Required]
        [DataMember(IsRequired=true)]
        public DateTime LastUpdated { get; set; }
        [Required]
        [DataMember(IsRequired = true)]
        public string LastUpdatedBy { get; set; }

        public Pattern Pattern { get; set; }
        // Had to include these for the StyleAPI JSON calls
        [NotMapped]
        [DataMember]
        public string PatternName { get; set; }
        [NotMapped]
        [DataMember]
        public int BrandId { get; set; }
    }

    public class PIMModel
    {
        [Required]
        [DisplayName("Brand")]
        public int BrandId { get; set; }
        [DisplayName("Brand")]
        public SelectList Brands { get; set; }
    }

    public class ViewProductsModel
    {
        public List<Style> Styles { get; set; }
    }
}