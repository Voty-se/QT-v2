using System.Collections.Generic;

namespace QT.Models
{
    public static class ProductContainer
    {
        public static List<StandarProduct> Products => new List<StandarProduct>
        {
            new StandarProduct
            {
                Id = 1,
                Name = "Kontinentalsäng",
                Text = "",
                Price = 500,
                Time = 30
            },
            new StandarProduct
            {
                Id = 2,
                Name = "Ramsäng u. Förvaring",
                Text = "",
                Price = 500,
                Time = 30
            },
            new StandarProduct
            {
                Id = 3,
                Name = "Ramsäng m. Förvaring",
                Text = "",
                Price = 750,
                Time = 60
            },
            new StandarProduct
            {
                Id = 4,
                Name = "Garderob",
                Text = "",
                Price = 1500,
                Time = 120
            },
            new StandarProduct
            {
                Id = 5,
                Name = "Sängbord",
                Text = "(1-2 st.)",
                Price = 300,
                Time = 45
            },
            new StandarProduct
            {
                Id = 6,
                Name = "Matbord",
                Text = "",
                Price = 400,
                Time = 45
            },
            new StandarProduct
            {
                Id = 7,
                Name = "Stolar",
                Text = "(4-6 stolar)",
                Price = 600,
                Time = 60
            },
            new StandarProduct
            {
                Id = 8,
                Name = "Vitrinskåp",
                Text = "",
                Price = 1299,
                Time = 120
            },
            new StandarProduct
            {
                Id = 9,
                Name = "Soffbord",
                Text = "",
                Price = 300,
                Time = 45
            },
            new StandarProduct
            {
                Id = 10,
                Name = "Byrå",
                Text = "",
                Price = 600,
                Time = 90
            },
            new StandarProduct
            {
                Id = 11,
                Name = "Soffa",
                Text = "(1-2 soffor)",
                Price = 500,
                Time = 30
            },
            new StandarProduct
            {
                Id = 12,
                Name = "Byggbar soffa",
                Text = "",
                Price = 1000,
                Time = 60
            },
            new StandarProduct
            {
                Id = 13,
                Name = "Hyllkombination",
                Text = "",
                Price = 1999,
                Time = 150
            },
            new StandarProduct
            {
                Id = 14,
                Name = "Mediabänk",
                Text = "",
                Price = 700,
                Time = 60
            },
            new StandarProduct
            {
                Id = 15,
                Name = "Fåtöljer",
                Text = "",
                Price = 400,
                Time = 30
            },
            new StandarProduct
            {
                Id = 16,
                Name = "Kontorsstol",
                Text = "",
                Price = 400,
                Time = 30
            },
            new StandarProduct
            {
                Id = 17,
                Name = "Skrivbord",
                Text = "",
                Price = 500,
                Time = 60
            },
            new StandarProduct
            {
                Id = 18,
                Name = "Hyllor",
                Text = "",
                Price = 300,
                Time = 40
            },
            new StandarProduct
            {
                Id = 19,
                Name = "Skoskåp",
                Text = "",
                Price = 500,
                Time = 60
            },
            new StandarProduct
            {
                Id = 20,
                Name = "Hallkombination",
                Text = "(1-4 delar)",
                Price = 1299,
                Time = 120
            },
            new StandarProduct
            {
                Id = 21,
                Name = "Skjutdörrsgarderob",
                Text = "3tim",
                Price = 2499,
                Time = 180
            }
        };
    }

    public class StandarProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public decimal Price { get; set; }
        public int Time { get; set; }
    }
}