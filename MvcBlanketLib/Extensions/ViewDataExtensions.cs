using System.Web.Routing;

namespace System.Web.Mvc
{
	/// <summary>
	/// The class with extensions
	/// </summary>
	public static class ViewDataExtensions
	{
		/// <summary>
		/// Genereates key for model based on type full name
		/// </summary>
		/// <typeparam name="TModel">The model's type</typeparam>
		/// <returns>Key of model which can be used to find instance</returns>
		public static String GetKey<TModel>()
		{
			return typeof(TModel).FullName;
		}

		/// <summary>
		/// Resolves instance of model in ViewDataDictionary. Creates if model does not exists
		/// </summary>
		/// <typeparam name="TModel">The model's type</typeparam>
		/// <param name="viewData">The ViewData container for model</param>
		/// <returns>Instance of model</returns>
		public static TModel Model<TModel>(this ViewDataDictionary viewData)
		{
			String key = GetKey<TModel>();

			if (viewData.ContainsKey(key))
			{
				Object model = viewData[key];

				if ((model != null) && (model.GetType().Equals(typeof(TModel))))
				{
					return (TModel)model;
				}
			}

			TModel newModel = Activator.CreateInstance<TModel>();
			viewData[key] = newModel;
			return newModel;
		}

		/// <summary>
		/// Resolves instance of model in ViewPage.ViewData dictionary. Creates if model does not exists
		/// </summary>
		/// <typeparam name="TModel">The model's type</typeparam>
		/// <param name="viewPage">The current ViewPage</param>
		/// <returns>Instance of model</returns>
		public static TModel Model<TModel>(this System.Web.Mvc.ViewPage viewPage)
		{
			return viewPage.ViewData.Model<TModel>();
		}

		/// <summary>
		/// Resolves instance of model in ViewMasterPage.ViewData dictionary. Creates if model does not exists
		/// </summary>
		/// <typeparam name="TModel">The model's type</typeparam>
		/// <param name="viewMasterPage">The current ViewMasterPage</param>
		/// <returns>Instance of model</returns>
		public static TModel Model<TModel>(this System.Web.Mvc.ViewMasterPage viewMasterPage)
		{
			return viewMasterPage.ViewData.Model<TModel>();
		}

		/// <summary>
		/// Resolves instance of model in ViewUserControl.ViewData dictionary. Creates if model does not exists
		/// </summary>
		/// <typeparam name="TModel">The model's type</typeparam>
		/// <param name="viewControl">The current ViewUserControl</param>
		/// <returns>Instance of model</returns>
		public static TModel Model<TModel>(this System.Web.Mvc.ViewUserControl viewControl)
		{
			return viewControl.ViewData.Model<TModel>();
		}

		/// <summary>
		/// Checks model to exists in ViewDataDictionary
		/// </summary>
		/// <typeparam name="TModel">The model's type</typeparam>
		/// <param name="viewData">The current ViewDataDictionary container</param>
		/// <returns>Instance of model</returns>
		public static bool ModelExists<TModel>(this ViewDataDictionary viewData)
		{
			String key = GetKey<TModel>();

			if (viewData.ContainsKey(key))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Inserts instance of model into ViewDataDictionary container
		/// </summary>
		/// <typeparam name="TModel">The model's type</typeparam>
		/// <param name="viewData">The ViewDataDictionary container</param>
		/// <param name="model">Instance of model</param>
		public static void AttachViewModel<TModel>(this ViewDataDictionary viewData, TModel model)
		{
			String key = GetKey<TModel>();
			viewData[key] = model;
		}

		/// <summary>
		/// Inserts instance of model into RouteValueDictionary container
		/// </summary>
		/// <typeparam name="TModel">The model's type</typeparam>
		/// <param name="routeValueDictionary">The current RouteValueDictionary</param>
		/// <param name="model">Instance of model</param>
		public static void AttachViewModel<TModel>(this RouteValueDictionary routeValueDictionary, TModel model)
		{
			String key = GetKey<TModel>();

			if (routeValueDictionary.ContainsKey(key))
			{
				routeValueDictionary[key] = model;
			}
			else
			{
				routeValueDictionary.Add(key, model);
			}
		}

		/// <summary>
		/// Resolves instance of model in RouteData. Creates if model does not exists
		/// </summary>
		/// <typeparam name="TModel">The model's type</typeparam>
		/// <param name="routeData">The current RouteData</param>
		/// <param name="model">Instance of model to insert into route data</param>
		public static void AttachViewModel<TModel>(this RouteData routeData, TModel model)
		{
			String key = GetKey<TModel>();

			if (routeData.Values.ContainsKey(key))
			{
				routeData.Values[key] = model;
			}
			else
			{
				routeData.Values.Add(key, model);
			}
		}

		/// <summary>
		/// Resolves instance of model in RouteValueDictionary. Creates if model does not exists
		/// </summary>
		/// <typeparam name="TModel">The model's type</typeparam>
		/// <param name="routeValueDictionary">The current RouteValueDictionary</param>
		/// <returns>Instance of model</returns>
		public static TModel Model<TModel>(this RouteValueDictionary routeValueDictionary)
		{
			String key = GetKey<TModel>();

			if (routeValueDictionary.ContainsKey(key))
			{
				Object model = routeValueDictionary[key];

				if (model.GetType().Equals(typeof(TModel)))
				{
					return (TModel)model;
				}
			}

			TModel newModel = Activator.CreateInstance<TModel>();
			routeValueDictionary.Add(key, newModel);
			return newModel;
		}

		/// <summary>
		/// Resolves instance of model in ViewDataDictionary. Creates if model does not exists
		/// </summary>
		/// <typeparam name="TModel">The model's type</typeparam>
		/// <param name="htmlHelper">The current HtmlHelper</param>
		/// <returns>Instance of model</returns>
		public static TModel Model<TModel>(this HtmlHelper htmlHelper)
		{
			return htmlHelper.ViewData.Model<TModel>();
		}

		/// <summary>
		/// Checks model for exists in current ViewData container
		/// </summary>
		/// <typeparam name="TModel">The model's type</typeparam>
		/// <param name="htmlHelper">The current HtmlHelper</param>
		/// <returns>Instance of model</returns>
		public static bool ModelExists<TModel>(this HtmlHelper htmlHelper)
		{
			return htmlHelper.ViewData.ModelExists<TModel>();
		}

		/// <summary>
		/// Resolves instance of model in ViewDataDictionary. Creates if model does not exists
		/// </summary>
		/// <typeparam name="TModel">The model's type</typeparam>
		/// <param name="controller">The current controller</param>
		/// <returns>Instance of model</returns>
		public static TModel Model<TModel>(this Controller controller)
		{
			return controller.ViewData.Model<TModel>();
		}

		/// <summary>
		/// Inserts instance of model into ViewDataDictionary container
		/// </summary>
		/// <typeparam name="TModel">The model's type</typeparam>
		/// <param name="controller">The current controller</param>
		/// <param name="model">The instance of model to insert into ViewDataDictionary</param>
		public static void AttachViewModel<TModel>(this Controller controller, TModel model)
		{
			controller.ViewData.AttachViewModel<TModel>(model);
		}
	}
}
