﻿using System.Reflection;

using Android.App;
using Android.OS;
using Xamarin.Android.NUnitLite;

namespace AndroidTestProject
{
	[Activity (Label = "AndroidTestProject", MainLauncher = true)]
	public class MainActivityTest : TestSuiteActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			// tests can be inside the main assembly
			AddTest (Assembly.GetExecutingAssembly ());
			// or in any reference assemblies
			// AddTest (typeof (Your.Library.TestClass).Assembly);

			// Once you called base.OnCreate(), you cannot add more assemblies.
			base.OnCreate (bundle);
		}
	}
}

