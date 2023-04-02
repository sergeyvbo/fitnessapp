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
    public partial class Workouts
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

        protected IEnumerable<Fitnessapp.Models.dev.Workout> workouts;

        protected RadzenDataGrid<Fitnessapp.Models.dev.Workout> grid0;
        protected bool isEdit = true;
        protected override async Task OnInitializedAsync()
        {
            workouts = await devService.GetWorkouts(new Query { Expand = "Exercise" });

            exercisesForexerciseId = await devService.GetExercises();
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            isEdit = false;
            workout = new Fitnessapp.Models.dev.Workout();
        }

        protected async Task EditRow(Fitnessapp.Models.dev.Workout args)
        {
            isEdit = true;
            workout = args;
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Fitnessapp.Models.dev.Workout workout)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await devService.DeleteWorkout(workout.id);

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
                    Detail = $"Unable to delete Workout" 
                });
            }
        }
        protected bool errorVisible;
        protected Fitnessapp.Models.dev.Workout workout;

        protected IEnumerable<Fitnessapp.Models.dev.Exercise> exercisesForexerciseId;

        protected async Task FormSubmit()
        {
            try
            {
                var result = isEdit ? await devService.UpdateWorkout(workout.id, workout) : await devService.CreateWorkout(workout);

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