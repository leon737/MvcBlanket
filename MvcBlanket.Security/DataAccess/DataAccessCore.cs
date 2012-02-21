namespace MvcBlanket.Security.DataAccess
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
