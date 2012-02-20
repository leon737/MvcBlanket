using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using MvcBlanketLib.Extensions;

namespace MvcBlanketLib.Helpers
{
	/// <summary>
	/// Specifies that the field must compare favourably with the named field, if objects to check are not of the same type
	/// false will be return
	/// </summary>
	[AttributeUsageAttribute(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
	public class CompareValuesAttribute : ValidationAttribute
	{
		/// <summary>
		/// The other property to compare to
		/// </summary>
		public string OtherProperty { get; set; }

		public CompareValues Criteria { get; set; }

		/// <summary>
		/// Creates the attribute
		/// </summary>
		/// <param name="otherProperty">The other property to compare to</param>
		public CompareValuesAttribute(string otherProperty, CompareValues criteria)
		{
			if (otherProperty == null)
				throw new ArgumentNullException("otherProperty");

			OtherProperty = otherProperty;
			Criteria = criteria;
		}

		/// <summary>
		/// Determines whether the specified value of the object is valid.  For this to be the case, the objects must be of the same type
		/// and satisfy the comparison criteria. Null values will return false in all cases except when both
		/// objects are null.  The objects will need to implement IComparable for the GreaterThan,LessThan,GreatThanOrEqualTo and LessThanOrEqualTo instances
		/// </summary>
		/// <param name="value">The value of the object to validate</param>
		/// <param name="validationContext">The validation context</param>
		/// <returns>A validation result if the object is invalid, null if the object is valid</returns>
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			// the the other property
			var property = validationContext.ObjectType.GetProperty(OtherProperty);

			// check it is not null
			if (property == null)
				return new ValidationResult(String.Format("Unknown property: {0}.", OtherProperty));

			// check types
			if (validationContext.ObjectType.GetProperty(validationContext.MemberName).PropertyType != property.PropertyType)
				return new ValidationResult(String.Format("The types of {0} and {1} must be the same.", validationContext.DisplayName, OtherProperty));

			// get the other value
			var other = property.GetValue(validationContext.ObjectInstance, null);

			// equals to comparison,
			if (Criteria == CompareValues.EqualTo)
			{
				if (Object.Equals(value, other))
					return null;
			}
			else if (Criteria == CompareValues.NotEqualTo)
			{
				if (!Object.Equals(value, other))
					return null;
			}
			else
			{
				// check that both objects are IComparables
				if (!(value is IComparable) || !(other is IComparable))
					return new ValidationResult(String.Format("{0} and {1} must both implement IComparable", validationContext.DisplayName, OtherProperty));

				// compare the objects
				var result = Comparer.Default.Compare(value, other);

				switch (Criteria)
				{
					case CompareValues.GreaterThan:
						if (result > 0)
							return null;
						break;
					case CompareValues.LessThan:
						if (result < 0)
							return null;
						break;
					case CompareValues.GreatThanOrEqualTo:
						if (result >= 0)
							return null;
						break;
					case CompareValues.LessThanOrEqualTo:
						if (result <= 0)
							return null;
						break;
				}
			}

			// got this far must mean the items don't meet the comparison criteria
			return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
		}

		/// <summary>
		/// Applies formatting to an error message.
		/// </summary>
		/// <param name="name">The name to include in the error message</param>
		/// <returns></returns>
		public override string FormatErrorMessage(string name)
		{
			return String.Format(CultureInfo.CurrentCulture, base.ErrorMessageString, name, OtherProperty, Criteria.Description());
		}

		/// <summary>
		/// retrieve the object to compare to
		/// </summary>
		/// <returns></returns>
		object GetOther(ValidationContext context)
		{
			return null;
		}
	}

	/// <summary>
	/// Indicates a comparison criteria used by the CompareValues attribute
	/// </summary>
	public enum CompareValues
	{
		[Description("=")]
		EqualTo,
		[Description("!=")]
		NotEqualTo,
		[Description(">")]
		GreaterThan,
		[Description("<")]
		LessThan,
		[Description(">=")]
		GreatThanOrEqualTo,
		[Description("<=")]
		LessThanOrEqualTo
	}
}
