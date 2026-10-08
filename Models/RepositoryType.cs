using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DemoRepo.Models
{
    // enum = une liste fermée de valeurs possibles
    public enum RepositoryType
    {
        Fake,    // repository en mémoire
        AdoNet   // repository SQL Server
    }
}
