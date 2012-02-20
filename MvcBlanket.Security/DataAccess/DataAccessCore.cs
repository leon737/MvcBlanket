using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Security.DataAccess.DataContexts;

namespace Security.DataAccess
{
	internal static class DataAccessCore
	{
		public static SecurityDataContext Security
		{
			get
			{
				return new SecurityDataContext();
			}
		}		
	}

}
