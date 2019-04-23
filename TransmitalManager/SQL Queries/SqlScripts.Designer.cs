﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TransmittalManager.SQL_Queries {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class SqlScripts {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SqlScripts() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TransmittalManager.SQL_Queries.SqlScripts", typeof(SqlScripts).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --insert into TransFiles (TransId, FileName, FileId, Version, Revision)
        ///--values (&apos;@Tid&apos;, &apos;@FileName&apos;, &apos;@fileId&apos;,&apos;@Ver&apos;,&apos;@Rev&apos;)
        ///--SELECT SCOPE_IDENTITY();  
        ///
        ///
        ///insert into TransFiles (TransId, FileName, FileId, Version, Revision)
        ///values (@Tid, @FileName, @fileId,@Ver,@Rev)
        ///SELECT SCOPE_IDENTITY();  
        ///
        ///.
        /// </summary>
        internal static string AddFilesToTransmittal {
            get {
                return ResourceManager.GetString("AddFilesToTransmittal", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /****** Script for SelectTopNRows command from SSMS  ******/
        ///SELECT [ProjNumber],[ProjName]
        ///  FROM [Projects].
        /// </summary>
        internal static string GetAllProjects {
            get {
                return ResourceManager.GetString("GetAllProjects", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT * 
        ///FROM Transmittals t
        ///order by t.Id.
        /// </summary>
        internal static string GetAllTransmittals {
            get {
                return ResourceManager.GetString("GetAllTransmittals", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /****** Script for SelectTopNRows command from SSMS  ******/
        ///--use GekkoVault;
        ///--SELECT u.[UserID]
        ///--      ,[Username]      
        ///--      ,[Email]      
        ///--      ,[FullName]
        ///--      ,[Initials]           
        ///--	  ,gm.GroupID
        ///--	  ,g.Groupname
        ///--  FROM Users u
        ///--  inner join [GroupMembers] gm on gm.UserId = u.UserID 
        ///--  inner join Groups g on g.GroupID = gm.GroupID
        ///--  where [Enabled] = 1
        ///--  and g.Groupname = &apos;Drafters&apos;
        ///--  or g.Groupname = &apos;Librarians&apos;
        ///--  order by u.UserID
        ///
        ///
        ///
        ///  SELECT u.[UserID [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string GetAllUsers {
            get {
                return ResourceManager.GetString("GetAllUsers", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to  SELECT *
        ///
        ///FROM TransFiles tf 
        ///inner join Transmittal t on t.Id = tf.[TransId] 
        ///where tf.FileName like {0}
        ///{1}
        ///order by tf.TransId.
        /// </summary>
        internal static string GetTransmitalsFileName {
            get {
                return ResourceManager.GetString("GetTransmitalsFileName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to  SELECT *
        ///
        ///FROM TransFiles tf 
        ///where tf.TransId = {0}
        ///order by tf.Id.
        /// </summary>
        internal static string GetTransmittalFiles {
            get {
                return ResourceManager.GetString("GetTransmittalFiles", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to  SELECT *
        ///	from Transmittals t 
        ///	where t.Id &lt;&gt; 0
        ///{0}
        ///order by t.Id.
        /// </summary>
        internal static string GetTransmittals {
            get {
                return ResourceManager.GetString("GetTransmittals", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --insert into Transmittals ({0})
        ///--value {1}
        ///--output Inserted.Id
        ///
        ///insert into Transmittals ({0})
        ///values ({1})
        ///--output Inserted.Id
        ///SELECT SCOPE_IDENTITY().
        /// </summary>
        internal static string NewTransmittal {
            get {
                return ResourceManager.GetString("NewTransmittal", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to delete from 
        ///	TransFiles
        ///where Id in ({0}) 
        ///.
        /// </summary>
        internal static string RemoveFiles {
            get {
                return ResourceManager.GetString("RemoveFiles", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --update TransFiles
        ///--set {1}
        ///--where Id = {0}
        ///
        ///
        ///
        ///update TransFiles
        ///set 
        ///--TransId = ,
        ///FileName = &apos;@FileName&apos;,
        ///FileId = &apos;@id&apos;,
        ///Version = &apos;@Ver&apos;,
        ///Revision = &apos;@Rev&apos;
        ///where Id = {0} and TransId={1}.
        /// </summary>
        internal static string UpdateFile {
            get {
                return ResourceManager.GetString("UpdateFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to update Transmittals
        ///set {1}
        ///where Id = {0}.
        /// </summary>
        internal static string UpdateTransmittal {
            get {
                return ResourceManager.GetString("UpdateTransmittal", resourceCulture);
            }
        }
    }
}