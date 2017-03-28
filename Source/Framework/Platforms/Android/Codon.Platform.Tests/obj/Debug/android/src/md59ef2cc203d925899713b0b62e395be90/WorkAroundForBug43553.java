package md59ef2cc203d925899713b0b62e395be90;


public class WorkAroundForBug43553
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("Codon.WorkAroundForBug43553, Codon.Platform, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", WorkAroundForBug43553.class, __md_methods);
	}


	public WorkAroundForBug43553 () throws java.lang.Throwable
	{
		super ();
		if (getClass () == WorkAroundForBug43553.class)
			mono.android.TypeManager.Activate ("Codon.WorkAroundForBug43553, Codon.Platform, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

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
