using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiggyMarker
{
    internal class Order
    {
        public string OrderNumber;
        List<Recipe> Recipes;
        public int RecipeCount { get { return Recipes.Count; } }
        ErrorHandler Errors;
        public Order(string filePath, ErrorHandler errorHandler) 
        {
            Errors = errorHandler;
            Recipes = new List<Recipe>();
            OrderNumber = Path.GetFileName(filePath).Split("-")[0];
            AddFile(filePath);
        }
        public void AddFile(string filePath)
        {
            int index = GetRecipeIndex(filePath);
            if (index >= 0) Recipes[index].AddFile(filePath);
            else Recipes.Add(new Recipe(filePath));
        }
        private int GetRecipeIndex(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            string recipeID = fileName.Split("-")[1];
            for (int i = 0; i < Recipes.Count; i++) if (Recipes[i].RecipeID == recipeID) return i;
            return -1;
        }
        public string GetOrderType()
        {
            bool hasSR = false, hasJR = false;
            foreach (Recipe recipe in Recipes)
            {
                switch (recipe.JobType)
                {
                    case "JR":
                        hasJR = true;
                        break;
                    case "SR":
                        hasSR = true;
                        break;
                    case "ERROR":
                        ErrorCatcher.Errors.Add(new string[] {OrderNumber, recipe.RecipeID});
                        Errors.AddGeneralError(String.Format("{0}-{1}", OrderNumber, recipe.RecipeID));
                        break;
                }
            }
            if (hasSR)
            {
                if (hasJR) return "COMBO";
                return "SR";
            }
            if (hasJR) return "JR";
            return "ERROR";

        }
        public int WorkOrder(string type, int sequence, bool combo)
        {
            for(int i = 0; i < Recipes.Count; i++)
            {
                if (Recipes[i].JobType == type) 
                {
                    sequence  = Recipes[i].WorkRecipe(sequence, combo);
                } 
            }
            return sequence;
        }
        public string[][] GetOrderSummary()
        {
            List<string[]> answer = new List<string[]>();
            for(int i = 0; i < Recipes.Count; i++)
            {
                answer.Add([OrderNumber, Recipes[i].RecipeID, Recipes[i].QTY]);
            }
            return answer.ToArray();
        }
    }
}
