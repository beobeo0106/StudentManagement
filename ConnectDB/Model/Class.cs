using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConnectDB.Models
{
    public class Class
    {
        [Key]
        public int ClassId { get; set; }

        [Required]
        public string ClassName { get; set; }

        // Liên kết ngược lại: Một lớp có danh sách nhiều sinh viên
        public ICollection<Student> Students { get; set; }
    }
}