
function autoadjustheight(iframeid)
{
	// Find the height of the internal page
	var doc_height = document.getElementById(iframeid).contentWindow.document.body.scrollHeight;
	
	// Change the height of the iframe
	document.getElementById(iframeid).height = doc_height;
}

