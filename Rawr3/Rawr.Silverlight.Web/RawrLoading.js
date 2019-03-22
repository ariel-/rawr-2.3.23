if (!window.Rawr.Silverlight.Web)
	window.Rawr.Silverlight.Web = {};

Rawr.Silverlight.Web.RawrLoading = function() 
{
}

Rawr.Silverlight.Web.RawrLoading.prototype =
{
	handleLoad: function(plugIn, userContext, rootElement) 
	{
		this.plugIn = plugIn;
		
		// Sample button event hookup: Find the button and then attach event handlers
		// this.button = rootElement.children.getItem(0);	
		
		// this.button.addEventListener("MouseDown", Silverlight.createDelegate(this, this.handleMouseDown));
	}
	
	// Sample event handler
	//handleMouseDown: function(sender, eventArgs) 
	//{
	//}
}