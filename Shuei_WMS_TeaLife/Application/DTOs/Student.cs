using Magicodes.ExporterAndImporter.Core;
using Magicodes.ExporterAndImporter.Pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    [PdfExporter(Name = "学生信息")]
    public class Student
    {
        /// <summary>
        ///     姓名
        /// </summary>
        [ExporterHeader(DisplayName = "姓名")]
        [Display(Name = "Display姓名")]
        public string Name { get; set; }
        /// <summary>
        ///     年龄
        /// </summary>
        [ExporterHeader(DisplayName = "年龄")]
        public int Age { get; set; }
    }
}
