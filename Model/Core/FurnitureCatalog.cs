using Model.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Core
{
    public partial class FurnitureCatalog : IFurnitureCatalog, ISortable
    {
        public string Name { get; set; }
        public string Season { get; set; }
        public Furniture[] Items { get; set; }
        public FurnitureCatalog()
        {
            Items = new Furniture[0];
        }
    }
}
