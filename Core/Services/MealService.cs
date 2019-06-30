using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataContext.Data;
using DataContext.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Core.Services
{
    public class MealService
    {
        private readonly CaloriesContext _context;

        public MealService(CaloriesContext context)
        {
            _context = context;
        }

        public Task<List<Meal>> GetMealsAsync()
        {
            return _context.Meals
                .ToListAsync();
        }
        
        public Task<Meal> GetRecipesForMealIdAsync(int mealId)
        {
            return GetMealDecoratedWithRelations()
                .SingleAsync(m => m.Id == mealId);
        }

        public Task<Meal> AddRecipeToMealAsync(int recipeId, int mealId)
        {
            var meal = GetMealDecoratedWithRelations()
                .Single(m => m.Id == mealId);
            meal.Recipes.Add(new MealRecipe
            {
                RecipeId = recipeId, CreatedAt = DateTime.Now, MealId = mealId
            });
            _context.SaveChanges();

            return Task.FromResult(meal);
        }

        private IIncludableQueryable<Meal, Recipe> GetMealDecoratedWithRelations()
        {
            return _context.Meals
                .Include(m => m.Recipes)
                .ThenInclude(x => x.Recipe);
        }

        public Task<Meal> RemoveRecipeFromMealAsync(int recipeId, int mealId, DateTime createdAt)
        {
            var meal = GetMealDecoratedWithRelations()
                .Single(m => m.Id == mealId);
            var recipe = meal.Recipes.Single(x => x.RecipeId == recipeId && x.CreatedAt == createdAt);
            meal.Recipes.Remove(recipe);
            _context.SaveChanges();
            
            return Task.FromResult(meal);
        }
    }
}