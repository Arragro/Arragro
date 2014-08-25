using System.Collections.Generic;
namespace Arragro.Common.Tests.ModelsAndHelpers
{
    public class ModelFoo
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public static class ModelFooExtentions
    {
        public static List<ModelFoo> InitialiseAndLoadModelFoos(this List<ModelFoo> modelFoos)
        {
            modelFoos = new List<ModelFoo>();
            modelFoos.Add(new ModelFoo { Id = 1, Name = "Test 1" });
            modelFoos.Add(new ModelFoo { Id = 2, Name = "Test 2" });

            return modelFoos;
        }
    }
}
