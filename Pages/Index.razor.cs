using Fitnessapp.Models.dev;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Fitnessapp.Pages
{
    public partial class Index
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
        protected Fitnessapp.devService devService { get; set; }

        protected System.Linq.IQueryable<Fitnessapp.Models.dev.Day> days;
        protected int SelectedDay;

        protected TimeSpan WarmUpTime = TimeSpan.FromMinutes(5);
        protected TimeSpan ExerciseTime = TimeSpan.FromMinutes(2);
        protected TimeSpan CoolDownTime = TimeSpan.FromMinutes(5);
        protected IQueryable<Exercise> exercises;

        protected List<Workout> workouts;

        
        protected override async Task OnInitializedAsync()
        {
            days = await devService.GetDays();
        }

        protected async void WorkoutDaySelectedItemChanged(System.Object args)
        {
            if (SelectedDay == 0)
            {
                return;
            }
            var query = new Query()
            {
                Filter = $@"f => f.day_id == @0",
                FilterParameters = new object[] { SelectedDay }
            };
            exercises = await devService.GetExercises(query);

            var prevWorkouts = await devService.GetLastWorkoutsByDay(SelectedDay);
            var prevWorkoutsList = prevWorkouts.ToList();
            workouts = new List<Workout>();

            foreach (var exercise in exercises)
            {
                var prevWorkout = prevWorkoutsList.FirstOrDefault(x => x.exercise_id == exercise.id);

                workouts.Add( new Workout {
                    date = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc),
                    exercise_id = exercise.id,
                    weight1 = prevWorkout?.weight1 ?? 0,
                    weight2 = prevWorkout?.weight2 ?? 0,
                    weight3 = prevWorkout?.weight3 ?? 0,
                    reps1 = prevWorkout?.reps1 ?? 0,
                    reps2 = prevWorkout?.reps2 ?? 0,
                    reps3 = prevWorkout?.reps3 ?? 0  
                });
            }
           
            
        }

        protected string ExerciseNameById(int id)
        {
            return exercises.ToList().FirstOrDefault(e=>e.id == id).name;
        }

        protected async void SaveWorkout()
        {
            foreach (var workout in workouts)
            {
                await devService.CreateWorkout(workout);
            }
            
        }

        
    }
}