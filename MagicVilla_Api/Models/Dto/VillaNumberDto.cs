﻿using System.ComponentModel.DataAnnotations;

namespace MagicVilla_Api.Models.Dto
{
    public class VillaNumberDto
    {


        [Required]
        public int VillaNo { get; set; }

        public string SpecialDetails { get; set; }

        [Required]
        public int VillaId { get; set; }
    }
}
