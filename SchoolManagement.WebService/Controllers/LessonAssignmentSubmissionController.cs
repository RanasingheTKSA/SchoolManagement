﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Business.Interfaces.LessonData;
using SchoolManagement.ViewModel.Lesson;
using SchoolManagement.WebService.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagement.WebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonAssignmentSubmissionController : ControllerBase
    {
        private readonly ILessonAssignmentSubmissionService lessonassignmentsubmissionService;
        private readonly IIdentityService identityService;

        public LessonAssignmentSubmissionController(ILessonAssignmentSubmissionService essaystudentanswerService, IIdentityService identityService)
        {
            this.lessonassignmentsubmissionService = lessonassignmentsubmissionService;
            this.identityService = identityService;
        }

        [HttpPost]

        public async Task<ActionResult> Post([FromBody] LessonAssignmentSubmissionViewModel vm)
        {
            var userName = identityService.GetUserName();
            var response = await lessonassignmentsubmissionService.SaveLessonAssignmentSubmission(vm, userName);
            return Ok(response);
        }

        [HttpGet]

        public ActionResult GetAllLessonAssignmentSubmissions()
        {
            var response = lessonassignmentsubmissionService.GetAllLessonAssignmentSubmissions();
            return Ok(response);
        }

    }
}