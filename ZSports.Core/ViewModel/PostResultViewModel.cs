using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSports.Core.ViewModel
{
    public class PostResultViewModel
    {
        public bool Success { get; set; }
        public string Error { get; set; } = default!;
        public string Warning { get; set; } = default!;
        public dynamic Model { get; set; } = default!;

        public PostResultViewModel() 
        {
            Success = true;
        }

        public PostResultViewModel(dynamic model)
        {
            Success = model is not null;

            if (Success)
                Model = model!;
        }

        public PostResultViewModel(dynamic model, string warning)
        {
            Success = model is not null;
            Warning = warning;

            if (Success)
                Model = model!;
        }

        public PostResultViewModel(string error)
        {
            Success = false;
            Error = error;
        }
    }
}
