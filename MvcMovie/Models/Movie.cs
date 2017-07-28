using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcMovie.Models
{
    public static class Common
    {
        public const string movieMetadataRegexPattern = @"^[A-Z]+[a-zA-Z''-'\s]*$";
    }

    public class Movie
    {
        public int Id { get; set; }

        [StringLength(maximumLength: 20, MinimumLength = 3)]
        public String Title { get; set; }

        [Display(Name = "Release Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReleaseDate { get; set; }

        [Required]
        [StringLength(maximumLength: 30)]
        [RegularExpression(Common.movieMetadataRegexPattern)]
        public string Genre { get; set; }

        [Range(1, 100)]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [StringLength(maximumLength: 5)]
        [RegularExpression(Common.movieMetadataRegexPattern)]
        public string Rating { get; set; }
    }
}