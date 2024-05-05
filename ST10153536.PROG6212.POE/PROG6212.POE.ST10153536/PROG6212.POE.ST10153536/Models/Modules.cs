using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROG6212.POE.ST10153536.Models
{
    public class Modules
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int ModuleId { get; set; }

        [Required]
        [StringLength(10)]
        public string? Code { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        [Range(1, 30)]
        public int Credits { get; set; }

        [Required]
        [Range(1, 20)]
        public int ClassHoursPerWeek { get; set; }

        [Required]
        public int NumberOfWeeks { get; set; }

        [Required]
        public DateTime StartDate { get; set; }
        public double? HoursSpent { get; set; }
        [Display(Name = "Hours Left")]

        public int UserId { get; set; }
        [ForeignKey("UserId")]

        [Display(Name = "Self-Study Hours per Week")]
        public double? SelfStudyHours { get; set; }

        [Display(Name = "Remaining Self-Study Hours for Current Week:")]
        public double? CurrentWeekSelfStudyRemain { get; set; }

    }

    public class RecordedHours
    {
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int Hours { get; set; }

        public int ModuleId { get; set; }

        public Modules Module { get; set; }
    }
}
