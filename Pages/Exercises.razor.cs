using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace Fitnessapp.Pages
{
    public partial class Exercises
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        public devService devService { get; set; }

        protected IEnumerable<Fitnessapp.Models.dev.Exercise> exercises;

        protected RadzenDataGrid<Fitnessapp.Models.dev.Exercise> grid0;
        protected bool isEdit = true;
        protected override async Task OnInitializedAsync()
        {
            exercises = await devService.GetExercises(new Query { Expand = "Day" });

            daysFordayId = await devService.GetDays();
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            isEdit = false;
            exercise = new Fitnessapp.Models.dev.Exercise();
        }

        protected async Task EditRow(Fitnessapp.Models.dev.Exercise args)
        {
            isEdit = true;
            exercise = args;
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Fitnessapp.Models.dev.Exercise exercise)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await devService.DeleteExercise(exercise.id);

                    if (deleteResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                { 
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error", 
                    Detail = $"Unable to delete Exercise" 
                });
            }
        }
        protected bool errorVisible;
        protected Fitnessapp.Models.dev.Exercise exercise;

        protected IEnumerable<Fitnessapp.Models.dev.Day> daysFordayId;

        protected async Task FormSubmit()
        {
            try
            {
                var result = isEdit ? await devService.UpdateExercise(exercise.id, exercise) : await devService.CreateExercise(exercise);

            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {

        }
    }
}