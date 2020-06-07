using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeSoft.Models
{
    public class Soft
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Rate { get; set; }
        public string FileName { get; set; }
        public string YoutubeLink { get; set; }
        public string SoftIdentity { get; set; }
        public string PictureLink { get; set; }
        public int Cattegory { get; set; }
        public DateTime Date { get; set; }
    }
}
