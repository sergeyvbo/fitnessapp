using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Radzen;

using Fitnessapp.Data;
using Fitnessapp.Models.dev;

namespace Fitnessapp
{
    public partial class devService
    {
        devContext Context
        {
           get
           {
             return this.context;
           }
        }

        private readonly devContext context;
        private readonly NavigationManager navigationManager;

        public devService(devContext context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);


        public async Task ExportWorkoutsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dev/workouts/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dev/workouts/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportWorkoutsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dev/workouts/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dev/workouts/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnWorkoutsRead(ref IQueryable<Fitnessapp.Models.dev.Workout> items);

        public async Task<IQueryable<Fitnessapp.Models.dev.Workout>> GetWorkouts(Query query = null)
        {
            var items = Context.Workouts.AsQueryable();

            items = items.Include(i => i.Exercise);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }

            OnWorkoutsRead(ref items);

            return await Task.FromResult(items);
        }

        internal async Task<IQueryable<Workout>> GetLastWorkoutsByDay(int dayId)
        {
            var exerciseIds = Context.Exercises
                .Where(e => e.day_id == dayId);

            var prevWorkoutDate = Context.Workouts
                .Where(w => exerciseIds.Any(e => e.id == w.exercise_id))
                .OrderByDescending(w => w.date)
                .Take(1)
                .Select(w => w.date)
                .FirstOrDefault();

            var workouts = Context.Workouts
                .Where(w => w.date == prevWorkoutDate);

            return await Task.FromResult(workouts);

        }

        partial void OnWorkoutGet(Fitnessapp.Models.dev.Workout item);

