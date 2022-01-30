using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mixed.Models
{
    public class HomeModel
    {
        public List<Item> items;
        public List<Collection> collections;
        public HomeModel(List<Item> items, List<Collection> collections)
        {
            this.items = items;
            this.collections = collections;

        }
    }
}
