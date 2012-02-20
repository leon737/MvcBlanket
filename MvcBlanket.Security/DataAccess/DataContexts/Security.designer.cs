﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.431
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Security.DataAccess.DataContexts
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="EasyPartner")]
	internal partial class SecurityDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertRole(Role instance);
    partial void UpdateRole(Role instance);
    partial void DeleteRole(Role instance);
    partial void InsertUser2Role(User2Role instance);
    partial void UpdateUser2Role(User2Role instance);
    partial void DeleteUser2Role(User2Role instance);
    partial void InsertUser(User instance);
    partial void UpdateUser(User instance);
    partial void DeleteUser(User instance);
    partial void InsertUser2ParentUser(User2ParentUser instance);
    partial void UpdateUser2ParentUser(User2ParentUser instance);
    partial void DeleteUser2ParentUser(User2ParentUser instance);
    #endregion
		
		public SecurityDataContext() : 
				base(global::Security.Properties.Settings.Default.SecurityConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public SecurityDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public SecurityDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public SecurityDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public SecurityDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<Role> Roles
		{
			get
			{
				return this.GetTable<Role>();
			}
		}
		
		public System.Data.Linq.Table<User2Role> User2Roles
		{
			get
			{
				return this.GetTable<User2Role>();
			}
		}
		
		public System.Data.Linq.Table<User> Users
		{
			get
			{
				return this.GetTable<User>();
			}
		}
		
		public System.Data.Linq.Table<User2ParentUser> User2ParentUsers
		{
			get
			{
				return this.GetTable<User2ParentUser>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="Security.Role")]
	public partial class Role : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private string _Name;
		
		private string _Description;
		
		private EntitySet<User2Role> _User2Roles;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    partial void OnDescriptionChanging(string value);
    partial void OnDescriptionChanged();
    #endregion
		
		public Role()
		{
			this._User2Roles = new EntitySet<User2Role>(new Action<User2Role>(this.attach_User2Roles), new Action<User2Role>(this.detach_User2Roles));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="NVarChar(100) NOT NULL", CanBeNull=false)]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Description", DbType="NVarChar(MAX)")]
		public string Description
		{
			get
			{
				return this._Description;
			}
			set
			{
				if ((this._Description != value))
				{
					this.OnDescriptionChanging(value);
					this.SendPropertyChanging();
					this._Description = value;
					this.SendPropertyChanged("Description");
					this.OnDescriptionChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Role_User2Role", Storage="_User2Roles", ThisKey="Id", OtherKey="RoleId")]
		public EntitySet<User2Role> User2Roles
		{
			get
			{
				return this._User2Roles;
			}
			set
			{
				this._User2Roles.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_User2Roles(User2Role entity)
		{
			this.SendPropertyChanging();
			entity.Role = this;
		}
		
		private void detach_User2Roles(User2Role entity)
		{
			this.SendPropertyChanging();
			entity.Role = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="Security.User2Role")]
	public partial class User2Role : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private int _UserId;
		
		private int _RoleId;
		
		private bool _Access;
		
		private EntityRef<Role> _Role;
		
		private EntityRef<User> _User;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnUserIdChanging(int value);
    partial void OnUserIdChanged();
    partial void OnRoleIdChanging(int value);
    partial void OnRoleIdChanged();
    partial void OnAccessChanging(bool value);
    partial void OnAccessChanged();
    #endregion
		
		public User2Role()
		{
			this._Role = default(EntityRef<Role>);
			this._User = default(EntityRef<User>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_UserId", DbType="Int NOT NULL")]
		public int UserId
		{
			get
			{
				return this._UserId;
			}
			set
			{
				if ((this._UserId != value))
				{
					if (this._User.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnUserIdChanging(value);
					this.SendPropertyChanging();
					this._UserId = value;
					this.SendPropertyChanged("UserId");
					this.OnUserIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_RoleId", DbType="Int NOT NULL")]
		public int RoleId
		{
			get
			{
				return this._RoleId;
			}
			set
			{
				if ((this._RoleId != value))
				{
					if (this._Role.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnRoleIdChanging(value);
					this.SendPropertyChanging();
					this._RoleId = value;
					this.SendPropertyChanged("RoleId");
					this.OnRoleIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Access", DbType="bit NOT NULL")]
		public bool Access
		{
			get
			{
				return this._Access;
			}
			set
			{
				if ((this._Access != value))
				{
					this.OnAccessChanging(value);
					this.SendPropertyChanging();
					this._Access = value;
					this.SendPropertyChanged("Access");
					this.OnAccessChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Role_User2Role", Storage="_Role", ThisKey="RoleId", OtherKey="Id", IsForeignKey=true)]
		public Role Role
		{
			get
			{
				return this._Role.Entity;
			}
			set
			{
				Role previousValue = this._Role.Entity;
				if (((previousValue != value) 
							|| (this._Role.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Role.Entity = null;
						previousValue.User2Roles.Remove(this);
					}
					this._Role.Entity = value;
					if ((value != null))
					{
						value.User2Roles.Add(this);
						this._RoleId = value.Id;
					}
					else
					{
						this._RoleId = default(int);
					}
					this.SendPropertyChanged("Role");
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_User2Role", Storage="_User", ThisKey="UserId", OtherKey="Id", IsForeignKey=true)]
		public User User
		{
			get
			{
				return this._User.Entity;
			}
			set
			{
				User previousValue = this._User.Entity;
				if (((previousValue != value) 
							|| (this._User.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._User.Entity = null;
						previousValue.User2Roles.Remove(this);
					}
					this._User.Entity = value;
					if ((value != null))
					{
						value.User2Roles.Add(this);
						this._UserId = value.Id;
					}
					else
					{
						this._UserId = default(int);
					}
					this.SendPropertyChanged("User");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="Security.[User]")]
	public partial class User : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private System.Nullable<int> _GroupId;
		
		private string _Login;
		
		private System.Data.Linq.Binary _Password;
		
		private string _Salt;
		
		private string _PasswordQuestion;
		
		private string _PasswordAnswer;
		
		private string _FirstName;
		
		private string _SecondName;
		
		private string _LastName;
		
		private string _EMail;
		
		private System.DateTime _RegDate;
		
		private System.DateTime _Modified;
		
		private System.Nullable<System.DateTime> _LastLogon;
		
		private System.Nullable<System.DateTime> _LastActivity;
		
		private bool _IsActive;
		
		private bool _IsGroup;
		
		private System.Guid _ActivationKey;
		
		private EntitySet<User2Role> _User2Roles;
		
		private EntitySet<User> _Users;
		
		private EntitySet<User2ParentUser> _ChildUsers;
		
		private EntitySet<User2ParentUser> _ParentUsers;
		
		private EntityRef<User> _User1;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnGroupIdChanging(System.Nullable<int> value);
    partial void OnGroupIdChanged();
    partial void OnLoginChanging(string value);
    partial void OnLoginChanged();
    partial void OnPasswordChanging(System.Data.Linq.Binary value);
    partial void OnPasswordChanged();
    partial void OnSaltChanging(string value);
    partial void OnSaltChanged();
    partial void OnPasswordQuestionChanging(string value);
    partial void OnPasswordQuestionChanged();
    partial void OnPasswordAnswerChanging(string value);
    partial void OnPasswordAnswerChanged();
    partial void OnFirstNameChanging(string value);
    partial void OnFirstNameChanged();
    partial void OnSecondNameChanging(string value);
    partial void OnSecondNameChanged();
    partial void OnLastNameChanging(string value);
    partial void OnLastNameChanged();
    partial void OnEMailChanging(string value);
    partial void OnEMailChanged();
    partial void OnRegDateChanging(System.DateTime value);
    partial void OnRegDateChanged();
    partial void OnModifiedChanging(System.DateTime value);
    partial void OnModifiedChanged();
    partial void OnLastLogonChanging(System.Nullable<System.DateTime> value);
    partial void OnLastLogonChanged();
    partial void OnLastActivityChanging(System.Nullable<System.DateTime> value);
    partial void OnLastActivityChanged();
    partial void OnIsActiveChanging(bool value);
    partial void OnIsActiveChanged();
    partial void OnIsGroupChanging(bool value);
    partial void OnIsGroupChanged();
    partial void OnActivationKeyChanging(System.Guid value);
    partial void OnActivationKeyChanged();
    #endregion
		
		public User()
		{
			this._User2Roles = new EntitySet<User2Role>(new Action<User2Role>(this.attach_User2Roles), new Action<User2Role>(this.detach_User2Roles));
			this._Users = new EntitySet<User>(new Action<User>(this.attach_Users), new Action<User>(this.detach_Users));
			this._ChildUsers = new EntitySet<User2ParentUser>(new Action<User2ParentUser>(this.attach_ChildUsers), new Action<User2ParentUser>(this.detach_ChildUsers));
			this._ParentUsers = new EntitySet<User2ParentUser>(new Action<User2ParentUser>(this.attach_ParentUsers), new Action<User2ParentUser>(this.detach_ParentUsers));
			this._User1 = default(EntityRef<User>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_GroupId", DbType="Int")]
		public System.Nullable<int> GroupId
		{
			get
			{
				return this._GroupId;
			}
			set
			{
				if ((this._GroupId != value))
				{
					if (this._User1.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnGroupIdChanging(value);
					this.SendPropertyChanging();
					this._GroupId = value;
					this.SendPropertyChanged("GroupId");
					this.OnGroupIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Login", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string Login
		{
			get
			{
				return this._Login;
			}
			set
			{
				if ((this._Login != value))
				{
					this.OnLoginChanging(value);
					this.SendPropertyChanging();
					this._Login = value;
					this.SendPropertyChanged("Login");
					this.OnLoginChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Password", DbType="Binary(16)", UpdateCheck=UpdateCheck.Never)]
		public System.Data.Linq.Binary Password
		{
			get
			{
				return this._Password;
			}
			set
			{
				if ((this._Password != value))
				{
					this.OnPasswordChanging(value);
					this.SendPropertyChanging();
					this._Password = value;
					this.SendPropertyChanged("Password");
					this.OnPasswordChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Salt", DbType="NVarChar(4) NOT NULL", CanBeNull=false)]
		public string Salt
		{
			get
			{
				return this._Salt;
			}
			set
			{
				if ((this._Salt != value))
				{
					this.OnSaltChanging(value);
					this.SendPropertyChanging();
					this._Salt = value;
					this.SendPropertyChanged("Salt");
					this.OnSaltChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PasswordQuestion", DbType="NVarChar(500)")]
		public string PasswordQuestion
		{
			get
			{
				return this._PasswordQuestion;
			}
			set
			{
				if ((this._PasswordQuestion != value))
				{
					this.OnPasswordQuestionChanging(value);
					this.SendPropertyChanging();
					this._PasswordQuestion = value;
					this.SendPropertyChanged("PasswordQuestion");
					this.OnPasswordQuestionChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PasswordAnswer", DbType="NVarChar(500)")]
		public string PasswordAnswer
		{
			get
			{
				return this._PasswordAnswer;
			}
			set
			{
				if ((this._PasswordAnswer != value))
				{
					this.OnPasswordAnswerChanging(value);
					this.SendPropertyChanging();
					this._PasswordAnswer = value;
					this.SendPropertyChanged("PasswordAnswer");
					this.OnPasswordAnswerChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FirstName", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string FirstName
		{
			get
			{
				return this._FirstName;
			}
			set
			{
				if ((this._FirstName != value))
				{
					this.OnFirstNameChanging(value);
					this.SendPropertyChanging();
					this._FirstName = value;
					this.SendPropertyChanged("FirstName");
					this.OnFirstNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SecondName", DbType="NVarChar(50)")]
		public string SecondName
		{
			get
			{
				return this._SecondName;
			}
			set
			{
				if ((this._SecondName != value))
				{
					this.OnSecondNameChanging(value);
					this.SendPropertyChanging();
					this._SecondName = value;
					this.SendPropertyChanged("SecondName");
					this.OnSecondNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LastName", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string LastName
		{
			get
			{
				return this._LastName;
			}
			set
			{
				if ((this._LastName != value))
				{
					this.OnLastNameChanging(value);
					this.SendPropertyChanging();
					this._LastName = value;
					this.SendPropertyChanged("LastName");
					this.OnLastNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_EMail", DbType="NVarChar(200) NOT NULL", CanBeNull=false)]
		public string EMail
		{
			get
			{
				return this._EMail;
			}
			set
			{
				if ((this._EMail != value))
				{
					this.OnEMailChanging(value);
					this.SendPropertyChanging();
					this._EMail = value;
					this.SendPropertyChanged("EMail");
					this.OnEMailChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_RegDate", DbType="DateTime NOT NULL")]
		public System.DateTime RegDate
		{
			get
			{
				return this._RegDate;
			}
			set
			{
				if ((this._RegDate != value))
				{
					this.OnRegDateChanging(value);
					this.SendPropertyChanging();
					this._RegDate = value;
					this.SendPropertyChanged("RegDate");
					this.OnRegDateChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Modified", DbType="DateTime NOT NULL")]
		public System.DateTime Modified
		{
			get
			{
				return this._Modified;
			}
			set
			{
				if ((this._Modified != value))
				{
					this.OnModifiedChanging(value);
					this.SendPropertyChanging();
					this._Modified = value;
					this.SendPropertyChanged("Modified");
					this.OnModifiedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LastLogon", DbType="DateTime")]
		public System.Nullable<System.DateTime> LastLogon
		{
			get
			{
				return this._LastLogon;
			}
			set
			{
				if ((this._LastLogon != value))
				{
					this.OnLastLogonChanging(value);
					this.SendPropertyChanging();
					this._LastLogon = value;
					this.SendPropertyChanged("LastLogon");
					this.OnLastLogonChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LastActivity", DbType="DateTime")]
		public System.Nullable<System.DateTime> LastActivity
		{
			get
			{
				return this._LastActivity;
			}
			set
			{
				if ((this._LastActivity != value))
				{
					this.OnLastActivityChanging(value);
					this.SendPropertyChanging();
					this._LastActivity = value;
					this.SendPropertyChanged("LastActivity");
					this.OnLastActivityChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IsActive", DbType="Bit NOT NULL")]
		public bool IsActive
		{
			get
			{
				return this._IsActive;
			}
			set
			{
				if ((this._IsActive != value))
				{
					this.OnIsActiveChanging(value);
					this.SendPropertyChanging();
					this._IsActive = value;
					this.SendPropertyChanged("IsActive");
					this.OnIsActiveChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IsGroup", DbType="Bit NOT NULL")]
		public bool IsGroup
		{
			get
			{
				return this._IsGroup;
			}
			set
			{
				if ((this._IsGroup != value))
				{
					this.OnIsGroupChanging(value);
					this.SendPropertyChanging();
					this._IsGroup = value;
					this.SendPropertyChanged("IsGroup");
					this.OnIsGroupChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ActivationKey", DbType="uniqueidentifier")]
		public System.Guid ActivationKey
		{
			get
			{
				return this._ActivationKey;
			}
			set
			{
				if ((this._ActivationKey != value))
				{
					this.OnActivationKeyChanging(value);
					this.SendPropertyChanging();
					this._ActivationKey = value;
					this.SendPropertyChanged("ActivationKey");
					this.OnActivationKeyChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_User2Role", Storage="_User2Roles", ThisKey="Id", OtherKey="UserId")]
		public EntitySet<User2Role> User2Roles
		{
			get
			{
				return this._User2Roles;
			}
			set
			{
				this._User2Roles.Assign(value);
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_User", Storage="_Users", ThisKey="Id", OtherKey="GroupId")]
		public EntitySet<User> Users
		{
			get
			{
				return this._Users;
			}
			set
			{
				this._Users.Assign(value);
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_User2ParentUser", Storage="_ChildUsers", ThisKey="Id", OtherKey="ParentUserId")]
		public EntitySet<User2ParentUser> ChildUsers
		{
			get
			{
				return this._ChildUsers;
			}
			set
			{
				this._ChildUsers.Assign(value);
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_User2ParentUser1", Storage="_ParentUsers", ThisKey="Id", OtherKey="UserId")]
		public EntitySet<User2ParentUser> ParentUsers
		{
			get
			{
				return this._ParentUsers;
			}
			set
			{
				this._ParentUsers.Assign(value);
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_User", Storage="_User1", ThisKey="GroupId", OtherKey="Id", IsForeignKey=true)]
		public User Group
		{
			get
			{
				return this._User1.Entity;
			}
			set
			{
				User previousValue = this._User1.Entity;
				if (((previousValue != value) 
							|| (this._User1.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._User1.Entity = null;
						previousValue.Users.Remove(this);
					}
					this._User1.Entity = value;
					if ((value != null))
					{
						value.Users.Add(this);
						this._GroupId = value.Id;
					}
					else
					{
						this._GroupId = default(Nullable<int>);
					}
					this.SendPropertyChanged("Group");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_User2Roles(User2Role entity)
		{
			this.SendPropertyChanging();
			entity.User = this;
		}
		
		private void detach_User2Roles(User2Role entity)
		{
			this.SendPropertyChanging();
			entity.User = null;
		}
		
		private void attach_Users(User entity)
		{
			this.SendPropertyChanging();
			entity.Group = this;
		}
		
		private void detach_Users(User entity)
		{
			this.SendPropertyChanging();
			entity.Group = null;
		}
		
		private void attach_ChildUsers(User2ParentUser entity)
		{
			this.SendPropertyChanging();
			entity.ParentUser = this;
		}
		
		private void detach_ChildUsers(User2ParentUser entity)
		{
			this.SendPropertyChanging();
			entity.ParentUser = null;
		}
		
		private void attach_ParentUsers(User2ParentUser entity)
		{
			this.SendPropertyChanging();
			entity.ChildUser = this;
		}
		
		private void detach_ParentUsers(User2ParentUser entity)
		{
			this.SendPropertyChanging();
			entity.ChildUser = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="Security.User2ParentUser")]
	public partial class User2ParentUser : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private int _UserId;
		
		private int _ParentUserId;
		
		private EntityRef<User> _ParentUser;
		
		private EntityRef<User> _ChildUser;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnUserIdChanging(int value);
    partial void OnUserIdChanged();
    partial void OnParentUserIdChanging(int value);
    partial void OnParentUserIdChanged();
    #endregion
		
		public User2ParentUser()
		{
			this._ParentUser = default(EntityRef<User>);
			this._ChildUser = default(EntityRef<User>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_UserId", DbType="Int NOT NULL")]
		public int UserId
		{
			get
			{
				return this._UserId;
			}
			set
			{
				if ((this._UserId != value))
				{
					if (this._ChildUser.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnUserIdChanging(value);
					this.SendPropertyChanging();
					this._UserId = value;
					this.SendPropertyChanged("UserId");
					this.OnUserIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ParentUserId", DbType="Int NOT NULL")]
		public int ParentUserId
		{
			get
			{
				return this._ParentUserId;
			}
			set
			{
				if ((this._ParentUserId != value))
				{
					if (this._ParentUser.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnParentUserIdChanging(value);
					this.SendPropertyChanging();
					this._ParentUserId = value;
					this.SendPropertyChanged("ParentUserId");
					this.OnParentUserIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_User2ParentUser", Storage="_ParentUser", ThisKey="ParentUserId", OtherKey="Id", IsForeignKey=true)]
		public User ParentUser
		{
			get
			{
				return this._ParentUser.Entity;
			}
			set
			{
				User previousValue = this._ParentUser.Entity;
				if (((previousValue != value) 
							|| (this._ParentUser.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._ParentUser.Entity = null;
						previousValue.ChildUsers.Remove(this);
					}
					this._ParentUser.Entity = value;
					if ((value != null))
					{
						value.ChildUsers.Add(this);
						this._ParentUserId = value.Id;
					}
					else
					{
						this._ParentUserId = default(int);
					}
					this.SendPropertyChanged("ParentUser");
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_User2ParentUser1", Storage="_ChildUser", ThisKey="UserId", OtherKey="Id", IsForeignKey=true)]
		public User ChildUser
		{
			get
			{
				return this._ChildUser.Entity;
			}
			set
			{
				User previousValue = this._ChildUser.Entity;
				if (((previousValue != value) 
							|| (this._ChildUser.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._ChildUser.Entity = null;
						previousValue.ParentUsers.Remove(this);
					}
					this._ChildUser.Entity = value;
					if ((value != null))
					{
						value.ParentUsers.Add(this);
						this._UserId = value.Id;
					}
					else
					{
						this._UserId = default(int);
					}
					this.SendPropertyChanged("ChildUser");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591
