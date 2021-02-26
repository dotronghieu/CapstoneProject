using System;
using Capstone.Project.Data.Models;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Project.Services.IServices
{
    public interface ITypeService
    {
        IEnumerable<Data.Models.Type> GetAllType();
    }
}
