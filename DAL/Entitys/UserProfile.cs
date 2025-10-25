using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace Gps.DAL.Entitys
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("UserProfile")]
    public partial class UserProfile
    {
           public UserProfile(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
           public int Id {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string UserId {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string Avatar {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string MapDefaultView {get;set;}

    }
}
