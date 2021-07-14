﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Model.Master
{
    public class ClassTeacher
    {
		public int ClassNameId { get; set; }
		public int AcademicLevelId { get; set; }
		public int AcademicYearId { get; set; }
		public int TeacherId { get; set; }
		public bool IsPrimary { get; set; }
		public bool IsActive { get; set; }
		public DateTime CreatedOn { get; set; }
		public int? CreatedById { get; set; }
		public DateTime UpdatedOn { get; set; }
		public int? UpdatedById { get; set; }

		//public virtual ClassName ClassNameId { get; set; }
		//public virtual AcademicLevel AcademicLevelId { get; set; }
		//public virtual AcademicYear AcademicYearId { get; set; }
		//public virtual Teacher TeacherId { get; set; }
		//public virtual User CreatedById { get; set; }
		//public virtual User UpdatedById { get; set; }
	}
}