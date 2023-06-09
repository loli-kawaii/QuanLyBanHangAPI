﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHangAPI.Data
{
    [Table("NhaCungCap")]
    public class NhaCungCap
    {
        [Key]
        public int MaNhaCungCap { get; set; }
        [Required]
        [MaxLength(255)]
        public string TenNhaCungCap { get; set; }
        public string TrangChu { get; set; }
    }
}
