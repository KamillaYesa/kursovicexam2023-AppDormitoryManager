//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AppDormitoryManager
{
    using System;
    using System.Collections.Generic;
    
    public partial class User
    {
        public int ID_User { get; set; }
        public string FullNameUser { get; set; }
        public Nullable<int> LoginUser { get; set; }
    
        public virtual Login Login { get; set; }
    }
}
