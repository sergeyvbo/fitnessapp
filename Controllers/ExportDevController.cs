using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using Fitnessapp.Data;

namespace Fitnessapp.Controllers
{
    public partial class ExportdevController : ExportController
    {
        private readonly devContext context;
        private readonly devService service;

        public ExportdevController(devContext context, devService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/dev/workouts/csv")]
        [HttpGet("/export/dev/workouts/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportWorkoutsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetWorkouts(), Request.Query), fileName);
        }

        [HttpGet("/export/dev/workouts/excel")]
        [HttpGet("/export/dev/workouts/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportWorkoutsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetWorkouts(), Request.Query), fileName);
        }

        [HttpGet("/export/dev/exercises/csv")]
        [HttpGet("/export/dev/exercises/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportExercisesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetExercises(), Request.Query), fileName);
        }

        [HttpGet("/export/dev/exercises/excel")]
        [HttpGet("/export/dev/exercises/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportExercisesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetExercises(), Request.Query), fileName);
        }

        [HttpGet("/export/dev/days/csv")]
        [HttpGet("/export/dev/days/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportDaysToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetDays(), Request.Query), fileName);
        }

        [HttpGet("/export/dev/days/excel")]
        [HttpGet("/export/dev/days/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportDaysToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetDays(), Request.Query), fileName);
        }
    }
}
