package md5e0d9a58f8ef4c80c20dd923cca887f60;


public class TestResultActivity
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("Xamarin.Android.NUnitLite.TestResultActivity, Xamarin.Android.NUnitLite, Version=1.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", TestResultActivity.class, __md_methods);
	}


	public TestResultActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == TestResultActivity.class)
			mono.android.TypeManager.Activate ("Xamarin.Android.NUnitLite.TestResultActivity, Xamarin.Android.NUnitLite, Version=1.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