        public async Task<Fitnessapp.Models.dev.Workout> GetWorkoutById(int id)
        {
            var items = Context.Workouts
                              .AsNoTracking()
                              .Where(i => i.id == id);

            items = items.Include(i => i.Exercise);
  
            var itemToReturn = items.FirstOrDefault();

            OnWorkoutGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnWorkoutCreated(Fitnessapp.Models.dev.Workout item);
        partial void OnAfterWorkoutCreated(Fitnessapp.Models.dev.Workout item);

        public async Task<Fitnessapp.Models.dev.Workout> CreateWorkout(Fitnessapp.Models.dev.Workout workout)
        {
            OnWorkoutCreated(workout);

            var existingItem = Context.Workouts
                              .Where(i => i.id == workout.id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            workout.date = DateTime.SpecifyKind(workout.date, DateTimeKind.Utc);

            try
            {
                Context.Workouts.Add(workout);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(workout).State = EntityState.Detached;
                throw;
            }

            OnAfterWorkoutCreated(workout);

            return workout;
        }

        public async Task<Fitnessapp.Models.dev.Workout> CancelWorkoutChanges(Fitnessapp.Models.dev.Workout item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnWorkoutUpdated(Fitnessapp.Models.dev.Workout item);
        partial void OnAfterWorkoutUpdated(Fitnessapp.Models.dev.Workout item);

        public async Task<Fitnessapp.Models.dev.Workout> UpdateWorkout(int id, Fitnessapp.Models.dev.Workout workout)
        {
            OnWorkoutUpdated(workout);

            var itemToUpdate = Context.Workouts
                              .Where(i => i.id == workout.id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(workout);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterWorkoutUpdated(workout);

            return workout;
        }

        partial void OnWorkoutDeleted(Fitnessapp.Models.dev.Workout item);
        partial void OnAfterWorkoutDeleted(Fitnessapp.Models.dev.Workout item);

        public async Task<Fitnessapp.Models.dev.Workout> DeleteWorkout(int id)
        {
            var itemToDelete = Context.Workouts
                              .Where(i => i.id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnWorkoutDeleted(itemToDelete);


            Context.Workouts.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterWorkoutDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportExercisesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dev/exercises/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dev/exercises/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportExercisesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dev/exercises/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dev/exercises/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnExercisesRead(ref IQueryable<Fitnessapp.Models.dev.Exercise> items);

        public async Task<IQueryable<Fitnessapp.Models.dev.Exercise>> GetExercises(Query query = null)
        {
            var items = Context.Exercises.AsQueryable();

            items = items.Include(i => i.Day);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }

            OnExercisesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnExerciseGet(Fitnessapp.Models.dev.Exercise item);

        public async Task<Fitnessapp.Models.dev.Exercise> GetExerciseById(int id)
        {
            var items = Context.Exercises
                              .AsNoTracking()
                              .Where(i => i.id == id);

            items = items.Include(i => i.Day);
  
            var itemToReturn = items.FirstOrDefault();

            OnExerciseGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnExerciseCreated(Fitnessapp.Models.dev.Exercise item);
        partial void OnAfterExerciseCreated(Fitnessapp.Models.dev.Exercise item);

        public async Task<Fitnessapp.Models.dev.Exercise> CreateExercise(Fitnessapp.Models.dev.Exercise exercise)
        {
            OnExerciseCreated(exercise);

            var existingItem = Context.Exercises
                              .Where(i => i.id == exercise.id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Exercises.Add(exercise);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(exercise).State = EntityState.Detached;
                throw;
            }

            OnAfterExerciseCreated(exercise);

            return exercise;
        }

        public async Task<Fitnessapp.Models.dev.Exercise> CancelExerciseChanges(Fitnessapp.Models.dev.Exercise item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnExerciseUpdated(Fitnessapp.Models.dev.Exercise item);
        partial void OnAfterExerciseUpdated(Fitnessapp.Models.dev.Exercise item);

        public async Task<Fitnessapp.Models.dev.Exercise> UpdateExercise(int id, Fitnessapp.Models.dev.Exercise exercise)
        {
            OnExerciseUpdated(exercise);

            var itemToUpdate = Context.Exercises
                              .Where(i => i.id == exercise.id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(exercise);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterExerciseUpdated(exercise);

            return exercise;
        }

        partial void OnExerciseDeleted(Fitnessapp.Models.dev.Exercise item);
        partial void OnAfterExerciseDeleted(Fitnessapp.Models.dev.Exercise item);

        public async Task<Fitnessapp.Models.dev.Exercise> DeleteExercise(int id)
        {
            var itemToDelete = Context.Exercises
                              .Where(i => i.id == id)
                              .Include(i => i.Workouts)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnExerciseDeleted(itemToDelete);


            Context.Exercises.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterExerciseDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportDaysToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dev/days/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dev/days/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportDaysToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dev/days/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dev/days/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnDaysRead(ref IQueryable<Fitnessapp.Models.dev.Day> items);

        public async Task<IQueryable<Fitnessapp.Models.dev.Day>> GetDays(Query query = null)
        {
            var items = Context.Days.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }

            OnDaysRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnDayGet(Fitnessapp.Models.dev.Day item);

        public async Task<Fitnessapp.Models.dev.Day> GetDayById(int id)
        {
            var items = Context.Days
                              .AsNoTracking()
                              .Where(i => i.id == id);

  
            var itemToReturn = items.FirstOrDefault();

            OnDayGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnDayCreated(Fitnessapp.Models.dev.Day item);
        partial void OnAfterDayCreated(Fitnessapp.Models.dev.Day item);

        public async Task<Fitnessapp.Models.dev.Day> CreateDay(Fitnessapp.Models.dev.Day day)
        {
            OnDayCreated(day);

            var existingItem = Context.Days
                              .Where(i => i.id == day.id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Days.Add(day);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(day).State = EntityState.Detached;
                throw;
            }

            OnAfterDayCreated(day);

            return day;
        }

        public async Task<Fitnessapp.Models.dev.Day> CancelDayChanges(Fitnessapp.Models.dev.Day item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnDayUpdated(Fitnessapp.Models.dev.Day item);
        partial void OnAfterDayUpdated(Fitnessapp.Models.dev.Day item);

        public async Task<Fitnessapp.Models.dev.Day> UpdateDay(int id, Fitnessapp.Models.dev.Day day)
        {
            OnDayUpdated(day);

            var itemToUpdate = Context.Days
                              .Where(i => i.id == day.id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(day);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterDayUpdated(day);

            return day;
        }

        partial void OnDayDeleted(Fitnessapp.Models.dev.Day item);
        partial void OnAfterDayDeleted(Fitnessapp.Models.dev.Day item);

        public async Task<Fitnessapp.Models.dev.Day> DeleteDay(int id)
        {
            var itemToDelete = Context.Days
                              .Where(i => i.id == id)
                              .Include(i => i.Exercises)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnDayDeleted(itemToDelete);


            Context.Days.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterDayDeleted(itemToDelete);

            return itemToDelete;
        }

        
    }
}