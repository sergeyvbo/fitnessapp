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
    public partial class Days
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

        protected IEnumerable<Fitnessapp.Models.dev.Day> days;

        protected RadzenDataGrid<Fitnessapp.Models.dev.Day> grid0;
        protected bool isEdit = true;
        protected override async Task OnInitializedAsync()
        {
            days = await devService.GetDays();
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            isEdit = false;
            day = new Fitnessapp.Models.dev.Day();
        }

        protected async Task EditRow(Fitnessapp.Models.dev.Day args)
        {
            isEdit = true;
            day = args;
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Fitnessapp.Models.dev.Day day)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await devService.DeleteDay(day.id);

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
                    Detail = $"Unable to delete Day" 
                });
            }
        }
        protected bool errorVisible;
        protected Fitnessapp.Models.dev.Day day;

        protected async Task FormSubmit()
        {
            try
            {
                var result = isEdit ? await devService.UpdateDay(day.id, day) : await devService.CreateDay(day);

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