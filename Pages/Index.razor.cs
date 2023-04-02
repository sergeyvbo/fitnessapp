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
        protected IQueryable<Fitnessapp.Models.dev.Exercise> exercises;

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
        }
    }
}